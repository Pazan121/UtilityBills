using System.Reflection;

namespace UtilityBills.Core;

/// <summary>Версія збірки з Directory.Build.props.</summary>
public static class VersionInfo
{
    /// <summary>Повертає інформаційну версію збірки без службового суфікса після плюса.</summary>
    /// <returns>Версія у форматі МАЖОР.МІНОР.ПАТЧ.</returns>
    public static string Current()
    {
        var assembly = typeof(VersionInfo).Assembly;
        var attribute = assembly.GetCustomAttribute<
            AssemblyInformationalVersionAttribute>();
        var raw = attribute?.InformationalVersion ?? "0.0.0";
        var plus = raw.IndexOf('+');
        return plus < 0 ? raw : raw[..plus];
    }
}
