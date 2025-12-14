namespace TL.SharedKernel.Business.Aggregates;

public abstract class Event(Guid id) : Entity<Guid>(id);
