using LinqToDB.Data;


namespace HealthCheck.Modules.DataBase;

/// <summary>
/// Хелпер для работы с базой данных
/// </summary>
internal sealed class DataBaseHelper : IDisposable
{
    /// <summary>
    /// Делегат контекста базы данных
    /// </summary>
    private readonly Func<DataConnection>? _db;

    /// <summary>
    /// Соединение с базой данных
    /// </summary>
    public DataConnection? DataConnection { get; private set; }

    public DataBaseHelper(Func<DataConnection> db)
    {
        _db = db;
    }

    ~DataBaseHelper()
    {
        Dispose();
    }

    /// <summary>
    /// Метод для установления подключения к базе данных
    /// </summary>
    /// <param name="errorMessage">Сообщение об ошибке, если такая возникла</param>
    /// <returns>Возвращает true, если соединение открыто</returns>
    public bool OpenConnectionDB(out string? errorMessage)
    {
        if (_db == null)
        {
            errorMessage = "Отсутствует информация о контексте базы данных";
            return false;
        }

        try
        {
            DataConnection = _db();

            if (!IsOpenConnection(DataConnection))
            {
                errorMessage = "Не удалось установить соединение с базой данных";
                return false;
            }

            errorMessage = null;
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = $"Не удалось установить соединение с базой данных: {ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// Проверяет открыто ли соединение
    /// </summary>
    public bool IsOpenConnection(DataConnection dataConnection) => dataConnection.Connection.State == System.Data.ConnectionState.Open;

    /// <summary>
    /// Метод для получения информации о таблицах в базе данных
    /// </summary>
    /// <param name="dataConnection">Контекст базы данных</param>
    /// <param name="errorMessage">Сообщение об ошибке, если такая возникла</param>
    /// <returns>Возвращает список таблиц с информацией</returns>
    public IEnumerable<TableInfo>? GetTableStatistics(DataConnection dataConnection, out string? errorMessage)
    {
        if (dataConnection == null)
        {
            errorMessage = "Отсутствует информация о контексте базы данных";
            return null;
        }

        if (!IsOpenConnection(dataConnection))
        {
            errorMessage = "Соединение с базой данных не открыто";
            return null;
        }

        var queryReader = "";
        switch (dataConnection.DataProvider.Name)
        {
            case string provider when provider.Contains("SqlServer"):
                queryReader = "SELECT schemas.name AS 'table_schema', tables.name AS 'table_name', SUM(partitions.rows) AS 'rows_count' FROM sys.tables AS tables INNER JOIN sys.partitions AS partitions ON partitions.OBJECT_ID = tables.OBJECT_ID INNER JOIN sys.schemas ON tables.schema_id = schemas.schema_id GROUP BY schemas.name, tables.name";
                break;

            case string provider when provider.Contains("PostgreSQL"):
                queryReader = "WITH tbl AS (SELECT table_schema, table_name FROM information_schema.tables WHERE table_type = 'BASE TABLE' AND table_schema NOT IN ('pg_catalog', 'information_schema')) SELECT table_schema, table_name, (xpath('/row/c/text()', query_to_xml(format('select count(*) as c from %I.%I', table_schema, TABLE_NAME), FALSE, TRUE, '')))[1]::text::int AS rows_count FROM tbl";
                break;
        }

        if (string.IsNullOrWhiteSpace(queryReader))
        {
            errorMessage = "Не удалось корректно составить запрос на основе провайдера базы данных";
            return null;
        }

        try
        {
            using var reader = dataConnection.ExecuteReader(queryReader);

            var result = new List<TableInfo>();

            while (reader.Reader?.Read() == true)
            {
                var tableSchema = reader.Reader[0]?.ToString() ?? "";
                var tableName = reader.Reader[1]?.ToString() ?? "";
                int.TryParse(reader.Reader[2]?.ToString(), out var rowsCount);

                result.Add(new TableInfo(tableSchema, tableName, rowsCount));
            }

            reader.Reader?.Close();
            reader.Dispose();

            errorMessage = null;
            return result;
        }
        catch (Exception ex)
        {
            errorMessage = $"Ошибка при проверке наличия таблиц в базе данных: {ex.Message}";
            return null;
        }
    }

    /// <summary>
    /// Метод для проверки правд доступа до таблицы
    /// </summary>
    /// <param name="dataConnection">Контекст базы данных</param>
    /// <param name="tablesInfo">Информация о таблицах в БД</param>
    /// <param name="errorMessage">Сообщение об ошибке, если такая возникла</param>
    public bool AddTablesAccess(DataConnection dataConnection, IEnumerable<TableInfo> tablesInfo, out string? errorMessage)
    {
        if (dataConnection == null)
        {
            errorMessage = "Отсутствует информация о контексте базы данных";
            return false;
        }

        if (!IsOpenConnection(dataConnection))
        {
            errorMessage = "Соединение с базой данных не открыто";
            return false;
        }

        // Формируем запрос к БД
        var tablesQuery = Array.Empty<string>();
        switch (dataConnection.DataProvider.Name)
        {
            case string provider when provider.Contains("SqlServer"):
                tablesQuery = tablesInfo.Select(tableInfo => $"SELECT entity_name as table_full_name FROM fn_my_permissions('{tableInfo.FullName}', 'OBJECT') WHERE subentity_name = '' AND permission_name = 'SELECT'").ToArray();
                break;

            case string provider when provider.Contains("PostgreSQL"):
                tablesQuery = tablesInfo.Select(tableInfo => $"SELECT table_schema || '.' || table_name as table_full_name FROM information_schema.role_table_grants WHERE table_schema='{tableInfo.TableSchema}' AND table_name='{tableInfo.TableName}' AND privilege_type = 'SELECT'").ToArray();
                break;
        }

        var queryReader = string.Join(" UNION ", tablesQuery);

        if (string.IsNullOrWhiteSpace(queryReader))
        {
            errorMessage = "Не удалось корректно составить запрос на основе провайдера базы данных";
            return false;
        }

        try
        {
            using var reader = dataConnection.ExecuteReader(queryReader);

            var tablesNames = new List<string>();

            while (reader.Reader?.Read() == true)
            {
                var tableFullName = reader.Reader[0]?.ToString() ?? "";
                tablesNames.Add(tableFullName);
            }

            reader.Reader?.Close();
            reader.Dispose();

            foreach (var tableInfo in tablesInfo)
            {
                tableInfo.IsAccess = tablesNames.Contains(tableInfo.FullName);
            }

            errorMessage = null;
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = $"Ошибка при проверке доступа к таблице в базе данных: {ex.Message}";
            return false;
        }
    }


    #region IDisposable

    /// <summary>
    /// Освобождение ресурсов
    /// </summary>
    public void Dispose()
    {
        if (DataConnection != null)
        {
            DataConnection.Dispose();
            DataConnection = null;
        }
    }

    #endregion IDisposable
}