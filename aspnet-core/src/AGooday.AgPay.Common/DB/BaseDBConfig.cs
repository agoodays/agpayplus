using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Utils;

namespace AGooday.AgPay.Common.DB
{
    public class BaseDBConfig
    {
        private static readonly string defaultConnection = Appsettings.App(["ConnectionStrings", "DefaultConnection"]);
        private static readonly string defaultConnectionFile = Appsettings.App(["ConnectionStrings", "DefaultConnectionFile"]);

        private static readonly string sqliteConnection = Appsettings.App(["AppSettings", "Sqlite", "SqliteConnection"]);
        private static readonly bool isSqliteEnabled = Appsettings.App(["AppSettings", "Sqlite", "Enabled"]).ObjToBool();

        private static readonly string sqlServerConnection = Appsettings.App(["AppSettings", "SqlServer", "SqlServerConnection"]);
        private static readonly string sqlServerConnectionFile = Appsettings.App(["AppSettings", "SqlServer", "SqlServerConnectionFile"]);
        private static readonly bool isSqlServerEnabled = (Appsettings.App(["AppSettings", "SqlServer", "Enabled"])).ObjToBool();

        private static readonly string mySqlConnection = Appsettings.App(["AppSettings", "MySql", "MySqlConnection"]);
        private static readonly string mySqlConnectionFile = Appsettings.App(["AppSettings", "MySql", "MySqlConnectionFile"]);
        private static readonly bool isMySqlEnabled = Appsettings.App(["AppSettings", "MySql", "Enabled"]).ObjToBool();

        private static readonly string oracleConnection = Appsettings.App(["AppSettings", "Oracle", "OracleConnection"]);
        private static readonly string oracleConnectionFile = Appsettings.App(["AppSettings", "Oracle", "OracleConnectionFile"]);
        private static readonly bool isOracleEnabled = Appsettings.App(["AppSettings", "Oracle", "Enabled"]).ObjToBool();

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
            else if (isOracleEnabled)
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
            else if (isOracleEnabled)
            {
                DbType = DataBaseType.Oracle;
                return DifDBConnOfSecurity(@$"{oracleConnectionFile}", oracleConnection);
            }
            else
            {
                DbType = DataBaseType.MySql;
                var defaultConnectionString = DifDBConnOfSecurity(defaultConnectionFile, defaultConnection);

                return string.IsNullOrWhiteSpace(defaultConnectionString) ? "server=localhost;port=3306;uid=root;pwd=mysql*;database=agpayplusdb_dev" : defaultConnectionString;
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
