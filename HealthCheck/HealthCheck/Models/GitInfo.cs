namespace HealthCheck.Models;

/// <summary>
/// Содержит в себе информацию из репозитория git
/// </summary>
public partial class GitInfo
{
    private GitInfo() { }

    public static GitInfoBuilder GetBuilder()
    {
        return new GitInfoBuilder();
    }


    #region Свойства из GIT

    public string Tag { get; private set; }

    public string Branch { get; private set; }

    public string Version { get; private set; }

    public string Commit { get; private set; }

    public string CommitDate { get; private set; }

    #endregion Свойства из GIT
}