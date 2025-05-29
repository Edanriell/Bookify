using Bookify.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
	public void Configure ( EntityTypeBuilder<OutboxMessage> builder )
	{
		builder.ToTable (
				name : "outbox_messages"
			);

		builder.HasKey (
				keyExpression : outboxMessage => outboxMessage.Id
			);

		builder.Property (
					propertyExpression : outboxMessage => outboxMessage.Content
				).
			HasColumnType (
					typeName : "jsonb"
				);
	}
}
