using NServiceBus;

namespace Transactions.Messages.Commands
{
    public class RejectTrafficTransferCommand : ICommand
    {
        public string TrafficId { get; private set; }

        public RejectTrafficTransferCommand(string trafficId)
        {
            TrafficId = trafficId;
        }
    }
}
