#!/usr/bin/python
# Script to control Bernd's Arteon

debug = False

from we_connect_client import *

import re
import requests
import json
import sys

# import correct lib for python v3.x or fallback to v2.x
try:
    import urllib.parse as urlparse
except ImportError:
    # Python 2
    import urlparse

# ---- uncomment to enble http request debugging
try:
    import http.client as http_client
except ImportError:
    # Python 2
    import httplib as http_client
import logging
# ---- end uncomment
import argparse

# import libraries
import lib_mqtt as MQTT

MQTT_TOPIC_IN = "/carnet/#"
MQTT_TOPIC = "/carnet"
MQTT_QOS = 0

def mqtt(s,url_base):
    MQTT.mqttc.publish(MQTT_TOPIC + '/vehicles-owners-verification', CarNetPost(s,'https://www.portal.volkswagen-we.com/portal/group/de/edit-profile','/-/profile/get-vehicles-owners-verification'), qos=0, retain=True)
    #MQTT.mqttc.publish(MQTT_TOPIC + '/request-vsr', CarNetPost(s,url_base, '/-/vsr/request-vsr'), qos=0, retain=True)
    MQTT.mqttc.publish(MQTT_TOPIC + '/new-messages', CarNetPost(s,url_base, '/-/msgc/get-new-messages'), qos=0, retain=True)
    MQTT.mqttc.publish(MQTT_TOPIC + '/vsr', CarNetPost(s,url_base, '/-/vsr/get-vsr'), qos=0, retain=True)
    MQTT.mqttc.publish(MQTT_TOPIC + '/location', CarNetPost(s,url_base, '/-/cf/get-location'), qos=0, retain=True)
    MQTT.mqttc.publish(MQTT_TOPIC + '/vehicle-details', CarNetPost(s,url_base, '/-/vehicle-info/get-vehicle-details'), qos=0, retain=True)
    MQTT.mqttc.publish(MQTT_TOPIC + '/emanager', CarNetPost(s,url_base, '/-/emanager/get-emanager'), qos=0, retain=True)
    #MQTT.mqttc.publish(MQTT_TOPIC + '/request-status', CarNetPost(s,url_base, '/-/rah/get-request-status'), qos=0, retain=True)
    MQTT.mqttc.publish(MQTT_TOPIC + '/status', CarNetPost(s,url_base, '/-/rah/get-status'), qos=0, retain=True)
    MQTT.mqttc.publish(MQTT_TOPIC + '/destination', CarNetPost(s,url_base, '/-/dimp/get-destinations'), qos=0, retain=True)
    MQTT.mqttc.publish(MQTT_TOPIC + '/tours', CarNetPost(s,url_base, '/-/dimp/get-tours'), qos=0, retain=True)
    MQTT.mqttc.publish(MQTT_TOPIC + '/news', CarNetPost(s,url_base, '/-/news/get-news'), qos=0, retain=True)
    #MQTT.mqttc.publish(MQTT_TOPIC + '/latest-trip-statistics', CarNetPost(s,url_base, '/-/rts/get-latest-trip-statistics'), qos=0, retain=True)
    MQTT.mqttc.publish(MQTT_TOPIC + '/car-details', CarNetPost(s,url_base, '/-/mainnavigation/load-car-details/' + getVin(s, url_base, 0)), qos=0, retain=True)
    MQTT.mqttc.publish(MQTT_TOPIC + '/preferred-dealer', CarNetPost(s,url_base, '/-/mainnavigation/get-preferred-dealer'), qos=0, retain=True)
    MQTT.mqttc.publish(MQTT_TOPIC + '/ppoi-list', CarNetPost(s,url_base, '/-/ppoi/get-ppoi-list'), qos=0, retain=True)
    MQTT.mqttc.publish(MQTT_TOPIC + '/fences', CarNetPost(s,url_base, '/-/geofence/get-fences'), qos=0, retain=True)
    return 0

