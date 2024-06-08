using HealthCheck.Checks;
using HealthCheck.Interfaces;
using HealthCheck.Models;
using HealthCheck.Models.DataBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection;


namespace HealthCheck.Modules.Extensions;

/// <summary>
/// Расширения для WebApplicationBuilder, IHealthChecksBuilder, IServiceCollection
/// </summary>
public static class WebApplicationBuilderExtension
{
    #region Private

    private static readonly HashSet<string> _checkNames = new();

    /// <summary>
    /// Добавление в билдер проверок, реализующих интерфейс IHealthCheck
    /// </summary>
    private static void AddAssemblyChecks(Assembly assembly, IHealthChecksBuilder builder, ServiceProvider provider)
    {
        var types = assembly.GetTypes().Where(w => w.GetInterfaces().Contains(typeof(IHealthCheck))).ToArray();

        if (types is null || types.Length == 0)
        {
            return;
        }

        var gitIsRegistered = provider.GetService(typeof(GitInfo)) != null;
        var dbSettingsIsRegistered = provider.GetService(typeof(DataBaseSettings)) != null;

        foreach (var type in types)
        {
            if (type.IsInterface || type.IsAbstract)
            {
                continue;
            }

            if (type == typeof(GitCheck) && !gitIsRegistered
                || type == typeof(DataBaseCheck) && !dbSettingsIsRegistered)
            {
                continue;
            }

            var instance = provider != null ? (IHealthCheck)provider.GetService(type) : null;

            instance ??= (IHealthCheck)Activator.CreateInstance(type);

            if (instance is null)
            {
                continue;
            }

            var checkName = instance is INamedCheck check ? check.CheckName : instance.GetType().Name;

            if (_checkNames.Contains(checkName))
            {
                continue;
            }

            _checkNames.Add(checkName);

            builder.AddCheck(checkName, instance);
        }
    }

    /// <summary>
    /// Получение проверок из текущей сборки
    /// </summary>
    private static IHealthChecksBuilder GetChecksFromCurrentAssembly(this IHealthChecksBuilder builder, IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        if (assembly is null)
        {
            return builder;
        }

        var provider = services.BuildServiceProvider();

        AddAssemblyChecks(assembly, builder, provider);

        return builder;
    }

    /// <summary>
    /// Получение проверок из заданной сборки
    /// </summary>
    private static IHealthChecksBuilder GetChecksFromGivenAssembly<T>(this IHealthChecksBuilder builder, IServiceCollection services)
    {
        var assembly = typeof(T).Assembly;

        if (assembly is null)
        {
            return builder;
        }

        var provider = services.BuildServiceProvider();

        AddAssemblyChecks(assembly, builder, provider);

        return builder;
    }

    #endregion Private


    /// <summary>
    /// Зарегистрировать отдельный HealthCheck, не допуская регистрации под одним именем
    /// </summary>
    /// <typeparam name="T">Класс проверок</typeparam>
    /// <param name="builder">Билдер</param>
    /// <param name="checkName">Имя, под которым регистрировать</param>
    public static IHealthChecksBuilder SafeAddCheck<T>(this IHealthChecksBuilder builder, string checkName) where T : class, IHealthCheck
    {
        if (_checkNames.Contains(checkName))
        {
            return builder;
        }

        _checkNames.Add(checkName);

        return builder.AddCheck<T>(checkName);
    }

    /// <summary>
    /// Автоматическое подключение проверок из списка сервисов
    /// </summary>
    /// <param name="services">Сервисы (для DI)</param>
    public static IServiceCollection AutoDiscoverChecks(this IServiceCollection services)
    {
        var builder = services.AddHealthChecks();

        builder.GetChecksFromCurrentAssembly(services);

        return services;
    }

    /// <summary>
    /// Автоматическое подключение проверок из списка сервисов
    /// </summary>
    /// <typeparam name="T">Тип, лежащий в сборке, где надо найти хелсчеки</typeparam>
    /// <param name="services">Сервисы (для DI)</param>
    public static IServiceCollection AutoDiscoverChecks<T>(this IServiceCollection services)
    {
        var builder = services.AddHealthChecks();

        builder.GetChecksFromGivenAssembly<T>(services);

        return services;
    }

    /// <summary>
    /// Автоматически найти все хелсчеки для текущей сборки в применении к IHealthChecksBuilder
    /// </summary>
    /// <param name="builder">Билдер</param>
    /// <param name="services">Сервисы (для DI)</param>
    public static IHealthChecksBuilder AutoDiscoverChecks(this IHealthChecksBuilder builder, IServiceCollection services) => builder.GetChecksFromCurrentAssembly(services);

    /// <summary>
    /// Автоматически найти все хелсчеки для текущей сборки в применении к WebApplicationBuilder
    /// </summary>
    /// <param name="builder">Билдер</param>
    /// <param name="services">Сервисы (для DI)</param>
    public static IHealthChecksBuilder AutoDiscoverChecks(this WebApplicationBuilder builder, IServiceCollection services)
    {
        var checkBuilder = builder.Services.AddHealthChecks();

        return checkBuilder.GetChecksFromCurrentAssembly(services);
    }

    /// <summary>
    /// Автоматически найти все хелсчеки для сборки, содержащей тип Т. Применяется к IHealthChecksBuilder
    /// </summary>
    /// <typeparam name="T">Тип, лежащий в сборке, где надо найти хелсчеки</typeparam>
    /// <param name="builder">Сам билдер</param>
    /// <param name="services">Сервисы (для DI)</param>
    public static IHealthChecksBuilder AutoDiscoverChecks<T>(this IHealthChecksBuilder builder, IServiceCollection services) => builder.GetChecksFromGivenAssembly<T>(services);

    /// <summary>
    /// Автоматически найти все хелсчеки для сборки, содержащей тип Т. Применяется к WebApplicationBuilder
    /// </summary>
    /// <typeparam name="T">Тип, лежащий в сборке, где надо найти хелсчеки</typeparam>
    /// <param name="builder">Сам билдер</param>
    /// <param name="services">Сервисы (для DI)</param>
    public static IHealthChecksBuilder AutoDiscoverChecks<T>(this WebApplicationBuilder builder, IServiceCollection services)
    {
        var checkBuilder = builder.Services.AddHealthChecks();

        return checkBuilder.GetChecksFromGivenAssembly<T>(services);
    }
}