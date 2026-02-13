namespace Blobs;

using Microsoft.AspNetCore.StaticFiles;

/// <summary>
/// The string extensions.
/// </summary>
internal static class StringExtensions
{
    private static readonly FileExtensionContentTypeProvider Provider = new();

    /// <summary>
    /// Gets the content type of a file based on its extension.
    /// </summary>
    /// <param name="fileName">The file name including extension.</param>
    /// <returns>The content type.</returns>
    public static string GetContentType(this string fileName)
    {
        if (Provider.TryGetContentType(fileName, out var contentType))
        {
            return contentType;
        }

        return "application/octet-stream";
    }
}
