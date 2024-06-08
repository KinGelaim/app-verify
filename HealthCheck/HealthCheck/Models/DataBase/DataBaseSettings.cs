using LinqToDB.Data;


namespace HealthCheck.Models.DataBase;

/// <summary>
/// Настройки для проверки базы данных
/// </summary>
public sealed class DataBaseSettings
{
    /// <summary>
    /// Делегат контекста базы данных
    /// </summary>
    public Func<DataConnection>? DataConnection { get; private set; }

    /// <summary>
    /// Расширенные настройки проверки таблиц базы данных
    /// </summary>
    internal DataBaseExtendedSettings[]? TablesSettings { get; private set; }

    /// <summary>
    /// Наименование таблиц для проведения проверок
    /// </summary>
    internal string[]? TablesFullNames => TablesSettings?.Select(tableSettings => tableSettings.TableFullName).ToArray();

    /// <summary>
    /// Конструктор настроек проверки базы данных
    /// </summary>
    /// <param name="dataConnection">Делегат контекста базы данных</param>
    public DataBaseSettings(Func<DataConnection>? dataConnection)
    {
        DataConnection = dataConnection;
    }

    /// <summary>
    /// Конструктор настроек проверки базы данных
    /// </summary>
    /// <param name="dataConnection">Делегат контекста базы данных</param>
    /// <param name="tablesInfo">Информация о таблицах</param>
    /// <param name="isMiniOptimize">Флаг о необходимости оптимизировать запросы к базе данных</param>
    public DataBaseSettings(Func<DataConnection>? dataConnection, IEnumerable<(string tableSchema, string tableName)> tablesInfo, bool isMiniOptimize = true) : this(dataConnection)
    {
        TablesSettings = tablesInfo.Select(tableInfo => new DataBaseExtendedSettings(tableInfo.tableSchema, tableInfo.tableName, true, true, isMiniOptimize)).ToArray();
    }

    /// <summary>
    /// Конструктор настроек проверки базы данных
    /// </summary>
    /// <param name="dataConnection">Делегат контекста базы данных</param>
    /// <param name="tablesSettings">Расширенные настройки для проверки таблиц базы данных</param>
    public DataBaseSettings(Func<DataConnection>? dataConnection, DataBaseExtendedSettings[]? tablesSettings) : this(dataConnection)
    {
        TablesSettings = tablesSettings;
    }
}