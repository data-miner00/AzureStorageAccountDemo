namespace Service.Results;

using System.Text;

public class HtmlResult : IResult
{
    private readonly string html;

    public HtmlResult(string html)
    {
        this.html = html;
    }

    public async Task ExecuteAsync(HttpContext context)
    {
        context.Response.ContentType = "text/html";
        context.Response.ContentLength = Encoding.UTF8.GetByteCount(this.html);
        await context.Response.WriteAsync(this.html);
    }
}
