using System.ComponentModel.DataAnnotations;

namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Invalid params
/// </summary>
public sealed class InvalidParameters : Error
{
    /// <summary>
    /// Invalid parameters
    /// </summary>
    public IEnumerable<Parameter> Parameters { get; }

    public override string Message => "Invalid params.";

    public InvalidParameters(IEnumerable<Parameter> parameters)
    {
        Parameters = parameters;
    }

    /// <summary>
    /// Parameter
    /// </summary>
    public sealed class Parameter
    {
        /// <summary>
        /// Ошибачное значение
        /// </summary>
        /// <example>email.ru</example>
        public object? Value { get; }

        /// <summary>
        /// Наименование параметра
        /// </summary>
        /// <example>email</example>
        public string? Name { get; }

        /// <summary>
        /// Путь к параметру
        /// </summary>
        /// <example>userProfile.email</example>
        [Required]
        public string Path { get; }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Error
        /// </summary>
        public object? Error { get; }

        public Parameter(object? value, string? name, string path, string message, object? error)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(message);

            Value = value;
            Name = name;
            Path = path;
            Message = message;
            Error = error;
        }
    }
}
