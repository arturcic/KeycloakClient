public class BuildPackages
{
    public ICollection<BuildPackage> Nuget { get; private set; }

    public static BuildPackages GetPackages(
        DirectoryPath nugetRooPath,
        BuildVersion version,
        string[] packageIds)
    {
        var toNugetPackage = BuildPackage(nugetRooPath, version.NugetVersion);
        var nugetPackages = packageIds.Select(toNugetPackage).ToArray();

        return new BuildPackages {
            Nuget = nugetPackages
        };
    }

    private static Func<string, BuildPackage> BuildPackage(
        DirectoryPath nugetRooPath,
        string version)
    {
        return package => new BuildPackage(
            id: package,
            packagePath: nugetRooPath.CombineWithFilePath(string.Concat(package, ".", version, ".nupkg")));
    }
}

public class BuildPackage
{
    public string Id { get; private set; }
    public FilePath PackagePath { get; private set; }
    public string PackageName { get; private set; }

    public BuildPackage(
        string id,
        FilePath packagePath)
    {
        Id = id;
        PackagePath = packagePath;
        PackageName = PackagePath.GetFilename().ToString();
    }
}

public class BuildArtifacts
{
    public ICollection<BuildArtifact> All { get; private set; }

    public static BuildArtifacts GetArtifacts(FilePath[] artifacts)
    {
        var toBuildArtifact = BuildArtifact("build-artifact");
        var buildArtifacts = artifacts.Select(toBuildArtifact).ToArray();

        return new BuildArtifacts {
            All = buildArtifacts.ToArray(),
        };
    }

    private static Func<FilePath, BuildArtifact> BuildArtifact(string containerName)
    {
        return artifactPath => new BuildArtifact(containerName: containerName, artifactPath: artifactPath);
    }
}

public class BuildArtifact
{
    public string ContainerName { get; private set; }
    public FilePath ArtifactPath { get; private set; }
    public string ArtifactName { get; private set; }

    public BuildArtifact(
        string containerName,
        FilePath artifactPath)
    {
        ContainerName = containerName;
        ArtifactPath = artifactPath.FullPath;
        ArtifactName = ArtifactPath.GetFilename().ToString();
    }
}