using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;
using System.ComponentModel.DataAnnotations;

namespace EmployeMasterCrud.Common
{
    public class CommonFunctions
    {
        public static DateTime ConvertToIST(DateTime utcDateTime)
        {
            TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, istTimeZone);
        }

        public static string CreateImageFileName(IFormFile postedFile)
        {
            string fileName = string.Empty;

            string postedFileExtension = System.IO.Path.GetExtension(postedFile.FileName);

            try
            {
                fileName = Guid.NewGuid().ToString();
                fileName = fileName + postedFileExtension;
            }
            catch
            {
                fileName = string.Empty;
            }

            return fileName;
        }

        public static string getExceptionMessage(Exception ex)
        {
            string message = string.Empty;

            switch (ex)
            {
                case SqlException sqlExp:
                    message = sqlExp.Message;
                    break;

                case ArgumentNullException argNullEx:
                    message = argNullEx.Message;
                    break;

                case ArgumentOutOfRangeException argOutOfRangeEx:
                    message = argOutOfRangeEx.Message;
                    break;

                case ArgumentException argExp:
                    message = argExp.Message;
                    break;

                case InvalidOperationException invalidOpEx:
                    message = invalidOpEx.Message;
                    break;

                case FileNotFoundException fileNotFdEx:
                    message = fileNotFdEx.Message;
                    break;

                case IOException ioEx:
                    message = ioEx.Message;
                    break;

                case ValidationException validtnEx:
                    message = validtnEx.Message;
                    break;

                case DbUpdateConcurrencyException dbUpdateConEx:
                    message = dbUpdateConEx.Message;
                    break;

                case DbUpdateException dbUpdateEx:
                    message = dbUpdateEx.Message;
                    break;

                case TimeoutException timeoutEx:
                    message = timeoutEx.Message;
                    break;

                case HttpRequestException httpReqEx:
                    message = httpReqEx.Message;
                    break;

                default:
                    message = ex.Message;
                    break;

            }

            return message;
        }

        public static bool AddEntryOfLog(Logger logger, Exception? ex, string? jsonInput, string? textMessage)
        {
            bool status = false;
            string stringTextMessage = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(textMessage))
                {
                    stringTextMessage = textMessage;
                }
                else
                {
                    stringTextMessage = "";
                }

                #region if exception present 
                if (ex != null)
                {
                    try
                    {
                        LogEventInfo theEvent = new LogEventInfo(NLog.LogLevel.Error, null, ex.Message);

                        if (!string.IsNullOrEmpty(jsonInput))
                        {
                            theEvent.Properties["InputParameters"] = jsonInput;
                        }

                        theEvent.Exception = ex;
                        logger.Log(theEvent);

                        status = true;
                    }
                    catch { /*if log get exception then ignore */ }
                }
                #endregion
                else
                {
                    try
                    {
                        LogEventInfo theNLogEvent = new LogEventInfo(NLog.LogLevel.Debug, null, stringTextMessage);

                        if (!string.IsNullOrEmpty(jsonInput))
                        {
                            theNLogEvent.Properties["InputParameters"] = jsonInput;
                        }

                        logger.Log(theNLogEvent);

                        status = true;
                    }
                    catch { /*if log get exception then ignore */  }
                }


            }
            catch { /*if log get exception then ignore */  }

            return status;
        }
    }
}
