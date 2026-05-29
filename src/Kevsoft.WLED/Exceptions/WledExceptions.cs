namespace Kevsoft.WLED;

/// <summary>
/// Base type for all errors raised by the WLED client.
/// </summary>
public class WledException : Exception
{
    /// <summary>Creates a new <see cref="WledException"/>.</summary>
    public WledException(string message) : base(message)
    {
    }

    /// <summary>Creates a new <see cref="WledException"/> wrapping an inner exception.</summary>
    public WledException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Raised when the device could not be reached (transport failure, DNS, timeout, refused connection).
/// </summary>
public sealed class WledConnectionException : WledException
{
    /// <summary>Creates a new <see cref="WledConnectionException"/>.</summary>
    public WledConnectionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Raised when the device returned a non-success (non-2xx) HTTP response.
/// </summary>
public sealed class WledResponseException : WledException
{
    /// <summary>Creates a new <see cref="WledResponseException"/>.</summary>
    public WledResponseException(int statusCode, string? body)
        : base($"The WLED device responded with HTTP status {statusCode}.")
    {
        StatusCode = statusCode;
        Body = body;
    }

    /// <summary>The HTTP status code returned by the device.</summary>
    public int StatusCode { get; }

    /// <summary>The response body, if any.</summary>
    public string? Body { get; }
}

/// <summary>
/// Raised when the device firmware is older than the minimum version the client supports.
/// </summary>
public sealed class WledUnsupportedVersionException : WledException
{
    /// <summary>Creates a new <see cref="WledUnsupportedVersionException"/>.</summary>
    public WledUnsupportedVersionException(string version, string minimumVersion)
        : base($"The WLED device firmware version '{version}' is older than the minimum supported version '{minimumVersion}'.")
    {
        Version = version;
        MinimumVersion = minimumVersion;
    }

    /// <summary>The firmware version reported by the device.</summary>
    public string Version { get; }

    /// <summary>The minimum firmware version supported by the client.</summary>
    public string MinimumVersion { get; }
}
