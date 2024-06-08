using HealthCheck.Models.Enum;


namespace HealthCheck.Models;

public partial class Check
{
    /// <summary>
    /// Билдер проверки
    /// </summary>
    public sealed class CheckBuilder
    {
        private readonly Check _check = new();

        /// <summary>
        /// Устанавливает, что проверка успешна
        /// </summary>
        /// <param name="description">Сообщение для клиента</param>
        public CheckBuilder Passed(string? description)
        {
            if (_check._status != CheckStatus.Undefined)
            {
                return this;
            }

            _check._status = CheckStatus.Passed;

            _check.Description = description;

            return this;
        }

        /// <summary>
        /// Устанавливает, что проверка прошла частично
        /// </summary>
        /// <param name="warning">Предупреждение для клиента</param>
        /// <param name="description">Сообщение для клиента</param>
        public CheckBuilder Partially(string? warning = null, string? description = null)
        {
            if (_check._status != CheckStatus.Undefined)
            {
                return this;
            }

            _check._status = CheckStatus.Partially;

            _check.Warning = warning;

            _check.Description = description;

            return this;
        }

        /// <summary>
        /// Устанавливает, что проверка прошла с ошибкой
        /// </summary>
        /// <param name="error">Ошибка для клиента</param>
        /// <param name="description">Сообщение для клиента</param>
        public CheckBuilder Error(string? error = null, string? description = null)
        {
            if (_check._status != CheckStatus.Undefined)
            {
                return this;
            }

            _check._status = CheckStatus.Error;

            _check.Error = error;

            _check.Description = description;

            return this;
        }

        /// <summary>
        /// Устанавливает наименование проверки
        /// </summary>
        /// <param name="name">Наименование</param>
        public CheckBuilder SetName(string? name)
        {
            _check.Name = name;

            return this;
        }

        /// <summary>
        /// Устанавливает сообщение для клиента
        /// </summary>
        /// <param name="description">Описание проверки</param>
        public CheckBuilder SetDescription(string? description)
        {
            _check.Description = description;

            return this;
        }

        /// <summary>
        /// Устанавливает дату-время начала проверки
        /// </summary>
        /// <param name="dateTime">Дата и время начала</param>
        public CheckBuilder StartAt(DateTime dateTime)
        {
            _check._checkStartTime = dateTime;

            return this;
        }

        /// <summary>
        /// Устанавливает дату-время конца проверки
        /// </summary>
        /// <param name="dateTime">Дата и время окончания</param>
        public CheckBuilder EndAt(DateTime dateTime)
        {
            _check._checkEndTime = dateTime;

            return this;
        }

        /// <summary>
        /// Выдача экземпляра результата проверки
        /// </summary>
        public Check Build()
        {
            return _check;
        }
    }
}