using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobieCommuters.Models
{
    public class tbl_Customer_Master
    {
        public System.Guid cust_Id { get; set; }
        public string cust_FirstName { get; set; }
        public string cust_LastName { get; set; }
        public string cust_Email { get; set; }
        public string cust_MobileNo { get; set; }
        public string cust_FacebookID { get; set; }
        public string cust_GoogleID { get; set; }
        public string cust_TwitterID { get; set; }
        public Nullable<int> cust_Gender { get; set; }
        public Nullable<System.DateTime> cust_DOB { get; set; }
        public string cust_Password { get; set; }
        public string cust_IMEI { get; set; }
        public string cust_Manifacturer { get; set; }
        public Nullable<System.DateTime> cust_CreatedDate { get; set; }
        public Nullable<System.DateTime> cust_ModifiedDate { get; set; }
        public string cust_DeviceToken { get; set; }
        public Nullable<int> cust_IsActive { get; set; }
        public Nullable<int> cust_IsDelted { get; set; }
        public string cust_Remarks { get; set; }
        public string cust_AddnlRemarks { get; set; }
        public string cust_ProfileImage { get; set; }
        public Nullable<System.Guid> cust_Createdby { get; set; }
        public Nullable<System.Guid> cust_Modifiedby { get; set; }
        public string cust_Language { get; set; }
    }
}