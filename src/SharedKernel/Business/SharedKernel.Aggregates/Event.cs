namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Событие
/// </summary>
/// <param name="id">Идентификатор события</param>
public abstract class Event(Guid id) : Entity<Guid>(id);
