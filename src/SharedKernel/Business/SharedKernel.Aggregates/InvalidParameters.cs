using System.ComponentModel.DataAnnotations;

namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
///     Invalid params
/// </summary>
public sealed class InvalidParameters : Error
{
    /// <summary>
    ///     Создать <see cref="InvalidValue" />
    /// </summary>
    /// <param name="parameters"></param>
    public InvalidParameters(IEnumerable<Parameter> parameters)
    {
        Parameters = parameters;
    }

    /// <summary>
    ///     Invalid parameters
    /// </summary>
    public IEnumerable<Parameter> Parameters { get; }

    /// <inheritdoc />
    public override string Message => "Invalid params.";

    /// <summary>
    ///     Parameter
    /// </summary>
    public sealed class Parameter
    {
        /// <summary>
        ///     Creates <see cref="Parameter" />
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="name">Name</param>
        /// <param name="path">Path</param>
        /// <param name="message">Error message</param>
        /// <param name="error">Error</param>
        public Parameter(object? value, string name, string path, string message, object? error)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(message);

            Value = value;
            Name = name;
            Path = path;
            Message = message;
            Error = error;
        }

        /// <summary>
        ///     Ошибачное значение
        /// </summary>
        /// <example>email.ru</example>
        public object? Value { get; }

        /// <summary>
        ///     Наименование параметра
        /// </summary>
        /// <example>email</example>
        [Required]
        public string Name { get; }

        /// <summary>
        ///     Путь к параметру
        /// </summary>
        /// <example>userProfile.email</example>
        [Required]
        public string Path { get; }

        /// <summary>
        ///     Сообщение об ошибке
        /// </summary>
        public string Message { get; }

        /// <summary>
        ///     Error
        /// </summary>
        public object? Error { get; }
    }
}