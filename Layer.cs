using System;
using System.Collections.Generic;

namespace LinearClassifier
{
    class Layer
    {
        //Конструктор слоя
        public Layer(int NumberOfNeurons, int NumberOfNeuronInputs)
        {
            this.NumberOfNeurons = NumberOfNeurons;
            this.NumberOfNeuronInputs = NumberOfNeuronInputs;
            for (int i = 0; i < NumberOfNeurons; i++)
            {
                Neuron ObjectNeuron = new Neuron(NumberOfNeuronInputs);
                Neurons.Add(ObjectNeuron);
            }
        }
        // Количество нейронов.
        private int _numberOfNeurons { get; set; }
        public int NumberOfNeurons
        {
            get
            { return _numberOfNeurons; }
            set
            {
                _numberOfNeurons = value;
            }
        }
        // Набор нейронов
        private List<Neuron> _neurons = new List<Neuron>();
        public List<Neuron> Neurons
        {
            get
            { return _neurons; }
            set
            {
                bool _volume = value.Count == _neurons.Count;         // Проверка соответствия объёмов.
                bool _type = true;                                    // TODO: Написать проверку соответствия типов.
                if (_volume && _type)
                {
                    List<Neuron> element = value;
                    _neurons = (element);
                }
                else
                {
                    Console.WriteLine("А у вас ошибка! Количество или тип входных Neurons не соответствует количеству, установленному для слоя");
                }
            }
        }
        // Количество входов на нейрон.
        private int _numberOfNeuronInputs { get; set; }
        public int NumberOfNeuronInputs
        {
            private get
            { return _numberOfNeuronInputs; }
            set
            {
                _numberOfNeuronInputs = value;
            }
        }
        private static Random _random = new Random();
        // Инициализация весов слоя случайными значениями.
        internal void WeightsInitialization()
        {
            for (int i = 0; i < _numberOfNeurons; i++)
            {
                for (int j = 0; j < _numberOfNeuronInputs; j++)
                {
                    Neurons[i].Weights[j] = _random.NextDouble();
                }
                Neurons[i].Bias = _random.NextDouble();
            }
        }
        // Метод нелинейности ReLu.
        private const double Slope = 0.001;
        private const int Offset = 100;
        public List<double> ReLu()
        {
            List<double> relu = new List<double>(_numberOfNeurons);
            relu.Clear();
            foreach (Neuron neuron in _neurons)
            {
                neuron.SetOutput();
                double output = neuron.Output;
                if (output <= Offset && output >= 0)
                {
                    relu.Add(output);
                }
                else if (output < 0)
                {
                    relu.Add(output * Slope);
                }
                else
                {
                    relu.Add(output * Slope + Offset * (1 - Slope));
                }
            }
            return relu;
        }
        // Метод нелинейности Sigmoid.
        public List<double> Sigmoid()
        {
            List<double> _sigmoid = new List<double>(_numberOfNeurons);
            foreach (Neuron neuron in _neurons)
            {
                neuron.SetOutput();
                double _output = neuron.Output;
                _sigmoid.Add(1 / (1 + Math.Exp(-_output)));
            }
            return _sigmoid;
        }
        // Метод регуляризации весов слоя.
        public double Regularization()
        {
            double sum = 0.0;
            foreach (Neuron neuron in _neurons)
            {
                for (int i =0; i < neuron.Weights.Capacity; i++)
                {
                    sum += neuron.Weights[i];
                }
                sum += neuron.Bias;
            }
            return sum;
        }
    }
}
