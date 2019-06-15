using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobieCommuters.Models
{
    public class LoginResult
    {
        // public tbl_Customer_Master UserInfo { get; set; }
        public tbl_Customer_Master UserInfo { get; set; }
        public List<VehicleType> VehicleTypeList { get; set; }
    }
    public class VehicleType
    {
        public int TypeID { get; set; }
        public string Description { get; set; }
        public Guid VehicleRefTransID { get; set; }
    }
}