
using fbapiwsd_api.Domains;
using Microsoft.AspNetCore.Http.Extensions;

namespace fbapiwsd_api.Handlers;
public class CreateHandler<T> : IHandler where T : BaseDomain
{
    IResult IHandler.Handler(HttpContext context)
    {
        T? entity;
        var storage = (Storage<T>?)context.RequestServices.GetServices(typeof(Storage<T>)).First();
        try
        {
            entity = context.Request.ReadFromJsonAsync<T>().Result;
            if (entity != null)
            {
                entity.Id = Guid.NewGuid();
                storage?.Add(entity.Id, entity);
            }
        }
        catch (System.Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
        return Results.Created(new Uri($"{context.Request.GetDisplayUrl()}/{entity?.Id}"), entity);
    }
}
