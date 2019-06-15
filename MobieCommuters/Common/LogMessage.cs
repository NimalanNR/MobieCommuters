using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;

using System.Diagnostics;
using System.Xml;

namespace MobieCommuters.Common
{
    public class LogMessage
    {
        public static string strLogFilePath = string.Empty;
        private static StreamWriter sw = null;
        /// <summary>
        /// Setting LogFile path. If the logfile path is 
        /// null then it will update error info into LogFile.txt under
        /// application directory.
        /// </summary>
        public string LogFilePath
        {
            set
            {
                strLogFilePath = value;
            }
            get
            {
                return strLogFilePath;
            }
        }
        public static bool WriteLog(string message)
        {
            try
            {
                //Check whether logging is enabled or not
                bool bLoggingEnabled = false;
                string strPathName = String.Empty;
                bLoggingEnabled = CheckLoggingEnabled();

                //Don't process more if the logging 
                if (false == bLoggingEnabled)
                    return true;
                if (strLogFilePath.Equals(string.Empty))
                {
                    //Get Default log file path "LogFile.txt"
                    strPathName = GetLogFilePath();
                }
                else
                {
                    //If the log file path is not empty but the file
                    //is not available it will create it
                    if (true != File.Exists(strLogFilePath))
                    {
                        if (false == CheckDirectory(strLogFilePath))
                            return false;

                        FileStream fs = new FileStream(strLogFilePath,
                                FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        fs.Close();
                    }
                    strPathName = strLogFilePath;
                }
                bool bReturn = true;

                // write the error log to that text file
                if (true != WriteToTraceFile(strPathName,message))
                {
                    bReturn = false;
                }
                return bReturn;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static bool WriteToTraceFile(string strPathName,
                                string message)
        {
            bool bReturn = false;
            string strException = string.Empty;
            try
            {
                sw = new StreamWriter(strPathName, true);
                sw.WriteLine(message);
                sw.Flush();
                sw.Close();
                bReturn = true;
            }
            catch (Exception)
            {
                bReturn = false;
            }
            return bReturn;
        }

        private static string GetLogFilePath()
        {
            try
            {
                // get the base directory
                string baseDir = AppDomain.CurrentDomain.BaseDirectory +
                               AppDomain.CurrentDomain.RelativeSearchPath;

                // search the file below the current directory
                string retFilePath = baseDir + "//" + "LogFile.txt";

                // if exists, return the path
                if (File.Exists(retFilePath) == true)
                    return retFilePath;
                //create a text file
                else
                {
                    if (false == CheckDirectory(strLogFilePath))
                        return string.Empty;

                    FileStream fs = new FileStream(retFilePath,
                          FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs.Close();
                }

                return retFilePath;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static bool CheckDirectory(string strLogPath)
        {
            try
            {
                int nFindSlashPos = strLogPath.Trim().LastIndexOf("\\");
                string strDirectoryname =
                           strLogPath.Trim().Substring(0, nFindSlashPos);

                if (false == Directory.Exists(strDirectoryname))
                    Directory.CreateDirectory(strDirectoryname);
                return true;
            }
            catch (Exception)
            {
                return false;

            }
        }

        private static bool CheckLoggingEnabled()
        {
            return isLoggingEnabled();
        }
     
        private static bool isLoggingEnabled()
        {
            try
            {
                string value = System.Configuration.ConfigurationManager.AppSettings["LoggingEnabled"];
                int isLoggingEnable = Convert.ToInt16(value);
                if(isLoggingEnable==0)
                {
                    return false;
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }

}