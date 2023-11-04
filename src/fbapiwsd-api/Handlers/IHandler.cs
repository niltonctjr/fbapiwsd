using Microsoft.AspNetCore.Mvc;

namespace fbapiwsd_api.Handlers;
public interface IHandler
{
    IResult Handler(HttpContext context);
}
