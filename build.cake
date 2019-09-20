// Install modules
#module nuget:?package=Cake.DotNetTool.Module&version=0.3.0

// Install addins.
#addin "nuget:?package=Newtonsoft.Json&version=11.0.2"
#addin "nuget:?package=Cake.Codecov&version=0.7.0"
#addin "nuget:?package=Cake.Coverlet&version=2.3.4"
#addin "nuget:?package=Cake.Docker&version=0.10.1"
#addin "nuget:?package=Cake.Incubator&version=5.1.0"
#addin "nuget:?package=Cake.Http&version=0.7.0"

#tool "nuget:?package=nuget.commandline&version=5.2.0"

// Install .NET Core Global tools.
#tool "dotnet:?package=Codecov.Tool&version=1.7.2"
#tool "dotnet:?package=GitVersion.Tool&version=5.0.1"
#tool "dotnet:?package=GitReleaseManager.Tool&version=0.8.0"

// Load other scripts.
#load "./build/utils/parameters.cake"
#load "./build/utils/utils.cake"

#load "./build/tasks.cake"

//////////////////////////////////////////////////////////////////////
// PARAMETERS
//////////////////////////////////////////////////////////////////////
bool publishingError = false;
bool singleStageRun = true;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup<BuildParameters>(context =>
{
    EnsureDirectoryExists("artifacts");
    var parameters = BuildParameters.GetParameters(context);
    var gitVersion = GetVersion(parameters);
    parameters.Initialize(context, gitVersion);

    // Increase verbosity?
    if (parameters.IsMainBranch && (context.Log.Verbosity != Verbosity.Diagnostic)) {
        Information("Increasing verbosity to diagnostic.");
        context.Log.Verbosity = Verbosity.Diagnostic;
    }

    Information("Building version {0} of KeycloakClient ({1}, {2})",
        parameters.Version.SemVersion,
        parameters.Configuration,
        parameters.Target);

    Information("Repository info : IsMainRepo {0}, IsMainBranch {1}, IsTagged: {2}, IsPullRequest: {3}",
        parameters.IsMainRepo,
        parameters.IsMainBranch,
        parameters.IsTagged,
        parameters.IsPullRequest);

    return parameters;
});

Teardown<BuildParameters>((context, parameters) =>
{
    try
    {
        Information("Starting Teardown...");

        Information("Repository info : IsMainRepo {0}, IsMainBranch {1}, IsTagged: {2}, IsPullRequest: {3}",
            parameters.IsMainRepo,
            parameters.IsMainBranch,
            parameters.IsTagged,
            parameters.IsPullRequest);

        Information("Finished running tasks.");
    }
    catch (Exception exception)
    {
        Error(exception.Dump());
    }
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("Pack")
    .IsDependentOn("Pack-Nuget")
    .IsDependentOn("Publish-Coverage")
    .Finally(() =>
{
});

Task("Publish")
    .IsDependentOn("Publish-AzurePipeline")
    .IsDependentOn("Publish-NuGet")
    .Finally(() =>
{
    if (publishingError)
    {
        throw new Exception("An error occurred during the publishing of KeycloakClient. All publishing tasks have been attempted.");
    }
});

Task("Release")
    .IsDependentOn("Release-Notes")
    .Finally(() =>
{
});

Task("Default")
    .IsDependentOn("Pack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
RunTarget(target);
