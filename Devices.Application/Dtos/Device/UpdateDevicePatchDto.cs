using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using static Devices.Domain.Enums;

namespace Devices.Application.Dtos.Device
{
    public class UpdateDevicePatchDto
    {
        public string? Name { get; set; }
        public string? Brand { get; set; }

        public DeviceState? State { get; set; }
    }
}
