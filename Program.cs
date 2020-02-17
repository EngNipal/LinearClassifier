using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace LinearClassifier
{
    class Program
    {
        static void Main(string[] args)
        {
            Cube TrainCube = new Cube();                                            // Создание куба.
            TrainCube.SetSolved();                                                  // Установка исходной позиции.
            int TaskDimension = TrainCube.State.Capacity;                           // Объём входных данных.
            const int Layer_1_Dimension = 64;                                       // Размер первого слоя.
            const int OutputDimension = 9;                                          // Размер выходного слоя.
            List<double> Layer_1_Output = new List<double>(Layer_1_Dimension);      // Выходные значения первого слоя.
            List<double> OutputLayer_Output = new List<double>(OutputDimension);    // Выходные значения выходного слоя.
            Random Rnd = new Random();                                              // Рандомайзер.

            // Создание слоёв, наполнение их нейронами и инициализация весов нейронов.
            Layer Layer_1 = new Layer(Layer_1_Dimension, "Alone");
            for (int i = 0; i < Layer_1_Dimension; i++)
            {
                Layer_1.SetOfNeurons[i] = new Neuron(TaskDimension);
                for (int j = 0; j < TaskDimension; j++)
                {
                    Layer_1.SetOfNeurons[i].Weights[j] = Rnd.NextDouble();
                }
                Layer_1.SetOfNeurons[i].Bias = Rnd.NextDouble();
            }
            Layer OutputLayer = new Layer(OutputDimension, "Out");
            for (int i = 0; i < OutputDimension; i++)
            {
                OutputLayer.SetOfNeurons[i] = new Neuron(Layer_1_Dimension);
                for (int j = 0; j < TaskDimension; j++)
                {
                    OutputLayer.SetOfNeurons[i].Weights[j] = Rnd.NextDouble();
                }
                OutputLayer.SetOfNeurons[i].Bias = Rnd.NextDouble();
            }
            // ----- Генерация скрамбла рандомайзером -----            
            int ScrLength;
            ScrLength = Rnd.Next(1, 10);  // Рандомная длина скрамбла от 1 до 10

            // массив запоминает скрамбл
            int[] Scramble = new int[10];
            for (byte i = 0; i < ScrLength; i++)
            {
                if (i > 0)
                {
                    switch (Scramble[i - 1])
                    {
                        case 1:
                        case 2:
                        case 3:
                            Scramble[i] = Rnd.Next((int)Moves.U, (int)Moves.F2);
                            break;
                        case 7:
                        case 8:
                        case 9:
                            Scramble[i] = Rnd.Next((int)Moves.R, (int)Moves.U2);
                            break;
                        case 4:
                        case 5:
                        case 6:
                            if (Rnd.NextDouble() < 0.5)
                            { Scramble[i] = Rnd.Next((int)Moves.R, (int)Moves.R2); }
                            else
                            { Scramble[i] = Rnd.Next((int)Moves.F, (int)Moves.F2); }
                            break;
                        default:                        
                            Console.WriteLine("Получен неверный номер хода! Номер должен быть от 1 до 9.");
                            break;
                    }
                }
                else
                {
                    Scramble[i] = Rnd.Next(1, 9);
                }
                MakeAMoveNumberN(TrainCube, Scramble[i]);               // Скрамблим куб
            }
            // Передаём состояние куба на вход всем нейронам первого слоя.
            for (int i = 0; i < Layer_1_Dimension; i++)
            {
                for (int j = 0; j < TaskDimension; j++)
                {
                    Layer_1.SetOfNeurons[i].Inputs[j] = TrainCube.State[j];
                }
            }
            Layer_1_Output = Layer_1.ReLu();
            // Layer_1_Output = Layer_1.Sigmoid();
            // Передаём выход первого слоя на вход нейронам выходного слоя.
            for (int i = 0; i < OutputDimension; i++)
            {
                OutputLayer.SetOfNeurons[i].Inputs = Layer_1_Output;
            }
                                                                                                // TODO: Написать софтмакс.
        } // end Main
        // Метод, делающий ход с номером N на передаваемом кубе
        static void MakeAMoveNumberN(Cube SomeCube, int N)
        {
            switch (N)
            {
                case 1:
                    SomeCube.R();
                    break;
                case 2:
                    SomeCube.Rp();
                    break;
                case 3:
                    SomeCube.R2();
                    break;
                case 4:
                    SomeCube.U();
                    break;
                case 5:
                    SomeCube.Up();
                    break;
                case 6:
                    SomeCube.U2();
                    break;
                case 7:
                    SomeCube.F();
                    break;
                case 8:
                    SomeCube.Fp();
                    break;
                case 9:
                    SomeCube.F2();
                    break;
            }
        }
        // Перечисление ходов
        enum Moves
        {
            R = 1,
            Rp = 2,
            R2 = 3,
            U = 4,
            Up = 5,
            U2 = 6,
            F = 7,
            Fp = 8,
            F2 = 9
        }
    }
}
