namespace TL.SharedKernel.Business.Aggregates;

public abstract class AggregateRoot<TKey>(TKey id) : Entity<TKey>(id)
    where TKey : struct, IComparable
{
    private readonly List<Event> _events = [];

    public IReadOnlyCollection<Event> Events => _events;

    protected void Raise(Event @event) => _events.Add(@event);
}
