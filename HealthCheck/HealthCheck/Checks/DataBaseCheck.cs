using HealthCheck.Interfaces;
using HealthCheck.Models;
using HealthCheck.Models.DataBase;
using HealthCheck.Models.Enum;
using HealthCheck.Modules.DataBase;
using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace HealthCheck.Checks;

/// <summary>
/// Создание результатов проверки базы данных
/// </summary>
public sealed class DataBaseCheck : INamedCheck
{
    /// <summary>
    /// Наименование проверки
    /// </summary>
    public string CheckName => "DataBaseCheck";

    private readonly DataBaseSettings? _dbSettings;

    public DataBaseCheck() { }

    public DataBaseCheck(DataBaseSettings? dbSettings)
    {
        _dbSettings = dbSettings;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (_dbSettings?.DataConnection is null)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Отсутствует контекст для работы с базой данных"));
        }

        try
        {
            // Создаём хелпер для работой с базой данных
            using var dbHelper = new DataBaseHelper(_dbSettings.DataConnection);

            // Создаём билдер для проверок БД
            var dbCheckBuilder = new DataBaseCheckBuilder();

            // Проверяем подключение к базе данных
            var isOpenConnectionDB = dbHelper.OpenConnectionDB(out var errorMessage);
            dbCheckBuilder.AddCheckConnectionDB(isOpenConnectionDB, errorMessage);

            // Если подключение было установлено, то нужно проверить таблицы
            if (isOpenConnectionDB && _dbSettings.TablesSettings != null && _dbSettings.TablesSettings.Any())
            {
                // Получаем список таблиц
                var tablesInfo = dbHelper.GetTableStatistics(dbHelper.DataConnection, out errorMessage);

                // Проверяем доступ до таблиц
                dbHelper.AddTablesAccess(dbHelper.DataConnection, tablesInfo, out errorMessage);

                // Проверяем есть ли среди таблицы нужные таблицы
                dbCheckBuilder.AddCheckTablesInDB(tablesInfo, _dbSettings.TablesFullNames);

                foreach (var tableSettings in _dbSettings.TablesSettings)
                {
                    var tableInfo = tablesInfo.Where(tableInfo => tableInfo.FullName == tableSettings.TableFullName).FirstOrDefault();

                    if (tableSettings.IsNeedCheckConnectionTable || tableInfo == null)
                    {
                        dbCheckBuilder.AddCheckConnection(tableInfo, tableSettings.TableFullName);
                    }

                    if (tableSettings.IsNeedCheckFillingTable && tableInfo != null && (tableSettings.IsNeedCheckConnectionTable && tableInfo.IsAccess || !tableSettings.IsNeedCheckConnectionTable))
                    {
                        dbCheckBuilder.AddCheckFilling(tableInfo, tableSettings.TableFullName);
                    }
                }
            }

            // Не забываем освободить ресурсы
            dbHelper.Dispose();

            // Получаем результаты проверки
            var data = dbCheckBuilder.Build();

            var partial = false;
            var error = false;
            foreach (var keyValuePair in data)
            {
                foreach (var value in data.Values)
                {
                    if (value is Check check)
                    {
                        switch (check.Status)
                        {
                            case CheckStatus.Partially:
                                partial = true;
                                break;

                            case CheckStatus.Error:
                                error = true;
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            var result = HealthCheckResult.Healthy("Проверка пройдена", data);

            if (partial && !error)
            {
                result = HealthCheckResult.Degraded("Проверка пройдена не полностью", null, data);
            }

            if (error)
            {
                result = HealthCheckResult.Unhealthy("Проверка не пройдена", null, data);
            }

            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Ошибка при проверке базы данных", ex));
        }
    }
}