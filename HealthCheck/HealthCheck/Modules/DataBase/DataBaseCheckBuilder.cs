using HealthCheck.Models;


namespace HealthCheck.Modules.DataBase;

/// <summary>
/// Строитель для проверок базы данных
/// </summary>
internal sealed class DataBaseCheckBuilder
{
    /// <summary>
    /// Результаты проверки
    /// </summary>
    private Dictionary<string, object> _data = new Dictionary<string, object>();

    /// <summary>
    /// Проверка подключения к базе данных
    /// </summary>
    /// <param name="isOpenConnection">Открыто ли соединение</param>
    /// <param name="errorMessage">Сообщение об ошибке</param>
    /// <returns>Возвращает true, если соединение установлено</returns>
    public void AddCheckConnectionDB(bool isOpenConnection, string? errorMessage)
    {
        var checkBuilderConnection = Check.GetBuilder()
            .SetName("Установка соединения с базой данных")
            .StartAt(DateTime.Now);

        if (isOpenConnection)
        {
            checkBuilderConnection.Passed("Проверка пройдена");
        }
        else
        {
            checkBuilderConnection.Error("Не удалось установить соединение с базой данных", errorMessage);
        }

        var checkConnectionDB = checkBuilderConnection
            .EndAt(DateTime.Now)
            .Build();

        _data.Add("Подключение к базе данных", checkConnectionDB);
    }

    /// <summary>
    /// Проверка наличия таблицы в базе данных
    /// </summary>
    /// <param name="tablesInfo">Информация о таблицах из БД</param>
    /// <param name="tablesName">Наименования таблиц</param>
    public void AddCheckTablesInDB(IEnumerable<TableInfo> tablesInfo, string[]? tablesName)
    {
        if (tablesName == null || !tablesName.Any())
        {
            return;
        }

        var checkBuilderTable = Check.GetBuilder()
            .SetName($"Проверка наличия таблиц в базе данных")
            .StartAt(DateTime.Now);

        var currentTablesNames = tablesInfo.Select(tableInfo => tableInfo.FullName).ToArray();
        var except = tablesName.Except(currentTablesNames).ToArray();

        if (!except.Any())
        {
            checkBuilderTable.Passed("Проверка пройдена");
        }
        else
        {
            var tables = string.Join(",", except);
            checkBuilderTable.Error($"Таблицы не обнаружены", $"Не найдены таблицы: {tables}");
        }

        var check = checkBuilderTable
            .EndAt(DateTime.Now)
            .Build();

        _data.Add($"Проверка наличия таблиц в базе данных", check);
    }

    /// <summary>
    /// Проверка наличия доступа к таблице в базе данных
    /// </summary>
    /// <param name="tableInfo">Информация о таблице в БД</param>
    /// <param name="tableFullName">Полное наименование таблицы</param>
    public void AddCheckConnection(TableInfo? tableInfo, string tableFullName)
    {
        var checkBuilderTable = Check.GetBuilder()
            .SetName($"Проверка на доступ к {tableFullName}")
            .StartAt(DateTime.Now);

        if (tableInfo?.IsAccess == true)
        {
            checkBuilderTable.Passed("Проверка пройдена");
        }
        else
        {
            checkBuilderTable.Error($"Нет прав доступа", $"Нет прав доступа к таблице {tableFullName}");
        }

        var check = checkBuilderTable
            .EndAt(DateTime.Now)
            .Build();

        var isSuccess = _data.TryAdd($"Проверка наличия доступа к таблице {tableFullName}", check);

        if (!isSuccess)
        {
            var checkError = Check.GetBuilder()
                .SetName($"Проверка на доступ к {tableFullName}")
                .Error("Проверка не удалась", $"Результаты проверки доступа к таблице {tableFullName} уже имеются в результирующих данных")
                .Build();

            _data.Add($"Не удалось добавить результат наличия доступа к таблицы {tableFullName} - {DateTime.Now}", checkError);
        }
    }

    /// <summary>
    /// Наличие данных в таблице
    /// </summary>
    /// <remarks>
    /// Если в таблице отсутствует информация, то ставится пометка о частичным прохождении
    /// </remarks>
    /// <param name="tableInfo">Информация о таблице в БД</param>
    /// <param name="tableFullName">Полное наименование таблицы</param>
    public void AddCheckFilling(TableInfo? tableInfo, string tableFullName)
    {
        var checkBuilderTable = Check.GetBuilder()
            .SetName($"Проверка заполненности таблицы {tableFullName}")
            .StartAt(DateTime.Now);

        if (tableInfo?.RowsCount >= 1)
        {
            checkBuilderTable.Passed($"Проверка пройдена, количество записей: {tableInfo.RowsCount}");
        }
        else
        {
            checkBuilderTable.Partially($"Таблица пустая", $"В таблице {tableFullName} отсутствуют данные");
        }

        var check = checkBuilderTable
            .EndAt(DateTime.Now)
            .Build();

        var isSuccess = _data.TryAdd($"Заполненность таблицы {tableFullName}", check);

        if (!isSuccess)
        {
            var checkError = Check.GetBuilder()
                .SetName($"Проверка заполненности таблицы {tableFullName}")
                .Error("Проверка не удалась", $"Результаты проверки заполненности таблицы {tableFullName} уже имеются в результирующих данных")
                .Build();

            _data.Add($"Не удалось добавить результат заполненности таблицы {tableFullName} - {DateTime.Now}", checkError);
        }
    }

    /// <summary>
    /// Выдача результатов проверок
    /// </summary>
    public Dictionary<string, object> Build()
    {
        return _data;
    }
}