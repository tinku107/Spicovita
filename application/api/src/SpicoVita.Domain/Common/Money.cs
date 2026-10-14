namespace SpicoVita.Domain.Common;

public static class Money
{
    public static decimal Round(decimal v) => Math.Round(v, 2, MidpointRounding.AwayFromZero);

    /// <summary>Intra-state GST split (e.g. 5% => 2.5% CGST + 2.5% SGST).</summary>
    public static (decimal Cgst, decimal Sgst) IntraStateGst(decimal taxable, decimal ratePercent)
    {
        var half = Round(taxable * ratePercent / 200m);
        return (half, half);
    }
}
