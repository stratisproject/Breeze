using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Breeze.Api.Models
{
    public class HeartbeatMonitor
    {
        public DateTime LastBeat { get; set; }
        public TimeSpan HeartbeatInterval { get; set; }
    }
}
