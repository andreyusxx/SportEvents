using System.Reflection;

namespace SportEvents.Core;

/// <summary>
/// Повертає семантичну версію збірки, задану у Directory.Build.props.
/// </summary>
public static class VersionInfo
{
    /// <summary>
    /// Отримує поточну версію збірки без суфікса хешу коміту.
    /// </summary>
    /// <returns>Рядок семантичної версії у форматі МАЖОР.МІНОР.ПАТЧ.</returns>
    public static string Current()
    {
        Assembly assembly = typeof(VersionInfo).Assembly;
        AssemblyInformationalVersionAttribute? attribute =
            assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        string raw = attribute?.InformationalVersion ?? "0.0.0";
        int plus = raw.IndexOf('+', StringComparison.Ordinal);
        return plus < 0 ? raw : raw[..plus];
    }
}
