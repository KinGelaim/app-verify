using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace HealthCheck.Interfaces;

/// <summary>
/// Именованный HealthCheck
/// </summary>
public interface INamedCheck : IHealthCheck
{
    /// <summary>
    /// Наименование проверки
    /// </summary>
    string CheckName { get; }
}