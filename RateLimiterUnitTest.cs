using System.Net;
using FluentAssertions;

namespace CodebridgeTestAPI.Tests;

[Collection("Tests")]
public class RateLimiterUnitTest : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    public RateLimiterUnitTest(ApiWebApplicationFactory application)
    {
        _client = application.CreateClient();
    }

    [Fact]
    public async Task Make10RequestsPerSecond()
    {
        var result = Enumerable.Range(0, 10);
        
        var tasks = result.Select(async x => {
            var response = await _client.GetAsync("dogs");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        });
        
        await Task.WhenAll(tasks);
    }
    
    [Fact]
    public async Task Make11RequestsExpectingError()
    {
        var result = Enumerable.Range(0, 11);
        
        var tasks = result.Select(async number => {
            var response = await _client.GetAsync("dogs");
            if(number == 11) response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        });
        
        await Task.WhenAll(tasks);
    }
}