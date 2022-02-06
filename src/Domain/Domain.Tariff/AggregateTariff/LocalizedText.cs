namespace Domain.Tariff.AggregateTariff;

public class LocalizedText
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
            throw new ArgumentException("Value can be not empty", nameof(ru));

        Ru = ru;
    }

    private void SetEn(string en)
    {
        if (string.IsNullOrWhiteSpace(en))
            throw new ArgumentException("Value can be not empty", nameof(en));

        En = en;
    }

    private void SetOrigin(string origin)
    {
        if (string.IsNullOrWhiteSpace(origin))
            throw new ArgumentException("Value can be not empty", nameof(origin));

        Origin = origin;
    }
}