using PhilipDaubmeier.DigitalstromClient.Model;
using PhilipDaubmeier.DigitalstromClient.Model.Auth;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.Energy;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using PhilipDaubmeier.DigitalstromClient.Model.Heating;
using PhilipDaubmeier.DigitalstromClient.Model.PropertyTree;
using PhilipDaubmeier.DigitalstromClient.Model.SensorData;
using PhilipDaubmeier.DigitalstromClient.Model.Structure;
using PhilipDaubmeier.DigitalstromClient.Model.ZoneData;
using PhilipDaubmeier.DigitalstromClient.Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhilipDaubmeier.DigitalstromClient
{
    public class DigitalstromDssClient : DigitalstromAuthenticatingClientBase
    {
        /// <summary>
        /// Connects to the Digitalstrom DSS REST webservice with the given connection
        /// provider uris and credentials.
        /// </summary>
        /// <param name="connectionProvider">All necessary connection infos like uris and
        /// authentication data needed to use for the webservice or to perform a new or
        /// renewed authentication</param>
        public DigitalstromDssClient(IDigitalstromConnectionProvider connectionProvider)
            : base(connectionProvider) { }

        /// <summary>
        /// Connects to the Digitalstrom DSS REST webservice at the given uri with
        /// the given app and user credentials given via the authentication data object. If
        /// a valid application token is given in the auth data, it is used directly.
        /// </summary>
        /// <param name="uri">The uri of the Digitalstrom DSS</param>
        /// <param name="authData">The authentication information needed to use for
        /// the webservice or to perform a new or renewed authentication</param>
        public DigitalstromDssClient(Uri uri, IDigitalstromAuth authData)
            : base(new DigitalstromConnectionProvider(new List<Uri>() { uri }, authData)) { }

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
        public DigitalstromDssClient(Uri localBaseUri, Uri internetBaseUri, IDigitalstromAuth authData)
            : base(new DigitalstromConnectionProvider(new List<Uri>() { localBaseUri, internetBaseUri }, authData)) { }

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
            return await Load<SensorValuesResponse>(new Uri("/json/apartment/getSensorValues", UriKind.Relative));
        }

        /// <summary>
        /// Returns an object containing the structure of the apartment. This
        /// includes detailed information about all zones, groups and devices.
        /// </summary>
        public async Task<Apartment> GetStructure()
        {
            return (await Load<StructureResponse>(new Uri("/json/apartment/getStructure", UriKind.Relative)))?.Apartment;
        }

        /// <summary>
        /// Get the current status of temperature control in all zones.
        /// </summary>
        /// <returns>List of zones with their status values each</returns>
        public async Task<TemperatureControlStatusResponse> GetTemperatureControlStatus()
        {
            return await Load<TemperatureControlStatusResponse>(new Uri("/json/apartment/getTemperatureControlStatus", UriKind.Relative));
        }

        /// <summary>
        /// Returns a list of all temperature control preset values of all zones. Every 
        /// control operation mode has up to 15 presets defined, where 6 of them are 
        /// actually used by the system.
        /// </summary>
        /// <returns>List of zones with their control values each</returns>
        public async Task<TemperatureControlValuesResponse> GetTemperatureControlValues()
        {
            return await Load<TemperatureControlValuesResponse>(new Uri("/json/apartment/getTemperatureControlValues", UriKind.Relative));
        }

        /// <summary>
        /// Get the configuration of the temperature control settings for all zones.
        /// </summary>
        /// <returns>List of zones with their config values each</returns>
        public async Task<TemperatureControlConfigResponse> GetTemperatureControlConfig()
        {
            return await Load<TemperatureControlConfigResponse>(new Uri("/json/apartment/getTemperatureControlConfig", UriKind.Relative));
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
        /// <param name="zone">Zone ID</param>
        /// <param name="comfort">Comfort Preset value for operation mode 1: "Comfort"</param>
        /// <param name="economy">Economy Preset value for operation mode 2: "Economy"</param>
        /// <param name="night">Night Preset value for operation mode 4: "Night"</param>
        /// <param name="holiday">Holiday Preset value for operation mode 5: "Holiday"</param>
        /// <param name="cooling">Cooling Preset value for operation mode 6: "Cooling"</param>
        /// <param name="coolingOff">CoolingOff Preset value for operation mode 7: "CoolingOff"</param>
        /// <param name="off">Off Preset value for operation mode 0: "Off"</param>
        /// <param name="notUsed">NotUsed Preset value for operation mode 3: "Not Used"</param>
        public async Task SetTemperatureControlValues(Zone zone, int? comfort = null, int? economy = null, int? night = null, int? holiday = null, int? cooling = null, int? coolingOff = null, int? off = null, int? notUsed = null)
        {
            await Load(new Uri("/json/zone/setTemperatureControlValues", UriKind.Relative)
                .AddQuery("id", zone).AddQuery("Off", off).AddQuery("Comfort", comfort).AddQuery("Economy", economy)
                .AddQuery("NotUsed", notUsed).AddQuery("Night", night).AddQuery("Holiday", holiday)
                .AddQuery("Cooling", cooling).AddQuery("CoolingOff", coolingOff));
        }

        /// <summary>
        /// Excutes the scene in a zone for a group of devices.
        /// </summary>
        /// <param name="zone">Zone ID of the scene to call</param>
        /// <param name="group">Target group to call the scene</param>
        /// <param name="scene">Scene to call</param>
        /// <param name="force">Boolean value, if set a forced scene call is issued. Optional</param>
        public async Task CallScene(Zone zone, Group group, Scene scene, bool? force = null)
        {
            await Load(new Uri("/json/zone/callScene", UriKind.Relative)
                .AddQuery("id", zone).AddQuery("groupID", group)
                .AddQuery("sceneNumber", scene).AddQuery("force", force));
        }

        /// <summary>
        /// Returns a list of scenes which can be called in the given zone
        /// </summary>
        /// <param name="zone">Zone ID</param>
        /// <param name="group">The target group</param>
        public async Task<ReachableScenesResponse> GetReachableScenes(Zone zone, Group group)
        {
            return await Load<ReachableScenesResponse>(new Uri("/json/zone/getReachableScenes", UriKind.Relative)
                .AddQuery("id", zone).AddQuery("groupID", group ?? (int?)null));
        }

        /// <summary>
        /// Returns the scene that has been executed last for a group in a zone.
        /// </summary>
        /// <param name="zone">Zone ID</param>
        /// <param name="group">The target group</param>
        public async Task<LastCalledScenesResponse> GetLastCalledScene(Zone zone, Group group = null)
        {
            return await Load<LastCalledScenesResponse>(new Uri("/json/zone/getLastCalledScene", UriKind.Relative)
                .AddQuery("id", zone).AddQuery("groupID", group ?? (int?)null));
        }
        
        /// <summary>
        /// Returns a list of all zones, each with the last called sceneNumber for all groups.
        /// </summary>
        public async Task<ZonesAndLastCalledScenesResponse> GetZonesAndLastCalledScenes()
        {
            return await QueryPropertyTree<ZonesAndLastCalledScenesResponse>(
                "/apartment/zones/*(ZoneID)/groups/*(group,lastCalledScene)");
        }
        
        /// <summary>
        /// Returns a list of all zones, each with all sensors and their last sensor values.
        /// </summary>
        public async Task<ZonesAndSensorValuesResponse> GetZonesAndSensorValues()
        {
            return await QueryPropertyTree<ZonesAndSensorValuesResponse>(
                "/apartment/zones/*(ZoneID)/groups/group0/sensor/*(type,value,time)");
        }
        
        /// <summary>
        /// Returns a list of all circuits and an info about their energy metering capabilities
        /// </summary>
        public async Task<MeteringCircuitsResponse> GetMeteringCircuits()
        {
            return await QueryPropertyTree<MeteringCircuitsResponse>(
                "/apartment/dSMeters/*(dSUID,name)/capabilities(metering)");
        }

        /// <summary>
        /// Returns a list of all circuits and their associated zone ids.
        /// </summary>
        public async Task<CircuitZonesResponse> GetCircuitZones()
        {
            return await QueryPropertyTree<CircuitZonesResponse>(
                "/apartment/dSMeters/*(dSUID)/zones/*(ZoneID)");
        }

        /// <summary>
        /// Returns the given DSS property tree query result. Type parameter T has to match the queried structure.
        /// </summary>
        public async Task<T> QueryPropertyTree<T>(string query) where T : class, IWiremessagePayload
        {
            return await Load<T>(new Uri($"/json/property/query?query={query}", UriKind.Relative));
        }

        /// <summary>
        /// Returns a time series of values for the total power consumption of all circuits.
        /// </summary>
        /// <param name="resolution">The desired resolution in seconds for the values</param>
        /// <param name="count">The requested number of values</param>
        public async Task<EnergyMeteringResponse> GetTotalEnergy(int resolution, int count)
        {
            return await Load<EnergyMeteringResponse>(new Uri("/json/metering/getValues", UriKind.Relative)
                .AddQuery("dsuid", ".meters(all)").AddQuery("type", "consumption")
                .AddQuery("resolution", resolution).AddQuery("valueCount", count));
        }

        /// <summary>
        /// Returns a time series of values for the power consumption of the given circuit.
        /// </summary>
        /// <param name="meterDsuid">The circuits digitalstrom unique id</param>
        /// <param name="resolution">The desired resolution in seconds for the values</param>
        /// <param name="count">The requested number of values</param>
        public async Task<EnergyMeteringResponse> GetEnergy(Dsuid meterDsuid, int resolution, int count)
        {
            return await Load<EnergyMeteringResponse>(new Uri("/json/metering/getValues", UriKind.Relative)
                .AddQuery("dsuid", meterDsuid).AddQuery("type", "consumption")
                .AddQuery("resolution", resolution).AddQuery("valueCount", count));
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
            await Load(new Uri("/json/event/subscribe", UriKind.Relative)
                .AddQuery("name", name.Name).AddQuery("subscriptionID", subscriptionID));
        }

        /// <summary>
        /// Unsubscribes for the previously registered events by giving the event name
        /// and the unique subscriptionId.
        /// </summary>
        /// <param name="name">Identifier for the event</param>
        /// <param name="subscriptionID">Numerical unique value</param>
        public async Task Unsubscribe(IEventName name, int subscriptionID)
        {
            await Load(new Uri("/json/event/unsubscribe", UriKind.Relative)
                .AddQuery("name", name.Name).AddQuery("subscriptionID", subscriptionID));
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
            return await Load<EventPollingResponse>(new Uri("/json/event/get", UriKind.Relative)
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
            await Load(new Uri("/json/event/raise", UriKind.Relative)
                .AddQuery("name", name.Name).AddQuery("parameter", parameters));
        }
    }
}