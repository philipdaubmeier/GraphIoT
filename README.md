# Python Volkswagen CarNet Client

This is a Python client for Volkswagen CarNet, it simulates the behaviour of the CarNet web page. It allows users to retrieve information about the vehicle (location, temperature and mileage), next to this the Window melt and Climat functionalities can be started from the Python script. This was built based on work from wez3 and reneboer.

# Installation

Clone the Github repo and if you want to use mqtt, enter the correct MQTT-broker host and port in the lib_mqtt.py:

```
MQTT_HOST = "<hostname or IP>" <- host where your mqtt broker runs
MQTT_PORT = <port> <- port of your mqtt service, default is 1883
```

# Usage

Run the script with arguments. If you only own one car in VW we, the vin parameter is obsolete. The following are supported:
```
python3 vw_carnet_web.py -u <username> -p <password> -s <spin> -v <vin> -c retrieveCarNetInfo
```

```
python3 vw_carnet_web.py  -u <username> -p <password> -s <spin> -v <vin> -c startClimat
```

```
python3 vw_carnet_web.py  -u <username> -p <password> -s <spin> -v <vin> -c stopClimat
```

```
python3 vw_carnet_web.py  -u <username> -p <password> -s <spin> -v <vin> -c startWindowMelt
```

```
python3 vw_carnet_web.py  -u <username> -p <password> -s <spin> -v <vin> -c stopWindowMelt
```

...
(see code for more options)

Send all data to the MQTT broker configured in lib_mqtt:
```
python3 vw_carnet.py -u <username> -p <password> -s <spin> -v <vin> -c mqtt
```

See also [FHEM integration](https://forum.fhem.de/index.php/topic,83090.msg886586.html#msg886586)


ORIGINAL README FROM RENEBOER:
# python-carnet-client
Python script we_connect_clinet.py emulates the VW WE Connect web site to send commands to your car and get status.

You must have a VW WE Connect (formerly CarNet) userid and password. Also make sure to logon to the portal https://www.portal.volkswagen-we.com first before using the script. The VW site prompts for several items at first logon the script does not handle.

This script requires the [requests](https://github.com/kennethreitz/requests) library. To install it, run `pip install requests`.

Based of work from wez3 at https://github.com/wez3/volkswagen-carnet-client
It has similar functions and Charging control for electric VW's

The first two parameters are your userid and password (in single quotes!), the optional third is the command.

Avaible commands to the script are:
  startCharge, stopCharge, getCharge, startClimat, stopClimat, getClimat, startClimate, getClimate, stopClimate, startWindowMelt, stopWindowMelt, getWindowMelt

If no command is specified the full car status is retreived.

Command example:
```
python we_connect_client.py '<userid>' '<pwd>' startCharge
```


