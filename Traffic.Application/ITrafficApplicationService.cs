using System;
using System.Threading.Tasks;
using Traffic.Application.Dtos;

namespace Traffic.Application
{
    public interface ITrafficApplicationService
    {
        Task<PerformTrafficTransferResponseDto> PerformTransfer(PerformTrafficTransferRequestDto dto);
    }
}
