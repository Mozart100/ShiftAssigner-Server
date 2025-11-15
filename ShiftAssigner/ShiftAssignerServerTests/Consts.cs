using System.ComponentModel;
using System.IO;
using System.Linq;

namespace ShiftAssignerServer.Tests.Infrastructure;

public class PathLocator
{
    public const string HttpBaseurl = $"http://localhost:8080/";




    public static string Combine(params string[] args)
    {
        var path = HttpBaseurl;
        if (args is not null && args.Length > 0)
        {
            var parts = new[] { path }.Concat(args).ToArray();
            path = Path.Combine(parts);
        }

        return path;
    }
}
