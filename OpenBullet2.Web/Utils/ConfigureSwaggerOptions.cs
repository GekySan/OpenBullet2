﻿using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Xml.XPath;

namespace OpenBullet2.Web.Utils;

internal class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    private readonly IHostEnvironment _hostEnvironment;

    public ConfigureSwaggerOptions(
        IApiVersionDescriptionProvider provider,
        IHostEnvironment hostEnvironment)
    {
        _provider = provider;
        _hostEnvironment = hostEnvironment;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // Add swagger document for every API version discovered
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                CreateVersionInfo(description));
        }

        var xmlDocFile = Path.Combine(AppContext.BaseDirectory, $"{_hostEnvironment.ApplicationName}.xml");
        if (File.Exists(xmlDocFile))
        {
            var comments = new XPathDocument(xmlDocFile);
            options.OperationFilter<XmlCommentsOperationFilter>(comments);
            options.SchemaFilter<XmlCommentsSchemaFilter>(comments);
        }
    }

    public void Configure(string? name, SwaggerGenOptions options) 
        => Configure(options);

    private static OpenApiInfo CreateVersionInfo(
            ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "Heroes API",
            Version = description.ApiVersion.ToString()
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}
