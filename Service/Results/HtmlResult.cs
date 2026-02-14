namespace Service.Results;

using System.Text;

/// <summary>
/// The result object that contains HTML.
/// </summary>
public class HtmlResult : IResult
{
    private readonly string html;

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlResult"/> class.
    /// </summary>
    /// <param name="html">The html string.</param>
    public HtmlResult(string html)
    {
        this.html = html;
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        httpContext.Response.ContentType = "text/html";
        httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(this.html);
        await httpContext.Response.WriteAsync(this.html);
    }
}
