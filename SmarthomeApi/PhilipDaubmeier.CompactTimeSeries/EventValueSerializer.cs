using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PhilipDaubmeier.CompactTimeSeries
{
    public class EventValueSerializer<TValue> : IEventSerializer<Tuple<DateTime, TValue>> where TValue : struct
    {
        /// <summary>
        /// Private helper class for creating Tuples that inherit the type parameter
        /// from the serializer, but construct them from a known number type. For instance,
        /// if you have a int value you can neither construct nor cast it to a generic type
        /// parameter T and also not build a Tuple from it. This factory, however, can
        /// be created with type parmeter T and then be casted to a concrete type.
        /// 
        /// Example usage:
        ///     var factory = new TupleFactory<T>();
        ///     int value = 23;
        ///     (factory as TupleFactory<int>).Set(time, value);
        ///     Tuple<DateTime, T> result = factory.Create();
        /// </summary>
        private class TupleFactory<Tfac> where Tfac : struct
        {
            private DateTime _time;
            private Tfac _value;

            public void Set(DateTime time, Tfac value)
            {
                _time = time;
                _value = value;
            }

            public Tuple<DateTime, Tfac> Create()
            {
                return new Tuple<DateTime, Tfac>(_time, _value);
            }
        }

        private TupleFactory<TValue> _tupleFactory = new TupleFactory<TValue>();

        /// <summary>
        /// See <see cref="IEventSerializer{TEvent}.SizeOf"/>
        /// </summary>
        public int SizeOf => Marshal.SizeOf<TValue>();

        /// <summary>
        /// See <see cref="IEventSerializer{TEvent}.GetTimestampUtc(TEvent)"/>
        /// </summary>
        public DateTime GetTimestampUtc(Tuple<DateTime, TValue> eventObj) => eventObj?.Item1 ?? DateTime.MinValue;

        /// <summary>
        /// See <see cref="IEventSerializer{TEvent}.CanSerialize(TEvent)"/>
        /// </summary>
        public bool CanSerialize(Tuple<DateTime, TValue> eventObj) => true;

        /// <summary>
        /// See <see cref="IEventSerializer{TEvent}.Serialize(BinaryWriter, TEvent)"/>
        /// </summary>
        public void Serialize(BinaryWriter writer, Tuple<DateTime, TValue> eventObj)
        {
            if (typeof(TValue) != typeof(int) && typeof(TValue) != typeof(double))
                throw new InvalidOperationException("EventValueSerializer only supports serializing int and double");

            if (writer == null)
                throw new ArgumentNullException("writer");

            if (eventObj == null)
                throw new ArgumentNullException("eventObj");

            var nullable = new TValue?(eventObj.Item2);
            if (typeof(TValue) == typeof(int))
                writer.Write((nullable as int?) ?? default(int));
            else if (typeof(TValue) == typeof(double))
                writer.Write((nullable as double?) ?? default(double));
        }

        /// <summary>
        /// See <see cref="IEventSerializer{TEvent}.Deserialize(BinaryReader, DateTime)"/>
        /// </summary>
        public Tuple<DateTime, TValue> Deserialize(BinaryReader reader, DateTime timestampUtc)
        {
            if (typeof(TValue) != typeof(int) && typeof(TValue) != typeof(double))
                throw new InvalidOperationException("EventValueSerializer only supports deserializing int and double");

            if (reader == null)
                throw new ArgumentNullException("reader");

            if (typeof(TValue) == typeof(int))
                (_tupleFactory as TupleFactory<int>).Set(timestampUtc, reader.ReadInt32());
            else if (typeof(TValue) == typeof(double))
                (_tupleFactory as TupleFactory<double>).Set(timestampUtc, reader.ReadDouble());
            
            return _tupleFactory.Create();
        }
    }
}