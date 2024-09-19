using System.Reflection;

[assembly: AssemblyTitle(BuildInfo.Name)]
[assembly: AssemblyCopyright($"Created by {BuildInfo.Author}")]

[assembly: AssemblyVersion(BuildInfo.Version)]
[assembly: AssemblyFileVersion(BuildInfo.Version)]

internal static class BuildInfo
{
    internal const string Name = "Sky Co-op Server";
    internal const string Author = "Filigrani & REDcat";
    internal const string Version = "1.0.0";
}