using NServiceBus;

namespace Transactions.Messages.Events
{
    public class TrafficTransferRejectedEvent : IEvent
    {
        public string TrafficId { get; protected set; }

        public TrafficTransferRejectedEvent(string trafficId)
        {
            TrafficId = trafficId;
        }
    }
}
