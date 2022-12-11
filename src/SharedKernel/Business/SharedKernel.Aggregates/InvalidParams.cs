namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Invalid params
/// </summary>
public sealed class InvalidParams : Error
{
    /// <summary>
    /// Invalid parameters
    /// </summary>
    public IEnumerable<Param> Params { get; }

    /// <inheritdoc />
    public override string Message => "Invalid params.";

    /// <summary>
    /// Создать <see cref="InvalidValue"/>
    /// </summary>
    /// <param name="params"></param>
    public InvalidParams(IEnumerable<Param> @params)
    {
        Params = @params;
    }
}