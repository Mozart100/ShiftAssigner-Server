using System.IO;
using System.Linq;

namespace ShiftAssignerServer.Tests.Infrastructure;

public class PathLocator
{
    public static string Combine(string domain,params string[] segments)
    {
        var path = domain;
        if (segments is not null && segments.Length > 0)
        {
            var parts = new[] { path }.Concat(segments).ToArray();
            path = Path.Combine(parts);
        }

        return path;
    }
}
