using System;
using System.Collections.Generic;
using System.Web.Http;
using MobieCommuters.Common;
using MobieCommuters.Models;
using MobieCommuters.Repos;
namespace MobieCommuters.Controllers
{
    public class CustomerController : ApiController
    {
        CustomerRepo _repoObj;
        public CustomerController()
        {
            _repoObj = new CustomerRepo();
            LogMessage.strLogFilePath = @"C:\ProgramData\Logs\ErrorLogFile.txt";
        }

        [ActionName("ValidateLogin")]
        [HttpPost]
        public ResultObj<LoginResult> UserInfo([FromBody]RequestData data)
        {
            LogMessage.WriteLog("At Customer Controller Validate Login Method");
            return _repoObj.UserInfo(data);
        }

        [Route("api/customer/UserSignUp")]
        [ActionName("UserSignUp")]
        [HttpPost]
        public ResultObj<tbl_Customer_Master> UserSignUp([FromBody]tbl_Customer_Master Userinfo)
        {
            return _repoObj.UserUpdate(Userinfo);
        }

        [Route("api/customer/UserProfileUpdate")]
        [ActionName("UserProfileUpdate")]
        [HttpPost]
        public ResultObj<tbl_Customer_Master> UserProfileUpdate(tbl_Customer_Master Userinfo)
        {

            return _repoObj.UserProfileUpdate(Userinfo, "");
        }


        [Route("api/customer/SocialSignUp")]
        [ActionName("SocialSignUp")]
        [HttpPost]
        public ResultObj<tbl_Customer_Master> SocialSignUp(tbl_Customer_Master Userinfo)
        {
            return _repoObj.SocialSignUp(Userinfo);
        }

        [Route("api/customer/SocialLogin")]
        [ActionName("SocialLogin")]
        [HttpPost]
        public ResultObj<tbl_Customer_Master> SocialLogin([FromBody]SocialLoginData data)
        {
            return _repoObj.SocialLogin(data);
        }

        [Route("api/customer/ChangePassword")]
        [ActionName("ChangePassword")]
        [HttpPost]
        public ResultObj<tbl_Customer_Master> ChangePassword([FromBody]PasswordData data)
        {
            return _repoObj.UpdatePassword(data);
        }

        [Route("api/customer/Logout")]
        [ActionName("Logout")]
        [HttpPost]
        public ResultObj<tbl_Customer_Master> Logout([FromBody]LogoutData data)
        {
            return _repoObj.Logout(data);
        }

        [Route("api/customer/ForgotPassword")]
        [ActionName("ForgotPassword")]
        [HttpPost]
        public ResultObj<tbl_Customer_Master> ForgotPassword([FromBody]Email email)
        {
            return _repoObj.ForgotPassword(email);
        }
    }
}
