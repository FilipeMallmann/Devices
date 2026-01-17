using System;
using System.Collections.Generic;
using System.Text;

namespace Devices.Domain
{
    public class DeviceModel
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = default!;
        public string Brand { get; set; } = default!;
        public Enums.DeviceState State { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
