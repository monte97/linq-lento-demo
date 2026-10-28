namespace LinqDeepDive.Domain.Models;

public record TimeSlot(
    TimeOnly Start,
    TimeOnly End
)
{
    public bool Overlaps(TimeSlot other) =>
        Start < other.End && other.Start < End;

    public override string ToString() => $"{Start:HH:mm}-{End:HH:mm}";
}
