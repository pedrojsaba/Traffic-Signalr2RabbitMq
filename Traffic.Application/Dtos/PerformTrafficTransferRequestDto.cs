using System;
using System.Collections.Generic;
using System.Text;

namespace Traffic.Application.Dtos
{
    public class PerformTrafficTransferRequestDto
    {
        public string TrafficId { get; set; }
        public DateTime Register { get; set; }
        public string Plate { get; set; }
        public decimal Speed { get; set; }
        public string Photo { get; set; }
        public string SourceId { get; set; }
    }
}
