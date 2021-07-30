# Configure Grafana datasource

## Prerequisites

If you want to configure grafana in your *development* environment:

* You have set up your dev environment with Grafana, see [here](../setup/setup_development.md).

If you want to configure grafana in your *production* environment:

* You have set up your dev environment (see [here](../setup/setup_development.md)) **and** production environment with Grafana (see [here](../setup/setup_production.md)).

## Setup admin user

After you start the application and go to the grafana url (`http://localhost:53685/grafana` or `https://your.domain/path-to-graphiot-website/grafana`) in your browser, you should see the grafana login page.

Enter the initial magic credentials with username `admin` and password: `admin`. You should be promted to create a real admin user with a username and password of your choice.

## Create datasource

In the `Configuration` menu, go to `Data Sources`. Add a new datasource and select `Graphite`.

In the `Graphite` datasource configuration panel, set the URL for dev to:

    http://localhost:53685/api/graphite

or for production to (replace your domain and website base path and append `/api/graphite`):

    https://your.domain/path-to-graphiot-website/api/graphite

and set the `Version` to `1.1.x`.

It should look like follows:

![Grafana JSON datasource screenshot](https://philip.daubmeier.de/github/graphiot/doc/grafana_graphite_datasource.png)

Hit `Safe & Test` to make sure the datasource works.

## Next steps

* [Create a dashboard](configure_dashboard.md)
