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

        public int CompareTo(object value)
        {
            return _scene.CompareTo((value as Scene)?._scene ?? value);
        }

        public bool Equals(Scene scene)
        {
            return this == scene;
        }

        public override bool Equals(object obj)
        {
            return this == (obj as Scene);
        }

        public override int GetHashCode()
        {
            return _scene.GetHashCode();
        }

        public override string ToString()
        {
            return Enum.GetName(typeof(SceneCommand), _scene);
        }
        
        public string ToDisplayString()
        {
            switch (_scene)
            {
                case SceneCommand.Preset0: return "Aus";
                case SceneCommand.Preset1: return "Szene 1";
                case SceneCommand.Preset2: return "Szene 2";
                case SceneCommand.Preset3: return "Szene 3";
                case SceneCommand.Preset4: return "Szene 4";
                case SceneCommand.Preset10: return "Szene 10 (Aus)";
                case SceneCommand.Preset11: return "Szene 11";
                case SceneCommand.Preset12: return "Szene 12";
                case SceneCommand.Preset13: return "Szene 13";
                case SceneCommand.Preset14: return "Szene 14";
                case SceneCommand.Preset20: return "Szene 20 (Aus)";
                case SceneCommand.Preset21: return "Szene 21";
                case SceneCommand.Preset22: return "Szene 22";
                case SceneCommand.Preset23: return "Szene 23";
                case SceneCommand.Preset24: return "Szene 24";
                case SceneCommand.Preset30: return "Szene 30 (Aus)";
                case SceneCommand.Preset31: return "Szene 31";
                case SceneCommand.Preset32: return "Szene 32";
                case SceneCommand.Preset33: return "Szene 33";
                case SceneCommand.Preset34: return "Szene 34";
                case SceneCommand.Preset40: return "Szene 40 (Aus)";
                case SceneCommand.Preset41: return "Szene 41";
                case SceneCommand.Preset42: return "Szene 42";
                case SceneCommand.Preset43: return "Szene 43";
                case SceneCommand.Preset44: return "Szene 44";
                case SceneCommand.Area1On: return "Bereich1 Ein";
                case SceneCommand.Area2On: return "Bereich2 Ein";
                case SceneCommand.Area3On: return "Bereich3 Ein";
                case SceneCommand.Area4On: return "Bereich4 Ein";
                case SceneCommand.Area1Off: return "Bereich1 Aus";
                case SceneCommand.Area2Off: return "Bereich2 Aus";
                case SceneCommand.Area3Off: return "Bereich3 Aus";
                case SceneCommand.Area4Off: return "Bereich4 Aus";
                case SceneCommand.AutoOff: return "Langsam Aus";
                case SceneCommand.Impulse: return "Impuls";
                case SceneCommand.AutoStandby: return "A-Standby";
                case SceneCommand.Decrement: return "dunkler";
                case SceneCommand.Increment: return "heller";
                case SceneCommand.Minimum: return "Min";
                case SceneCommand.Maximum: return "Max";
                case SceneCommand.Stop: return "Stop";
                case SceneCommand.Panic: return "Panik";
                case SceneCommand.Standby: return "Standby";
                case SceneCommand.DeepOff: return "Raum aus";
                case SceneCommand.Sleeping: return "Schlafen";
                case SceneCommand.Wakeup: return "Aufwachen";
                case SceneCommand.Present: return "Kommen";
                case SceneCommand.Absent: return "Gehen";
                case SceneCommand.DoorBell: return "Klingeln";
                case SceneCommand.Alarm1: return "Alarm 1";
                case SceneCommand.Alarm2: return "Alarm 2";
                case SceneCommand.Alarm3: return "Alarm 3";
                case SceneCommand.Alarm4: return "Alarm 4";
                case SceneCommand.Fire: return "Feuer";
                case SceneCommand.Smoke: return "Rauch";
                case SceneCommand.Water: return "Wasser";
                case SceneCommand.Gas: return "Gas";
                case SceneCommand.Wind: return "Wind";
                case SceneCommand.NoWind: return "kein Wind";
                case SceneCommand.Rain: return "Regen";
                case SceneCommand.NoRain: return "kein Regen";
                case SceneCommand.Hail: return "Hagel";
                case SceneCommand.NoHail: return "kein Hagel";
                default: return string.Format("Unbekannt ({0})", (int)_scene);
            }
        }
    }
}