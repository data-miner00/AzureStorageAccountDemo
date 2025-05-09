namespace Blobs;

using Microsoft.AspNetCore.StaticFiles;

internal static class FileExtensions
{
    private static readonly FileExtensionContentTypeProvider Provider = new FileExtensionContentTypeProvider();

    public static string GetContentType(this string fileName)
    {
        if (Provider.TryGetContentType(fileName, out var contentType))
        {
            return contentType;
        }

        return "application/octet-stream";
    }
}
