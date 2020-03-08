using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearClassifier
{
    class Position : ICloneable
    {
        public Position()
        {
            State = new int[StateDimension]; // { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6 } - решённое состояние.
            Value = 0.0;
        }
        public Position(int[] State, double Value)
        {
            this.State = State;
            this.Value = Value;
        }
        private const int StateDimension = Program.TaskDimension;
        // Интерфейс для возможности копирования экземпляра объекта.
        public object Clone()
        {
            return new Position
            {
                State = this.State,
                Value = this.Value
            };
        }
        // Состояние - собственно позиция.
        private int[] _state { get; set; }
        public int[] State
        {
            get
            {
                int[] result = new int[StateDimension];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = _state[i];
                }
                return result;
            }
            set
            {
                if (_state == null)
                {
                    _state = new int[StateDimension];
                    _state.Initialize();
                }
                if (value.Length == _state.Length && value.GetType() == _state.GetType())
                {
                    for (int i = 0; i < _state.Length; i++)
                    {
                        int element = value[i];
                        _state[i] = element;
                    }
                }
                else
                {
                    Console.WriteLine("Несоответствие в Position.State");
                }

            }
        }
        // Оценка позиции (назначается нейросетью).
        private double _value { get; set; }
        public double Value
        {
            get
            {
                double result = _value;
                return result;
            }
            set
            {
                if (value.GetType() == _value.GetType())
                {
                    double helper = value;
                    _value = helper;
                }
                else
                {
                    Console.WriteLine("Несоответствие в Position.Value");
                }
            }
        }
    }
}
