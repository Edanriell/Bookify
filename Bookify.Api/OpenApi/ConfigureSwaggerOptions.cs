using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bookify.Api.OpenApi;

public sealed class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
	private readonly IApiVersionDescriptionProvider _provider;

	public ConfigureSwaggerOptions ( IApiVersionDescriptionProvider provider ) { _provider = provider; }

	public void Configure ( SwaggerGenOptions options )
	{
		// Allows us to get access to version descriptions property and
		foreach ( var description in _provider.ApiVersionDescriptions )
			// Add each API version to the Swagger document. 
		{
			options.SwaggerDoc (
					name : description.GroupName,
					info : CreateVersionInfo (
							apiVersionDescription : description
						)
				);
		}
	}

	public void Configure ( string? name, SwaggerGenOptions options )
	{
		Configure (
				options : options
			);
	}

	private static OpenApiInfo CreateVersionInfo ( ApiVersionDescription apiVersionDescription )
	{
		// Creates an OpenAPi info object and return it as an argument for the SwaggerDoc method.
		var openApiInfo = new OpenApiInfo
						  {
							  Title = $"Bookify.Api v{apiVersionDescription.ApiVersion}",
							  Version = apiVersionDescription.ApiVersion.ToString()
						  };

		if ( apiVersionDescription.IsDeprecated )
			openApiInfo.Description += " This API version has been deprecated.";

		return openApiInfo;
	}
}
