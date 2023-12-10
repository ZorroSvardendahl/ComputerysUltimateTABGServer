﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerysUltimateTABGServer.DataTypes.Vehicles
{
    public struct Vehicle()
    {
        public int vehicleID = 0;
        public int vehicleIndex = 0;
        public List<Seat> seats = [];
        public List<DamageableVehiclePart> damageableVehicleParts = [];
    }
}
