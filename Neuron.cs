﻿using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace LinearClassifier
{
    public class Neuron
    {
        // Конструктор нейрона, требующий количество входов на нейрон.
        public Neuron (int NumberOfInputs)
        { this.NumberOfInputs = NumberOfInputs; }
        // Количество входов нейрона.
        private static int _numberOfInputs { get; set; }
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
        private List<double> _inputs = new List<double>(_numberOfInputs);
        public List<double> Inputs
        {
            get
            { return _inputs; }
            set
            {
                if (value.Count == _inputs.Count)
                {
                    for (int i = 0; i < value.Count; i++)
                    {
                        double element = value[i];
                        _inputs.Add(element);
                    }
                }
                else
                {
                    Console.WriteLine("А у вас ошибка! Количество входных Inputs не соответствует количеству, установленному для нейрона");
                    // Exception
                }
            }
        }
        // Набор весов.
        private List<double> _weights = new List<double>(_numberOfInputs);
        public List<double> Weights
        {
            get
            { return _weights; }
            set
            {
                if (value.Count == _weights.Count)
                {
                    for (int i = 0; i < value.Count; i++)
                    {
                        double element = value[i];
                        _weights[i] = element;
                    }
                }
                else
                {
                    Console.WriteLine("А у вас ошибка! Количество входных Weights не соответствует количеству, установленному для нейрона");
                    // Exception
                }
            }
        }
        // Вес смещения.
        private double _bias { get; set; }
        public double Bias
        {
            get
            { return _bias; }
            set
            { _bias = value; }
        }
        // Сумматор
        internal void Summator()
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
            get { return _output; }            
        }
    }
}
