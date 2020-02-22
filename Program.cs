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
            // Первый слой.
            Layer Layer_1 = new Layer(Layer_1_Dimension, TaskDimension, "Alone");
            Layer_1.WeightsInitialization();
            // Выходной слой.
            Layer OutputLayer = new Layer(OutputDimension, Layer_1_Dimension, "Out");
            OutputLayer.WeightsInitialization();
            // ----- Генерация скрамбла рандомайзером -----            
            int ScrLength;
            ScrLength = Rnd.Next(1, 10);  // Рандомная длина скрамбла от 1 до 10.
            // Массив запоминает скрамбл.
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
                    Layer_1.Neurons[i].Inputs[j] = TrainCube.State[j];
                }
            }
            // Функция нелинейности.
            Layer_1_Output = Layer_1.ReLu();
            // Layer_1_Output = Layer_1.Sigmoid();
            // Передаём выход первого слоя на вход нейронам выходного слоя.
            // Считаем выходное значение каждого нейрона (метод Summator).
            double sum = 0.0;
            foreach (Neuron neuron in OutputLayer.Neurons)
            {
                neuron.Inputs = Layer_1_Output;
                neuron.Summator();
                sum += Math.Exp(neuron.Output);
            }
            List<double> Policy = new List<double>(OutputDimension);
            double Softmax = 0.0;
            int ResultMove = 0;
            for (int i = 0; i < OutputDimension; i++)
            {
                Policy.Add(Math.Exp(OutputLayer.Neurons[i].Output) / sum);
                if (Policy[i] > Softmax)
                {
                    Softmax = Policy[i];
                    ResultMove = i + 1;
                }
            }
            Console.WriteLine("Был сгенерирован следующий скрамбл:");
            int index = 0;
            while (Scramble[index] > 0)
            {
                Console.Write(Scramble[index] + " ");
                index++;
            }
            Console.WriteLine();
            Console.WriteLine($"Итоговый ход: {ResultMove}");
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
