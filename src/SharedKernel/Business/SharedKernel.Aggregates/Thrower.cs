namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Thrower
/// </summary>
public struct Thrower
{
    /// <summary>
    /// Выбрасывет исключение <see cref="ErrorException"/>
    /// </summary>
    /// <param name="error"></param>
    /// <exception cref="ErrorException"></exception>
    public static void Throw(Error error)
    {
        throw new ErrorException(error);
    }
}