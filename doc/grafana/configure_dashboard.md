# Configure Grafana dashboard

## Prerequisites

* You have set up Grafana with an admin user and the datasource, see [here](configure_datasource.md).

## Create dashboard

Create a new dashboard in grafana and add a panel by clicking `Add Query`

![Grafana add query screenshot](https://philip.daubmeier.de/github/graphiot/doc/grafana_add_query.png)

You should see the query configuration, where you can add a new query. Select `Graphite` as datasource.

You can select one or more time series metrics by clicking next to `Series` where you get autocompletion based on the selected path:

![Grafana dashboard query screenshot](https://philip.daubmeier.de/github/graphiot/doc/grafana_dashboard_query1.png)

You can also select `*` as wildcard for any parts of the metrics path.

Additionally, a time series query can be refined with applying functions to the query, which get executed on server side by GraphIoT before sending the result values to grafana.

Just explore all available functions by clicking the `+` sign and the autocompletion drop down that will pop up.

For example, you can filter the result set of time series via a regular expression via the `grep` function, rename the resulting metric with `alias`, change the resolution with `resample` functions and much more. Also, many of the function parameters also feature autocompletion suggestions for time spans or enum values.

Your query could look like this for instance:

![Grafana dashboard query screenshot](https://philip.daubmeier.de/github/graphiot/doc/grafana_dashboard_query2.png)

Also, if you already defined variables (see chapter [variables](configure_variables_annotations.md)) in your dashboard, you can also include them into your query. This even works with multiple valued variables:

![Grafana dashboard query screenshot](https://philip.daubmeier.de/github/graphiot/doc/grafana_dashboard_query3.png)

## Next steps

* [Add variables and annotations](configure_variables_annotations.md)
