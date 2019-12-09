using NServiceBus;
using System;

namespace Traffic.Messages.Commands
{
    public class RequestTrafficTransferCommand: ICommand
    {
        public int TrafficId { get; protected set; }
        public DateTime Register { get; protected set; }
        public string Plate { get; protected set; }
        public decimal Speed { get; protected set; }
        public string Photo { get; protected set; }
        public string SourceId { get; protected set; }

        public RequestTrafficTransferCommand(int trafficId, DateTime register, string plate, decimal speed, string photo)
        {
            TrafficId = trafficId;
            Register = register;
            Plate = plate;
            Speed = speed;
            Photo = photo;
        }
    }
}
