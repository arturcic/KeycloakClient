public class BuildPaths
{
    public BuildFiles Files { get; private set; }
    public BuildDirectories Directories { get; private set; }

    public static BuildPaths GetPaths(
        ICakeContext context,
        BuildParameters parameters,
        string configuration,
        BuildVersion version
        )
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }
        if (string.IsNullOrEmpty(configuration))
        {
            throw new ArgumentNullException(nameof(configuration));
        }
        if (version == null)
        {
            throw new ArgumentNullException(nameof(version));
        }

        var semVersion = version.SemVersion;

        var artifactsDir          = (DirectoryPath)(context.Directory("./artifacts") + context.Directory("v" + semVersion));
        var nugetRootDir          = artifactsDir.Combine("nuget");
        var testResultsOutputDir  = artifactsDir.Combine("test-results");

        var zipArtifactPathCoreClr = artifactsDir.CombineWithFilePath("KeycloakClient-corefx-v" + semVersion + ".zip");

        var releaseNotesOutputFilePath = artifactsDir.CombineWithFilePath("releasenotes.md");

        // Directories
        var buildDirectories = new BuildDirectories(
            artifactsDir,
            testResultsOutputDir,
            nugetRootDir);

        // Files
        var buildFiles = new BuildFiles(
            context,
            zipArtifactPathCoreClr,
            releaseNotesOutputFilePath);

        return new BuildPaths
        {
            Files = buildFiles,
            Directories = buildDirectories
        };
    }
}

public class BuildFiles
{
    public FilePath ZipArtifactPathCoreClr { get; private set; }
    public FilePath ReleaseNotesOutputFilePath { get; private set; }

    public BuildFiles(
        ICakeContext context,
        FilePath zipArtifactPathCoreClr,
        FilePath releaseNotesOutputFilePath
        )
    {
        ZipArtifactPathCoreClr = zipArtifactPathCoreClr;
        ReleaseNotesOutputFilePath = releaseNotesOutputFilePath;
    }
}

public class BuildDirectories
{
    public DirectoryPath Artifacts { get; private set; }
    public DirectoryPath NugetRoot { get; private set; }
    public DirectoryPath TestResultsOutput { get; private set; }
    public ICollection<DirectoryPath> ToClean { get; private set; }

    public BuildDirectories(
        DirectoryPath artifactsDir,
        DirectoryPath testResultsOutputDir,
        DirectoryPath nugetRootDir
        )
    {
        Artifacts = artifactsDir;
        TestResultsOutput = testResultsOutputDir;
        NugetRoot = nugetRootDir;
        ToClean = new[] {
            Artifacts,
            TestResultsOutput,
            NugetRoot
        };
    }
}
