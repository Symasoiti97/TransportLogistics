using Domain.Tariff.Abstracts;

namespace Domain.Tariff.AggregateTariff;

public class LocalizedText : ValueObject
{
    public string Ru { get; private set; }
    public string En { get; private set; }
    public string Origin { get; private set; }

    public LocalizedText(string ru, string en, string origin)
    {
        SetRu(ru);
        SetEn(en);
        SetOrigin(origin);
    }

    private void SetRu(string ru)
    {
        if (string.IsNullOrWhiteSpace(ru))
            throw new ArgumentException("Value can't be empty", nameof(ru));

        Ru = ru;
    }

    private void SetEn(string en)
    {
        if (string.IsNullOrWhiteSpace(en))
            throw new ArgumentException("Value can't be empty", nameof(en));

        En = en;
    }

    private void SetOrigin(string origin)
    {
        if (string.IsNullOrWhiteSpace(origin))
            throw new ArgumentException("Value can't be empty", nameof(origin));

        Origin = origin;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        return new[] {En, Ru, Origin};
    }
}