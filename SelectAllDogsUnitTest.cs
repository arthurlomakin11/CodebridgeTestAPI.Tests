using System.Net.Http.Json;
using FluentAssertions;

namespace CodebridgeTestAPI.Tests;

public class SelectAllDogsUnitTest: BaseUnitTest
{
    public SelectAllDogsUnitTest(ApiWebApplicationFactory application): base(application) {}

    [Fact]
    public async Task SelectAllDogs()
    {
        var dogs = await _client.GetFromJsonAsync<Dog[]>("dogs");

        SampleData.DogsList.Should().BeEquivalentTo(dogs);
    }
}