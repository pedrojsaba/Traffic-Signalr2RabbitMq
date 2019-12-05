using System;

namespace Traffic.Domain
{
    public class Traffic
    {
        public virtual int TrafficId { get; protected set; }
        public virtual string Register { get; protected set; }
        public virtual string Plate { get; protected set; }
        public virtual decimal Speed { get; protected set; }
        public virtual string Photo{ get; protected set; }
        public virtual int SourceId { get; protected set; }
        public virtual DateTime OpenedAtUtc { get; protected set; }
        public virtual DateTime UpdatedAtUtc { get; protected set; }
    }
}
