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

Task("Pack-Nuget")
    .IsDependentOn("Build")
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