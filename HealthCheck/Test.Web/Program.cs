using HealthCheck.Checks;
using HealthCheck.Models;
using HealthCheck.Models.DataBase;
using HealthCheck.Modules.Extensions;
using LinqToDB.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

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

builder.Services.AddSingleton(gitinfo);
builder.Services.AddSingleton<GitCheck>();

// Проверка баз данных
//AddMsSqlServices(builder);
//AddPostgreSqlServices(builder);

// Автоматический поиск проверок
builder.Services.AutoDiscoverChecks().AutoDiscoverChecks<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseHealthChecksJson();
app.UseHealthChecksHtml();

app.MapRazorPages();

app.Run();

void AddMsSqlServices(WebApplicationBuilder builder)
{
    var firstDb = () => new DataConnection("SqlServer", "Application Name=TestHealth;Data Source=localhost;Initial Catalog=first-db;user id=user;password=password");
    var dataBaseSettingsFirstDb = new DataBaseSettings(firstDb, new (string, string)[] { ("dbo", "db_version"), ("dbo", "asd") });
    var dataBaseSettingsFirstDbWithoutOptimize = new DataBaseSettings(firstDb, new (string, string)[] { ("dbo", "db_version"), ("dbo", "asd") }, false);

    var SQLTest = () => new DataConnection("SqlServer", "Application Name=TestHealth;Data Source=localhost;Initial Catalog=SQLTest;user id=user;password=password");
    var tablesSettings = new DataBaseExtendedSettings[]
    {
                new DataBaseExtendedSettings("dbo", "Table_1", true, true),
                new DataBaseExtendedSettings("dbo", "TABLE_2", true, true),
                new DataBaseExtendedSettings("dbo", "test", true, false),
                new DataBaseExtendedSettings("dbo", "test_2", false, true)
    };
    var dataBaseSettingsSQLTest = new DataBaseSettings(SQLTest, tablesSettings);

    var SQLTestUser = () => new DataConnection("SqlServer", "Application Name=TestHealth;Data Source=localhost;Initial Catalog=SQLTest;user id=test_health;password=password2");
    var dataBaseSettingsSQLTestUser = new DataBaseSettings(SQLTestUser);

    var notAccessUser = () => new DataConnection("SqlServer", "Application Name=TestHealth;Data Source=localhost;Initial Catalog=second-db;user id=test_health;password=password2");
    var dataBaseSettingsNotAccessUser = new DataBaseSettings(notAccessUser);

    var notExistDB = () => new DataConnection("SqlServer", "Application Name=TestHealth;Data Source=localhost;Initial Catalog=NotExist;user id=user;password=password");
    var dataBaseSettingsNotExist = new DataBaseSettings(notExistDB);

    var secondDb = () => new DataConnection("SqlServer", "Application Name=TestHealth;Data Source=localhost;Initial Catalog=second-db;user id=user;password=asd");
    var dataBaseSettingsSecondDb = new DataBaseSettings(secondDb);

    var thirdDb = () => new DataConnection("SqlServer", "Application Name=TestHealth;Data Source=another_host;Initial Catalog=vkabonauth_dev;user id=user;password=password");
    var dataBaseSettingsThirdDb = new DataBaseSettings(thirdDb, new (string, string)[] { ("dbo", "Abonent"), ("dbo", "AbonentCard"), ("dbo", "test") });
    var dataBaseSettingsThirdDbDoubleCheck = new DataBaseSettings(thirdDb, new (string, string)[] { ("dbo", "Abonent"), ("dbo", "AbonentCard"), ("dbo", "Abonent") });

    builder.Services.AddHealthChecks().AddCheck("Проверка базы данных FirstDb", new DataBaseCheck(dataBaseSettingsFirstDb));
    builder.Services.AddHealthChecks().AddCheck("Проверка базы данных FirstDb (без мини оптимизации)", new DataBaseCheck(dataBaseSettingsFirstDbWithoutOptimize));
    builder.Services.AddHealthChecks().AddCheck("Проверка базы данных SQLTest", new DataBaseCheck(dataBaseSettingsSQLTest));
    builder.Services.AddHealthChecks().AddCheck("Проверка базы данных SQLTest (доступ есть)", new DataBaseCheck(dataBaseSettingsSQLTestUser));
    builder.Services.AddHealthChecks().AddCheck("Проверка базы данных SQLTest (доступа нет)", new DataBaseCheck(dataBaseSettingsNotAccessUser));
    builder.Services.AddHealthChecks().AddCheck("Проверка базы данных NotExist", new DataBaseCheck(dataBaseSettingsNotExist));
    builder.Services.AddHealthChecks().AddCheck("Проверка базы на другом сервере", new DataBaseCheck(dataBaseSettingsThirdDb));
    builder.Services.AddHealthChecks().AddCheck("Случайно дважды проверяется одна и та же таблица", new DataBaseCheck(dataBaseSettingsThirdDbDoubleCheck));

    // Добавляем часы
    //var clock = new Clock(new DateTime(2023, 09, 14, 09, 47, 14));
    //builder.Services.AddHealthChecks().AddCheck("Проверка базы данных second-db (с часами)", new DataBaseCheck(dataBaseSettingsTruckScales, clock));
}

