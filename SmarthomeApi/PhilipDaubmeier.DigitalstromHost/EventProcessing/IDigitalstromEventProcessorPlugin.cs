using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromClient.Model.Events;
using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromHost.EventProcessing
{
    public interface IDigitalstromEventProcessorPlugin
    {
        /// <summary>
        /// Returns the event names this plugin is handling
        /// </summary>
        IEnumerable<IEventName> EventNames { get; }

        /// <summary>
        /// Returns the current time span of the underlying event stream of this plugin
        /// </summary>
        TimeSeriesSpan Span { get; }

        /// <summary>
        /// Writes the given event to the event stream of this plugin
        /// </summary>
        void WriteToEventStream(DssEvent dsEvent);

        /// <summary>
        /// Checks if the last event in the event stream is identical to the given one
        /// and the timestamp differs by at most the given milliseconds.
        /// </summary>
        bool HasDuplicate(DssEvent dsEvent, int milliseconds);

        /// <summary>
        /// Reads the event stream for the given day from the db or creates a new one if not existing yet.
        /// </summary>
        void ReadOrCreateEventStream(DateTime date);

        /// <summary>
        /// Saves the event stream to the database by updating an existing row or creating a new one if
        /// not existing yet.
        /// </summary>
        void SaveEventStreamToDb();
    }
}