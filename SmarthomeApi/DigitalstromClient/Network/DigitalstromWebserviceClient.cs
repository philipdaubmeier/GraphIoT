using DigitalstromClient.Model;
using DigitalstromClient.Model.Apartment;
using DigitalstromClient.Model.Events;
using DigitalstromClient.Model.Heating;
using DigitalstromClient.Model.PropertyTree;
using DigitalstromClient.Model.Structure;
using DigitalstromClient.Model.ZoneData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalstromClient.Network
{
    public class DigitalstromWebserviceClient : AbstractDigitalstromClient
    {
        /// <summary>
        /// Connects to the Digitalstrom DSS REST webservice at the given uri with the given
        /// app and user credentials given via the authentication data object. If
        /// a valid application token is given in the auth data, it is used directly.
        /// </summary>
        /// <param name="baseUri">The uri of the Digitalstrom DSS RESTful webservice</param>
        /// <param name="authData">The authentication information needed to use for
        /// the webservice or to perform a new or renewed authentication</param>
        public DigitalstromWebserviceClient(Uri baseUri, IDigitalstromAuth authData)
            : base(new List<Uri>() { baseUri }, authData) { }

        /// <summary>
        /// Connects to the Digitalstrom DSS REST webservice at one of the given uris with
        /// the given app and user credentials given via the authentication data object. If
        /// a valid application token is given in the auth data, it is used directly.
        /// It is first tried to connect to the local uri - if this fails, i.e. the client
        /// is outside the local LAN, the internet base uri is used subsequently.
        /// </summary>
        /// <param name="localBaseUri">The uri of the Digitalstrom DSS in the local LAN</param>
        /// <param name="internetBaseUri">The internet exposed uri of the Digitalstrom DSS</param>
        /// <param name="authData">The authentication information needed to use for
        /// the webservice or to perform a new or renewed authentication</param>
        public DigitalstromWebserviceClient(Uri localBaseUri, Uri internetBaseUri, IDigitalstromAuth authData)
            : base(new List<Uri>() { localBaseUri, internetBaseUri }, authData) { }

        /// <summary>
        /// Returns a list of sensor relevant for the apartment.
        /// For the apartment the temperature, humidity, and brightness are
        /// sensor types that are tracked.Additionally there is
        /// For each zone the temperature, humidity, CO2 concentration and
        /// brightness are sensor types that are tracked. Typically there is
        /// one device as a zone reference for these values.
        /// If there is no standard device defined for a sensor type or if no
        /// measurement is available there is neither the value or time field
        /// returned.
        /// </summary>
        public async Task<SensorValuesResponse> GetSensorValues()
        {
            return await Load<SensorValuesResponse>(new Uri(await GetBaseUri(), "/json/apartment/getSensorValues"));
        }

        /// <summary>
        /// Returns an object containing the structure of the apartment. This
        /// includes detailed information about all zones, groups and devices.
        /// </summary>
        public async Task<StructureResponse> GetStructure()
        {
            return await Load<StructureResponse>(new Uri(await GetBaseUri(), "/json/apartment/getStructure"));
        }

        /// <summary>
        /// Get the current status of temperature control in all zones.
        /// </summary>
        /// <returns>List of zones with their status values each</returns>
        public async Task<TemperatureControlStatusResponse> GetTemperatureControlStatus()
        {
            return await Load<TemperatureControlStatusResponse>(new Uri(await GetBaseUri(), "/json/apartment/getTemperatureControlStatus"));
        }

        /// <summary>
        /// Returns a list of all temperature control preset values of all zones. Every 
        /// control operation mode has up to 15 presets defined, where 6 of them are 
        /// actually used by the system.
        /// </summary>
        /// <returns>List of zones with their control values each</returns>
        public async Task<TemperatureControlValuesResponse> GetTemperatureControlValues()
        {
            return await Load<TemperatureControlValuesResponse>(new Uri(await GetBaseUri(), "/json/apartment/getTemperatureControlValues"));
        }

        /// <summary>
        /// Get the configuration of the temperature control settings for all zones.
        /// </summary>
        /// <returns>List of zones with their config values each</returns>
        public async Task<TemperatureControlConfigResponse> GetTemperatureControlConfig()
        {
            return await Load<TemperatureControlConfigResponse>(new Uri(await GetBaseUri(), "/json/apartment/getTemperatureControlConfig"));
        }

        /// <summary>
        /// Set the temperature control operation mode preset values for a zone.
        /// Single values can be given and others that do not change may be omitted.
        /// </summary>
        /// <remarks>
        /// Notice: For operation mode ”PID Control” the given values are nominal
        /// temperatures, and for operation mode ”Fixed Values” the given values
        /// are absolute control values.
        /// </remarks>
        /// <param name="id">Zone ID</param>
        /// <param name="comfort">Comfort Preset value for operation mode 1: "Comfort"</param>
        /// <param name="economy">Economy Preset value for operation mode 2: "Economy"</param>
        /// <param name="night">Night Preset value for operation mode 4: "Night"</param>
        /// <param name="holiday">Holiday Preset value for operation mode 5: "Holiday"</param>
        /// <param name="cooling">Cooling Preset value for operation mode 6: "Cooling"</param>
        /// <param name="coolingOff">CoolingOff Preset value for operation mode 7: "CoolingOff"</param>
        /// <param name="off">Off Preset value for operation mode 0: "Off"</param>
        /// <param name="notUsed">NotUsed Preset value for operation mode 3: "Not Used"</param>
        public async Task SetTemperatureControlValues(int id, int? comfort = null, int? economy = null, int? night = null, int? holiday = null, int? cooling = null, int? coolingOff = null, int? off = null, int? notUsed = null)
        {
            await Load(new Uri(await GetBaseUri(), "/json/zone/setTemperatureControlValues")
                .AddQuery("id", id).AddQuery("Off", off).AddQuery("Comfort", comfort).AddQuery("Economy", economy)
                .AddQuery("NotUsed", notUsed).AddQuery("Night", night).AddQuery("Holiday", holiday)
                .AddQuery("Cooling", cooling).AddQuery("CoolingOff", coolingOff));
        }

        /// <summary>
        /// Excutes the scene sceneNumber in a zone for a group of devices.
        /// </summary>
        /// <param name="id">Zone ID of the scene to call</param>
        /// <param name="groupID">Number of the target group</param>
        /// <param name="sceneNumber">Scene number to call</param>
        /// <param name="force">Boolean value, if set a forced scene call is issued. Optional</param>
        public async Task CallScene(int id, int groupID, int sceneNumber, bool? force = null)
        {
            await Load(new Uri(await GetBaseUri(), "/json/zone/callScene")
                .AddQuery("id", id).AddQuery("groupID", groupID)
                .AddQuery("sceneNumber", sceneNumber).AddQuery("force", force));
        }

        /// <summary>
        /// Returns a list of groups which can be controlled by pushbuttons which are actually present in the zone
        /// </summary>
        /// <param name="id">Zone ID</param>
        /// <param name="groupID">Number of the target group</param>
        public async Task<ReachableScenesResponse> GetReachableScenes(int id, int groupID)
        {
            return await Load<ReachableScenesResponse>(new Uri(await GetBaseUri(), "/json/zone/getReachableScenes")
                .AddQuery("id", id).AddQuery("groupID", groupID));
        }

        /// <summary>
        /// Returns the sceneNumber which has been executed last for a group in a zone.
        /// </summary>
        /// <param name="id">Zone ID</param>
        /// <param name="groupID">Number of the target group</param>
        public async Task<LastCalledScenesResponse> GetLastCalledScene(int id, int? groupID = null)
        {
            return await Load<LastCalledScenesResponse>(new Uri(await GetBaseUri(), "/json/zone/getLastCalledScene")
                .AddQuery("id", id).AddQuery("groupID", groupID));
        }
        
        /// <summary>
        /// Returns a list of all zones, each with the last called sceneNumber for all groups.
        /// </summary>
        public async Task<ZonesAndLastCalledScenesResponse> GetZonesAndLastCalledScenes()
        {
            return await Load<ZonesAndLastCalledScenesResponse>(new Uri(await GetBaseUri(), 
                "/json/property/query?query=/apartment/zones/*(ZoneID)/groups/*(group,lastCalledScene)"));
        }
        
        /// <summary>
        /// Returns a list of all zones, each with all sensors and their last sensor values.
        /// </summary>
        public async Task<ZonesAndSensorValuesResponse> GetZonesAndSensorValues()
        {
            return await Load<ZonesAndSensorValuesResponse>(new Uri(await GetBaseUri(),
                "/json/property/query?query=/apartment/zones/*(ZoneID)/groups/group0/sensor/*(type,value,time)"));
        }
        
        /// <summary>
        /// Subscribe to an event with the given name and registers the callers
        /// subscriptionId. A unique subscriptionId can be selected by the subscriber.
        /// It is possible to subscribe to several events reusing the same subscriptionId.
        /// </summary>
        /// <param name="name">Identifier for the event</param>
        /// <param name="subscriptionID">Numerical unique value</param>
        public async Task Subscribe(IEventName name, int subscriptionID)
        {
            await Load(new Uri(await GetBaseUri(), "/json/event/subscribe")
                .AddQuery("name", name.name).AddQuery("subscriptionID", subscriptionID));
        }

        /// <summary>
        /// Unsubscribes for the previously registered events by giving the event name
        /// and the unique subscriptionId.
        /// </summary>
        /// <param name="name">Identifier for the event</param>
        /// <param name="subscriptionID">Numerical unique value</param>
        public async Task Unsubscribe(IEventName name, int subscriptionID)
        {
            await Load(new Uri(await GetBaseUri(), "/json/event/unsubscribe")
                .AddQuery("name", name.name).AddQuery("subscriptionID", subscriptionID));
        }

        /// <summary>
        /// Get event and context information for an event subscription. All events
        /// subscribed with the given Id will be handled by this call. An optional
        /// timeout value in milliseconds can be specified and will block the call
        /// until either an event or the timeout occurs. If the timeout value is zero
        /// or missing the call will not timeout.
        /// </summary>
        /// <param name="subscriptionID">Numerical unique value</param>
        /// <param name="timeout">Numerical value, timeout in milli seconds</param>
        public async Task<EventPollingResponse> PollForEvents(int subscriptionID, int? timeout = null)
        {
            return await Load<EventPollingResponse>(new Uri(await GetBaseUri(), "/json/event/get")
                .AddQuery("subscriptionID", subscriptionID).AddQuery("timeout", timeout));
        }

        /// <summary>
        /// Raises an event and appends it to the digitalSTROM-Server event queue.
        /// Details of the digitalSTROMServer event processing can be found in the
        /// system-interfaces document.
        /// </summary>
        /// <remarks>
        /// Notice: System events should be treated as reserved and must not be
        /// raised by external applications. In this term system events are events
        /// which originate from the digitalSTROM system lower layers.
        /// </remarks>
        /// <param name="name">Identifier for event</param>
        /// <param name="parameters">List of key-value pairs</param>
        public async Task RaiseEvent(IEventName name, List<KeyValuePair<string, string>> parameters = null)
        {
            await Load(new Uri(await GetBaseUri(), "/json/event/raise")
                .AddQuery("name", name.name).AddQuery("parameter", parameters));
        }
    }
}
