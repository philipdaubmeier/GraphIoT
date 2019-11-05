# Python Volkswagen CarNet Client

This is a Python client for Volkswagen CarNet - aka VW We Connect -, it simulates the behaviour of the CarNet web page. It allows users to retrieve information about the vehicle (such as location, temperature and mileage), next to this the Window melt and Climat functionalities can be started from the Python script. This was built based on work from wez3 and reneboer.

# Installation

Clone the Github repo and if you want to use mqtt, enter the correct MQTT-broker host and port in the lib_mqtt.py:

```
MQTT_HOST = "<hostname or IP>" <- host where your mqtt broker runs
MQTT_PORT = <port> <- port of your mqtt service, default is 1883
```

Install requests and paho-mqtt:

```
pip install requests paho-mqtt
```
or for Python3:
```
pip3 install requests paho-mqtt
```



# Usage

Run the script with arguments. If you only own one car in VW we, the vin parameter is obsolete.

```
python3 we_connect_client.py -u <username> -p <password> [-s <spin>] [-v <vin>] -c <command> [-d]
```

Add -d for debugging output to the console if needed.

The following commands are supported:

* startCharge
* stopCharge
* getCharge
* startClimat
* startClimate
* stopClimat
* stopClimate
* getClimat
* getClimate
* startWindowMelt
* stopWindowMelt
* getWindowMelt
* getVIN
* remoteLock [spin parameter needed, availability is country-depending]
* remoteUnlock [spin parameter needed, availability is country-depending]
* startRemoteVentilation [spin parameter needed]
* stopRemoteVentilation
* startRemoteHeating [spin parameter needed]
* stopRemoteHeating
* getRemoteHeating
* getLatestReport    
* getGeofences
* getAlerts
* retrieveCarNetInfo

# Send all data to the MQTT broker configured in lib_mqtt:

To allow this, I added a new file called my-car.py to the original repository from reneboer. It reuses all functions from we_connect_client but adds one new command: mqtt.
```
python3 my-car.py -u <username> -p <password> -s <spin> -v <vin> -c mqtt
```
If you like, you can use my-car.py for all commands including the above - it will forward everything besides mqtt to the original code.
See also [FHEM integration](https://forum.fhem.de/index.php/topic,83090.msg886586.html#msg886586)

# Credits

All this is possible due to the excellent work of reneboer and wez3. See details in his repository [here](https://github.com/reneboer/python-carnet-client)

# Known issues

Calling startRemoteVentilation does not switch the car from heating to ventilation mode. It is not yet clear what makes the car switch over to the ventilation mode. Suggestions are welcome.
