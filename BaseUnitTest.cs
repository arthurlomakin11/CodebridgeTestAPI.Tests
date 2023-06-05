using Microsoft.Extensions.DependencyInjection;

namespace CodebridgeTestAPI.Tests;

[Collection("Tests")]
public abstract class BaseUnitTest : IClassFixture<ApiWebApplicationFactory>
{
    protected readonly HttpClient _client;

    protected BaseUnitTest(ApiWebApplicationFactory application)
    {
        _client = application.CreateClient();

        using var scope = application.Server.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var dbContext = scopedServices.GetRequiredService<DogsDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }
}