if __name__ == '__main__':

    # Init MQTT connections
    MQTT.init()

    # parse arguments
    parser = argparse.ArgumentParser(description='Control your Connected VW.')
    parser.add_argument('-u', '--user', required=False, help='Your WE-Connect user id.')
    parser.add_argument('-p', '--password', required=False, help='Your WE-Connect password.')
    parser.add_argument('-v', '--vin', help='Your car VIN if more cars on account.')
    parser.add_argument('-c', '--command', choices=['startCharge', 'stopCharge', 'getCharge', 'startClimate', 'stopClimate', 'getClimate', 'startWindowMelt', 'stopWindowMelt','getWindowMelt', 'getVIN', 'remoteLock', 'remoteUnlock', 'startRemoteVentilation', 'stopRemoteVentilation', 'startRemoteHeating', 'stopRemoteHeating', 'getRemoteHeating','getLatestReport', 'getAlerts', 'getGeofences', 'mqtt'], help='Command to send.')
    parser.add_argument('-s', '--spin', help='Your WE-Connect s-pin needed for some commands.')
    parser.add_argument('-i', '--index', type=int, default=0, choices=range(0, 9), help='To get the VIN for the N-th car.')
    parser.add_argument('-d', '--debug', action="store_true", help='Show debug commands.')
    args = parser.parse_args()
    CARNET_USERNAME = args.user
    CARNET_PASSWORD = args.password
    CARNET_COMMAND = ''
    CARNET_VIN = args.vin
    CARNET_SPIN = args.spin
    if args.command:
        CARNET_COMMAND = args.command
    if args.debug:
        debug = True

    # Enable debugging of http requests (gives more details on Python 2 than 3 it seems)
    if debug:
        http_client.HTTPConnection.debuglevel = 1
        logging.basicConfig()
        logging.getLogger().setLevel(logging.DEBUG)
        requests_log = logging.getLogger("urllib3")
        requests_log.setLevel(logging.DEBUG)
        requests_log.propagate = True

    session = requests.Session()
    # Get list of browsers the site can support
    # print(CarNetPost(session, portal_base_url + '/portal/en_GB/web/guest/home', '/-/mainnavigation/get-supported-browsers'))
    # Resp ex: {"errorCode":"0","supportedBrowsersResponse":{"browsers":[{"name":"MS Edge","minimalVersion":"15"},{"name":"Internet Explorer","minimalVersion":"11"},{"name":"Sa$

    # Get list of countries the site can support
    # print(CarNetPost(session, portal_base_url + '/portal/en_GB/web/guest/home', '/-/mainnavigation/get-countries'))

    url, msg = CarNetLogin(session, CARNET_USERNAME, CARNET_PASSWORD)
    if url == '':
        print('Failed to login', msg)
        sys.exit()

    # If a VIN is specified, put that in the base URL so more than just first car can be controlled (not tested)
    if CARNET_VIN:
        vin_start = url.rfind('/',1,-2)
        url = url[0:vin_start+1] + CARNET_VIN + '/'
    else:
        resp = getVIN(session, url, args.index)
        CARNET_VIN = resp.get('vin')

    if debug: print('Using VIN : ' + CARNET_VIN)

    # We need to load a car is spin commands are used
    if CARNET_SPIN:
        CarNetPost(session, url, '/-/mainnavigation/load-car-details/' + CARNET_VIN)

    if CARNET_COMMAND == 'startCharge':
        startCharge(session, url)
    elif CARNET_COMMAND == 'stopCharge':
        stopCharge(session, url)
    elif CARNET_COMMAND == 'getCharge':
        getCharge(session, url)
    elif CARNET_COMMAND == 'startClimat' or CARNET_COMMAND == 'startClimate':
        startClimat(session, url)
    elif CARNET_COMMAND == 'stopClimat' or CARNET_COMMAND == 'stopClimate':
        stopClimat(session, url)
    elif CARNET_COMMAND == 'getClimat' or CARNET_COMMAND == 'getClimate':
        getClimat(session, url)
    elif CARNET_COMMAND == 'startWindowMelt':
        startWindowMelt(session, url)
    elif CARNET_COMMAND == 'stopWindowMelt':
        stopWindowMelt(session, url)
    elif CARNET_COMMAND == 'getWindowMelt':
        getWindowMelt(session, url)
    elif CARNET_COMMAND == 'getVIN':
        print(getVIN(session, url, args.index))
    elif CARNET_COMMAND == 'remoteLock':
        remoteLock(session, url, CARNET_SPIN, CARNET_VIN)
    elif CARNET_COMMAND == 'remoteUnlock':
        remoteUnlock(session, url, CARNET_SPIN, CARNET_VIN)
    elif CARNET_COMMAND == 'startRemoteVentilation':
        startRemoteAccessVentilation(session, url, CARNET_SPIN, CARNET_VIN)
    elif CARNET_COMMAND == 'stopRemoteVentilation':
        stopRemoteAccessVentilation(session, url)
    elif CARNET_COMMAND == 'startRemoteHeating':
        startRemoteAccessHeating(session, url, CARNET_SPIN, CARNET_VIN)
    elif CARNET_COMMAND == 'stopRemoteHeating':
        stopRemoteAccessHeating(session, url)
    elif CARNET_COMMAND == 'getRemoteHeating':
        getRemoteAccessHeating(session, url)
    elif CARNET_COMMAND == 'getLatestReport':
        getLatestReport(session, url)
    elif CARNET_COMMAND == 'getGeofences':
        getGeofences(session, url)
    elif CARNET_COMMAND == 'getAlerts':
        getAlerts(session, url)
    elif CARNET_COMMAND == 'mqtt':
        mqtt(session, url)
    else:
        retrieveCarNetInfo(session, url)

    # End session properly
    print(CarNetPost(session, url, '/-/logout/revoke'))
