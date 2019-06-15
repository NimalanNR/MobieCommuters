using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MobieCommuters.Common;
using MobieCommuters.Models;
using System.Data.SqlClient;
using System.Configuration;

namespace MobieCommuters.Repos
{
    public class CustomerRepo 
    {
        public Utils _CommonUtility;
        ResultObj<tbl_Customer_Master> _resultObj = null;
        SqlConnection sqlConnection;
        SqlCommand sqlCommand;
        SqlDataReader sqlDataReader;
        tbl_Customer_Master customerObject;
        
        public CustomerRepo()
        {
            _CommonUtility = new Utils();
            LogMessage.strLogFilePath = @"C:\ProgramData\Logs\ErrorLogFile.txt";
            _resultObj = new ResultObj<tbl_Customer_Master>(false, "No Data", null, 0, 1);
        }

        public ResultObj<LoginResult> UserInfo(RequestData data)
        {
            LogMessage.WriteLog("At Customer Repo UserInfo Method");
            LoginResult LoginResult = new LoginResult();
            var connectionString = ConfigurationManager.ConnectionStrings["CharityManagement"].ConnectionString;
            string password = _CommonUtility.Encrypt(data.Password);
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                string command = "select * from [dbo].[tbl_Customer_Master] where ( cust_MobileNo = '" + data.Username +
                    "' or cust_Email = '" + data.Username + "')and (cust_Password = '" + password+"')";
                sqlCommand = new SqlCommand(command, sqlConnection);
                sqlConnection.Open();
               
                sqlDataReader = sqlCommand.ExecuteReader();
                  
                if(sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        customerObject = new tbl_Customer_Master();
                        customerObject.cust_Id = (Guid)sqlDataReader["cust_Id"];
                        if (!string.IsNullOrEmpty(sqlDataReader["cust_FirstName"].ToString()))
                            customerObject.cust_FirstName = sqlDataReader["cust_FirstName"].ToString();

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_LastName"].ToString()))
                            customerObject.cust_LastName = sqlDataReader["cust_LastName"].ToString();

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_MobileNo"].ToString()))
                            customerObject.cust_MobileNo = sqlDataReader["cust_MobileNo"].ToString();

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_FacebookID"].ToString()))
                            customerObject.cust_FacebookID = sqlDataReader["cust_FacebookID"].ToString();

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_GoogleID"].ToString()))
                            customerObject.cust_GoogleID = sqlDataReader["cust_GoogleID"].ToString();

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_TwitterID"].ToString()))
                            customerObject.cust_TwitterID = sqlDataReader["cust_TwitterID"].ToString();

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_IMEI"].ToString()))
                            customerObject.cust_IMEI = sqlDataReader["cust_IMEI"].ToString();

                        if (sqlDataReader["cust_IsActive"] != DBNull.Value)
                            customerObject.cust_IsActive = Convert.ToInt16(sqlDataReader["cust_IsActive"]);

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_Manifacturer"].ToString()))
                            customerObject.cust_Manifacturer = sqlDataReader["cust_Manifacturer"].ToString();

                        if (sqlDataReader["cust_Modifiedby"] != DBNull.Value)
                            customerObject.cust_Modifiedby = (Guid)sqlDataReader["cust_Modifiedby"];

                        if (sqlDataReader["cust_ModifiedDate"] != DBNull.Value)
                            customerObject.cust_ModifiedDate = Convert.ToDateTime(sqlDataReader["cust_ModifiedDate"]);

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_ProfileImage"].ToString()))
                            customerObject.cust_ProfileImage = sqlDataReader["cust_ProfileImage"].ToString();

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_Remarks"].ToString()))
                             customerObject.cust_Remarks = sqlDataReader["cust_Remarks"].ToString();
                        sqlDataReader.NextResult();
                    }
                    LoginResult.UserInfo = customerObject;
                    LoginResult.VehicleTypeList = null;
                }
                else
                {
                    return new ResultObj<LoginResult>(false, "Invalid Username or Password!", null, 0, 0);
                }
                
            }
            catch(Exception ex)
            {
                LogMessage.WriteLog("At Customer Repo UserInfo Method"+ex.Message.ToString());
                return new ResultObj<LoginResult>(false, ex.Message.ToString(), null, 0, 0);
            }
            finally
            {
                sqlDataReader.Close();
                sqlConnection.Close();
            }
             return new ResultObj<LoginResult>(true, "User Logged in Successfully!", LoginResult, 1, 1);
        }

        public ResultObj<tbl_Customer_Master> ForgotPassword(Email email)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["CharityManagement"].ConnectionString;
            string command = "select * from [dbo].[tbl_Customer_Master] where ( cust_Email = '" + email.userEMailId + "')";
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlCommand = new SqlCommand(command, sqlConnection);
                sqlConnection.Open();
                sqlDataReader = sqlCommand.ExecuteReader();
                if (!sqlDataReader.HasRows)
                {
                    _resultObj.Status = false;
                    _resultObj.Message = "OOPS! Your Email Id not Registered";
                    _resultObj.TotalCount = -1;
                    sqlDataReader.Close();
                    return _resultObj;
                }
                else
                {
                    sqlDataReader.Close();
                    string Newpassword = _CommonUtility.RandomString();
                    string encryptedPassword = _CommonUtility.Encrypt(Newpassword);
                    string query = "update [dbo].[tbl_Customer_Master]  set cust_Password = @cust_Password where(cust_Email = @cust_Email)";
                    SqlCommand updateCommand = new SqlCommand(query, sqlConnection);
                    updateCommand.CommandText = query;
                    updateCommand.Parameters.AddWithValue("@cust_Password", encryptedPassword);
                    updateCommand.Parameters.AddWithValue("@cust_Email", email.userEMailId);
                    _CommonUtility.SentMail(email.userEMailId, "Your Password Information from MOBIE", Newpassword);
                    updateCommand.ExecuteNonQuery();
                    _resultObj.Message = "Please Check your Email for New Password!";
                    _resultObj.Status = true;
                    _resultObj.TotalCount = 1;
                    _resultObj.Data = null;
                    return _resultObj;
                }

            }
            catch(Exception ex)
            {
                return new ResultObj<tbl_Customer_Master>(false,ex.Message.ToString(),null,1,1);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public ResultObj<tbl_Customer_Master> Logout(LogoutData data)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["CharityManagement"].ConnectionString;
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                string query = "update [dbo].[tbl_Customer_Master]  set cust_DeviceToken=@cust_DeviceToken where(cust_Id = @cust_Id)";
                sqlConnection.Open();
                sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.CommandText = query;
                sqlCommand.Parameters.AddWithValue("@cust_DeviceToken", string.Empty);
                sqlCommand.Parameters.AddWithValue("@cust_Id", data.Id);
                sqlCommand.ExecuteNonQuery();
                tbl_Customer_Master tbl_Customer_Master = new tbl_Customer_Master();
                tbl_Customer_Master.cust_Id = data.Id;
                return new ResultObj<tbl_Customer_Master>(true, "User Sucessfully Logged Out", tbl_Customer_Master, 1, 1);
            }
            catch(Exception ex)
            {
                return new ResultObj<tbl_Customer_Master>(false,ex.Message.ToString(),null,1,1);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public ResultObj<tbl_Customer_Master> UpdatePassword(PasswordData data)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["CharityManagement"].ConnectionString;
            try
            {
                string newPassword = _CommonUtility.Encrypt(data.cust_Password);
                sqlConnection = new SqlConnection(connectionString);
                string query = "update [dbo].[tbl_Customer_Master]  set cust_Password=@cust_Password where(cust_Id = @cust_Id)";
                sqlConnection.Open();
                sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.CommandText = query;
                sqlCommand.Parameters.AddWithValue("@cust_Id", data.cust_Id);
                sqlCommand.Parameters.AddWithValue("@cust_Password", newPassword);
                sqlCommand.ExecuteNonQuery();
                tbl_Customer_Master tbl_Customer_Master = new tbl_Customer_Master();
                tbl_Customer_Master.cust_Password = _CommonUtility.Decrypt(newPassword);
                tbl_Customer_Master.cust_Id = data.cust_Id;
                return new ResultObj<tbl_Customer_Master>(true,"Password Changed Sucessfully",tbl_Customer_Master,1,1);
            }
            catch(Exception ex)
            {
                return new ResultObj<tbl_Customer_Master>(false,ex.Message.ToString(),null,1,1);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public ResultObj<tbl_Customer_Master> SocialLogin(SocialLoginData data)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["CharityManagement"].ConnectionString;
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                string command = "select * from [dbo].[tbl_Customer_Master] where ( cust_FacebookID = '" + data.SocialID +
                    "' or cust_GoogleID = '" + data.SocialID + "' or cust_TwitterID ='"+data.SocialID + "') and (cust_Email = '" + data.Email + "' " +
                    "and cust_MobileNo = '"+data.MobileNo+"')";
                sqlCommand = new SqlCommand(command, sqlConnection);
                sqlConnection.Open();

                sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                    {
                        customerObject = new tbl_Customer_Master();
                        customerObject.cust_Id = (Guid)sqlDataReader["cust_Id"];
                        if (!string.IsNullOrEmpty(sqlDataReader["cust_FirstName"].ToString()))
                            customerObject.cust_FirstName = sqlDataReader["cust_FirstName"].ToString();

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_LastName"].ToString()))
                            customerObject.cust_LastName = sqlDataReader["cust_LastName"].ToString();

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_MobileNo"].ToString()))
                            customerObject.cust_MobileNo = sqlDataReader["cust_MobileNo"].ToString();

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_FacebookID"].ToString()))
                            customerObject.cust_FacebookID = sqlDataReader["cust_FacebookID"].ToString();

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_GoogleID"].ToString()))
                            customerObject.cust_GoogleID = sqlDataReader["cust_GoogleID"].ToString();

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_TwitterID"].ToString()))
                            customerObject.cust_TwitterID = sqlDataReader["cust_TwitterID"].ToString();

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_IMEI"].ToString()))
                            customerObject.cust_IMEI = sqlDataReader["cust_IMEI"].ToString();

                        if (sqlDataReader["cust_IsActive"] != DBNull.Value)
                            customerObject.cust_IsActive = Convert.ToInt16(sqlDataReader["cust_IsActive"]);

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_Manifacturer"].ToString()))
                            customerObject.cust_Manifacturer = sqlDataReader["cust_Manifacturer"].ToString();

                        if (sqlDataReader["cust_Modifiedby"] != DBNull.Value)
                            customerObject.cust_Modifiedby = (Guid)sqlDataReader["cust_Modifiedby"];

                        if (sqlDataReader["cust_ModifiedDate"] != DBNull.Value)
                            customerObject.cust_ModifiedDate = Convert.ToDateTime(sqlDataReader["cust_ModifiedDate"]);

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_ProfileImage"].ToString()))
                            customerObject.cust_ProfileImage = sqlDataReader["cust_ProfileImage"].ToString();

                        if (!string.IsNullOrEmpty(sqlDataReader["cust_Remarks"].ToString()))
                            customerObject.cust_Remarks = sqlDataReader["cust_Remarks"].ToString();
                        sqlDataReader.NextResult();
                    }
                    _resultObj.Status = true;
                    _resultObj.Message = "User Logged In Sucessfully";
                    _resultObj.Data = customerObject;
                    return _resultObj;
                    
                }
                else
                {
                    return new ResultObj<tbl_Customer_Master>(false, "Invalid Username or Password!", null, 0, 0);
                }

            }
            catch (Exception ex)
            {
                LogMessage.WriteLog("At Customer Repo UserInfo Method" + ex.Message.ToString());
                return new ResultObj<tbl_Customer_Master>(false, ex.Message.ToString(), null, 0, 0);
            }
            finally
            {
                sqlDataReader.Close();
                sqlConnection.Close();
            }
        }

        public ResultObj<tbl_Customer_Master> SocialSignUp(tbl_Customer_Master Userinfo)
        {
            LogMessage.WriteLog("At Customer Repo Social Signup Method");
            var connectionString = ConfigurationManager.ConnectionStrings["CharityManagement"].ConnectionString;
            string command = "select * from [dbo].[tbl_Customer_Master] where ( cust_MobileNo = '" + Userinfo.cust_MobileNo +
                    "' or cust_Email = '" + Userinfo.cust_Email + "')";
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlCommand = new SqlCommand(command, sqlConnection);
                sqlConnection.Open();
                sqlDataReader = sqlCommand.ExecuteReader();
                if (!sqlDataReader.HasRows)
                {
                    _resultObj = UserUpdate(Userinfo);
                    return _resultObj;
                }
                else
                {
                    sqlDataReader.Close();
                    string query = "update [dbo].[tbl_Customer_Master]  set cust_FacebookID=@cust_FacebookID,cust_GoogleID=@cust_GoogleID,cust_TwitterID" +
                                     "cust_TwitterID =cust_TwitterID where(cust_MobileNo = @cust_MobileNo or cust_Email = @cust_Email)";
                    SqlCommand updateCommand = new SqlCommand(query,sqlConnection);
                    updateCommand.CommandText = query;
                    sqlCommand.Parameters.AddWithValue("@cust_MobileNo", Userinfo.cust_MobileNo);
                    sqlCommand.Parameters.AddWithValue("@cust_Email", Userinfo.cust_Email);
                    if (Userinfo.cust_FacebookID == null)
                    {
                        sqlCommand.Parameters.AddWithValue("@cust_FacebookID", DBNull.Value);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@cust_FacebookID", Userinfo.cust_FacebookID);
                    }
                    if (Userinfo.cust_GoogleID == null)
                    {
                        sqlCommand.Parameters.AddWithValue("@cust_GoogleID", DBNull.Value);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@cust_FacebookID", Userinfo.cust_GoogleID);
                    }
                    if (Userinfo.cust_TwitterID == null)
                    {
                        sqlCommand.Parameters.AddWithValue("@cust_TwitterID", DBNull.Value);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@cust_FacebookID", Userinfo.cust_TwitterID);
                    }
                    updateCommand.ExecuteNonQuery();
                    _resultObj.Data = Userinfo;
                    _resultObj.Status = true;
                    _resultObj.Message = "Sucessfully signed up";
                    return _resultObj;
                }

            }
            catch(Exception ex)
            {
                LogMessage.WriteLog("Execption At Customer Repo Social Signup Method"+ex.Message.ToString());
                _resultObj.Status = false;
                _resultObj.Message = ex.Message.ToString();
                return _resultObj;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public ResultObj<tbl_Customer_Master> UserProfileUpdate(tbl_Customer_Master Userinfo, string ProfileBaseString)
        {
            LogMessage.WriteLog("At Customer Repo UserProfileUpdate Method");
            var connectionString = ConfigurationManager.ConnectionStrings["CharityManagement"].ConnectionString;
            string command = "update [dbo].[tbl_Customer_Master]  set cust_ProfileImage=@cust_ProfileImage where(cust_MobileNo " +
                "= @cust_MobileNo or cust_Email = @cust_Email)";
            sqlConnection = new SqlConnection(connectionString);
            sqlCommand = new SqlCommand(command, sqlConnection);
            sqlConnection.Open();
            try
            {
                string ChkPass = _CommonUtility.Encrypt(Userinfo.cust_Password);
                Userinfo.cust_Password = ChkPass;
                if (String.IsNullOrEmpty(Userinfo.cust_ProfileImage))
                {
                    Userinfo.cust_ProfileImage = "http://103.230.39.54/CommutersApp/ProfileImages/noimage.png";
                }
                if (!String.IsNullOrEmpty(ProfileBaseString))
                {
                    Userinfo.cust_ProfileImage = _CommonUtility.DatatoImage(ProfileBaseString, Userinfo.cust_Id.ToString(), Userinfo.cust_Id.ToString());
                }
                sqlCommand.CommandText = command;
                sqlCommand.Parameters.AddWithValue("@cust_ProfileImage",Userinfo.cust_ProfileImage);
                sqlCommand.Parameters.AddWithValue("@cust_MobileNo", Userinfo.cust_MobileNo);
                sqlCommand.Parameters.AddWithValue("@cust_Email", Userinfo.cust_Email);
                sqlCommand.ExecuteNonQuery();
                _resultObj.Status = true;
                _resultObj.Data = Userinfo;
                _resultObj.Data.cust_Password = _CommonUtility.Decrypt(_resultObj.Data.cust_Password);
                _resultObj.Message = "Your Profile Updated Successfully!";
                return _resultObj;
            }
            catch (Exception ex)
            {
                _resultObj.Message = ex.Message.ToString();
                LogMessage.WriteLog("Execption At Customer Repo -> UserProfileUpdate Method" + ex.Message.ToString());
                return _resultObj;
            }
            finally
            {
                sqlConnection.Close();

            }
        }


        public ResultObj<tbl_Customer_Master> UserUpdate(tbl_Customer_Master Userinfo)
        {
            LogMessage.WriteLog("At Customer Repo UserUpdate Method");
            var connectionString = ConfigurationManager.ConnectionStrings["CharityManagement"].ConnectionString;
            string encryptedPassword = _CommonUtility.Encrypt(Userinfo.cust_Password);
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                string command = "select * from [dbo].[tbl_Customer_Master] where ( cust_MobileNo = '" + Userinfo.cust_MobileNo +
                    "' or cust_Email = '" + Userinfo.cust_Email+"')";
                sqlCommand = new SqlCommand(command, sqlConnection);
                string query = null;
                SqlCommand updateUser = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();

                sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.HasRows)
                {
                    if (Userinfo.cust_Password == "")
                    {
                        Userinfo.cust_Modifiedby = Userinfo.cust_Id;
                        Userinfo.cust_ModifiedDate = DateTime.Now;
                        Userinfo.cust_ProfileImage = "http://103.230.39.54/CommutersApp/ProfileImages/noimage.jpg";
                        query = "insert into [MobieCommuters].[dbo].[tbl_Customer_Master] values" +
                             "(@cust_Id,@cust_FirstName,@cust_LastName,@cust_Email,@cust_MobileNo," +
                             "@cust_FacebookID,@cust_GoogleID,@cust_TwitterID,@cust_Gender,@cust_DOB," +
                             "@cust_Password,@cust_IMEI,@cust_Manifacturer,@cust_CreatedDate," +
                             "@cust_ModifiedDate,@cust_DeviceToken, @cust_IsActive,@cust_IsDelted," +
                             "@cust_Remarks,@cust_AddnlRemarks,@cust_ProfileImage,@cust_Createdby, @cust_Modifiedby)";
                        updateUser.CommandText = query;
                        updateUser.Parameters.AddWithValue("@cust_Id", Userinfo.cust_Id);
                        updateUser.Parameters.AddWithValue("@cust_FirstName", Userinfo.cust_FirstName);
                        updateUser.Parameters.AddWithValue("@cust_LastName", Userinfo.cust_LastName);
                        updateUser.Parameters.AddWithValue("@cust_Email", Userinfo.cust_Email);
                        updateUser.Parameters.AddWithValue("@cust_MobileNo", Userinfo.cust_MobileNo);
                        updateUser.Parameters.AddWithValue("@cust_FacebookID", Userinfo.cust_FacebookID);
                        updateUser.Parameters.AddWithValue("@cust_GoogleID", Userinfo.cust_GoogleID);
                        updateUser.Parameters.AddWithValue("@cust_TwitterID", Userinfo.cust_TwitterID);
                        updateUser.Parameters.AddWithValue("@cust_Gender", Userinfo.cust_Gender);
                        updateUser.Parameters.AddWithValue("@cust_DOB", Userinfo.cust_DOB);
                        updateUser.Parameters.AddWithValue("@cust_Password", encryptedPassword);
                        updateUser.Parameters.AddWithValue("@cust_IMEI", Userinfo.cust_IMEI);
                        updateUser.Parameters.AddWithValue("@cust_Manifacturer", Userinfo.cust_Manifacturer);
                        updateUser.Parameters.AddWithValue("@cust_CreatedDate", Userinfo.cust_CreatedDate);
                        updateUser.Parameters.AddWithValue("@cust_ModifiedDate", Userinfo.cust_ModifiedDate);
                        updateUser.Parameters.AddWithValue("@cust_DeviceToken", Userinfo.cust_DeviceToken);
                        updateUser.Parameters.AddWithValue("@cust_IsActive", Userinfo.cust_IsActive);
                        updateUser.Parameters.AddWithValue("@cust_IsDelted", Userinfo.cust_IsDelted);
                        updateUser.Parameters.AddWithValue("@cust_Remarks", Userinfo.cust_Remarks);
                        updateUser.Parameters.AddWithValue("@cust_AddnlRemarks", Userinfo.cust_AddnlRemarks);
                        updateUser.Parameters.AddWithValue("@cust_ProfileImage", Userinfo.cust_ProfileImage);
                        updateUser.Parameters.AddWithValue("@cust_Createdby", Userinfo.cust_Createdby);
                        updateUser.Parameters.AddWithValue("@cust_Modifiedby", Userinfo.cust_Modifiedby);
                        updateUser.ExecuteNonQuery();
                        _resultObj.Message = "Your Profile Updated Successfully!";
                        _resultObj.Data = Userinfo;
                        _resultObj.Status = true;
                        return _resultObj; ;
                    }
                    else
                    {
                        return new ResultObj<tbl_Customer_Master>
                            (false, "You are attempting duplicate record creation. Please create unique Mobile Number/ Email ", null, 0, 0); 
                    }
                }
                else
                {
                    sqlDataReader.Close();
                    Userinfo.cust_Id = Guid.NewGuid();
                    Userinfo.cust_Createdby = Userinfo.cust_Id;
                    Userinfo.cust_ProfileImage = "http://103.230.39.54/CommutersApp/ProfileImages/noimage.jpg";
                    Userinfo.cust_CreatedDate = DateTime.Now;
                    Userinfo.cust_Modifiedby = Userinfo.cust_Id;
                    query = "insert into [MobieCommuters].[dbo].[tbl_Customer_Master] values" +
                            "(@cust_Id,@cust_FirstName,@cust_LastName,@cust_Email,@cust_MobileNo," +
                            "@cust_FacebookID,@cust_GoogleID,@cust_TwitterID,@cust_Gender,@cust_DOB," +
                            "@cust_Password,@cust_IMEI,@cust_Manifacturer,@cust_CreatedDate," +
                            "@cust_ModifiedDate,@cust_DeviceToken, @cust_IsActive,@cust_IsDelted," +
                            "@cust_Remarks,@cust_AddnlRemarks,@cust_ProfileImage,@cust_Createdby, @cust_Modifiedby)";
                    updateUser.CommandText = query;
                    updateUser.Parameters.AddWithValue("@cust_Id", Userinfo.cust_Id);
                    updateUser.Parameters.AddWithValue("@cust_FirstName", Userinfo.cust_FirstName);
                    updateUser.Parameters.AddWithValue("@cust_LastName", Userinfo.cust_LastName);
                    updateUser.Parameters.AddWithValue("@cust_Email", Userinfo.cust_Email);
                    updateUser.Parameters.AddWithValue("@cust_MobileNo", Userinfo.cust_MobileNo);
                    updateUser.Parameters.AddWithValue("@cust_FacebookID", Userinfo.cust_FacebookID);
                    updateUser.Parameters.AddWithValue("@cust_GoogleID", Userinfo.cust_GoogleID);
                    updateUser.Parameters.AddWithValue("@cust_TwitterID", Userinfo.cust_TwitterID);
                    updateUser.Parameters.AddWithValue("@cust_Gender", Userinfo.cust_Gender);
                    updateUser.Parameters.AddWithValue("@cust_DOB", Userinfo.cust_DOB);
                    updateUser.Parameters.AddWithValue("@cust_Password", encryptedPassword);
                    updateUser.Parameters.AddWithValue("@cust_IMEI", Userinfo.cust_IMEI);
                    updateUser.Parameters.AddWithValue("@cust_Manifacturer", Userinfo.cust_Manifacturer);
                    updateUser.Parameters.AddWithValue("@cust_CreatedDate", Userinfo.cust_CreatedDate);
                    updateUser.Parameters.AddWithValue("@cust_ModifiedDate", Userinfo.cust_ModifiedDate);
                    updateUser.Parameters.AddWithValue("@cust_DeviceToken", Userinfo.cust_DeviceToken);
                    updateUser.Parameters.AddWithValue("@cust_IsActive", Userinfo.cust_IsActive);
                    updateUser.Parameters.AddWithValue("@cust_IsDelted", Userinfo.cust_IsDelted);
                    updateUser.Parameters.AddWithValue("@cust_Remarks", Userinfo.cust_Remarks);
                    updateUser.Parameters.AddWithValue("@cust_AddnlRemarks", Userinfo.cust_AddnlRemarks);
                    updateUser.Parameters.AddWithValue("@cust_ProfileImage", Userinfo.cust_ProfileImage);
                    updateUser.Parameters.AddWithValue("@cust_Createdby", Userinfo.cust_Createdby);
                    updateUser.Parameters.AddWithValue("@cust_Modifiedby", Userinfo.cust_Modifiedby);
                    updateUser.ExecuteNonQuery();
                }
                _resultObj.Message = "Your Profile Updated Successfully!";
                _resultObj.Data = Userinfo;
                _resultObj.Status = true;
                return _resultObj;
            }
            catch (Exception ex)
            {
                LogMessage.WriteLog("Execption At Customer Repo User update Method" + ex.Message.ToString());
                _resultObj.Message = ex.Message.ToString();
                return _resultObj;
            }
            finally
            {
                sqlDataReader.Close();
                sqlConnection.Close();
            }
        }
    }
}