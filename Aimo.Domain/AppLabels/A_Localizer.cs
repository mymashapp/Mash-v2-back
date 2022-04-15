using System.Reflection;
using Microsoft.Extensions.Localization;

namespace Aimo.Domain.Labels;

#region Declaration

public partial interface ILocalizable
{
}

public partial interface ILocalizer<T> where T : ILocalizable
{
    string? this[string key] { get; }
    string? this[string key, params object[] args] { get; }
}

#endregion

#region Classes

public partial class App : ILocalizable
{
}

public partial class Validation : ILocalizable
{
}

#endregion

public class Localizer<T> : ILocalizer<T> where T : ILocalizable
{
    private readonly IStringLocalizer _localizer;

    public Localizer(IStringLocalizerFactory factory)
    {
        var typeInfo = typeof(T).GetTypeInfo();
        var assemblyName = new AssemblyName(typeInfo?.Assembly?.FullName ?? string.Empty).Name ?? string.Empty;
        _localizer = factory.Create(typeInfo?.Name /*?.FullName*/ ?? string.Empty, assemblyName);
    }

    public string? this[string key] => _localizer[key];
    public string? this[string key, params object[] args] => _localizer[key, args];
}