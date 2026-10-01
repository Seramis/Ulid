#if NET9_0_OR_GREATER
using Microsoft.AspNetCore.OpenApi;
#if NET10_0_OR_GREATER
using Microsoft.OpenApi;
#else
using Microsoft.OpenApi.Models;
#endif

namespace ByteAether.Ulid.AspNetCore;

/// <summary>
/// Represents ULID values as 26-character strings in OpenAPI documents.
/// </summary>
internal sealed class SchemaTransformer : IOpenApiSchemaTransformer
{
	/// <inheritdoc/>
	public Task TransformAsync(
		OpenApiSchema schema,
		OpenApiSchemaTransformerContext context,
		CancellationToken cancellationToken)
	{
		var type = Nullable.GetUnderlyingType(context.JsonTypeInfo.Type) ?? context.JsonTypeInfo.Type;
		if (type != typeof(Ulid))
		{
			return Task.CompletedTask;
		}

#if NET10_0_OR_GREATER
		schema.Type = JsonSchemaType.String;
#else
		schema.Type = "string";
#endif
		schema.Format = "ulid";
		schema.MinLength = Ulid.UlidStringLength;
		schema.MaxLength = Ulid.UlidStringLength;
		schema.Properties?.Clear();
		schema.Items = null;

		return Task.CompletedTask;
	}
}
#endif