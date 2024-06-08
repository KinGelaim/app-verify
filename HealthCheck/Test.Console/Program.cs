using HealthCheck.Models;
using HealthCheck.Modules.Extensions;
using HealthCheck.Modules.Kestrel;


namespace Test.ConsoleApp;

internal class Program
{
    public static async Task Main(string[] args)
    {
        // Информация по гиту
        var version = $"{ThisAssembly.Git.SemVer.Major}.{ThisAssembly.Git.SemVer.Minor}.{ThisAssembly.Git.SemVer.Patch}";

        var date = DateTime.Parse(ThisAssembly.Git.CommitDate).ToDateAndTime();

        var gitinfo = GitInfo
            .GetBuilder()
            .AddBranch(ThisAssembly.Git.Branch)
            .AddTag(ThisAssembly.Git.Tag)
            .AddVersion(version)
            .AddCommit(ThisAssembly.Git.Commit)
            .AddCommitDate(date)
            .Build();

        // Конфигурация приложения
        var jsonPath = "/health/jsontest";
        var port = 9998;

        // Создание и запуск веб-сервера для ожидания обращений
        await KestrelHealthCheck
            .GetBuilder()
            .AddGitInfo(gitinfo)
            .RegisterUrls(port)
            .RegisterStandard()
            .RegisterFrom<Program>()
            .UseEndpoint(jsonPath)
            .Build()
            .RunAsync();

        var count = 0;

        while (true)
        {
            Console.WriteLine($"Выполнение работы, цикл {count++}");

            await Task.Delay(100000);
        }
    }
}