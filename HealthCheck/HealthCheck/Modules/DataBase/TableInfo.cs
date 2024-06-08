namespace HealthCheck.Modules.DataBase;

/// <summary>
/// Информация о таблице
/// </summary>
internal sealed class TableInfo
{
    /// <summary>
    /// Схема таблицы
    /// </summary>
    internal string TableSchema { get; set; }

    /// <summary>
    /// Наименования таблицы
    /// </summary>
    internal string TableName { get; set; }

    /// <summary>
    /// Полное наименование таблицы: схема.наименование_таблицы
    /// </summary>
    internal string FullName => $"{TableSchema}.{TableName}";

    /// <summary>
    /// Количество строк в таблице
    /// </summary>
    internal int RowsCount { get; set; }

    /// <summary>
    /// Есть ли доступ на чтение к таблице
    /// </summary>
    internal bool IsAccess { get; set; }

    internal TableInfo(string tableSchema, string tableName, int rowsCount)
    {
        TableSchema = tableSchema;
        TableName = tableName;
        RowsCount = rowsCount;
    }
}