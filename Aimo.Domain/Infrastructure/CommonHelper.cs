#nullable disable
#nullable enable annotations
using Aimo.Core.Infrastructure;

namespace Aimo.Domain.Infrastructure;

/// <summary>
/// Represents a common helper
/// </summary>
public partial class CommonHelper //TODO: do we really need this class?
{
    #region Properties

    /// <summary>
    /// Gets or sets the default file provider
    /// </summary>
    public static IAppFileProvider DefaultFileProvider { get; set; }

    #endregion
}