using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearClassifier
{
    class Node
    {
        public Node(Position Position, double[] MovePolicy)
        {
            this.Position = Position;
            this.MovePolicy = MovePolicy;
        }
        private Position _position { get; set; }
        public Position Position
        {
            get
            {
                Position result = _position;
                return result;
            }
            set
            {
                Position helper = value;
                _position = helper;
            }
        }
        private double Visit {get; set;}
        private double[] _movePolicy { get; set; }
        public double[] MovePolicy
        {
            get
            {
                double[] result = new double[_movePolicy.Length];
                for (int i = 0; i < _movePolicy.Length; i++)
                {
                    result[i] = _movePolicy[i];
                }
                return result;
            }
            set
            {
                if (value.Length == _movePolicy.Length && value.GetType() == _movePolicy.GetType())
                {
                    for (int i = 0; i < _movePolicy.Length; i++)
                    {
                        _movePolicy[i] = value[i];
                    }
                }
                else
                {
                    Console.WriteLine("Несоответствие");
                }
            }
        }
    }
}
