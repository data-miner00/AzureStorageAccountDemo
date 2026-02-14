namespace Service.Constraints;

/// <summary>
/// A constraint to set maximum count for route parameter.
/// </summary>
public class MaxCountConstraint : IRouteConstraint
{
    private readonly int maxCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="MaxCountConstraint"/> class.
    /// Defaults to 20 max count.
    /// </summary>
    public MaxCountConstraint()
        : this(20)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaxCountConstraint"/> class.
    /// </summary>
    /// <param name="maxCount">The max count.</param>
    public MaxCountConstraint(int maxCount)
    {
        this.maxCount = maxCount;
    }

    /// <inheritdoc/>
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
