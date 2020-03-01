using System;
using System.Collections.Generic;

namespace LinearClassifier
{
    public class Neuron
    {
        // Конструктор нейрона, требующий количество входов на нейрон.
        public Neuron (int NumberOfInputs)
        {
            this.NumberOfInputs = NumberOfInputs;
            double startValue = 0.0;
            for (int i = 0; i < NumberOfInputs; i++)
            {
                Inputs.Add(startValue);
                Weights.Add(startValue);
            }
        }
        // Количество входов нейрона.
        private int _numberOfInputs { get; set; }
        public int NumberOfInputs
        {
            get
            { return _numberOfInputs; }
            set
            {
                _numberOfInputs = value;
            }
        }
        // Набор входных значений.
        private List<double> _inputs = new List<double>();
        public List<double> Inputs
        {
            get
            { return _inputs; }
            set
            {
                if (value.Count == _inputs.Count && value.GetType() == _inputs.GetType())
                {
                    List<double> element = value;
                    _inputs = element;
                }
                else
                {
                    Console.WriteLine("А у вас ошибка! Количество или тип входных Inputs не соответствует количеству или типу, установленным для нейрона");
                }
            }
        }
        // Набор весов.
        private List<double> _weights = new List<double>();
        public List<double> Weights
        {
            get
            {
                return _weights;
            }
            set
            {
                if (value.Count == _weights.Count && value.GetType() == _weights.GetType())
                {
                    List<double> element = value;
                    _weights = element;
                }
                else
                {
                    Console.WriteLine("А у вас ошибка! Количество или тип входных Weights не соответствует количеству или типу, установленным для нейрона");
                }
            }
        }
        // Вес смещения.
        private double _bias { get; set; }
        public double Bias
        {
            get
            {
                return _bias;
            }
            set
            {
                if (value.GetType() == _bias.GetType())
                {
                    _bias = value;
                }
                else
                {
                    Console.WriteLine("А у вас ошибка! Тип входного Bias не соответствует типу, установленному для нейрона");
                }
            }
        }
        // Сумматор
        internal void SetOutput()
        {
            _output = 0.0;
            for (int i = 0; i < _weights.Count; i++)
            {
                _output += _weights[i] * _inputs[i];
            }
            _output += _bias;
        }
        // Выход нейрона
        private double _output { get; set; }
        public double Output
        {
            get
            {
                return _output;
            }
        }
    }
}
