using System.Reflection;

namespace OpenShock.Internal.Common.Utils;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class GitHashAttribute(string hash) : Attribute
{
    private string Hash { get; } = hash;

    public static readonly string FullHash = Assembly.GetEntryAssembly()?.GetCustomAttribute<GitHashAttribute>()?.Hash ?? "error";
}
