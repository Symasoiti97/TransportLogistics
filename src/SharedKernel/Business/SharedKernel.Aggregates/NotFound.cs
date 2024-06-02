namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Ошибка, ресурс не найден
/// </summary>
public class NotFound : Error
{
    /// <summary>
    /// Создать <see cref="NotFound" />
    /// </summary>
    /// <param name="details"></param>
    public NotFound(string details) : base(details)
    {
    }

    /// <inheritdoc />
    public override string Message => "Not found.";
}