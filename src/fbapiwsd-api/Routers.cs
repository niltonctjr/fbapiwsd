using System.Reflection;
using fbapiwsd_api.Domains;
using fbapiwsd_api.Handlers;
using Microsoft.AspNetCore.Http.HttpResults;

namespace fbapiwsd_api;

public static class Routers
{
    public static WebApplication AddRouters(this WebApplication app)
    {
        var apiPath = "/api";

        var a = Assembly.GetExecutingAssembly();
        var allTypes = a.GetTypes().Where(t => t.BaseType == typeof(BaseDomain));

        app.MapGet("version", () =>
        {
            var result = new
            {
                app_version = a.GetName().Version?.ToString(),
                last_commit = ThisAssembly.Git.Commit,
                last_commit_at = ThisAssembly.Git.CommitDate,
            };

            return Results.Json(result);
        });

        foreach (var type in allTypes)
        {

            var handle = (IHandler)Activator.CreateInstance(typeof(ConsultHandler<>).MakeGenericType(type));
            app.MapGet($"{apiPath}/{type.Name}/", handle.Handler);

            handle = (IHandler)Activator.CreateInstance(typeof(CreateHandler<>).MakeGenericType(type));
            app.MapPost($"{apiPath}/{type.Name}/", handle.Handler);

        }

        return app;
    }

}
