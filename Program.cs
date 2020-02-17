using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace LinearClassifier
{
    class Program
    {
        static void Main(string[] args)
        {
            Cube TrainCube = new Cube();                        // Создание куба.
            int TaskDimension = TrainCube.State.Capacity;       // Объём входных данных.
            const int Layer_1_Dimension = 64;                   // Размер первого слоя.
            const int OutputDimension = 9;                      // Размер выходного слоя.
            Random Rnd = new Random();                          // Рандомайзер.

            // Создание слоёв, наполнение их нейронами и инициализация весов нейронов.
            Layer Layer_1 = new Layer(Layer_1_Dimension, "Alone");
            for (int i = 0; i < Layer_1_Dimension; i++)
            {
                Layer_1.SetOfNeurons[i] = new Neuron(TaskDimension);
                for (int j = 0; j < TaskDimension; j++)
                {
                    Layer_1.SetOfNeurons[i].Weights[j] = Rnd.NextDouble();
                }
            }
            Layer OutputLayer = new Layer(OutputDimension, "Out");
            for (int i = 0; i < OutputDimension; i++)
            {
                OutputLayer.SetOfNeurons[i] = new Neuron(Layer_1_Dimension);
                for (int j = 0; j < TaskDimension; j++)
                {
                    OutputLayer.SetOfNeurons[i].Weights[j] = Rnd.NextDouble();
                }
            }

        }
    }
}
