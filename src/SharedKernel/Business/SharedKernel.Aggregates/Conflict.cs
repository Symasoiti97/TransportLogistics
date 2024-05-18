namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Ошибка некорректного параметра
/// </summary>
public sealed class Conflict(string details) : Error(details)
{
    /// <inheritdoc />
    public override string Message => "Conflict.";
}