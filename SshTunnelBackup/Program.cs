using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SshTunnelBackup
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static int Main(string[] args)
        {
            log.Info($"Starting SHH Tunnel Backup");

            int exitCode = 0;// success
            if (Configuration.RUN_MYSQLDUMP)
            {
                if (Configuration.USE_SSH_TUNNEL_FOR_MYSQL)
                {
                    using (var client = NewSshClient(ref exitCode))
                    {
                        client.Connect();
                        exitCode += ExecuteMySqlDump(client);
                        client.Disconnect();
                    }
                }
                else
                {
                    exitCode += ExecuteMySqlDump(null);
                }
            }

            if (Configuration.RUN_PGDUMP)
            {
                if (Configuration.USE_SSH_TUNNEL_FOR_PG)
                {
                    using (var client = NewSshClient(ref exitCode))
                    {
                        client.Connect();
                        exitCode += ExecutePgDump(client);
                        client.Disconnect();
                    }
                }
                else
                {
                    exitCode += ExecutePgDump(null);
                }
            }

            log.Info($"SHH Tunnel Backup Shutdown");
            Environment.Exit(exitCode);
            return exitCode;
        }

        static SshClient NewSshClient(ref int exitCode)
        {
            var client = new SshClient(Configuration.SSH_IP, int.Parse(Configuration.SSH_PORT), Configuration.SSH_USER, Configuration.SSH_PASS);
            int newExitCode = 0;
            client.ErrorOccurred += (sender, e) =>
            {
                if (e.Exception != null && e.Exception.Message != null)
                {
                    log.Error(e.Exception.Message);
                    newExitCode = 1;// SSH Connection error
                    return;
                }
            };
            exitCode += newExitCode;
            return client;
        }

        static int ExecuteMySqlDump(SshClient client)
        {
            if (Configuration.USE_SSH_TUNNEL_FOR_MYSQL)
            {
                if (client == null)
                {
                    throw new Exception("SSH Client cannot be null when tunnel is expected!");
                }
                var port = new ForwardedPortLocal("127.0.0.1", uint.Parse(Configuration.MYSQL_PORT), "localhost", uint.Parse(Configuration.MYSQL_LOCAL_PORT));
                client.AddForwardedPort(port);
                port.Start();
            }

            return MysqlDump();
        }

        static int MysqlDump()
        {
            int exitCode = 0;//success
            string dumpFilePath = $"{Configuration.BACKUP_PATH}\\{Configuration.MYSQL_DBNAME}_backup_{DateTime.Now.ToString("yyyyMMddHHmm")}.sql";

            PrepareDir(dumpFilePath);

            using (Process dump = new Process())
            {
                string args = $"-u {Configuration.MYSQL_USER} -p{Configuration.MYSQL_PASSWORD} --single-transaction --host=127.0.0.1 --port={Configuration.MYSQL_LOCAL_PORT} {Configuration.MYSQL_DBNAME} -r {dumpFilePath}";
                dump.StartInfo.FileName = string.IsNullOrEmpty(Configuration.MYSQL_DUMP_PATH) ? "mysqldump.exe" : Configuration.MYSQL_DUMP_PATH;
                dump.StartInfo.Arguments = args;
                dump.StartInfo.RedirectStandardOutput = true;
                dump.StartInfo.RedirectStandardError = true;
                dump.StartInfo.UseShellExecute = false;
                dump.StartInfo.CreateNoWindow = false;
                dump.Start();


                string err = dump.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(err))
                {
                    log.Error(err);
                    exitCode = 2;//MySQL Dump error
                }

                dump.BeginOutputReadLine();
                dump.WaitForExit();
                dump.CancelOutputRead();
            }
            return exitCode;
        }

        static int ExecutePgDump(SshClient client)
        {
            if (Configuration.USE_SSH_TUNNEL_FOR_PG)
            {
                if (client == null)
                {
                    throw new Exception("SSH Client cannot be null when tunnel is expected!");
                }
                var port = new ForwardedPortLocal("127.0.0.1", uint.Parse(Configuration.PG_PORT), "localhost", uint.Parse(Configuration.PG_LOCAL_PORT));
                client.AddForwardedPort(port);
                port.Start();
            }
            return PgDump();
        }

        static int PgDump()
        {
            int exitCode = 0;//success

            string dumpFilePathFull = $"{Configuration.BACKUP_PATH}\\{Configuration.PG_DBNAME}_backup_full_{DateTime.Now.ToString("yyyyMMddHHmm")}.pgBackup";
            string dumpFilePathSchemaOnly = $"{Configuration.BACKUP_PATH}\\{Configuration.PG_DBNAME}_backup_schema_only_{DateTime.Now.ToString("yyyyMMddHHmm")}.sql";

            PrepareDir(dumpFilePathFull);
            PrepareDir(dumpFilePathSchemaOnly);

            using (Process dump = new Process())
            {
                string args = $"--schema-only --format=plain --host={Configuration.PG_HOST} --port={Configuration.PG_PORT} --username={Configuration.PG_USER} --file={dumpFilePathSchemaOnly} {Configuration.PG_DBNAME}";
                dump.StartInfo.EnvironmentVariables["PGPASSWORD"] = Configuration.PG_PASSWORD;
                dump.StartInfo.FileName = string.IsNullOrEmpty(Configuration.PG_DUMP_PATH) ? "pg_dump.exe" : Configuration.PG_DUMP_PATH;
                dump.StartInfo.Arguments = args;
                dump.StartInfo.RedirectStandardOutput = true;
                dump.StartInfo.RedirectStandardError = true;
                dump.StartInfo.UseShellExecute = false;
                dump.StartInfo.CreateNoWindow = false;
                dump.Start();


                string err = dump.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(err))
                {
                    log.Error(err);
                    exitCode = 8;//PG Dump error 1
                }

                dump.BeginOutputReadLine();
                dump.WaitForExit();
                dump.CancelOutputRead();
            }

            using (Process dump = new Process())
            {
                string args = $"--format=custom --compress=9 --format=plain --host={Configuration.PG_HOST} --port={Configuration.PG_PORT} --username={Configuration.PG_USER} --file={dumpFilePathFull} {Configuration.PG_DBNAME}";
                dump.StartInfo.EnvironmentVariables["PGPASSWORD"] = Configuration.PG_PASSWORD;
                dump.StartInfo.FileName = string.IsNullOrEmpty(Configuration.PG_DUMP_PATH) ? "pg_dump.exe" : Configuration.PG_DUMP_PATH;
                dump.StartInfo.Arguments = args;
                dump.StartInfo.RedirectStandardOutput = true;
                dump.StartInfo.RedirectStandardError = true;
                dump.StartInfo.UseShellExecute = false;
                dump.StartInfo.CreateNoWindow = false;
                dump.Start();


                string err = dump.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(err))
                {
                    log.Error(err);
                    exitCode = 16;//PG Dump error 2
                }

                dump.BeginOutputReadLine();
                dump.WaitForExit();
                dump.CancelOutputRead();
            }

            return exitCode;
        }

        static void PrepareDir(string dumpFilePath)
        {
            var parentDir = Directory.GetParent(dumpFilePath);
            if (!parentDir.Exists)
            {
                parentDir.Create();
            }
        }
    }
}
