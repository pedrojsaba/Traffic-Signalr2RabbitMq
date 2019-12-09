using NServiceBus;

namespace Traffic.Messages.Commands
{
    public class CompleteTrafficTransferCommand: ICommand
    {
        public string TrafficId { get; private set; }

        public CompleteTrafficTransferCommand(string trafficId)
        {
            TrafficId = trafficId;
        }
    }
}
