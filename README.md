# Python Volkswagen CarNet Client

This is a Python client for Volkswagen CarNet, it simulates the behaviour of the CarNet app. It allows users to retrieve information about the vehicle (location, temperature and mileage), next to this the Window melt and Climat functionalities can be started from the Python script.

# Installation

Clone the Github page and modify the following values in the vw_carnet.py script:
```
CARNET_USERNAME = ''
CARNET_PASSWORD = ''
CARNET_SPIN = ''
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
