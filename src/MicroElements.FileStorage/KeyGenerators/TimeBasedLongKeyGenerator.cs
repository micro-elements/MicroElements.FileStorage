// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage.KeyGenerators
{
    /// <summary>
    /// TimeBasedLongKeyGenerator generates key as long value. Very usefull in scenarios where long key needed.
    /// Value depends on time, nodeId and counter. There are two precisions for long: 64 bit and 53 bit (see <see cref="Precision"/>).
    /// Parts of generated keys 53bit:
    /// Time    : yyMMddHHmmss (12 digits)
    /// NodeId  : 00           ( 2 digits)
    /// Counter : 00           ( 2 digits)
    /// MaxCapacity: 99 values per second for one nodeId, max 99 nodes.
    /// MaxValue: 89_12_31_23_59_59_99_99 (16 digits)
    /// ==================================================================
    /// Parts of generated keys 64bit:
    /// Time    : yyMMddHHmmss (12 digits)
    /// NodeId  : 000          ( 3 digits)
    /// Counter : 0000         ( 4 digits)
    /// MaxCapacity: 9999 values per second for one nodeId? max 999 nodes.
    /// MaxValue: 92_12_31_23_59_59_999_9999 (19 digits)
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class TimeBasedLongKeyGenerator<T> : IKeyGenerator<T> where T : class
    {
        private static long MaxValue = 92_12_31_23_59_59_999_9999; // 19 digits
        private static long MaxValJs = 89_12_31_23_59_59_99_99;    // 16 digits

        private readonly Precision _precision;
        private long _timeWindow;
        private int _counter;

        public enum Precision
        {
            /// <summary>
            /// Standard .Net long precision (64bit).
            /// </summary>
            BigNumber64Bits,

            /// <summary>
            /// JavaScript IEEE 754 numbers (53bit).
            /// For explanation see: http://www.ietf.org/mail-archive/web/json/current/msg00297.html
            /// </summary>
            JavaScript53Bits
        }

        public KeyType KeyStrategy => KeyType.UniqId;

        /// <summary>
        /// GetNow function.
        /// </summary>
        public Func<DateTime> GetNow { get; }

        /// <summary>
        /// GetNode function.
        /// </summary>
        public Func<int> GetNode { get; }

        /// <summary>
        /// GetCounter function.
        /// </summary>
        public Func<int> GetCounter { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeBasedLongKeyGenerator{T}"/> class.
        /// </summary>
        /// <param name="getNow">GetNow function.</param>
        /// <param name="getNode">GetNode function.</param>
        /// <param name="getCounter">GetCounter function.</param>
        /// <param name="precision">Precision for long value. <see cref="Precision.JavaScript53Bits"/> by default.</param>
        public TimeBasedLongKeyGenerator(Func<DateTime> getNow, Func<int> getNode, Func<int> getCounter, Precision precision = Precision.JavaScript53Bits)
        {
            _precision = precision;
            GetNow = getNow;
            GetNode = getNode;
            GetCounter = getCounter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeBasedLongKeyGenerator{T}"/> class.
        /// </summary>
        /// <param name="nodeId">NodeId in range 1..99</param>
        /// <param name="precision">Precision for long value. <see cref="Precision.JavaScript53Bits"/> by default.</param>
        public TimeBasedLongKeyGenerator(int nodeId = 1, Precision precision = Precision.JavaScript53Bits)
        {
            if (precision == Precision.JavaScript53Bits && nodeId < 1 && nodeId > 99)
                throw new ArgumentException("NodeId should be in range 1..99");
            if (precision == Precision.BigNumber64Bits && nodeId < 1 && nodeId > 999)
                throw new ArgumentException("NodeId should be in range 1..999");

            _precision = precision;
            GetNow = () => DateTime.Now;
            GetNode = () => nodeId;
            GetCounter = CounterDefault;
        }

        /// <inheritdoc />
        public Key GetNextKey(IDataStore dataStore, T entity)
        {
            if (_precision == Precision.JavaScript53Bits)
            {
                var dateTimePart = GetNow().ToString("yyMMddHHmmss");    // 12 digits (time)
                var nodePart = GetNode().ToString("00");                 // 2 digits  (node)
                var counterPart = GetCounter().ToString("00");           // 2 digits  (counter)
                var keyValue = $"{dateTimePart}{nodePart}{counterPart}"; // 16 digits (total)

                return new Key(KeyType.UniqId, keyValue);
            }
            else
            {
                var dateTimePart = GetNow().ToString("yyMMddHHmmss");    // 12 digits (time)
                var nodePart = GetNode().ToString("000");                // 3 digits  (node)
                var counterPart = GetCounter().ToString("0000");         // 4 digits  (counter)
                var keyValue = $"{dateTimePart}{nodePart}{counterPart}"; // 19 digits (total)

                return new Key(KeyType.UniqId, keyValue);
            }
        }

        private int CounterDefault()
        {
            var timeWindow = Trim(GetNow(), TimeSpan.TicksPerSecond).Ticks;
            if (timeWindow != _timeWindow)
            {
                // Other time window so resetting counter
                Interlocked.Exchange(ref _counter, 0);
                Interlocked.Exchange(ref _timeWindow, timeWindow);
            }

            return Interlocked.Increment(ref _counter);
        }

        private static DateTime Trim(DateTime date, long roundTicks)
        {
            return new DateTime(date.Ticks - (date.Ticks % roundTicks));
        }
    }
}
