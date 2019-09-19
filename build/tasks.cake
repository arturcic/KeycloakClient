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