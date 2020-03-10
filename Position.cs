using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearClassifier
{
    public class Position : IEquatable<Position>
    {
        public ReadOnlyCollection<int> State { get; }
        public int HashCode { get; }

        public Position(IEnumerable<int> state)
        {
            State = Array.AsReadOnly(state?.ToArray() ?? Array.Empty<int>());
            HashCode = -1319491066;
            foreach (int item in State)
            {
                HashCode ^= item;
            }
        }

        public Position(params int[] state)
            : this((IEnumerable<int>)state)
        { }

        public override bool Equals(object obj) => Equals(obj as Position);

        public bool Equals(Position other)
        {
            if (other == null || State.Count != other.State.Count)
                return false;

            return State.SequenceEqual(other.State);
        }

        public override int GetHashCode()
            => HashCode;

        public double Value { get; set; }
        public Position(double value, IEnumerable<int> state)
            : this(state)
            => Value = value;

        public Position(double value, params int[] state)
            : this(value, (IEnumerable<int>)state)
        { }


    }
}