void AddPostgreSqlServices(WebApplicationBuilder builder)
{
    // Нет такого пользователя
    var notExsistUser = () => new DataConnection("PostgreSQL", "Application Name=TestHealth;server=localhost; user id=qwe_asd_zxc; password=password; database=healthcheck_test");
    var dataBaseSettingsNotExsistUser = new DataBaseSettings(notExsistUser);

    // Неправильный пароль
    var wrongPassword = () => new DataConnection("PostgreSQL", "Application Name=TestHealth;server=localhost; user id=healthcheck_test_role; password=qwe_asd_zxc; database=healthcheck_test");
    var dataBaseSettingsWrongPassword = new DataBaseSettings(wrongPassword);

    // Нет такой базы данных
    var notExistsDB = () => new DataConnection("PostgreSQL", "Application Name=TestHealth;server=localhost; user id=healthcheck_test_role; password=password; database=qwe_asd_zxc");
    var dataBaseSettingsNotExistDB = new DataBaseSettings(notExistsDB);

    // Нет доступа к схеме
    var schemaNotAccess = () => new DataConnection("PostgreSQL", "Application Name=TestHealth;server=localhost; user id=healthcheck_test_role; password=password; database=healthcheck_test");
    var dataBaseSettingsSchemaNoAccessDB = new DataBaseSettings(schemaNotAccess, new (string, string)[] { ("schema_without_access", "asd") });

    // Нет доступа к базе
    var notAccessDB = () => new DataConnection("PostgreSQL", "Application Name=TestHealth;server=localhost; user id=healthcheck_test_role; password=password; database=firstDb");
    var dataBaseNotAccessDB = new DataBaseSettings(notAccessDB, new (string, string)[] { ("dbo", "db_version") });

    // Проверка базы данных с оптимизацией и без (одной таблицы не существует)
    var firstCheckDB = () => new DataConnection("PostgreSQL", "Application Name=TestHealth;server=localhost; user id=healthcheck_test_role; password=password; database=healthcheck_test");
    var dataBaseSettingsFirstCheck = new DataBaseSettings(firstCheckDB, new (string, string)[] { ("dbo", "table_1"), ("dbo", "table_2"), ("dbo", "asd") });
    var dataBaseSettingsFirstCheckWithoutOptimize = new DataBaseSettings(firstCheckDB, new (string, string)[] { ("dbo", "table_1"), ("dbo", "table_2"), ("dbo", "asd") }, false);

    builder.Services.AddHealthChecks().AddCheck("PostgreSQL - нет такого пользователя", new DataBaseCheck(dataBaseSettingsNotExsistUser));
    builder.Services.AddHealthChecks().AddCheck("PostgreSQL - неправильный пароль", new DataBaseCheck(dataBaseSettingsWrongPassword));
    builder.Services.AddHealthChecks().AddCheck("PostgreSQL - нет такой базы данных", new DataBaseCheck(dataBaseSettingsNotExistDB));
    builder.Services.AddHealthChecks().AddCheck("PostgreSQL - нет доступа к схеме", new DataBaseCheck(dataBaseSettingsSchemaNoAccessDB));
    builder.Services.AddHealthChecks().AddCheck("PostgreSQL - нет доступа к базе данных (по умолчанию любой пользователь имеет доступ к любой БД, т.е. тест бесполезен, просто напоминание этого факта)", new DataBaseCheck(dataBaseNotAccessDB));
    builder.Services.AddHealthChecks().AddCheck("PostgreSQL - проверка базы данных с оптимизацией", new DataBaseCheck(dataBaseSettingsFirstCheck));
    builder.Services.AddHealthChecks().AddCheck("PostgreSQL - проверка базы данных без оптимизации", new DataBaseCheck(dataBaseSettingsFirstCheckWithoutOptimize));
}