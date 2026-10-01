using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace ByteAether.Ulid.AspNetCore.IntegrationTests.OpenApi;

public class SwashbuckleTests
{
	private sealed record UlidResponse(Ulid Id, Ulid? OptionalId);

	[Fact]
	public async Task UlidAndNullableUlidSchemasAreGeneratedAs26CharacterStrings()
	{
		var builder = WebApplication.CreateBuilder();
		builder.WebHost.UseTestServer();
		builder.Services.AddUlidRouteConstraint();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(options => options.SchemaFilter<UlidSchemaFilter>());

		await using var app = builder.Build();
		app.UseSwagger();
		app.MapGet("/ulids/{id:ulid}", (Ulid id) => TypedResults.Ok(new UlidResponse(id, id)));

		await app.StartAsync(TestContext.Current.CancellationToken);
		var client = app.GetTestClient();

		using var response = await client.GetAsync("/swagger/v1/swagger.json", TestContext.Current.CancellationToken);
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var document = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
		var serializedSchema = string.Concat(document.Where(character => !char.IsWhiteSpace(character)));
		Assert.Equal(1, serializedSchema.Split("\"format\":\"ulid\"").Length - 1);
		Assert.Contains("\"maxLength\":26", serializedSchema, StringComparison.Ordinal);
		Assert.Contains("\"optionalId\":", serializedSchema, StringComparison.Ordinal);
	}
}