namespace Service.Constraints;

public class MaxCountConstraint : IRouteConstraint
{
    private readonly int maxCount;

    public MaxCountConstraint()
        : this(20)
    {
    }

    public MaxCountConstraint(int maxCount)
    {
        this.maxCount = maxCount;
    }

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (!values.ContainsKey(routeKey))
        {
            return false;
        }

        if (values[routeKey] is null)
        {
            return false;
        }

        if (!int.TryParse(values[routeKey].ToString(), out var parsedCount))
        {
            return false;
        }

        return parsedCount <= this.maxCount;
    }
}
