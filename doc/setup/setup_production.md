# Setup production environment

## Prerequisites

* You have set up your dev environment with Grafana, see [here](setup_development.md).
* You have a Windows Server with IIS
    * with WebDeploy installed ([download](https://www.iis.net/downloads/microsoft/web-deploy)).
    * with current .NET Core runtime installed ([download](https://www.microsoft.com/net/permalink/dotnetcore-current-windows-runtime-bundle-installer)).

## Setup reverse proxy

A detailled documentation to host .NET Core on IIS can be found [here](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/).

1. Create a App Pool in IIS and give it a name, for example *".NET Core"* and set *".NET CLR version"* to *"No managed code"*
2. Create a web site and assign the App Pool just created
3. Configure the Environment Variable for selecting the `appsettings.Production.json` by doing the following steps:
    * Go to your application in IIS and choose Configuration Editor.
    * Select Configuration Editor 
    * Choose system.webServer/aspNetCore in "Section" combobox
    * Choose Applicationhost.config ... in "From" combobox.
    * Click on enviromentVariables element and open edit window.
    * Add environment variable named "ASPNETCORE_ENVIRONMENT" with content "Production"
    * Close the window and click Apply.

## Setup database

To set up the SQL Express database on the server, follow these steps:

1. Install SQL Server Express 2017 (or newer) on the server. Client Tools are not necessary.
2. Configure the Windows Firewall to allow TCP port 1433 on the local machine
3. Configure the SQL Server to enable TCP and on port 1433 via SQL Server Configuration Manager
4. Check the Windows username of the App Pool. Remember how you named your App Pool Identity before. The App Pool name ".NET Core" for example will result in a Windows User named *"IIS APPPOOL\.NET Core"*.
5. Configure the permissions with sqlcmd.exe (run `cmd` and start it with `sqlcmd -S .\SQLEXPRESS`) and run these SQL commands:

```sql
' Only for the first time, if the database does not exist yet
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

## Setup production config

In your dev environment go to the main application folder `src/GraphIoT.App/` and copy `appsettings.Development.json` and name it `appsettings.Production.json`

All credentials should stay the same for development and production, but you may want to set your logging level to only warning:

```json
"Logging": {
  "IncludeScopes": false,
  "LogLevel": {
    "Default": "Warning"
  },
  "Debug": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "Console": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```

And set your connection string to the SQL Server Express database instance you just created, with Windows integrated security authentication:

```json
"ConnectionStrings": {
  "SmarthomeDB": "Data Source=.\\SQLEXPRESS;Database=GraphIoT;Trusted_Connection=Yes;"
}
```

## Configure Grafana

Go to the grafana prod config folder `src/GraphIoT.Grafana/Grafana/conf/production/` and copy `custom.template.ini` and name it `custom.ini`

Edit this newly created `custom.ini` and enter your external URL:

```ini
# The full public facing url you use in browser, used for redirects and emails
# If you use reverse proxy and sub path specify full url (with sub path)
root_url = https://your.domain/path-to-graphiot-website/grafana
```

## Deployment

1. Go to the publish profiles folder `src/GraphIoT.App/Properties/PublishProfiles/` and copy `CustomProfile.template.pubxml` and name it `CustomProfile.pubxml`
2. In Visual Studio, right click on the main *"GraphIoT.App"* project in folder *"MainWebserver"* and select *"Publish..."*
3. Edit the *"Custom Profile"* and enter your server, web site name and credentials for WebDeploy
4. Hit *"Publish"*

Check if the application runs and is reachable. The database tables will be created automatically on first run.

Setup your grafana admin user, JSON datasource and dashboards.

You have set up and deployed the application!
