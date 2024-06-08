namespace HealthCheck.Models;

public partial class GitInfo
{
    public sealed class GitInfoBuilder
    {
        private readonly GitInfo _gitInfo = new();

        /// <summary>
        /// Добавляет ветку
        /// </summary>
        public GitInfoBuilder AddBranch(string branch)
        {
            _gitInfo.Branch = branch;

            return this;
        }

        /// <summary>
        /// Добавляет тег
        /// </summary>
        public GitInfoBuilder AddTag(string tag)
        {
            _gitInfo.Tag = tag;

            return this;
        }

        /// <summary>
        /// Добавляет версию
        /// </summary>
        public GitInfoBuilder AddVersion(string version)
        {
            _gitInfo.Version = version;

            return this;
        }

        /// <summary>
        /// Добавляет коммит
        /// </summary>
        public GitInfoBuilder AddCommit(string commit)
        {
            _gitInfo.Commit = commit;

            return this;
        }

        /// <summary>
        /// Добавляет дату коммита
        /// </summary>
        /// <param name="commitDate"></param>
        public GitInfoBuilder AddCommitDate(string commitDate)
        {
            _gitInfo.CommitDate = commitDate;

            return this;
        }

        /// <summary>
        /// Возвращает экземпляр результата проверки
        /// </summary>
        public GitInfo Build()
        {
            return _gitInfo;
        }
    }
}