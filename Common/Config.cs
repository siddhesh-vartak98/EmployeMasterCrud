using System.Xml.Linq;

namespace EmployeMasterCrud.Common
{
    public class Config
    {
        ////Local (siddhesh)
        //const string dataSource = @"DESKTOP-5ORVV5G\SQLEXPRESS";
        //const string dbName = "EmployeeDemoDB";
        //const string dbUserID = "sa";
        //const string dbPassword = "tft123456";

        const string dataSource = @"localhost\SQLEXPRESS";
        const string dbName = "EmployeeDemoDB";
        const string dbUserID = "sa";
        const string dbPassword = "sid@123456";

        public static string connectionString = @"Data Source=" + dataSource + "; Initial Catalog=" + dbName + "; User ID=" + dbUserID + "; Password=" + dbPassword + "; MultipleActiveResultSets=true; App=EntityFramework; Encrypt=false; TrustServerCertificate=true;";

        //Page Size
        public static int pageSize = 1;

        //Date Formats
        public static string dateTimeFormat = "dd MMM yyyy hh:mm tt";
        public static string dateTimeFormatTT = "dd/MM/yyyy hh:mm tt";
        public static string dateFormat = "dd MMM yyyy";
        public static string reverseDateFormat = "yyyy MM dd";
        public static string timeFormat = "hh:mm tt";
        public static string searchDateFormat = "yyyy-MM-dd";
        public const string dateForJSFormat = "yyyy-MM-dd";
        public const string dateForJSFormatTwo = "dd-MM-yyyy";
        public const string dateForPDFPrint = "dd/MM/yyyy";
        public static string yearFormat = "yyyy";

        public static string dateFormatPDFPrint = "dd/MM/yyyy";

        public static string ThumbnailImagePath = "/Content/ThumbnailImage/";
        public static string EmpFilePath = "/Content/Files/";
        public static string DocumentFilePath = "/Content/DocumentFiles/";

        public static string placeholderImageUrl = "/images/no-image-icon.png";

        public enum BannerCategory : int
        {
            festival = 1,
            homeCategory = 2
        }
    }
}
