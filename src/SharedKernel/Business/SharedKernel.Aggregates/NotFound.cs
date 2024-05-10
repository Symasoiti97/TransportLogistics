namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Ошибка, ресурс не найден
/// </summary>
public class NotFound : Error
{
    /// <inheritdoc />
    public override string Message => "Not found.";

    /// <summary>
    /// Создать <see cref="NotFound"/>
    /// </summary>
    /// <param name="details"></param>
    public NotFound(string details) : base(details)
    {
    }
}