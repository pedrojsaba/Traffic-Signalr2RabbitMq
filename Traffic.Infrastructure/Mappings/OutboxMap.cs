using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Traffic.Infrastructure.Mappings
{
    public class OutboxMap : ClassMapping<Outbox>
    {
        public OutboxMap()
        {
            Table("outbox");
            Id(
                idProperty: record => record.MessageId,
                idMapper: mapper =>
                {
                    mapper.Column("message_id");
                    mapper.Generator(Generators.Assigned);
                }
            );
            Property(
                property: record => record.Dispatched,
                mapping: mapper =>
                {
                    mapper.Column("dispatched");
                }
            );
            Property(
                property: record => record.DispatchedAt,
                mapping: mapper =>
                {
                    mapper.Column("dispatched_at");
                }
            );
            Property
            (
                property: record => record.TransportOperations,
                mapping: mapper => {
                    mapper.Column("transport_operations");
                    mapper.Type(NHibernateUtil.StringClob);
                }
            );
        }
    }

}
