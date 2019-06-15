using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobieCommuters.Models
{
    public class SocialLoginData
    {
        public string SocialID { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string DeviceToken { get; set; }
        public string DeviceType { get; set; }
    }
}