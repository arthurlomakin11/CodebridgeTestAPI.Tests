using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace CodebridgeTestAPI.Tests;

public class AddDogUnitTest: BaseUnitTest
{
    public AddDogUnitTest(ApiWebApplicationFactory application): base(application) {}

    [Fact]
    public async Task AddDog()
    {
        var response = await _client.PostAsJsonAsync("/dog", new Dog
        {
            Name = "Fox",
            Color = "Orange",
            Weight = 50,
            TailLength = 30
        });
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task AddDogAlreadyExistsError()
    {
        var newDog = new Dog
        {
            Name = "Fox",
            Color = "Orange",
            Weight = 50,
            TailLength = 30
        };
        
        var response = await _client.PostAsJsonAsync("/dog", newDog);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var errorResponse = await _client.PostAsJsonAsync("/dog", newDog);
        
        var errorResponseContent = await errorResponse.Content.ReadAsStringAsync();
        Assert.Equal($"Dog with name {newDog.Name} already exists", errorResponseContent);
    }
    
    [Fact]
    public async Task EmptyModelError()
    {
        var response = await _client.PostAsJsonAsync("/dog", new {});
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task InvalidModelError()
    {
        var newDog = new Dog
        {
            Name = "Fox",
            Color = "Orange",
            Weight = 50,
            TailLength = 0
        };
        
        var response = await _client.PostAsJsonAsync("/dog", newDog);
        
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().Be("{\"TailLength\":[\"The field TailLength must be between 1 and 2147483647.\"]}");
    }
}