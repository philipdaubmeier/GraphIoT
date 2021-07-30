# Configure Grafana variables and annotations

## Prerequisites

* You have set up a Grafana dashboard, see [here](configure_dashboard.md).

## Create variables

You can create variables for Metric Ids like Digitalstrom rooms, groups, color or for Netatmo modules. Those can be used in dashboard panel queries or in annotation queries. For more information on this topic, see the [Grafana variables documentation](https://grafana.com/docs/grafana/latest/variables/).

**Example:**

![Grafana add variables screenshot](https://philip.daubmeier.de/github/graphiot/doc/grafana_variables.png)

## Create annotations

Annotations can be created for displaying non-equidistant data of the GraphIoT application, like Digitalstrom events.

**Example:**

![Grafana add annotations screenshot](https://philip.daubmeier.de/github/graphiot/doc/grafana_annotations.png)

Annotations can contain Grafana variables (see above).

Annotation queries must follow this JSON pattern (`event_names`, `group_ids` and `scene_ids` are mandatory):

```json
{
  "event_names": [ "callScene" ],
  "meter_ids": [ "a8fe..2d45" ],
  "zone_ids": [ 5, 10, 31071 ],
  "group_ids": [ 0, 1, 2 ],
  "scene_ids": [ 1, 5 ]
}
```

### Detailled description

| Element | Description |
| --- | --- |
| `event_names` | **Mandatory**. Only show events with one of the given event types. |
| `meter_ids` | Only show events that happened in zones (rooms) that are connected to one of the given DS meters. |
| `zone_ids` | Only show events that happened in zones (rooms) with one of the given ids. |
| `group_ids` | **Mandatory**. Only show events that happened in one of the given (color-)groups. |
| `scene_ids` | **Mandatory**. Only show events that affected one of the given scene ids. |
