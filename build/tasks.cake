Task("Clean")
    .Does<BuildParameters>((parameters) =>
{
    Information("Cleaning directories..");

    CleanDirectories("./src/**/bin/" + parameters.Configuration);
    CleanDirectories("./src/**/obj");

    CleanDirectories(parameters.Paths.Directories.ToClean);
});

Task("Build")
    .IsDependentOn("Clean")
    .Does<BuildParameters>((parameters) =>
{
    // build .Net code
    Build(parameters);
});

Task("Test")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.EnabledUnitTests, "Unit tests were disabled.")
    .IsDependentOn("Build")
    .Does<BuildParameters>((parameters) =>
{
    var frameworks = new[] { "netcoreapp2.1", "netcoreapp3.0" };
    var testResultsPath = parameters.Paths.Directories.TestResultsOutput;

    foreach(var framework in frameworks)
    {
        // run using dotnet test
        var actions = new List<Action>();
        var projects = GetFiles("./src/**/*.Test.csproj");
        foreach(var project in projects)
        {
            var projectName = $"{project.GetFilenameWithoutExtension()}.{framework}";
            var settings = new DotNetCoreTestSettings {
                Framework = framework,
                NoBuild = true,
                NoRestore = true,
                Configuration = parameters.Configuration,
                Filter = "Category=UnitTest"
            };

            if (!parameters.IsRunningOnMacOS) {
                settings.TestAdapterPath = new DirectoryPath(".");
                var resultsPath = MakeAbsolute(testResultsPath.CombineWithFilePath($"{projectName}.results.xml"));
                settings.Logger = $"xunit;LogFilePath={resultsPath}";
            }

            var outputFileTemplate = ($"{projectName}.coverage").Replace(".", "_");
            var coverletSettings = new CoverletSettings {
                CollectCoverage = true,
                CoverletOutputFormat = CoverletOutputFormat.opencover | CoverletOutputFormat.json,
                CoverletOutputDirectory = testResultsPath,
                CoverletOutputName = outputFileTemplate
            };               

            DotNetCoreTest(project.FullPath, settings, coverletSettings);
        }
    }
})
.ReportError(exception =>
{
    var error = (exception as AggregateException).InnerExceptions[0];
    Error(error.Dump());
})
.Finally(() =>
{
    var parameters = Context.Data.Get<BuildParameters>();
    var testResultsFiles = GetFiles(parameters.Paths.Directories.TestResultsOutput + "/*.results.xml");
    if (parameters.IsRunningOnAzurePipeline)
    {
        if (testResultsFiles.Any()) {
            var data = new TFBuildPublishTestResultsData {
                TestResultsFiles = testResultsFiles.ToArray(),
                TestRunner = TFTestRunnerType.XUnit
            };
            TFBuild.Commands.PublishTestResults(data);
        }
    }
});

Task("IntegrationTest")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.EnabledIntegrationTests, "IntegrationTest tests were disabled.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnLinux, "IntegrationTest tests can not run on Mac for now")
    .IsDependentOn("Test")
    .Does<BuildParameters>((parameters) =>
{
    var frameworks = new[] { "netcoreapp2.1", "netcoreapp3.0" };
    var testResultsPath = parameters.Paths.Directories.TestResultsOutput;

    foreach(var framework in frameworks)
    {
        // run using dotnet test
        StartDockerContainer();

        var actions = new List<Action>();
        var projects = GetFiles("./src/**/*.Test.csproj");
        foreach(var project in projects)
        {
            var projectName = $"{project.GetFilenameWithoutExtension()}.{framework}";
            var settings = new DotNetCoreTestSettings {
                Framework = framework,
                NoBuild = true,
                NoRestore = true,
                Configuration = parameters.Configuration,
                Filter = "Category=IntegrationTest"
            };

            if (!parameters.IsRunningOnMacOS) {
                settings.TestAdapterPath = new DirectoryPath(".");
                var resultsPath = MakeAbsolute(testResultsPath.CombineWithFilePath($"{projectName}.resultsIntegration.xml"));
                settings.Logger = $"xunit;LogFilePath={resultsPath}";
            }

            var outputFileTemplate = ($"{projectName}.coverage").Replace(".", "_");
            var coverletSettings = new CoverletSettings {
                CollectCoverage = true,
                CoverletOutputFormat = CoverletOutputFormat.opencover | CoverletOutputFormat.json,
                CoverletOutputDirectory = testResultsPath,
                MergeWithFile = $"{outputFileTemplate}.json",
                CoverletOutputName = outputFileTemplate
            };

            DotNetCoreTest(project.FullPath, settings, coverletSettings);
        }
        DockerRm(new DockerContainerRmSettings { Force = true }, "IntegrationTestContainer");
    }

})
.ReportError(exception =>
{
    var error = (exception as AggregateException).InnerExceptions[0];
    Error(error.Dump());
})
.Finally(() =>
{
    var parameters = Context.Data.Get<BuildParameters>();
    var testResultsFiles = GetFiles(parameters.Paths.Directories.TestResultsOutput + "/*.resultsIntegration.xml");
    if (parameters.IsRunningOnAzurePipeline)
    {
        if (testResultsFiles.Any()) {
            var data = new TFBuildPublishTestResultsData {
                TestResultsFiles = testResultsFiles.ToArray(),
                TestRunner = TFTestRunnerType.XUnit
            };
            TFBuild.Commands.PublishTestResults(data);
        }
    }
});

