using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SshTunnelBackup
{
    public static class Configuration
    {
        public static readonly bool RUN_MYSQLDUMP;
        public static readonly bool RUN_PGDUMP;

        public static readonly bool USE_SSH_TUNNEL_FOR_MYSQL;
        public static readonly bool USE_SSH_TUNNEL_FOR_PG;

        public static readonly string MYSQL_DUMP_PATH;
        public static readonly string PG_DUMP_PATH;
        public static readonly string BACKUP_PATH;
        public static readonly string SSH_IP;
        public static readonly string SSH_PORT;
        public static readonly string SSH_USER;
        public static readonly string SSH_PASS;

        public static readonly string MYSQL_LOCAL_PORT;
        public static readonly string MYSQL_HOST;
        public static readonly string MYSQL_PORT;
        public static readonly string MYSQL_DBNAME;
        public static readonly string MYSQL_USER;
        public static readonly string MYSQL_PASSWORD;
        
        public static readonly string PG_LOCAL_PORT;
        public static readonly string PG_HOST;
        public static readonly string PG_PORT;
        public static readonly string PG_DBNAME;
        public static readonly string PG_USER;
        public static readonly string PG_PASSWORD;

        static Configuration()
        {
            RUN_MYSQLDUMP = bool.Parse(ConfigurationManager.AppSettings["RUN_MYSQLDUMP"]);
            RUN_PGDUMP = bool.Parse(ConfigurationManager.AppSettings["RUN_PGDUMP"]);

            USE_SSH_TUNNEL_FOR_MYSQL = bool.Parse(ConfigurationManager.AppSettings["USE_SSH_TUNNEL_FOR_MYSQL"]);
            USE_SSH_TUNNEL_FOR_PG = bool.Parse(ConfigurationManager.AppSettings["USE_SSH_TUNNEL_FOR_PG"]);

            MYSQL_DUMP_PATH = ConfigurationManager.AppSettings["MYSQL_DUMP_PATH"];
            PG_DUMP_PATH = ConfigurationManager.AppSettings["PG_DUMP_PATH"];

            BACKUP_PATH = ConfigurationManager.AppSettings["BACKUP_PATH"];
            SSH_IP = ConfigurationManager.AppSettings["SSH_IP"];
            SSH_PORT = ConfigurationManager.AppSettings["SSH_PORT"];
            SSH_USER = ConfigurationManager.AppSettings["SSH_USER"];
            SSH_PASS = ConfigurationManager.AppSettings["SSH_PASS"];

            MYSQL_LOCAL_PORT = ConfigurationManager.AppSettings["MYSQL_LOCAL_PORT"];
            MYSQL_HOST = ConfigurationManager.AppSettings["MYSQL_HOST"];
            MYSQL_PORT = ConfigurationManager.AppSettings["MYSQL_PORT"];
            MYSQL_DBNAME = ConfigurationManager.AppSettings["MYSQL_DBNAME"];
            MYSQL_USER = ConfigurationManager.AppSettings["MYSQL_USER"];
            MYSQL_PASSWORD = ConfigurationManager.AppSettings["MYSQL_PASSWORD"];

            PG_LOCAL_PORT = ConfigurationManager.AppSettings["PG_LOCAL_PORT"];
            PG_HOST = ConfigurationManager.AppSettings["PG_HOST"];
            PG_PORT = ConfigurationManager.AppSettings["PG_PORT"];
            PG_DBNAME = ConfigurationManager.AppSettings["PG_DBNAME"];
            PG_USER = ConfigurationManager.AppSettings["PG_USER"];
            PG_PASSWORD = ConfigurationManager.AppSettings["PG_PASSWORD"];
        }
    }
}
