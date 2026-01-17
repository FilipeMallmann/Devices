using Devices.Domain.Interfaces;
using Devices.Infrastructure.Db;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devices.Infrastructure.Repositorys
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly DevicesDbContext _db;

        public DeviceRepository(DevicesDbContext db)
        {
            _db = db;
        }
    }
}
