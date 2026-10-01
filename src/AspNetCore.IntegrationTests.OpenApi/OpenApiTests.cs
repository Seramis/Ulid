#if IMPL_NET9_OR_GREATER
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace ByteAether.Ulid.AspNetCore.IntegrationTests.OpenApi;

public class OpenApiTests
{
	private sealed record UlidPayload(Ulid Id, Ulid? OptionalId = null);
	private sealed record UlidResponse(Ulid RouteId, Ulid Id, Ulid? OptionalId);

	[Fact]
	public async Task UlidRouteAndJsonPayloads_RoundTripAndAppearInGeneratedDocument()
	{
		var builder = WebApplication.CreateBuilder();
		builder.WebHost.UseTestServer();
		builder.Services.AddUlidRouteConstraint();
		builder.Services.AddOpenApi(options => options.AddUlidSchemaTransformer());

		await using var app = builder.Build();
		app.MapOpenApi();
		app.MapPost("/ulids/{id:ulid}", (Ulid id, UlidPayload payload) =>
			TypedResults.Ok(new UlidResponse(id, payload.Id, payload.OptionalId)));

		await app.StartAsync(TestContext.Current.CancellationToken);
		var client = app.GetTestClient();

		var expected = Ulid.New();
		using var response = await client.PostAsJsonAsync(
			$"/ulids/{expected}",
			new UlidPayload(expected),
			TestContext.Current.CancellationToken);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var returnedPayload = await response.Content.ReadFromJsonAsync<UlidResponse>(TestContext.Current.CancellationToken);
		Assert.Equal(expected, returnedPayload?.Id);
		Assert.Equal(expected, returnedPayload?.RouteId);
		Assert.Null(returnedPayload?.OptionalId);

		using var invalidRouteResponse = await client.PostAsJsonAsync(
			"/ulids/not-a-ulid",
			new UlidPayload(expected),
			TestContext.Current.CancellationToken);
		Assert.Equal(HttpStatusCode.NotFound, invalidRouteResponse.StatusCode);

		using var openApiResponse = await client.GetAsync("/openapi/v1.json", TestContext.Current.CancellationToken);
		openApiResponse.EnsureSuccessStatusCode();
		using var openApi = JsonDocument.Parse(await openApiResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
		var serializedSchema = string.Concat(openApi.RootElement.GetRawText().Where(character => !char.IsWhiteSpace(character)));
		Assert.Contains("\"format\":\"ulid\"", serializedSchema, StringComparison.Ordinal);
		Assert.Contains("\"maxLength\":26", serializedSchema, StringComparison.Ordinal);
	}
}
#endif