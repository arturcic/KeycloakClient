public class BuildCredentials
{
    public GitHubCredentials GitHub { get; private set; }
    public NugetCredentials Nuget { get; private set; }
    public CodeCovCredentials CodeCov { get; private set; }

    public static BuildCredentials GetCredentials(ICakeContext context)
    {
        return new BuildCredentials
        {
            GitHub     = GitHubCredentials.GetGitHubCredentials(context),
            Nuget      = NugetCredentials.GetNugetCredentials(context),
            CodeCov    = CodeCovCredentials.GetCodeCovCredentials(context),
        };
    }
}

public class GitHubCredentials
{
    public string UserName { get; private set; }
    public string Password { get; private set; }
    public string Token { get; private set; }

    public GitHubCredentials(string userName, string password, string token)
    {
        UserName = userName;
        Password = password;
        Token = token;
    }

    public static GitHubCredentials GetGitHubCredentials(ICakeContext context)
    {
        return new GitHubCredentials(
            context.EnvironmentVariable("GITHUB_USERNAME"),
            context.EnvironmentVariable("GITHUB_PASSWORD"),
            context.EnvironmentVariable("GITHUB_TOKEN"));
    }
}

public class NugetCredentials
{
    public string ApiKey { get; private set; }
    public string ApiUrl { get; private set; }

    public NugetCredentials(string apiKey, string apiUrl)
    {
        ApiKey = apiKey;
        ApiUrl = apiUrl;
    }

    public static NugetCredentials GetNugetCredentials(ICakeContext context)
    {
        return new NugetCredentials(
            context.EnvironmentVariable("NUGET_API_KEY"),
            context.EnvironmentVariable("NUGET_API_URL"));
    }
}

public class CodeCovCredentials
{
    public string Token { get; private set; }

    public CodeCovCredentials(string token)
    {
        Token = token;
    }

    public static CodeCovCredentials GetCodeCovCredentials(ICakeContext context)
    {
        return new CodeCovCredentials(context.EnvironmentVariable("CODECOV_TOKEN"));
    }
}
