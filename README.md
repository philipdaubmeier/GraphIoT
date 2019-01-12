# Python Volkswagen CarNet Client

This is a Python client for Volkswagen CarNet, it simulates the behaviour of the CarNet app. It allows users to retrieve information about the vehicle (location, temperature and mileage), next to this the Window melt and Climat functionalities can be started from the Python script.

# Installation

Clone the Github page and modify the following values in the vw_carnet.py script:
```
CARNET_USERNAME = ''
CARNET_PASSWORD = ''
CARNET_SPIN = '' <- PIN for executing actions like used in your VW app
```

Enter the correct MQTT-broker host and port in the lib_mqtt.py:

```
MQTT_HOST = "<hostname or IP>" <- host where your mqtt broker runs
MQTT_PORT = <port> <- port of your mqtt service, default is 1883
```

# Usage

Run the script with a argument. The following are supported:
```
python vw_carnet_web.py retrieveCarNetInfo
```

```
python vw_carnet_web.py startClimat
```

```
python vw_carnet_web.py stopClimat
```

```
python vw_carnet_web.py startWindowMelt
```

```
python vw_carnet_web.py stopWindowMelt
```

...
(see code for more options)

Send all data to the MQTT broker configured in lib_mqtt:
```
python vw_carnet.py mqtt
```

See also [FHEM integration](https://forum.fhem.de/index.php/topic,83090.msg886586.html#msg886586)
