using System;
using System.Collections.Generic;
using System.Text;

namespace Traffic.Application.Dtos
{
    public class PerformTrafficTransferRequestDto
    {
        public int TrafficId { get; set; }
        public DateTime Register { get; set; }
        public string Plate { get; set; }
        public decimal Speed { get; set; }
        public string Photo { get; set; }
        public int SourceId { get; set; }
    }
}
