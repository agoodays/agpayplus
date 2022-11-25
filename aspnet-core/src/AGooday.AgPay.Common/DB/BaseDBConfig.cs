using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Utils;

namespace AGooday.AgPay.Common.DB
{
    public class BaseDBConfig
    {
        private static string defaultConnection = Appsettings.app(new string[] { "ConnectionStrings", "DefaultConnection" });
        private static string defaultConnectionFile = Appsettings.app(new string[] { "ConnectionStrings", "DefaultConnectionFile" });

        private static string sqliteConnection = Appsettings.app(new string[] { "AppSettings", "Sqlite", "SqliteConnection" });
        private static bool isSqliteEnabled = (Appsettings.app(new string[] { "AppSettings", "Sqlite", "Enabled" })).ObjToBool();

        private static string sqlServerConnection = Appsettings.app(new string[] { "AppSettings", "SqlServer", "SqlServerConnection" });
        private static string sqlServerConnectionFile = Appsettings.app(new string[] { "AppSettings", "SqlServer", "SqlServerConnectionFile" });
        private static bool isSqlServerEnabled = (Appsettings.app(new string[] { "AppSettings", "SqlServer", "Enabled" })).ObjToBool();

        private static string mySqlConnection = Appsettings.app(new string[] { "AppSettings", "MySql", "MySqlConnection" });
        private static string mySqlConnectionFile = Appsettings.app(new string[] { "AppSettings", "MySql", "MySqlConnectionFile" });
        private static bool isMySqlEnabled = (Appsettings.app(new string[] { "AppSettings", "MySql", "Enabled" })).ObjToBool();

        private static string oracleConnection = Appsettings.app(new string[] { "AppSettings", "Oracle", "OracleConnection" });
        private static string oracleConnectionFile = Appsettings.app(new string[] { "AppSettings", "Oracle", "OracleConnectionFile" });
        private static bool IsOracleEnabled = (Appsettings.app(new string[] { "AppSettings", "Oracle", "Enabled" })).ObjToBool();

        public static DataBaseType DbType = InitDbType();

        public static string ConnectionString => InitConn();

        public static string GetConnectionString(params string[] conn) => DifDBConnOfSecurity(conn);

        private static DataBaseType InitDbType()
        {
            if (isSqliteEnabled)
            {
                return DataBaseType.Sqlite;
            }
            else if (isSqlServerEnabled)
            {
                return DataBaseType.SqlServer;
            }
            else if (isMySqlEnabled)
            {
                return DataBaseType.MySql;
            }
            else if (IsOracleEnabled)
            {
                return DataBaseType.Oracle;
            }
            else
            {
                return DataBaseType.SqlServer;
            }
        }

        private static string InitConn()
        {
            if (isSqliteEnabled)
            {
                DbType = DataBaseType.Sqlite;
                return sqliteConnection;
            }
            else if (isSqlServerEnabled)
            {
                DbType = DataBaseType.SqlServer;
                return DifDBConnOfSecurity(@$"{sqlServerConnectionFile}", sqlServerConnection);
            }
            else if (isMySqlEnabled)
            {
                DbType = DataBaseType.MySql;
                return DifDBConnOfSecurity(@$"{mySqlConnectionFile}", mySqlConnection);
            }
            else if (IsOracleEnabled)
            {
                DbType = DataBaseType.Oracle;
                return DifDBConnOfSecurity(@$"{oracleConnectionFile}", oracleConnection);
            }
            else
            {
                DbType = DataBaseType.SqlServer;
                var defaultConnectionString = DifDBConnOfSecurity(defaultConnectionFile, defaultConnection);

                return string.IsNullOrWhiteSpace(defaultConnectionString) ? "server=.;uid=sa;pwd=mssql*;database=DncZero" : defaultConnectionString;
            }
        }
        private static string DifDBConnOfSecurity(params string[] conn)
        {
            try
            {
                foreach (var item in conn)
                {
                    try
                    {
                        if (File.Exists(item))
                        {
                            return File.ReadAllText(item).Trim();
                        }
                    }
                    catch (Exception) { }
                }

                return conn[conn.Length - 1];
            }
            catch (Exception)
            {
                throw new Exception("数据库连接字符串配置有误，请检查 web 层下  appsettings.json 文件");
            }
        }
    }
}