void StartDockerContainer()
{
    var dockerSettings = new DockerContainerRunSettings
    {
        Name = "IntegrationTestContainer",
        Env =  new[] {
           "KEYCLOAK_USER=Admin",
           "KEYCLOAK_PASSWORD=Admin"
        },
        Publish = new[] {
           "8080:8080"
        },
        Detach = true
    };
    DockerRun(dockerSettings, "jboss/keycloak:7.0.1", null);

    string responseBody = "";
    do {
        try {
            responseBody = HttpGet("http://localhost:8080/auth/realms/master");
        } catch(Exception ex){
            System.Threading.Thread.Sleep(2000);
        }
    } while(responseBody.Length <= 0);
}

Task("Pack-Nuget")
    .IsDependentOn("Test")
    .IsDependentOn("IntegrationTest")
    .Does<BuildParameters>((parameters) =>
{
    var settings = new DotNetCorePackSettings
    {
        Configuration = parameters.Configuration,
        OutputDirectory = parameters.Paths.Directories.NugetRoot,
        NoBuild = true,
        NoRestore = true,
        MSBuildSettings = parameters.MSBuildSettings
    };

    DotNetCorePack("./src/KeycloakClient", settings);
});

Task("Publish-AzurePipeline")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows,       "Publish-AzurePipeline works only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAzurePipeline, "Publish-AzurePipeline works only on AzurePipeline.")
    .WithCriteria<BuildParameters>((context, parameters) => !parameters.IsPullRequest,           "Publish-AzurePipeline works only for non-PR commits.")
    .IsDependentOnWhen("Pack", singleStageRun)
    .Does<BuildParameters>((parameters) =>
{
    foreach(var artifact in parameters.Artifacts.All)
    {
        if (FileExists(artifact.ArtifactPath)) { TFBuild.Commands.UploadArtifact(artifact.ContainerName, artifact.ArtifactPath, artifact.ArtifactName); }
    }
    foreach(var package in parameters.Packages.All)
    {
        if (FileExists(package.PackagePath)) { TFBuild.Commands.UploadArtifact("packages", package.PackagePath, package.PackageName); }
    }
})
.OnError(exception =>
{
    Information("Publish-AzurePipeline Task failed, but continuing with next Task...");
    Error(exception.Dump());
    publishingError = true;
});

Task("Publish-NuGet")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.EnabledPublishNuget,      "Publish-NuGet was disabled.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows,       "Publish-NuGet works only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAzurePipeline, "Publish-NuGet works only on AzurePipeline.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsStableRelease() || parameters.IsPreRelease(), "Publish-NuGet works only for releases.")
    .IsDependentOnWhen("Pack-NuGet", singleStageRun)
    .Does<BuildParameters>((parameters) =>
{
    var apiKey = parameters.Credentials.Nuget.ApiKey;
    if(string.IsNullOrEmpty(apiKey)) {
        throw new InvalidOperationException("Could not resolve NuGet API key.");
    }

    var apiUrl = parameters.Credentials.Nuget.ApiUrl;
    if(string.IsNullOrEmpty(apiUrl)) {
        throw new InvalidOperationException("Could not resolve NuGet API url.");
    }

    foreach(var package in parameters.Packages.All)
    {
        if (FileExists(package.PackagePath))
        {
            // Push the package.
            NuGetPush(package.PackagePath, new NuGetPushSettings
            {
                ApiKey = apiKey,
                Source = apiUrl
            });
        }
    }
})
.OnError(exception =>
{
    Information("Publish-NuGet Task failed, but continuing with next Task...");
    Error(exception.Dump());
    publishingError = true;
});

Task("Publish-Coverage")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnLinux,       "Publish-Coverage works only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAzurePipeline, "Publish-Coverage works only on AzurePipeline.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsStableRelease() || parameters.IsPreRelease(), "Publish-Coverage works only for releases.")
    .IsDependentOnWhen("Test", singleStageRun)
    .Does<BuildParameters>((parameters) =>
{
    var coverageFiles = GetFiles(parameters.Paths.Directories.TestResultsOutput + "/*.opencover.xml");

    var token = parameters.Credentials.CodeCov.Token;
    if(string.IsNullOrEmpty(token)) {
        throw new InvalidOperationException("Could not resolve CodeCov token.");
    }

    foreach (var coverageFile in coverageFiles) {
        Codecov(new CodecovSettings {
            Files = new [] { coverageFile.ToString() },
            Token = token
        });
    }
});

Task("Release-Notes")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows,       "Release notes are generated only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAzurePipeline, "Release notes are generated only on AzurePipeline.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsStableRelease(),        "Release notes are generated only for stable releases.")
    .Does<BuildParameters>((parameters) =>
{
    var token = parameters.Credentials.GitHub.Token;
    if(string.IsNullOrEmpty(token)) {
        throw new InvalidOperationException("Could not resolve Github token.");
    }

    var repoOwner = BuildParameters.MainRepoOwner;
    var repository = BuildParameters.MainRepoName;
    GitReleaseManagerCreate(token, repoOwner, repository, new GitReleaseManagerCreateSettings {
        Milestone         = parameters.Version.Milestone,
        Name              = parameters.Version.Milestone,
        Prerelease        = true,
        TargetCommitish   = "master"
    });

    GitReleaseManagerAddAssets(token, repoOwner, repository, parameters.Version.Milestone, parameters.Paths.Files.ZipArtifactPathCoreClr.ToString());
    GitReleaseManagerClose(token, repoOwner, repository, parameters.Version.Milestone);

}).ReportError(exception =>
{
    Error(exception.Dump());
});
