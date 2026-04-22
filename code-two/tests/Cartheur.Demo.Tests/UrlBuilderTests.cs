using SoftAgent.Server;
using Xunit;

namespace Cartheur.Demo.Tests;

public class UrlBuilderTests
{
    [Fact]
    public void UrlPractice_ReturnRates_BuildsExpectedQuery()
    {
        var url = UrlPractice.ReturnRates("EUR_USD", "10", "midpoint", "M1", "0", "UTC");

        Assert.Equal(
            "https://api-fxpractice.oanda.com/v1/candles?instrument=EUR_USD&count=10&candleFormat=midpoint&granularity=M1&dailyAlignment=0&alignmentTimezone=UTC",
            url);
    }

    [Fact]
    public void UrlLive_ReturnAccountInformation_UsesFxTradeHost()
    {
        var url = UrlLive.ReturnAccountInformation(12345);

        Assert.Equal("https://api-fxtrade.oanda.com/v1/accounts/12345", url);
    }

    [Fact]
    public void UrlPractice_PlaceMarketOrder_ReturnsOrdersEndpoint()
    {
        var url = UrlPractice.PlaceMarketOrder(42, "EUR_USD", 100, "buy");

        Assert.Equal("https://api-fxpractice.oanda.com/v1/accounts/42/orders", url);
    }
}
