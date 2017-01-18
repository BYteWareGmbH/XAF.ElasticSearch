namespace BYteWare.Utils.Extension
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Partitioner who returns ordered elements as partitions
    /// </summary>
    /// <typeparam name="TSource">Type for the elements of the list</typeparam>
    public class OrderableListPartitioner<TSource> : OrderablePartitioner<TSource>
    {
        private readonly IList<TSource> input;

        /// <summary>
        /// Initalizes a new instance of the <see cref="OrderableListPartitioner&lt;TSource&gt;"/> class.
        /// </summary>
        /// <param name="input">The source list</param>
        public OrderableListPartitioner(IList<TSource> input)
            : base(true, false, true)
        {
            this.input = input;
        }

        /// <inheritdoc/>
        public override bool SupportsDynamicPartitions
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc/>
        public override IList<IEnumerator<KeyValuePair<long, TSource>>> GetOrderablePartitions(int partitionCount)
        {
            var dynamicPartitions = GetOrderableDynamicPartitions();
            var partitions = new IEnumerator<KeyValuePair<long, TSource>>[partitionCount];

            for (int i = 0; i < partitionCount; i++)
            {
                partitions[i] = dynamicPartitions.GetEnumerator();
            }
            return partitions;
        }

        /// <inheritdoc/>
        public override IEnumerable<KeyValuePair<long, TSource>> GetOrderableDynamicPartitions()
        {
            return new ListDynamicPartitions(input);
        }

        private class ListDynamicPartitions : IEnumerable<KeyValuePair<long, TSource>>
        {
            private readonly IList<TSource> input;
            private int pos;

            internal ListDynamicPartitions(IList<TSource> input)
            {
                this.input = input;
            }

            public IEnumerator<KeyValuePair<long, TSource>> GetEnumerator()
            {
                while (true)
                {
                    // Each task gets the next item in the list. The index is
                    // incremented in a thread-safe manner to avoid races.
                    var elemIndex = Interlocked.Increment(ref pos) - 1;

                    if (elemIndex >= input.Count)
                    {
                        yield break;
                    }

                    yield return new KeyValuePair<long, TSource>(
                        elemIndex, input[elemIndex]);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<KeyValuePair<long, TSource>>)this).GetEnumerator();
            }
        }
    }
}
