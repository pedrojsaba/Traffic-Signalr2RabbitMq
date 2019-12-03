using NServiceBus;

namespace Transactions.Messages.Events
{
    public class TrafficTransferCompletedEvent : IEvent
    {
        public string TrafficId { get; protected set; }

        public TrafficTransferCompletedEvent(string trafficId)
        {
            TrafficId = trafficId;
        }
    }
}
