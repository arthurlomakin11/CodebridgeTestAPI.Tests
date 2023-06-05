using System.Net.Http.Json;
using System.Text.Json;
using CodebridgeTestAPI.Features;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;

namespace CodebridgeTestAPI.Tests;

public class SelectPagedAndOrderedDogsUnitTest: BaseUnitTest
{
    public SelectPagedAndOrderedDogsUnitTest(ApiWebApplicationFactory application): base(application) {}

    [Fact]
    public async Task Select()
    {
        var json = JsonSerializer.Serialize(new
        {
            attribute = "name",
            order = OrderType.Desc.ToString(),
            pageNumber = "1",
            pageSize = "2"
        });
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        
        var dogs = await _client.GetFromJsonAsync<Dog[]>(QueryHelpers.AddQueryString("dogs", dictionary!));

        var expectedDog = SampleData.DogsList
            .OrderByDescending(d => d.Name)
            .Skip(0)
            .Take(2);

        dogs.Should().BeEquivalentTo(expectedDog);
    }
    
    [Fact]
    public async Task SelectWithLimit()
    {
        var json = JsonSerializer.Serialize(new
        {
            attribute = "name",
            order = OrderType.Desc.ToString(),
            limit = "1",
            pageNumber = "1",
            pageSize = "2"
        });
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        
        var dogs = await _client.GetFromJsonAsync<Dog[]>(QueryHelpers.AddQueryString("dogs", dictionary!));

        var expectedDog = SampleData.DogsList
            .OrderBy(d => d.Id)
            .Skip(0)
            .Take(1);

        dogs.Should().BeEquivalentTo(expectedDog);
    }

    [Fact]
    public async Task SelectWithInvalidAttribute()
    {
        var json = JsonSerializer.Serialize(new
        {
            attribute = "error",
            order = OrderType.Desc.ToString(),
            limit = "1",
            pageNumber = "1",
            pageSize = "2"
        });
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        
        var response = await _client.GetAsync(QueryHelpers.AddQueryString("dogs", dictionary!));
        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var error = await JsonSerializer.DeserializeAsync<ProblemDetailsWithErrors>(await response.Content.ReadAsStreamAsync(), jsonOptions);
        
        error!.Errors.Should().ContainKey("attribute");
    }
    
    [Fact]
    public async Task SelectWithInvalidPageNumber()
    {
        var json = JsonSerializer.Serialize(new
        {
            attribute = "error",
            order = OrderType.Desc.ToString(),
            limit = "1",
            pageNumber = "-1",
            pageSize = "2"
        });
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        
        var response = await _client.GetAsync(QueryHelpers.AddQueryString("dogs", dictionary!));
        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var error = await JsonSerializer.DeserializeAsync<ProblemDetailsWithErrors>(await response.Content.ReadAsStreamAsync(), jsonOptions);
        
        error!.Errors.Should().ContainKey("pageNumber");
    }
}