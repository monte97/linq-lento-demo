#if NETFRAMEWORK
// Solo per .NET Framework 4.8 (Task 7 del piano di carico): le due cose che il Domain usa e
// che 4.8 non ha. Il codice dei benchmark resta identico; cambia solo la libreria sotto.

namespace System.Runtime.CompilerServices
{
    /// <summary>Serve al compilatore per i record (proprietà init).</summary>
    internal static class IsExternalInit { }
}

namespace System
{
    /// <summary>
    /// TimeOnly minimo: quanto usano TimeSlot e DataGenerator (costruttore ore/minuti,
    /// confronti, uguaglianza, ToString con formato). Non è il tipo di .NET 6.
    /// </summary>
    public readonly struct TimeOnly : IComparable<TimeOnly>, IEquatable<TimeOnly>, IFormattable
    {
        private readonly long _ticks;

        public TimeOnly(int hour, int minute) => _ticks = new TimeSpan(hour, minute, 0).Ticks;

        public int Hour => (int)(_ticks / TimeSpan.TicksPerHour);
        public int Minute => (int)(_ticks / TimeSpan.TicksPerMinute % 60);

        public int CompareTo(TimeOnly other) => _ticks.CompareTo(other._ticks);
        public bool Equals(TimeOnly other) => _ticks == other._ticks;
        public override bool Equals(object? obj) => obj is TimeOnly t && Equals(t);
        public override int GetHashCode() => _ticks.GetHashCode();

        public static bool operator <(TimeOnly a, TimeOnly b) => a._ticks < b._ticks;
        public static bool operator >(TimeOnly a, TimeOnly b) => a._ticks > b._ticks;
        public static bool operator <=(TimeOnly a, TimeOnly b) => a._ticks <= b._ticks;
        public static bool operator >=(TimeOnly a, TimeOnly b) => a._ticks >= b._ticks;
        public static bool operator ==(TimeOnly a, TimeOnly b) => a._ticks == b._ticks;
        public static bool operator !=(TimeOnly a, TimeOnly b) => a._ticks != b._ticks;

        public string ToString(string? format, IFormatProvider? provider) =>
            new DateTime(_ticks).ToString(format ?? "HH:mm", provider);
        public string ToString(string format) => ToString(format, null);
        public override string ToString() => ToString("HH:mm");
    }
}
#endif
