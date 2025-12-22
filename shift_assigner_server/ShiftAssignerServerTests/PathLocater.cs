using System.IO;
using System.Linq;

namespace ShiftAssignerServer.Tests.Infrastructure;

public class PathLocator
{
    public static string Combine(string domain, params string[] segments)
    {
        var url = domain.TrimEnd('/');
        
        if (segments is not null && segments.Length > 0)
        {
            foreach (var segment in segments)
            {
                if (!string.IsNullOrEmpty(segment))
                {
                    var cleanSegment = segment.Trim('/');
                    if (!string.IsNullOrEmpty(cleanSegment))
                    {
                        url = $"{url}/{cleanSegment}";
                    }
                }
            }
        }

        return url;
    }
}
