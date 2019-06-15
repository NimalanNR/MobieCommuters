using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using PushSharp;
using PushSharp.Apple;
using PushSharp.Android;

namespace MobieCommuters.Common
{
    public class Utils
    {
        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public bool SentMail(string ToAddress, string Subject, string Message)
        {
            try
            {

                MailMessage Mail = new MailMessage();
                Mail.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FromAddress"].ToString());  // From Mail Address
                Mail.To.Add(new MailAddress(ToAddress)); // To Mail Address
                Mail.Subject = Subject;
                StringBuilder MailBody = new StringBuilder("Your New Mobie Password is   " + "  " + Message);
                //System.Net.Mail.Attachment attachment;
                Mail.Body = MailBody.ToString();
                Mail.IsBodyHtml = true;
                SmtpClient obj = new SmtpClient(System.Configuration.ConfigurationManager.AppSettings["SmtpClient"].ToString(), 587);
                obj.Credentials = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["FromAddress"].ToString(), System.Configuration.ConfigurationManager.AppSettings["MailPassword"].ToString());  // From Mail Address & Password
                obj.Timeout = 20000;
                //attachment = new System.Net.Mail.Attachment(AttcheMentPath);
                //Mail.Attachments.Add(attachment);
                obj.EnableSsl = true;
                obj.SendAsync(Mail, null);
                return true;
            }
            catch (Exception ex)
            {
               
                // Common.ErrorLog("SentMail =>" + ex.InnerException + " => " + ex.Message);
                return false;
            }

        }
        public void SendPushNotification(int DeviceType, string DeviceToken, string Message, string Data, Guid ID)
        {
            var push = new PushBroker();
            if (DeviceType == 1)
            {

                var obj = new { message = Message, data = Data, Id = ID };
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(obj);
                push.RegisterGcmService(new GcmPushChannelSettings("AIzaSyC-TbyCQ9HdAuYAJm0-gJZFQG4kTgBJ4Dg"));
                push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(DeviceToken)
                          .WithJson(json)
                         );
            }
            else if (DeviceType == 2)
            {
                var appleCert = File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~/mobie_development.p12"));
                push.RegisterAppleService(new ApplePushChannelSettings(appleCert, "mobie1234"));
                push.QueueNotification(new AppleNotification()
                                            .ForDeviceToken(DeviceToken)
                                            .WithAlert(Message)
                                            .WithBadge(1)
                                            .WithSound("sound.caf")
                                            .WithCustomItem("data", Data)
                                            .WithCustomItem("message", Message)
                                            .WithCustomItem("Id", ID)

                    );
            }

        }
        public void SendSMS(string MobileNo, string Message)
        {
            string strUrl = "http://api.mVaayoo.com/mvaayooapi/MessageCompose?user=rameshbalakrishnan1987@gmail.com:123456789&senderID=TEST SMS&receipientno=" + MobileNo + "&dcs=0&msgtxt=" + Message + "&state=4";
            WebRequest request = HttpWebRequest.Create(strUrl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream s = (Stream)response.GetResponseStream();
            StreamReader readStream = new StreamReader(s);
            string dataString = readStream.ReadToEnd();
            response.Close();
            s.Close();
            readStream.Close();
        }
        public string RandomString()
        {
            string rStr = Path.GetRandomFileName();
            rStr = rStr.Replace(".", ""); // For Removing the .
            return rStr;
        }
        public string DatatoImage(string ImageData, string ImagePath, string FileName)
        {
            string ImageName = "";
            if (ImageData.Contains(@"\"))
            {
                ImageData = ImageData.Replace(@"\", "");
            }
            var base64Data = Regex.Match(ImageData, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var binData = Convert.FromBase64String(base64Data);
            Image Img;
            using (var streamBitmap = new MemoryStream(binData))
            {
                using (Img = Image.FromStream(streamBitmap))
                {
                    string ext = "";
                    if (Img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                    {
                        ext = ".jpg";
                    }
                    else if (Img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                    {
                        ext = ".gif";
                    }
                    else
                    {
                        ext = ".png";
                    }

                    if (!Directory.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/ProfileImages/" + ImagePath + "/")))
                    {
                        Directory.CreateDirectory(System.Web.Hosting.HostingEnvironment.MapPath("~/ProfileImages/" + ImagePath + "/"));
                    }
                    if (File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/ProfileImages/" + ImagePath + "/" + FileName + ext)))
                    {
                        File.Delete(System.Web.Hosting.HostingEnvironment.MapPath("~/ProfileImages/" + ImagePath + "/" + FileName + ext));
                        Img.Save(System.Web.Hosting.HostingEnvironment.MapPath("~/ProfileImages/" + ImagePath + "/" + FileName + ext));
                        ImageName = "http://103.230.39.54/CommutersApp/ProfileImages/" + ImagePath + "/" + FileName + ext;
                    }
                    else
                    {
                        Img.Save(System.Web.Hosting.HostingEnvironment.MapPath("~/ProfileImages/" + ImagePath + "/" + FileName + ext));
                        ImageName = "http://103.230.39.54/CommutersApp/ProfileImages/" + ImagePath + "/" + FileName + ext;
                    }
                }
            }
            return ImageName;
        }
    }
}