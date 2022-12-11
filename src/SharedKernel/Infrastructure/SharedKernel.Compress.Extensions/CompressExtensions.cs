using System.IO.Compression;
using System.Text;

namespace TL.SharedKernel.Infrastructure.Compress.Extensions;

public static class CompressExtensions
{
    /// <summary>
    /// String compress
    /// </summary>
    /// <param name="stringToCompress">Any string</param>
    /// <returns>Compressed string</returns>
    public static string Compress(this string stringToCompress)
    {
        var bytesToCompress = Encoding.UTF8.GetBytes(stringToCompress);
        var outputStream = new MemoryStream();
        using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
        {
            gzipStream.Write(bytesToCompress, 0, bytesToCompress.Length);
        }

        return Convert.ToBase64String(outputStream.ToArray());
    }

    /// <summary>
    /// String decompress
    /// </summary>
    /// <param name="stringToDecompress">Compressed string</param>
    /// <returns>Decompressed string</returns>
    public static string Decompress(this string stringToDecompress)
    {
        var bytesToDecompress = Convert.FromBase64String(stringToDecompress);
        var inputStream = new MemoryStream(bytesToDecompress);
        var outputStream = new MemoryStream();
        using (var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
        {
            gZipStream.CopyTo(outputStream);
        }

        return Encoding.UTF8.GetString(outputStream.ToArray());
    }
}