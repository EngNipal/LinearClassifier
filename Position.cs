using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearClassifier
{
    class Position
    {
        public Position(int[] State, double Value)
        {
            this.State = State;
            this.Value = Value;
        }
        private int[] _state { get; set; }
        public int[] State
        {
            get
            {
                int[] result = new int[24];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = _state[i];
                }
                return result;
            }
            set
            {
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
                    Console.WriteLine("А у вас ошибка! Количество или тип входных элементов не соответствует количеству или типу, установленным для позиции");
                }

            }
        }
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
                    Console.WriteLine("Несоответствие типов");
                }
            }
        }
    }
}
