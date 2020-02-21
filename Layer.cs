using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace LinearClassifier
{
    class Layer
    {
        // Конструкторы слоя, обязательно требующие количество нейронов.
        public Layer(int NumberOfNeurons) : this(NumberOfNeurons, _layerName)
        { }
        public Layer(int NumberOfNeurons, string LayerName) : this(NumberOfNeurons, _layerName, _numberOfNeuronInputs)
        { }
        public Layer(int NumberOfNeurons, string LayerName, int NumberOfNeuronInputs)
        {
            this.NumberOfNeurons = NumberOfNeurons;
            this.LayerName = LayerName;
            Neuron ObjectNeuron = new Neuron (NumberOfNeuronInputs);
            for (int i = 0; i < NumberOfNeurons; i++)
            {
                Neurons.Add(ObjectNeuron);
            }
        }
        // Количество нейронов.
        private static int _numberOfNeurons { get; set; }
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
        private List<Neuron> _neurons = new List<Neuron>(_numberOfNeurons);
        public List<Neuron> Neurons
        {
            get
            { return _neurons; }
            set
            {
                bool _volume = value.Capacity == _neurons.Capacity;    // Проверка соответствия объёмов.
                bool _type = true;                                          // TODO: Написать проверку соответствия типов.
                if (_volume && _type)
                {
                    for (int i = 0; i < _neurons.Capacity; i++)
                    {
                        Neuron element = value[i];
                        _neurons.Add(element);
                    }
                }
                else
                {
                    Console.WriteLine($"А у вас ошибка! Количество или тип входных Neurons не соответствует количеству, установленному для слоя {_layerName}");
                    // Exception
                }
            }
        }
        // Количество входов на нейрон
        private static int _numberOfNeuronInputs { get; set; }
        public int NumberOfNeuronInputs
        {
            private get
            { return _numberOfNeuronInputs; }
            set
            {
                _numberOfNeuronInputs = value;
            }
        }
        // Имя слоя
        private static string _layerName {get; set;}
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
            foreach (Neuron neuron in _neurons)
            {
                neuron.Summator();
                double output = neuron.Output;
                if (output <= 1 && output >= 0)
                {
                    relu.Add(output);
                }
                else if (output < 0)
                {
                    relu.Add(output * k);
                }
                else
                {
                    relu.Add(output * k + (1 - k));
                }
            }
            return relu;
        }
        // Метод нелинейности Sigmoid.
        public List<double> Sigmoid()
        {
            List<double> sigmoid = new List<double>(_numberOfNeurons);
            foreach (Neuron neuron in _neurons)
            {
                neuron.Summator();
                double output = neuron.Output;
                sigmoid.Add(1 / (1 + Math.Exp(-output)));
            }
            return sigmoid;
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
