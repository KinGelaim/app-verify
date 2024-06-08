using System.ComponentModel;


namespace HealthCheck.Models.Enum;

/// <summary>
/// Варианты успешности пройденной проверки
/// </summary>
public enum CheckStatus
{
    [Description("Неопределённый")]
    Undefined = 0,

    [Description("Пройдена")]
    Passed = 1,

    [Description("Пройдена частично")]
    Partially = 2,

    [Description("Не пройдена")]
    Error = 3
}