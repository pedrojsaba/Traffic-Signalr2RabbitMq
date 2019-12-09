using NServiceBus;
using System;
using System.Threading.Tasks;
using Traffic.Application.Dtos;
using Traffic.Messages.Commands;

namespace Traffic.Application
{
    public class TrafficApplicationService : ITrafficApplicationService
    {
        private readonly IMessageSession _messageSession;

        public TrafficApplicationService(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public async Task<PerformTrafficTransferResponseDto> PerformTransfer(PerformTrafficTransferRequestDto dto)
        {
            try
            {
                //var trafficId = Guid.NewGuid().ToString();
                var command = new RequestTrafficTransferCommand(
                    dto.TrafficId,
                    dto.Register,
                    dto.Plate,
                    dto.Speed,
                    dto.Photo
                );
                await _messageSession.Send(command).ConfigureAwait(false);
                return new PerformTrafficTransferResponseDto
                {
                    Response = "OK"
                };
            }
            catch (Exception ex)
            {
                return new PerformTrafficTransferResponseDto
                {
                    Response = "ERROR: " + ex.Message + " -- " + ex.StackTrace
                };
            }
        }
    }
}
