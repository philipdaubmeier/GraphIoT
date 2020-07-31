using System;
using System.Globalization;

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

    public class Scene : IComparable, IComparable<Scene>, IEquatable<Scene>, IFormattable
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

        public static bool operator !=(Scene? scene1, Scene? scene2)
        {
            return !(scene1 == scene2);
        }

        public static bool operator ==(Scene? scene1, Scene? scene2)
        {
            if (scene1 is null || scene2 is null)
                return ReferenceEquals(scene1, scene2);
            return scene1._scene == scene2._scene;
        }

        public int CompareTo(Scene? value)
        {
            return _scene.CompareTo(value?._scene);
        }

        public int CompareTo(object? value)
        {
            return _scene.CompareTo((value as Scene)?._scene ?? value);
        }

        public bool Equals(Scene? scene)
        {
            return this == scene;
        }

        public override bool Equals(object? obj)
        {
            return obj is Scene scene && this == scene;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_scene);
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation
        /// using the specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">
        /// Null for an invariant default format 'ID {scene-id}: {scene-name}'.
        /// "D" or "d" for a localized displayable string of the scene name,
        /// if available for the given language of the format provider.
        /// </param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <returns>
        /// The string representation of the value of this instance as specified by format and provider.
        /// </returns>
        public string ToString(string? format = null, IFormatProvider? formatProvider = null)
        {
            if (format is null)
                return $"ID {(int)_scene}: {Enum.GetName(typeof(SceneCommand), _scene) ?? string.Empty}";

            if (!format.Equals("d", StringComparison.InvariantCultureIgnoreCase))
                throw new FormatException($"Did not recognize format '{format}'");

            if (formatProvider is CultureInfo culture)
                Locale.Scene.Culture = culture;

            return _scene switch
            {
                SceneCommand.Preset0 => Locale.Scene.Off,
                SceneCommand.Preset1 => Locale.Scene.Scene1,
                SceneCommand.Preset2 => Locale.Scene.Scene2,
                SceneCommand.Preset3 => Locale.Scene.Scene3,
                SceneCommand.Preset4 => Locale.Scene.Scene4,
                SceneCommand.Preset10 => Locale.Scene.Scene10,
                SceneCommand.Preset11 => Locale.Scene.Scene11,
                SceneCommand.Preset12 => Locale.Scene.Scene12,
                SceneCommand.Preset13 => Locale.Scene.Scene13,
                SceneCommand.Preset14 => Locale.Scene.Scene14,
                SceneCommand.Preset20 => Locale.Scene.Scene20,
                SceneCommand.Preset21 => Locale.Scene.Scene21,
                SceneCommand.Preset22 => Locale.Scene.Scene22,
                SceneCommand.Preset23 => Locale.Scene.Scene23,
                SceneCommand.Preset24 => Locale.Scene.Scene24,
                SceneCommand.Preset30 => Locale.Scene.Scene30,
                SceneCommand.Preset31 => Locale.Scene.Scene31,
                SceneCommand.Preset32 => Locale.Scene.Scene32,
                SceneCommand.Preset33 => Locale.Scene.Scene33,
                SceneCommand.Preset34 => Locale.Scene.Scene34,
                SceneCommand.Preset40 => Locale.Scene.Scene40,
                SceneCommand.Preset41 => Locale.Scene.Scene41,
                SceneCommand.Preset42 => Locale.Scene.Scene42,
                SceneCommand.Preset43 => Locale.Scene.Scene43,
                SceneCommand.Preset44 => Locale.Scene.Scene44,
                SceneCommand.Area1On => Locale.Scene.Area1On,
                SceneCommand.Area2On => Locale.Scene.Area2On,
                SceneCommand.Area3On => Locale.Scene.Area3On,
                SceneCommand.Area4On => Locale.Scene.Area4On,
                SceneCommand.Area1Off => Locale.Scene.Area1Off,
                SceneCommand.Area2Off => Locale.Scene.Area2Off,
                SceneCommand.Area3Off => Locale.Scene.Area3Off,
                SceneCommand.Area4Off => Locale.Scene.Area4Off,
                SceneCommand.AutoOff => Locale.Scene.AutoOff,
                SceneCommand.Impulse => Locale.Scene.Impulse,
                SceneCommand.AutoStandby => Locale.Scene.AutoStandby,
                SceneCommand.Decrement => Locale.Scene.DimDown,
                SceneCommand.Increment => Locale.Scene.DimUp,
                SceneCommand.Minimum => Locale.Scene.Min,
                SceneCommand.Maximum => Locale.Scene.Max,
                SceneCommand.Stop => Locale.Scene.Stop,
                SceneCommand.Panic => Locale.Scene.Panic,
                SceneCommand.Standby => Locale.Scene.Standby,
                SceneCommand.DeepOff => Locale.Scene.RoomOff,
                SceneCommand.Sleeping => Locale.Scene.Sleeping,
                SceneCommand.Wakeup => Locale.Scene.Wakeup,
                SceneCommand.Present => Locale.Scene.ComingHome,
                SceneCommand.Absent => Locale.Scene.LeaveHome,
                SceneCommand.DoorBell => Locale.Scene.Doorbell,
                SceneCommand.Alarm1 => Locale.Scene.Alarm1,
                SceneCommand.Alarm2 => Locale.Scene.Alarm2,
                SceneCommand.Alarm3 => Locale.Scene.Alarm3,
                SceneCommand.Alarm4 => Locale.Scene.Alarm4,
                SceneCommand.Fire => Locale.Scene.Fire,
                SceneCommand.Smoke => Locale.Scene.Smoke,
                SceneCommand.Water => Locale.Scene.Water,
                SceneCommand.Gas => Locale.Scene.Gas,
                SceneCommand.Wind => Locale.Scene.Wind,
                SceneCommand.NoWind => Locale.Scene.Nowind,
                SceneCommand.Rain => Locale.Scene.Rain,
                SceneCommand.NoRain => Locale.Scene.NoRain,
                SceneCommand.Hail => Locale.Scene.Hail,
                SceneCommand.NoHail => Locale.Scene.Nohail,
                _ => $"{Locale.Scene.Unknown} ({(int)_scene})"
            };
        }
    }
}