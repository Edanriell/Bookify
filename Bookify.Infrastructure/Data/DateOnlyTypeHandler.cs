using System.Data;
using Dapper;

namespace Bookify.Infrastructure.Data;

// We are using date-only types, in the date range value object, we need to tell dapper
// how to be able to map this type, because it doesn't support it out of the box.
internal sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
	// Methods will be invoked when sending a date value object to the
	// database or materializing one into memory.
	public override DateOnly Parse(object value)
	{
		return DateOnly.FromDateTime((DateTime)value);
	}

	public override void SetValue(IDbDataParameter parameter, DateOnly value)
	{
		parameter.DbType = DbType.Date;
		parameter.Value = value;
	}
}