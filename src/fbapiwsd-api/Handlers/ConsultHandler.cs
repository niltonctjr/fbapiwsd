using fbapiwsd_api.Domains;
using Newtonsoft.Json;


namespace fbapiwsd_api.Handlers;
public class ConsultHandler<T> : IHandler where T : BaseDomain
{
    public IResult Handler(HttpContext context)
    {
        QueryParameters? _qp = null;

        if (context.Request.Query != null && context.Request.Query.Any())
            _qp = new QueryParameters(context.Request.Query);

        var storage = (Storage<T>?)context.RequestServices.GetServices(typeof(Storage<T>)).First();
        var result = new
        {
            page = _qp?.Page,
            limit = _qp?.Limit,
            order = _qp?.Order,
            filter = _qp?.FilterObject<T>(),
        };
        var all = storage?.GetAll();
        return Results.Json(all);
    }
}
public class QueryParameters
{
    public int Page { get; private set; } = 0;
    public int Limit { get; private set; } = 50;
    public string Order { get; private set; } = "ASC";
    public string FilterString { get; private set; } = string.Empty;
    public T? FilterObject<T>() => string.IsNullOrEmpty(FilterString) ?
        default :
        JsonConvert.DeserializeObject<T>(FilterString);

    public QueryParameters(IQueryCollection collection)
    {
        if (getQueryString(collection, new string[] { "p", "page" }, out string page))
            Page = Convert.ToInt32(page);
        if (getQueryString(collection, new string[] { "l", "limit" }, out string limit))
            Limit = Convert.ToInt32(limit);
        if (getQueryString(collection, new string[] { "o", "order" }, out string order))
            Order = Convert.ToString(order);
        if (getQueryString(collection, new string[] { "f", "filter" }, out string filtersString))
            FilterString = Convert.ToString(filtersString);
    }

    private bool getQueryString(IQueryCollection collection, string[] keys, out string value)
    {
        value = string.Empty;
        if (collection.Any(q => keys.Contains(q.Key)))
            value = collection.FirstOrDefault(q => keys.Contains(q.Key)).Value.ToString();
        return !string.IsNullOrEmpty(value);
    }

}
