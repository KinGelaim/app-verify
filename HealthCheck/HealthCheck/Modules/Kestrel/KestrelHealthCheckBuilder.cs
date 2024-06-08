using HealthCheck.Checks;
using HealthCheck.Models;
using HealthCheck.Modules.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using System.Text;


namespace HealthCheck.Modules.Kestrel;

public partial class KestrelHealthCheck
{
    public sealed class KestrelHealthCheckBuilder
    {
        private readonly WebApplicationBuilder _builder = WebApplication.CreateBuilder();

        private readonly IHealthChecksBuilder _healthChecksBuilder;

        private WebApplication _app;

        private readonly StringBuilder _errorBuilder = new();

        public KestrelHealthCheckBuilder()
        {
            _healthChecksBuilder = _builder.Services.AddHealthChecks();
        }

        /// <summary>
        /// Добавить конечные точки (urls)
        /// </summary>
        /// <param name="port">Порт</param>
        /// <param name="allowLocalhost">Использовать ли локалхост для healthCheck</param>
        public KestrelHealthCheckBuilder RegisterUrls(int port, bool allowLocalhost = false)
        {
            if (port <= 0)
            {
                _errorBuilder.AppendLine($"{nameof(KestrelHealthCheckBuilder)}.{nameof(RegisterUrls)}: {nameof(port)} <= 0.");
                return this;
            }

            var ips = Dns.GetHostAddresses(Dns.GetHostName())
                .Where(ip => allowLocalhost || !IPAddress.IsLoopback(ip))
                .Select(ip => $"http://{ip}:{port}")
                .ToArray();

            if (!ips.Any())
            {
                _errorBuilder.AppendLine($"{nameof(KestrelHealthCheckBuilder)}.{nameof(RegisterUrls)}: Unable to bind healthCheck to IP.");
                return this;
            }

            _builder.WebHost.UseUrls(ips);

            return this;
        }

        /// <summary>
        /// Добавить в процесс создания HealthCheck gitinfo
        /// </summary>
        /// <param name="gitInfo">Информация по гиту</param>
        public KestrelHealthCheckBuilder AddGitInfo(GitInfo gitInfo)
        {
            if (gitInfo == null)
            {
                _errorBuilder.AppendLine($"{nameof(KestrelHealthCheckBuilder)}.{nameof(AddGitInfo)}: {nameof(gitInfo)} is null.");
                return this;
            }

            _builder.Services.AddSingleton(gitInfo);
            _builder.Services.AddSingleton<GitCheck>();

            return this;
        }

        /// <summary>
        /// Зарегистрировать отдельный HealthCheck, не допуская регистрации под одним именем
        /// </summary>
        /// <typeparam name="T">Класс проверок</typeparam>
        /// <param name="name">Имя, под которым регистрировать</param>
        public KestrelHealthCheckBuilder Register<T>(string name) where T : class, IHealthCheck
        {
            _healthChecksBuilder.SafeAddCheck<T>(name);

            return this;
        }

        /// <summary>
        /// Регистрация встроенных в библиотеку проверок
        /// </summary>
        public KestrelHealthCheckBuilder RegisterStandard()
        {
            _healthChecksBuilder.AutoDiscoverChecks(_builder.Services);

            return this;
        }

        /// <summary>
        /// Регистрация проверок из модуля
        /// </summary>
        /// <typeparam name="T">Тип, содержащийся в модуле, откуда подключать проверки</typeparam>
        public KestrelHealthCheckBuilder RegisterFrom<T>()
        {
            _healthChecksBuilder.AutoDiscoverChecks<T>(_builder.Services);

            return this;
        }

        /// <summary>
        /// Указание конечной точки  для доступа
        /// </summary>
        /// <param name="path">Путь относительно url</param>
        public KestrelHealthCheckBuilder UseEndpoint(string path)
        {
            _app = _builder.Build();

            _app.UseHealthChecks(path, new HealthCheckOptions
            {
                ResponseWriter = HealthWriter.JsonWriter
            });

            return this;
        }

        /// <summary>
        /// Создание готового к работе экземпляра модуля проверки
        /// </summary>
        public KestrelHealthCheck Build()
        {
            if (_errorBuilder.Length != 0)
            {
                throw new InvalidOperationException(_errorBuilder.ToString());
            }

            if (_app == null)
            {
                throw new InvalidOperationException($"Must specify endpoint: {nameof(UseEndpoint)}.");
            }

            return new KestrelHealthCheck(_app);
        }
    }
}