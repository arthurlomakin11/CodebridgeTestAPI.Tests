using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;

namespace CodebridgeTestAPI.Tests;

public class SelectOrderedDogsUnitTest: BaseUnitTest
{
    public SelectOrderedDogsUnitTest(ApiWebApplicationFactory application): base(application) {}

    [Fact]
    public async Task Select()
    {
        var json = JsonSerializer.Serialize(new
        {
            attribute = "tail_length",
            order = OrderType.Asc.ToString()
        });
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        
        var dogs = await _client.GetFromJsonAsync<Dog[]>(QueryHelpers.AddQueryString("dogs", dictionary!));

        var expectedList = SampleData.DogsList.OrderBy(d => d.TailLength).ToArray();
        
        Assert.Equivalent(dogs, expectedList);
    }
    
    [Fact]
    public async Task SelectDesc()
    {
        var json = JsonSerializer.Serialize(new
        {
            attribute = "tail_length",
            order = OrderType.Desc.ToString()
        });
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        
        var dogs = await _client.GetFromJsonAsync<Dog[]>(QueryHelpers.AddQueryString("dogs", dictionary!));

        var expectedList = SampleData.DogsList.OrderByDescending(d => d.TailLength).ToArray();
        
        Assert.Equivalent(dogs, expectedList);
    }

    [Fact]
    public async Task InvalidSelect()
    {
        var json = JsonSerializer.Serialize(new
        {
            attribute = "error_attribute",
            order = OrderType.Asc.ToString()
        });
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        
        var response = await _client.GetAsync(QueryHelpers.AddQueryString("dogs", dictionary!));
        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var error = await JsonSerializer.DeserializeAsync<ProblemDetailsWithErrors>(await response.Content.ReadAsStreamAsync(), jsonOptions);
        
        error!.Errors.Should().ContainKey("attribute");
    }
}