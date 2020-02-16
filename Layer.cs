using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace LinearClassifier
{
    class Layer
    {
        // Конструкторы слоя, обязательно требующие количество нейронов
        public Layer(int NumberOfNeurons) : this (NumberOfNeurons, _layerName)
        { }
        public Layer(int NumberOfNeurons, string name)
        {
            _numberOfNeurons = NumberOfNeurons;
            _layerName = name;
        }
        // Количество нейронов
        private static int _numberOfNeurons { get; set; }
        private List<Neuron> _setOfNeurons = new List<Neuron>(_numberOfNeurons);
        public List<Neuron> SetOfNeurons
        {
            get
            { return _setOfNeurons; }
            set
            {
                bool _volume = value.Capacity == _setOfNeurons.Capacity;    // Проверка соответствия объёмов
                bool _type = true;                                          // TODO: Написать проверку соответствия типов
                if (_volume && _type)
                {
                    for (int i = 0; i < _setOfNeurons.Capacity; i++)
                    {
                        Neuron element = value[i];
                        _setOfNeurons.Add(element);
                    }
                }
                else
                {
                    Console.WriteLine($"А у вас ошибка! Количество или тип входных Neurons не соответствует количеству, установленному для слоя {_layerName}");
                    // Exception
                }
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
                bool _type = true;                                          // TODO: Написать проверку соответствия типов
                if (_type)
                {
                    _layerName = value;
                }
                else
                {
                    Console.WriteLine("А у вас ошибка! Вы вводите/передаёте не строку для имени слоя");
                }
            }
        }
        // Метод регуляризации весов слоя
        public double Regularization()
        {
            double sum = 0.0;
            foreach (Neuron neuron in _setOfNeurons)
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
