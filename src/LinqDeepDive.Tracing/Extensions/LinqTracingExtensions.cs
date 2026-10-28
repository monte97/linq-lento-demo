using System.Diagnostics;

namespace LinqDeepDive.Tracing.Extensions;

public static class LinqTracingExtensions
{
    public static readonly ActivitySource Source = new("LinqDeepDive.Tracing");

    /// <summary>
    /// Wraps an IEnumerable with a span that tracks enumeration start/end.
    /// Useful to visualize when a LINQ pipeline is actually evaluated.
    /// </summary>
    public static IEnumerable<T> Monitor<T>(this IEnumerable<T> source, string operationName)
    {
        using var activity = Source.StartActivity(operationName);
        var count = 0;

        foreach (var item in source)
        {
            count++;
            yield return item;
        }

        activity?.SetTag("linq.element_count", count);
    }

    /// <summary>
    /// Wraps each element yield with a span — useful to see per-element cost.
    /// WARNING: high overhead, use only for small collections or debugging.
    /// </summary>
    public static IEnumerable<T> MonitorElements<T>(this IEnumerable<T> source, string operationName)
    {
        var index = 0;

        foreach (var item in source)
        {
            using var activity = Source.StartActivity($"{operationName}[{index}]");
            activity?.SetTag("linq.element_index", index);
            index++;
            yield return item;
        }
    }
}
