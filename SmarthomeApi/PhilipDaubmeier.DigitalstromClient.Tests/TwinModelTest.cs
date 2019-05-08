using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Model.PropertyTree;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.DigitalstromClient.Twin.Tests
{
    public class TwinModelTest
    {
        private readonly Zone zoneLivingroom = 3;
        private readonly Zone zoneKitchen = 32027;

        [Fact]
        public void TestIndexersValueGetSet()
        {
            var model = new ApartmentState();

            model[zoneLivingroom, Color.Yellow].Value = SceneCommand.Preset3;
            model[zoneLivingroom, SensorType.TemperatureIndoors].Value = new SensorTypeAndValues() { Value = 21 };

            Assert.Equal((int)SceneCommand.Preset3, (int)model[zoneLivingroom, Color.Yellow].Value);
            Assert.Equal(21d, model[zoneLivingroom, SensorType.TemperatureIndoors].Value.Value);

            model[zoneLivingroom, Color.Yellow].Value = SceneCommand.AutoOff;
            model[zoneLivingroom, SensorType.TemperatureIndoors].Value = new SensorTypeAndValues() { Value = 99 };

            Assert.Equal((int)SceneCommand.AutoOff, (int)model[zoneLivingroom, Color.Yellow].Value);
            Assert.Equal(99d, model[zoneLivingroom, SensorType.TemperatureIndoors].Value.Value);
        }

        [Fact]
        public void TestIsRoomExisting()
        {
            var model = new ApartmentState();

            Assert.False(model.IsRoomExisting(zoneLivingroom));
            Assert.False(model.IsRoomExisting(zoneKitchen));

            model[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset3;

            Assert.False(model.IsRoomExisting(zoneLivingroom));
            Assert.True(model.IsRoomExisting(zoneKitchen));

            model[zoneLivingroom, SensorType.TemperatureIndoors].Value = new SensorTypeAndValues() { Value = 99 };

            Assert.True(model.IsRoomExisting(zoneLivingroom));
            Assert.True(model.IsRoomExisting(zoneKitchen));
        }

        [Fact]
        public void TestHasSceneValue()
        {
            var model = new ApartmentState();

            Assert.False(model[zoneLivingroom].HasSceneValue(Color.Yellow));

            model[zoneLivingroom, Color.Yellow].Value = SceneCommand.Preset3;

            Assert.True(model[zoneLivingroom].HasSceneValue(Color.Yellow));

            // set back to default/invalid values again
            model[zoneLivingroom, Color.Yellow].Value = SceneCommand.Unknown;

            Assert.False(model[zoneLivingroom].HasSceneValue(Color.Yellow));
        }

        [Fact]
        public void TestHasSensorValue()
        {
            var model = new ApartmentState();

            Assert.False(model[zoneLivingroom].HasSensorValue(SensorType.TemperatureIndoors));

            model[zoneLivingroom, SensorType.TemperatureIndoors].Value = new SensorTypeAndValues() { Value = 99 };

            Assert.True(model[zoneLivingroom].HasSensorValue(SensorType.TemperatureIndoors));

            // set back to default/invalid values again
            model[zoneLivingroom, SensorType.TemperatureIndoors].Value = new SensorTypeAndValues() { Type = SensorType.UnknownType };

            Assert.False(model[zoneLivingroom].HasSensorValue(SensorType.TemperatureIndoors));
        }

        [Fact]
        public void TestGetRoomEnumerable()
        {
            var model = new ApartmentState();

            Assert.Equal(new List<KeyValuePair<Zone, RoomState>>(), model.ToList());

            model[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset3;

            Assert.Equal(new List<KeyValuePair<Zone, RoomState>>()
            {
                new KeyValuePair<Zone, RoomState>(zoneKitchen, model[zoneKitchen])
            }, model.ToList());

            model[zoneLivingroom, SensorType.TemperatureIndoors].Value = new SensorTypeAndValues() { Value = 99 };

            Assert.Equal(new List<KeyValuePair<Zone, RoomState>>()
            {
                new KeyValuePair<Zone, RoomState>(zoneLivingroom, model[zoneLivingroom]),
                new KeyValuePair<Zone, RoomState>(zoneKitchen, model[zoneKitchen])
            }, model.ToList());
        }

        [Fact]
        public void TestGetGroupEnumerable()
        {
            var model = new ApartmentState();

            Assert.Equal(new List<KeyValuePair<Group, SceneState>>(), model[zoneKitchen].Groups.ToList());

            model[zoneKitchen, Color.Yellow].Value = SceneCommand.Preset3;

            Assert.Equal(new List<KeyValuePair<Group, SceneState>>()
            {
                new KeyValuePair<Group, SceneState>(Color.Yellow, model[zoneKitchen, Color.Yellow])
            }, model[zoneKitchen].Groups.ToList());

            model[zoneKitchen, Color.Cyan].Value = SceneCommand.Preset3;

            Assert.Equal(new List<KeyValuePair<Group, SceneState>>()
            {
                new KeyValuePair<Group, SceneState>(Color.Yellow, model[zoneKitchen, Color.Yellow]),
                new KeyValuePair<Group, SceneState>(Color.Cyan, model[zoneKitchen, Color.Cyan])
            }, model[zoneKitchen].Groups.ToList());
        }

        [Fact]
        public void TestGetSensorEnumerable()
        {
            var model = new ApartmentState();

            Assert.Equal(new List<KeyValuePair<Sensor, SensorState>>(), model[zoneLivingroom].Sensors.ToList());

            model[zoneLivingroom, SensorType.BrightnessIndoors].Value = new SensorTypeAndValues() { Value = 51 };

            Assert.Equal(new List<KeyValuePair<Sensor, SensorState>>()
            {
                new KeyValuePair<Sensor, SensorState>(SensorType.BrightnessIndoors, model[zoneLivingroom, SensorType.BrightnessIndoors])
            }, model[zoneLivingroom].Sensors.ToList());

            model[zoneLivingroom, SensorType.TemperatureIndoors].Value = new SensorTypeAndValues() { Value = 99 };

            Assert.Equal(new List<KeyValuePair<Sensor, SensorState>>()
            {
                new KeyValuePair<Sensor, SensorState>(SensorType.TemperatureIndoors, model[zoneLivingroom, SensorType.TemperatureIndoors]),
                new KeyValuePair<Sensor, SensorState>(SensorType.BrightnessIndoors, model[zoneLivingroom, SensorType.BrightnessIndoors])
            }, model[zoneLivingroom].Sensors.ToList());
        }

        [Fact]
        public void TestConvenienceIndexers()
        {
            var model = new ApartmentState();

            model[zoneLivingroom, Color.Yellow].Value = SceneCommand.Preset3;
            model[zoneLivingroom, SensorType.TemperatureIndoors].Value = new SensorTypeAndValues() { Value = 21 };

            // model[zone, group] has to return the same as (model[zone])[group]
            Assert.Equal((int)model[zoneLivingroom][Color.Yellow].Value, (int)model[zoneLivingroom, Color.Yellow].Value);
            Assert.Equal(model[zoneLivingroom][SensorType.TemperatureIndoors].Value, model[zoneLivingroom, SensorType.TemperatureIndoors].Value);
        }

        [Fact]
        public void TestReadEmptyStates()
        {
            var model = new ApartmentState();

            // read state objects from completely empty model - the getter has to silently create them
            Assert.NotNull(model[zoneLivingroom, Color.Yellow]);
            Assert.NotNull(model[zoneLivingroom, SensorType.TemperatureIndoors]);
            Assert.NotNull(model[zoneLivingroom, SensorType.TemperatureIndoors].Value);

            // read values from completely empty model - the getter has to silently create the structure
            // and return default values, i.e. scene "Unknown" and sensors with empty value
            Assert.Equal((int)SceneCommand.Unknown, (int)model[zoneLivingroom, Color.Cyan].Value);
            Assert.Equal(0d, model[zoneLivingroom, SensorType.HumidityIndoors].Value.Value);

            // the same must work without using the convenience indexer with two parameters
            Assert.Equal((int)SceneCommand.Unknown, (int)model[zoneLivingroom][Color.Cyan].Value);
            Assert.Equal(0d, model[zoneLivingroom][SensorType.HumidityIndoors].Value.Value);
        }

        [Fact]
        public async Task TestStateValueTimestamp()
        {
            var model = new ApartmentState();

            model[zoneLivingroom, Color.Yellow].Value = SceneCommand.Preset3;
            var oldTimestamp = model[zoneLivingroom, Color.Yellow].Timestamp;

            await Task.Delay(2);

            model[zoneLivingroom, Color.Yellow].Value = SceneCommand.Preset0;
            var newTimestamp = model[zoneLivingroom, Color.Yellow].Timestamp;

            Assert.NotEqual(newTimestamp, oldTimestamp);
        }

        [Fact]
        public void TestRoomCollectionChangedEvents()
        {
            var model = new ApartmentState();

            // create scene state, one of them is left untouched the other is changed
            model[zoneLivingroom, Color.Cyan].Value = SceneCommand.Preset0;
            model[zoneLivingroom, Color.Yellow].Value = SceneCommand.Preset0;

            var eventCount = 0;
            NotifyCollectionChangedEventArgs args = null;
            model.CollectionChanged += (s, e) => { args = e; eventCount++; };

            // change existing scene state, should yield no collection change
            model[zoneLivingroom, Color.Yellow].Value = SceneCommand.Preset1;
            Assert.Null(args);

            // scene state was not created before, should yield 'add' collection event
            model[zoneKitchen, Color.Black].Value = SceneCommand.Preset4;
            Assert.Equal(NotifyCollectionChangedAction.Add, args.Action);
            Assert.Equal(new List<KeyValuePair<Zone, RoomState>>() { new KeyValuePair<Zone, RoomState>(zoneKitchen, model[zoneKitchen]) }, args.NewItems);
            Assert.Null(args.OldItems);

            Assert.Equal(1, eventCount);
        }

        [Fact]
        public void TestStateCollectionChangedEvents()
        {
            var model = new ApartmentState();

            // create scene and sensor states, half of them is left untouched the other half is changed
            model[zoneLivingroom, Color.Cyan].Value = SceneCommand.Preset0;
            model[zoneLivingroom, Color.Yellow].Value = SceneCommand.Preset0;
            model[zoneLivingroom, SensorType.TemperatureIndoors].Value = new SensorTypeAndValues() { Value = 21 };
            model[zoneLivingroom, SensorType.HumidityIndoors].Value = new SensorTypeAndValues() { Value = 54 };

            var eventCount = 0;
            NotifyCollectionChangedEventArgs args = null;
            model[zoneLivingroom].CollectionChanged += (s, e) => { args = e; eventCount++; };

            // change existing scene state, should yield no collection change
            model[zoneLivingroom, Color.Yellow].Value = SceneCommand.Preset1;
            Assert.Null(args);

            // change existing sensor state, should yield no collection change
            model[zoneLivingroom, SensorType.TemperatureIndoors].Value = new SensorTypeAndValues() { Value = 18 };
            Assert.Null(args);

            // scene state was not created before, should yield 'add' collection event
            model[zoneLivingroom, Color.Black].Value = SceneCommand.Preset4;
            Assert.Equal(NotifyCollectionChangedAction.Add, args.Action);
            Assert.Equal(new List<KeyValuePair<Group, SceneState>>()
            {
                new KeyValuePair<Group, SceneState>(Color.Black, model[zoneLivingroom, Color.Black])
            }, args.NewItems);
            Assert.Null(args.OldItems);

            // sensor state was not created before, should yield 'add' collection event
            model[zoneLivingroom, SensorType.BrightnessIndoors].Value = new SensorTypeAndValues() { Value = 30 };
            Assert.Equal(NotifyCollectionChangedAction.Add, args.Action);
            Assert.Equal(new List<KeyValuePair<Sensor, SensorState>>()
            {
                new KeyValuePair<Sensor, SensorState>(SensorType.BrightnessIndoors, model[zoneLivingroom, SensorType.BrightnessIndoors])
            }, args.NewItems);
            Assert.Null(args.OldItems);

            Assert.Equal(2, eventCount);
        }

        [Fact]
        public void TestPropertyChangedEvents()
        {
            var model = new ApartmentState();

            var changedCountValue = 0;
            var changedCountTimestamp = 0;
            void onChanged(object s, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "Value")
                    changedCountValue++;
                if (e.PropertyName == "Timestamp")
                    changedCountTimestamp++;
            }

            // create scene state, subscribe and change
            model[zoneLivingroom, Color.Yellow].Value = SceneCommand.Preset0;
            model[zoneLivingroom, Color.Yellow].PropertyChanged += onChanged;
            model[zoneLivingroom, Color.Yellow].Value = SceneCommand.Preset1;

            // scene state is not yet created, subscribe and change
            model[zoneKitchen, Color.Black].PropertyChanged += onChanged;
            model[zoneKitchen, Color.Black].Value = SceneCommand.Preset4;

            // create scene state, subscribe and do not change
            model[zoneLivingroom, Color.Cyan].Value = SceneCommand.Preset0;
            model[zoneLivingroom, Color.Cyan].PropertyChanged += onChanged;

            // scene state is not yet created, subscribe and do not change
            model[zoneKitchen, Color.Magenta].PropertyChanged += onChanged;

            Assert.Equal(2, changedCountValue);
            Assert.Equal(2, changedCountTimestamp);
        }
    }
}