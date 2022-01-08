namespace Kevsoft.WLED;

internal sealed class StringContentWithoutCharset : StringContent
{
    /// <summary>
    /// Remove the charset property from header because WLED can't deal with it
    /// </summary>
    /// <param name="content"></param>
    /// <param name="mediaType"></param>
    public StringContentWithoutCharset(string content, string mediaType) : base(content, Encoding.UTF8, mediaType)
    {
        Headers.ContentType.CharSet = "";
    }
}