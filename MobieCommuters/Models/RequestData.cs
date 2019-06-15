using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobieCommuters.Models
{
    public class RequestData
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string DeviceToken { get; set; }
        public string DeviceType { get; set; }
    }
}