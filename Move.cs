using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearClassifier
{
    class Move
    {
        public Move()
        {
            Policy = 0.0;
            Visit = 0;
            Quality = 0.0;
        }
        public Move(double Policy, int Visit, double Quality)
        {
            this.Policy = Policy;
            this.Visit = Visit;
            this.Quality = Quality;
        }
        // Вероятность хода, выдаваемая нейросетью.
        private double _policy { get; set; }
        public double Policy
        {
            get
            {
                double result;
                result = _policy;
                return result;
            }
            set
            {
                if (value.GetType() == _policy.GetType())
                {
                    double helper = value;
                    _policy = helper;
                }
                else
                {
                    Console.WriteLine("Несоответствие в Move.Policy");
                }
            }
        }
        // Количество проходов по этому ребру дерева.
        private int _visit { get; set; }
        public int Visit
        {
            get
            {
                int result = _visit;
                return result;
            }
            set
            {
                if (value.GetType() == _visit.GetType())
                {
                    int helper = value;
                    _visit = helper;
                    UpperBound = Policy / (1 + _visit);
                }
                else
                {
                    Console.WriteLine("Несоответствие в Move.Visit");
                }
            }
        }
        // Оценка выигрыша для этого хода.
        private double _quality { get; set; }
        public double Quality
        {
            get
            {
                double result = _quality;
                return result;
            }
            set
            {
                double helper = value;
                _quality = helper;
            }
        }
        // Доп показатель, учитывающий количество проходов по данному ребру.
        private double _upperBound { get; set; }
        public double UpperBound
        {
            get
            {
                double result = _upperBound;
                return result;
            }
            set
            {
                double helper = value;
                _upperBound = helper;
            }
        }
    }
}
