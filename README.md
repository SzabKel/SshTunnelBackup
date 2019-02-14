# SshTunnelBackup

## About

You can use this to schedule backups through an SSH Tunnel (optional) and save the backups to a local directory on Windows machienes.

Works by using a local installation of a backup utility (`mysqldump`, `pg_dump`).

Supported:
- MySQL
- PostgreSQL

## Prerequisites

### MySQL

You need a local installation of `mysqldump`. Fastest option is to download a [ZIP version of MariaDB](https://downloads.mariadb.org/), you don't need to install it this way and the `mysqldumnp` will still work.
Place it in an accessible directory (`C:\apps\mariadb-10.3.12` for example). 

Create a user capable of using mysqldump (`GRANT SELECT, SHOW VIEW, LOCK TABLES` and add access to the DB you need to backup). 
If you want to use an SSH tunnel, the user only need access from `localhost`.
The user must have a password to work!

### PostgreSQL

You need a local installation of `pg_dump`. You need to be careful to install a matching version of `pg_dump` (if your server runs on 11.x, you can't dump using 10.x)! 
You can download [EDB's installer](https://www.enterprisedb.com/downloads/postgres-postgresql-downloads) and only install the command line tools and pgAdmin, without the server itself.

Create a user capable of using `pg_dump` and able to connect.

### SSH

You don't need to install any additional tools to let the program execute, it uses `Renci.SshNet` to connect and to create port forwards.
However you will need an SSH user capable of logging into the server. Preferable a user without `sudo` rights.

### Destination

You will need a local folder to save the backups to.


## Configuration

Check the `App.config` (or `[EXE-Name].exe.config` if installed).

    -`RUN_MYSQLDUMP`: Set to `TRUE`, if you would like to run `mysqldump`.
    -`RUN_PGDUMP`: Set to `TRUE`, if you would like to run `pg_dump`.

    -`USE_SSH_TUNNEL_FOR_MYSQL`: Set to `TRUE`, if you would like to use an SSH tunnel when executing `mysqldump`.
    -`USE_SSH_TUNNEL_FOR_PG`: Set to `TRUE`, if you would like to use an SSH tunnel when executing `pg_dump`.
    
    -`MYSQL_DUMP_PATH`: The location of the `mysqldump.exe`, example `C:\mysql\bin\mysqldump.exe`
    -`PG_DUMP_PATH`: The location of the `pg_dump.exe`, example `C:\Program Files\PostgreSQL\11\bin\pg_dump.exe`
    -`BACKUP_PATH`: The backup directory
    
    -`SSH_IP`: The server's address for SSH (You only need to set it, if you want SSH).
    -`SSH_PORT`: The SSH port (You only need to set it, if you want SSH).
    -`SSH_USER`: The SSH user (You only need to set it, if you want SSH).
    -`SSH_PASS`: The SSH password (You only need to set it, if you want SSH).
    
    -`MYSQL_LOCAL_PORT`: The local port the remote one gets forwarded to. Default `3306`. (You only need to set it, if you want SSH).
    -`MYSQL_HOST`: The IP of the MySQL server.
    -`MYSQL_PORT`: MySQL's port. Default `3306`.
    -`MYSQL_DBNAME`: The database name you want to backup.
    -`MYSQL_USER`: The MySQL username.
    -`MYSQL_PASSWORD`: The MySQL password.
    
    -`PG_LOCAL_PORT`: The local port the remote one gets forwarded to. Default `5432`. (You only need to set it, if you want SSH).
    -`PG_HOST`: The IP of the PostgreSQL server.
    -`PG_PORT`: PostgreSQL's port. Default `5432`.
    -`PG_DBNAME`: The database name you want to backup.
    -`PG_USER`: The PostgreSQL username.
    -`PG_PASSWORD`: The PostgreSQL password.
    
 ## Example usage
 
 I had to backup two mysql and one postgresql database, located on different ubuntu servers. I made three backup destinations and created three app folders, where I copied the binaries for each. I configured each one with different config (for the three different servers).
 I scheduled the execution using [CloudBerry's software](https://www.cloudberrylab.com/backup/server/windows.aspx), which executes the exe's as a Pre-Action job, than reads the destination directory, takes the new files and upload them to Amazon S3.
