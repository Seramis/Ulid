using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;

namespace ByteAether.Ulid.AspNetCore.IntegrationTests;

public class RouteAndJsonTests
{
	private sealed record UlidPayload(Ulid Id);

	[Fact]
	public async Task RoutesAndJsonWorkWithoutOpenApiPackages()
	{
		var builder = WebApplication.CreateBuilder();
		builder.WebHost.UseTestServer();
		builder.Services.AddUlidRouteConstraint();

		await using var app = builder.Build();
		app.MapPost("/ulids/{id:ulid}", (Ulid id, UlidPayload payload) => TypedResults.Ok(payload.Id == id));

		await app.StartAsync(TestContext.Current.CancellationToken);
		var client = app.GetTestClient();
		var expected = Ulid.New();

		using var validResponse = await client.PostAsJsonAsync(
			$"/ulids/{expected}",
			new UlidPayload(expected),
			TestContext.Current.CancellationToken);
		Assert.Equal(HttpStatusCode.OK, validResponse.StatusCode);
		Assert.True(await validResponse.Content.ReadFromJsonAsync<bool>(TestContext.Current.CancellationToken));

		using var invalidResponse = await client.PostAsJsonAsync(
			"/ulids/not-a-ulid",
			new UlidPayload(expected),
			TestContext.Current.CancellationToken);
		Assert.Equal(HttpStatusCode.NotFound, invalidResponse.StatusCode);
	}
}