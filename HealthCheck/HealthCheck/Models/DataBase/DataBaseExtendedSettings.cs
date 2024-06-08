namespace HealthCheck.Models.DataBase;

/// <summary>
/// Расширенные настройки проверок для таблиц базы данных
/// </summary>
public sealed class DataBaseExtendedSettings
{
    /// <summary>
    /// Схема таблицы
    /// </summary>
    public string TableSchema { get; private set; }

    /// <summary>
    /// Наименование таблицы
    /// </summary>
    public string TableName { get; private set; }

    /// <summary>
    /// Полное наименование таблицы: схема.наименование_таблицы
    /// </summary>
    public string TableFullName => $"{TableSchema}.{TableName}";

    /// <summary>
    /// Флаг о необходимости проверить соединение с таблицей
    /// </summary>
    public bool IsNeedCheckConnectionTable { get; private set; }

    /// <summary>
    /// Флаг о необходимости проверить заполненность таблицы
    /// </summary>
    public bool IsNeedCheckFillingTable { get; private set; }

    /// <summary>
    /// Конструктор настроек
    /// </summary>
    /// <param name="tableSchema">Схема таблицы</param>
    /// <param name="tableName">Наименование таблицы</param>
    /// <param name="isNeedCheckConnectionTable">Флаг о необходимости проверить соединение с таблицей</param>
    /// <param name="isNeedCheckFillingTable">Флаг о необходимости проверить заполненность таблицы</param>
    /// <param name="isMiniOptimize">Флаг о необходимости оптимизировать запросы к базе данных</param>
    /// <remarks>Если стоит флаг оптимизации и поверка на заполненность, то проверка соединения с таблицей отключается</remarks>
    public DataBaseExtendedSettings(string tableSchema, string tableName, bool isNeedCheckConnectionTable, bool isNeedCheckFillingTable, bool isMiniOptimize = true)
    {
        TableSchema = tableSchema;
        TableName = tableName;
        IsNeedCheckConnectionTable = isNeedCheckConnectionTable;
        IsNeedCheckFillingTable = isNeedCheckFillingTable;

        if (isMiniOptimize && IsNeedCheckFillingTable)
        {
            IsNeedCheckConnectionTable = false;
        }
    }
}