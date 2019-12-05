using System.Collections.Generic;

namespace UpgFisi.Common.Domain
{
    public abstract class AggregateRoot : Entity
    {
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;
        protected virtual void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public virtual void ClearEvents()
        {
            _domainEvents.Clear();
        }
    }
}