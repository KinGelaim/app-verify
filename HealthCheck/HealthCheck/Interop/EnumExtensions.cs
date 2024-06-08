namespace HealthCheck.Interop;

/// <summary>
/// Расширения для перечислений
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Получение атрибута описания <see cref="System.ComponentModel.DescriptionAttribute"/>
    /// </summary>
    /// <param name="enumValue">Значение перечисления</param>
    /// <returns>Описание или null, если значение не было найдено</returns>
    public static string? ToDescription(this Enum enumValue)
    {
        var type = enumValue.GetType();
        var memInfo = type.GetMember(enumValue.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
        return (attributes.Length > 0) ? attributes[0].ToString() : null;
    }
}