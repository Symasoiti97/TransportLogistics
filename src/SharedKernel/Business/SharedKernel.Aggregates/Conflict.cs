namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Ошибка некорректного параметра
/// </summary>
public class Conflict : Error
{
    /// <inheritdoc />
    public override string Message => "Conflict.";
}
