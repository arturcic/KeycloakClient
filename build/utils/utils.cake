FilePath FindToolInPath(string tool)
{
    var pathEnv = EnvironmentVariable("PATH");
    if (string.IsNullOrEmpty(pathEnv) || string.IsNullOrEmpty(tool)) return tool;

    var paths = pathEnv.Split(new []{ IsRunningOnUnix() ? ':' : ';'},  StringSplitOptions.RemoveEmptyEntries);
    return paths.Select(path => new DirectoryPath(path).CombineWithFilePath(tool)).FirstOrDefault(filePath => FileExists(filePath.FullPath));
}

DirectoryPath HomePath()
{
    return IsRunningOnWindows()
        ? new DirectoryPath(EnvironmentVariable("HOMEDRIVE") +  EnvironmentVariable("HOMEPATH"))
        : new DirectoryPath(EnvironmentVariable("HOME"));
}

public static bool IsEnabled(ICakeContext context, string envVar, bool nullOrEmptyAsEnabled = true)
{
    var value = context.EnvironmentVariable(envVar);

    return string.IsNullOrWhiteSpace(value) ? nullOrEmptyAsEnabled : bool.Parse(value);
}

public static List<string> ExecGitCmd(ICakeContext context, string cmd)
{
    var gitPath = context.Tools.Resolve(context.IsRunningOnWindows() ? "git.exe" : "git");
    context.StartProcess(gitPath, new ProcessSettings { Arguments = cmd, RedirectStandardOutput = true }, out var redirectedOutput);

    return redirectedOutput.ToList();
}

void ReplaceTextInFile(FilePath filePath, string oldValue, string newValue, bool encrypt = false)
{
    Information("Replacing {0} with {1} in {2}", oldValue, !encrypt ? newValue : "******", filePath);
    var file = filePath.FullPath.ToString();
    System.IO.File.WriteAllText(file, System.IO.File.ReadAllText(file).Replace(oldValue, newValue));
}

GitVersion GetVersion(BuildParameters parameters)
{
    var settings = new GitVersionSettings
    {
        OutputType = GitVersionOutput.Json,
    };

    var gitVersion = GitVersion(settings);

    if (!parameters.IsLocalBuild && !(parameters.IsRunningOnAzurePipeline && parameters.IsPullRequest))
    {
        settings.UpdateAssemblyInfo = true;
        settings.LogFilePath = "console";
        settings.OutputType = GitVersionOutput.BuildServer;

        GitVersion(settings);
    }
    return gitVersion;
}

void Build(BuildParameters parameters)
{
    var sln = "./src/KeycloakClient.sln";
    DotNetCoreRestore(sln, new DotNetCoreRestoreSettings
    {
        Verbosity = DotNetCoreVerbosity.Minimal,
        Sources = new [] { "https://api.nuget.org/v3/index.json" },
        MSBuildSettings = parameters.MSBuildSettings
    });

    var slnPath = MakeAbsolute(new DirectoryPath(sln));
    DotNetCoreBuild(slnPath.FullPath, new DotNetCoreBuildSettings
    {
        Verbosity = DotNetCoreVerbosity.Minimal,
        Configuration = parameters.Configuration,
        NoRestore = true,
        MSBuildSettings = parameters.MSBuildSettings
    });
}