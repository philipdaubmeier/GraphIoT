using System;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core
{
    /// <summary>
    /// All scene commands supported by the DSS.
    /// </summary>
    public enum SceneCommand
    {
        ///<summary>Set output value to Preset 0 (Default: Off)</summary>
        Preset0 = 0,
        ///<summary>Set output value to Preset Area 1 Off (Default: Off)</summary>
        Area1Off = 1,
        ///<summary>Set output value to Area 2 Off (Default: Off)</summary>
        Area2Off = 2,
        ///<summary>Set output value to Area 3 Off (Default: Off)</summary>
        Area3Off = 3,
        ///<summary>Set output value to Area 4 Off (Default: Off)</summary>
        Area4Off = 4,
        ///<summary>Set output value to Preset 1 (Default: On)</summary>
        Preset1 = 5,
        ///<summary>Set output value to Preset Area 1 On (Default: On)</summary>
        Area1On = 6,
        ///<summary>Set output value to Area 2 On (Default: On)</summary>
        Area2On = 7,
        ///<summary>Set output value to Area 3 On (Default: On)</summary>
        Area3On = 8,
        ///<summary>Set output value to Area 4 On (Default: On)</summary>
        Area4On = 9,
        ///<summary>Next step to increment or decrement</summary>
        AreaSteppingContinue = 10,
        ///<summary>Decrement output value</summary>
        Decrement = 11,
        ///<summary>Increment output value</summary>
        Increment = 12,
        ///<summary>Minimum output value</summary>
        Minimum = 13,
        ///<summary>Maximum output value</summary>
        Maximum = 14,
        ///<summary>Stop output value change at current position</summary>
        Stop = 15,
        ///<summary>Reserved for future use</summary>
        Reserved = 16,
        ///<summary>Set output value to Preset 2</summary>
        Preset2 = 17,
        ///<summary>Set output value to Preset 3</summary>
        Preset3 = 18,
        ///<summary>Set output value to Preset 4</summary>
        Preset4 = 19,
        ///<summary>Set output value to Preset 12</summary>
        Preset12 = 20,
        ///<summary>Set output value to Preset 13</summary>
        Preset13 = 21,
        ///<summary>Set output value to Preset 14</summary>
        Preset14 = 22,
        ///<summary>Set output value to Preset 22</summary>
        Preset22 = 23,
        ///<summary>Set output value to Preset 23</summary>
        Preset23 = 24,
        ///<summary>Set output value to Preset 24</summary>
        Preset24 = 25,
        ///<summary>Set output value to Preset 32</summary>
        Preset32 = 26,
        ///<summary>Set output value to Preset 33</summary>
        Preset33 = 27,
        ///<summary>Set output value to Preset 34</summary>
        Preset34 = 28,
        ///<summary>Set output value to Preset 42</summary>
        Preset42 = 29,
        ///<summary>Set output value to Preset 43</summary>
        Preset43 = 30,
        ///<summary>Set output value to Preset 44</summary>
        Preset44 = 31,
        ///<summary>Set output value to Preset 10 (Default: Off)</summary>
        Preset10 = 32,
        ///<summary>Set output value to Preset 11 (Default: On)</summary>
        Preset11 = 33,
        ///<summary>Set output value to Preset 20 (Default: Off)</summary>
        Preset20 = 34,
        ///<summary>Set output value to Preset 21 (Default: On)</summary>
        Preset21 = 35,
        ///<summary>Set output value to Preset 30 (Default: Off)</summary>
        Preset30 = 36,
        ///<summary>Set output value to Preset 31 (Default: On)</summary>
        Preset31 = 37,
        ///<summary>Set output value to Preset 40 (Default: Off)</summary>
        Preset40 = 38,
        ///<summary>Set output value to Preset 41 (Default: On)</summary>
        Preset41 = 39,
        ///<summary>Slowly fade down to off value</summary>
        AutoOff = 40,
        ///<summary>Short impulse on the output</summary>
        Impulse = 41,
        ///<summary>Initial command to decrement output value</summary>
        Area1Decrement = 42,
        ///<summary>Initial command to increment output value</summary>
        Area1Increment = 43,
        ///<summary>Initial command to decrement output value</summary>
        Area2Decrement = 44,
        ///<summary>Initial command to increment output value</summary>
        Area2Increment = 45,
        ///<summary>Initial command to decrement output value</summary>
        Area3Decrement = 46,
        ///<summary>Initial command to increment output value</summary>
        Area3Increment = 47,
        ///<summary>Initial command to decrement output value</summary>
        Area4Decrement = 48,
        ///<summary>Initial command to increment output value</summary>
        Area4Increment = 49,
        ///<summary>Local off Device</summary>
        DeviceOff = 50,
        ///<summary>Local on Device</summary>
        DeviceOn = 51,
        ///<summary>Stop output value change at current position</summary>
        Area1Stop = 52,
        ///<summary>Stop output value change at current position</summary>
        Area2Stop = 53,
        ///<summary>Stop output value change at current position</summary>
        Area3Stop = 54,
        ///<summary>Stop output value change at current position</summary>
        Area4Stop = 55,
        ///<summary>Sun Protection</summary>
        SunProtection = 56,
        ///<summary>Reserved for future use</summary>
        Reserved2 = 57,
        ///<summary>Reserved for future use</summary>
        Reserved3 = 58,
        ///<summary>Reserved for future use</summary>
        Reserved4 = 59,
        ///<summary>Reserved for future use</summary>
        Reserved5 = 60,
        ///<summary>Reserved for future use</summary>
        Reserved6 = 61,
        ///<summary>Reserved for future use</summary>
        Reserved7 = 62,
        ///<summary>Reserved for future use</summary>
        Reserved8 = 63,
        ///<summary>Auto Standby</summary>
        AutoStandby = 64,
        ///<summary>Panic</summary>
        Panic = 65,
        ///<summary>Reserved for future use</summary>
        Reserved9 = 66,
        ///<summary>Standby</summary>
        Standby = 67,
        ///<summary>Depp Off</summary>
        DeepOff = 68,
        ///<summary>Sleeping</summary>
        Sleeping = 69,
        ///<summary>Wakeup</summary>
        Wakeup = 70,
        ///<summary>Present</summary>
        Present = 71,
        ///<summary>Absent</summary>
        Absent = 72,
        ///<summary>Door Bell</summary>
        DoorBell = 73,
        ///<summary>Alarm 1</summary>
        Alarm1 = 74,
        ///<summary>Zone Active</summary>
        ZoneActive = 75,
        ///<summary>Fire</summary>
        Fire = 76,
        ///<summary>Smoke Alarm</summary>
        Smoke = 77,
        ///<summary>Water Alarm</summary>
        Water = 78,
        ///<summary>Gas Alarm</summary>
        Gas = 79,
        ///<summary>Reserved for future use</summary>
        Reserved10 = 80,
        ///<summary>Reserved for future use</summary>
        Reserved11 = 81,
        ///<summary>Reserved for future use</summary>
        Reserved12 = 82,
        ///<summary>Alarm 2</summary>
        Alarm2 = 83,
        ///<summary>Alarm 3</summary>
        Alarm3 = 84,
        ///<summary>Alarm 4</summary>
        Alarm4 = 85,
        ///<summary>Wind</summary>
        Wind = 86,
        ///<summary>No Wind</summary>
        NoWind = 87,
        ///<summary>Rain</summary>
        Rain = 88,
        ///<summary>No Rain</summary>
        NoRain = 89,
        ///<summary>Hail</summary>
        Hail = 90,
        ///<summary>No Hail</summary>
        NoHail = 91,
        ///<summary>Unknown - the value was out of range while parsing</summary>
        Unknown = 92
    }

    public class Scene : IComparable, IComparable<Scene>, IEquatable<Scene>
    {
        private readonly SceneCommand _scene;

        public Scene(SceneCommand scene)
        {
            _scene = scene;
        }

        public static implicit operator Scene(SceneCommand scene)
        {
            return new Scene(scene);
        }

        public static implicit operator Scene(long sceneNumber)
        {
            return (int)sceneNumber;
        }

        public static implicit operator Scene(int sceneNumber)
        {
            if (sceneNumber < 0 || sceneNumber >= (int)SceneCommand.Unknown || sceneNumber == (int)SceneCommand.Reserved)
                return new Scene(SceneCommand.Unknown);

            return new Scene((SceneCommand)sceneNumber);
        }

        public static implicit operator Scene(string sceneID)
        {
            if (!int.TryParse(sceneID, out int sceneNumber))
                return new Scene(SceneCommand.Unknown);

            if (sceneNumber < 0 || sceneNumber >= (int)SceneCommand.Unknown || sceneNumber == (int)SceneCommand.Reserved)
                return new Scene(SceneCommand.Unknown);

            return new Scene((SceneCommand)sceneNumber);
        }

        public static implicit operator int(Scene scene)
        {
            return (int)scene._scene;
        }

        public static implicit operator SceneCommand(Scene scene)
        {
            return scene._scene;
        }

        public static bool operator !=(Scene scene1, Scene scene2)
        {
            return !(scene1 == scene2);
        }

        public static bool operator ==(Scene scene1, Scene scene2)
        {
            if (scene1 is null || scene2 is null)
                return ReferenceEquals(scene1, scene2);
            return scene1._scene == scene2._scene;
        }

        public int CompareTo(Scene value)
        {
            return _scene.CompareTo(value._scene);
        }

        public int CompareTo(object? value)
        {
            return _scene.CompareTo((value as Scene)?._scene ?? value);
        }

        public bool Equals(Scene scene)
        {
            return this == scene;
        }

        public override bool Equals(object? obj)
        {
            return obj is Scene scene && this == scene;
        }

        public override int GetHashCode()
        {
            return _scene.GetHashCode();
        }

        public override string ToString()
        {
            return Enum.GetName(typeof(SceneCommand), _scene) ?? string.Empty;
        }

        public string ToDisplayString()
        {
            return _scene switch
            {
                SceneCommand.Preset0 => "Aus",
                SceneCommand.Preset1 => "Szene 1",
                SceneCommand.Preset2 => "Szene 2",
                SceneCommand.Preset3 => "Szene 3",
                SceneCommand.Preset4 => "Szene 4",
                SceneCommand.Preset10 => "Szene 10 (Aus)",
                SceneCommand.Preset11 => "Szene 11",
                SceneCommand.Preset12 => "Szene 12",
                SceneCommand.Preset13 => "Szene 13",
                SceneCommand.Preset14 => "Szene 14",
                SceneCommand.Preset20 => "Szene 20 (Aus)",
                SceneCommand.Preset21 => "Szene 21",
                SceneCommand.Preset22 => "Szene 22",
                SceneCommand.Preset23 => "Szene 23",
                SceneCommand.Preset24 => "Szene 24",
                SceneCommand.Preset30 => "Szene 30 (Aus)",
                SceneCommand.Preset31 => "Szene 31",
                SceneCommand.Preset32 => "Szene 32",
                SceneCommand.Preset33 => "Szene 33",
                SceneCommand.Preset34 => "Szene 34",
                SceneCommand.Preset40 => "Szene 40 (Aus)",
                SceneCommand.Preset41 => "Szene 41",
                SceneCommand.Preset42 => "Szene 42",
                SceneCommand.Preset43 => "Szene 43",
                SceneCommand.Preset44 => "Szene 44",
                SceneCommand.Area1On => "Bereich1 Ein",
                SceneCommand.Area2On => "Bereich2 Ein",
                SceneCommand.Area3On => "Bereich3 Ein",
                SceneCommand.Area4On => "Bereich4 Ein",
                SceneCommand.Area1Off => "Bereich1 Aus",
                SceneCommand.Area2Off => "Bereich2 Aus",
                SceneCommand.Area3Off => "Bereich3 Aus",
                SceneCommand.Area4Off => "Bereich4 Aus",
                SceneCommand.AutoOff => "Langsam Aus",
                SceneCommand.Impulse => "Impuls",
                SceneCommand.AutoStandby => "A-Standby",
                SceneCommand.Decrement => "dunkler",
                SceneCommand.Increment => "heller",
                SceneCommand.Minimum => "Min",
                SceneCommand.Maximum => "Max",
                SceneCommand.Stop => "Stop",
                SceneCommand.Panic => "Panik",
                SceneCommand.Standby => "Standby",
                SceneCommand.DeepOff => "Raum aus",
                SceneCommand.Sleeping => "Schlafen",
                SceneCommand.Wakeup => "Aufwachen",
                SceneCommand.Present => "Kommen",
                SceneCommand.Absent => "Gehen",
                SceneCommand.DoorBell => "Klingeln",
                SceneCommand.Alarm1 => "Alarm 1",
                SceneCommand.Alarm2 => "Alarm 2",
                SceneCommand.Alarm3 => "Alarm 3",
                SceneCommand.Alarm4 => "Alarm 4",
                SceneCommand.Fire => "Feuer",
                SceneCommand.Smoke => "Rauch",
                SceneCommand.Water => "Wasser",
                SceneCommand.Gas => "Gas",
                SceneCommand.Wind => "Wind",
                SceneCommand.NoWind => "kein Wind",
                SceneCommand.Rain => "Regen",
                SceneCommand.NoRain => "kein Regen",
                SceneCommand.Hail => "Hagel",
                SceneCommand.NoHail => "kein Hagel",
                _ => string.Format("Unbekannt ({0})", (int)_scene),
            };
        }
    }
}