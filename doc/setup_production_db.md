# Setup Production DB

To set up the SQL Express database on the server, follow these steps

1. Install SQL Server Express 2017 (or newer) on the server. Client Tools are not necessary.
2. Configure the Windows Firewall to allow TCP port 1433 on the local machine
3. Configure the SQL Server to enable TCP and on port 1433 via SQL Server Configuration Manager
4. Configure Environment Variable for selecting the appsettings.Production.json by doing the following steps:
   1. Go to your application in IIS and choose Configuration Editor.
   2. Select Configuration Editor 
   3. Choose system.webServer/aspNetCore in "Section" combobox
   4. Choose Applicationhost.config ... in "From" combobox.
   5. Click on enviromentVariables element and open edit window.
   6. Add environment variable named "ASPNETCORE_ENVIRONMENT" with content "Production"
   7. Close the window and click Apply.
4. On IIS, look how the App Pool Identity is named (i.e. Windows User) for the App Pool of .NET Kestrel. The App Pool name ".NET Core" for example will result in a Windows User named "IIS APPPOOL\.NET Core".
5. Configure the permissions with sqlcmd.exe (run `cmd` and start it with `sqlcmd -S .\SQLEXPRESS`) and run these SQL commands:

```sql
' Optionally as first step, if the database does not exist yet
CREATE DATABASE [GraphIoT]
GO
```
      
```sql 
USE [GraphIoT]
GO
CREATE LOGIN [IIS APPPOOL\.NET Core] FROM WINDOWS;
GO
CREATE USER DotNetCore FOR LOGIN [IIS APPPOOL\.NET Core];
GO
GRANT ALL PRIVILEGES TO DotNetCore;
GO
sp_addrolemember @rolename = 'db_datareader', @membername = 'DotNetCore';
GO
sp_addrolemember @rolename = 'db_datawriter', @membername = 'DotNetCore';
GO
sp_addrolemember @rolename = 'db_owner', @membername = 'DotNetCore';
GO
```

# Backup Database

Run with sqlcmd.exe (run `cmd` and start it with `sqlcmd -S .\SQLEXPRESS`):

```sql
BACKUP DATABASE [GraphIoT] TO DISK='C:\bkp\graphiot_db.bak';
GO
```

And to restore:

```sql
RESTORE DATABASE [GraphIoT] FROM DISK='C:\bkp\graphiot_db.bak';
GO
```
