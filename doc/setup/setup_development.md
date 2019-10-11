# Setup dev environment

## Prerequisites

* Visual Studio is installed with Workloads *"ASP.NET and Web Development"* as well as *".NET Core Development"*, which should also include the component *"Sql Server Express LocalDB"*

## Setup IDE and solution

1. Clone repository from within Visual Studio and open `GraphIoT.sln` solution file.
2. Configure app settings:
    * In the main application folder `src/GraphIoT.App/` copy `appsettings.json` and name it `appsettings.Development.json`
    * Fill out all credentials in `appsettings.Development.json`, like `<user>`, `<pass>` etc.

## Setup Grafana

1. Download the newest version of Grafana *"Standalone Windows Binaries"* from [here](https://grafana.com/grafana/download?platform=windows).
2. Copy the files to the `src/GraphIoT.Grafana/Grafana` folder.

## Run

Set *"GraphIoT.App"* as start project and start with *"IIS Express"*.

* This should build the whole solution, start the application and should trigger the initial creation of the LocalDB database and create all necessary tables via code first migrations automatically.

You have set up your dev environment!
