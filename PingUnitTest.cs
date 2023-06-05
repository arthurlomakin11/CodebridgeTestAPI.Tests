namespace CodebridgeTestAPI.Tests;

[Collection("Tests")]
public class PingUnitTest : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    public PingUnitTest(ApiWebApplicationFactory application)
    {
        _client = application.CreateClient();
    }

    [Fact]
    public async Task TestPingEndpoint()
    {
        var response = await _client.GetStringAsync("/ping");
  
        Assert.Equal("Dogs house service. Version 1.0.1", response);
    }
}