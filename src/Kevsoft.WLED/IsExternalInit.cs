#if NETSTANDARD2_0
namespace System.Runtime.CompilerServices;

using System.ComponentModel;

/// <summary>
/// Polyfill that enables <c>init</c>-only setters and records on netstandard2.0.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
internal static class IsExternalInit
{
}
#endif
