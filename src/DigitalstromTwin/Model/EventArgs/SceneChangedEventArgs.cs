using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System;

namespace PhilipDaubmeier.DigitalstromTwin
{
    public class SceneChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The zone (i.e. room) where the scene was changed.
        /// </summary>
        public Zone Zone { get; internal set; }

        /// <summary>
        /// The group/color (i.e. light, shades, video, audio, etc.) where the scene was changed.
        /// </summary>
        public Group Group { get; internal set; }

        /// <summary>
        /// The new scene value after the change.
        /// </summary>
        public Scene Scene { get; internal set; }
    }
}