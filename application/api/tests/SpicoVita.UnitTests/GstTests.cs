using SpicoVita.Domain.Common;
namespace SpicoVita.UnitTests;

public class GstTests
{
    [Fact]
    public void FivePercent_IntraState_SplitsEvenly()
    {
        var (c, s) = Money.IntraStateGst(100m, 5m);
        Assert.Equal(2.50m, c);
        Assert.Equal(2.50m, s);
    }

    [Fact]
    public void Rounds_AwayFromZero() => Assert.Equal(0.13m, Money.Round(0.125m));
}
