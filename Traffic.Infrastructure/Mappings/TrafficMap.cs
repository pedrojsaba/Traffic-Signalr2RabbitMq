using FluentNHibernate.Mapping;

namespace Traffic.Infrastructure.Mappings
{
    public class TrafficMap : ClassMap<Domain.Traffic>
    {
        public TrafficMap()
        {
            Id(x => x.TrafficId).Column("traffic_id");
            Map(x => x.Register).Column("register");
            Map(x => x.Speed).Column("speed");
            Map(x => x.Plate).Column("plate");
            Map(x => x.Photo).Column("photo");
            Map(x => x.SourceId).Column("source_id");
            Map(x => x.OpenedAtUtc).Column("opened_at_utc");
            Map(x => x.UpdatedAtUtc).Column("updated_at_utc");
        }
    }
}
