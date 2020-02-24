using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace LinearClassifier
{
    class Layer
    {
        //Конструкторы слоя
        //public Layer() : this(_numberOfNeurons)
        //{ }
        //public Layer(int NumberOfNeurons) : this(NumberOfNeurons, _numberOfNeuronInputs)
        //{ }
        //public Layer(int NumberOfNeurons, int NumberOfNeuronInputs) : this(NumberOfNeurons, _numberOfNeuronInputs, _layerName)
        //{ }
        public Layer(int NumberOfNeurons, int NumberOfNeuronInputs, string LayerName)
        {
            this.NumberOfNeurons = NumberOfNeurons;
            this.NumberOfNeuronInputs = NumberOfNeuronInputs;
            this.LayerName = LayerName;
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
                    Console.WriteLine($"А у вас ошибка! Количество или тип входных Neurons не соответствует количеству, установленному для слоя {_layerName}");
                    // Exception
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
        // Имя слоя
        private string _layerName {get; set;}
        public string LayerName
        {
            get
            { return _layerName; }
            set
            {
                bool _type = true;                                          // TODO: Написать проверку соответствия типов.
                if (_type)
                {
                    _layerName = value;
                }
                else
                {
                    Console.WriteLine("А у вас ошибка! Вы вводите/передаёте не строку для имени слоя");
                    // Exception.
                }
            }
        }
        // Метод нелинейности ReLu.
        public List<double> ReLu()
        {
            const double k = 0.001;
            List<double> relu = new List<double>(_numberOfNeurons);
            relu.Clear();
            foreach (Neuron neuron in _neurons)
            {
                neuron.SetOutput();
                double output = neuron.Output;
                if (output <= 100 && output >= 0)
                {
                    relu.Add(output);
                }
                else if (output < 0)
                {
                    relu.Add(output * k);
                }
                else
                {
                    relu.Add(output * k + 100 * (1 - k));
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
