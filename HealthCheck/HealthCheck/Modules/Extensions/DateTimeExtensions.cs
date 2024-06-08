using System;


namespace HealthCheck.Modules.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Вывод даты-времени в виде 2023-01-01 13:23:10
    /// </summary>
    /// <param name="dateTime">Исходные дата-время</param>
    public static string ToDateAndTime(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd HH:mm:ss");
}