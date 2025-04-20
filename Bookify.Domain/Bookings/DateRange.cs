namespace Bookify.Domain.Bookings;

public record DateRange
{
	private DateRange()
	{
	}

	public DateOnly Start { get; init; }
	public DateOnly End { get; init; }

	// Helper property calculates what the length in days of date range is
	public int LengthInDays => End.DayNumber - Start.DayNumber;

	// Create method is used to create a new date range instance and its defensive
	// because it ensures that the start of the range can never be greater than the end  of the range
	// in which case it's going to throw an application exception.
	public static DateRange Create(DateOnly start, DateOnly end)
	{
		if (start > end) throw new ApplicationException("End date precedes start date");

		return new DateRange
			   {
				   Start = start,
				   End = end
			   };
	}
}