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
            const int MoveQuantity = 10;                                            // Количество ходов, которые делает сеть, прежде, чем получит feedback.
            const int Layer_1_Dimension = 64;                                       // Размер первого слоя.
            const int OutputDimension = 9;                                          // Размер выходного слоя.
            List<double> Layer_1_Output = new List<double>(Layer_1_Dimension);      // Выходные значения первого слоя.
            List<double> OutputLayer_Output = new List<double>(OutputDimension);    // Выходные значения выходного слоя.
            Random Rnd = new Random();                                              // Рандомайзер.
            double[,] Policy = new double[MoveQuantity, OutputDimension];           // Набор значений Policy на выходном слое.

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
                MakeAMoveByLabel(TrainCube, Scramble[i]);               // Скрамблим куб
            }
            Console.WriteLine("Был сгенерирован следующий скрамбл:");
            int index = 0;
            while (Scramble[index] > 0)
            {
                Console.Write(Scramble[index] + " ");
                index++;
            }
            Console.WriteLine();
            // Rollout - Основной прогон, генерирующий набор из MoveQuantity ходов.
            int[] NetworkMoves = new int[MoveQuantity];                             // Набор ходов, выданных сетью за 1 роллаут.
            int RolloutMove_i = 0;
            while (RolloutMove_i < MoveQuantity && !TrainCube.IsSolved())
            {
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
                //Layer_1_Output = Layer_1.Sigmoid();          // - Другая функция нелинейности.
                // Передаём выход первого слоя на вход нейронам выходного слоя.
                // Считаем выходное значение каждого нейрона (метод SetOutput).
                double SumOfExponents = 0.0;
                double Average = 0.0;
                foreach (Neuron neuron in OutputLayer.Neurons)
                {
                    neuron.Inputs = Layer_1_Output;
                    neuron.SetOutput();
                    OutputLayer_Output.Add(neuron.Output);
                    Average += neuron.Output;
                }
                Average /= OutputDimension;
                foreach (Neuron neuron in OutputLayer.Neurons)
                {
                    SumOfExponents += Math.Exp(neuron.Output - Average);
                }
                double Softmax = 0.0;
                int ResultMove = 0;
                for (int i = 0; i < OutputDimension; i++)
                {
                    Policy[RolloutMove_i, i] = (Math.Exp(OutputLayer.Neurons[i].Output - Average) / SumOfExponents);
                    if (Policy[RolloutMove_i, i] > Softmax)
                    {
                        Softmax = Policy[RolloutMove_i, i];
                        ResultMove = i + 1;
                    }
                }
                NetworkMoves[RolloutMove_i] = ResultMove;
                MakeAMoveByLabel(TrainCube, ResultMove);
                Console.WriteLine($"Ход сети №{RolloutMove_i + 1}: {ResultMove}");
                Console.ReadLine();
                OutputLayer_Output.Clear();
                RolloutMove_i++;
            } // end Rollout.

            // --------------------
            // ----- ОБУЧЕНИЕ -----
            // --------------------
            const double LearningRate = 0.1;
            int Reward;
            if (TrainCube.IsSolved())
            {
                Reward = 1;
            }
            else
            {
                Reward = -1;
            }
            for (int LearningMove_i = 0; LearningMove_i < MoveQuantity; LearningMove_i++)
            {
                // Определение Loss-функции.
                double[] LossFunction = new double[MoveQuantity];
                for (int i = 0; i < MoveQuantity; i++)
                {
                    LossFunction[LearningMove_i] -= Math.Log(Policy[LearningMove_i, i]);
                }
                LossFunction[LearningMove_i] *= Reward;                                     // Позже добавить регуляризацию.
                // Градиент Loss-функции по выходам OutputLayer.
                // Передаётся только туда, где был выдан ход. Остальным - ноль.
                double[] GradLossOverOutput = new double[OutputDimension];
                for (int i = 1; i <= OutputDimension; i++)
                {
                    if (i == NetworkMoves[LearningMove_i])
                    {
                        GradLossOverOutput[i] = (Policy[LearningMove_i, i] - 1) * Reward;
                    }
                    else
                    {
                        GradLossOverOutput[i] = 0;
                    }
                }
                // Градиент Loss-функции по весам выходного слоя.
                // Равен GradLossOverOutput умноженному на транспонированный инпут выходного слоя (т.е. аутпут первого).
                double[,] GradLossOverOutputWeights = new double[OutputDimension, Layer_1_Dimension];
                for (int i = 0; i < OutputDimension; i++)
                {
                    for (int j = 0; j < Layer_1_Dimension; j++)
                    {
                        GradLossOverOutputWeights[i, j] = GradLossOverOutput[i] * Layer_1_Output[j];
                    }
                }
                // Корректируем веса выходного слоя.
                for (int i = 0; i < OutputDimension; i++)
                {
                    for (int j = 0; j < Layer_1_Dimension; j++)
                    {
                        OutputLayer.Neurons[i].Weights[j] -= GradLossOverOutputWeights[i, j] * LearningRate;
                    }
                    OutputLayer.Neurons[i].Bias -= GradLossOverOutput[i] * LearningRate;
                }
                // Градиент Loss-функции по ReLu первого слоя.

            }
        } // end Main
        // Метод, делающий ход с меткой MoveLabel на передаваемом кубе
        static void MakeAMoveByLabel(Cube SomeCube, int MoveLabel)
        {
            switch (MoveLabel)
            {
                case 1:
                    SomeCube.MoveR();
                    break;
                case 2:
                    SomeCube.MoveRp();
                    break;
                case 3:
                    SomeCube.MoveR2();
                    break;
                case 4:
                    SomeCube.MoveU();
                    break;
                case 5:
                    SomeCube.MoveUp();
                    break;
                case 6:
                    SomeCube.MoveU2();
                    break;
                case 7:
                    SomeCube.MoveF();
                    break;
                case 8:
                    SomeCube.MoveFp();
                    break;
                case 9:
                    SomeCube.MoveF2();
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
