using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace LinearClassifier
{
    class Program
    {
        static Random Rnd = new Random();
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
            double[,] Policy = new double[MoveQuantity, OutputDimension];           // Набор значений Policy на выходном слое.

            // Создание слоёв, наполнение их нейронами и инициализация весов нейронов.
            // Первый слой.
            Layer Layer_1 = new Layer(Layer_1_Dimension, TaskDimension);
            Layer_1.WeightsInitialization();
            // Выходной слой.
            Layer OutputLayer = new Layer(OutputDimension, Layer_1_Dimension);
            OutputLayer.WeightsInitialization();
            // Создаём скрамбл.
            int ScrLength;
            ScrLength = Rnd.Next(1, 10);                                            // Рандомная длина скрамбла от 1 до 10.
            int[] Scramble = new int[10];
            Scramble = GetScramble(ScrLength);
            WriteScramble(Scramble);
            // Скрамблим куб.
            SetScramble(TrainCube, Scramble);
            // Rollout - Основной прогон, генерирующий набор из MoveQuantity ходов.
            int[] NetworkMoves = new int[MoveQuantity];                             // Набор ходов, выданных сетью за 1 роллаут.
            int RolloutMove_i = 0;
            while (RolloutMove_i < MoveQuantity && !TrainCube.IsSolved())           // TODO: Вынести роллаут в отдельный метод или класс.
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
                MakeMove(TrainCube, ResultMove);
                Console.WriteLine($"Ход сети №{RolloutMove_i + 1}: {ResultMove}");
                Console.ReadLine();
                OutputLayer_Output.Clear();
                RolloutMove_i++;
            }       // end Rollout.

            // --------------------
            // ----- ОБУЧЕНИЕ -----                                                 // TODO: вынести обучение в отдельный класс.
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
                double[] GradOutput = new double[OutputDimension];
                for (int i = 1; i <= OutputDimension; i++)
                {
                    if (i == NetworkMoves[LearningMove_i])
                    {
                        GradOutput[i] = (Policy[LearningMove_i, i] - 1) * Reward;
                    }
                    else
                    {
                        GradOutput[i] = 0;
                    }
                }
                // Градиент Loss-функции по весам выходного слоя.
                // Равен GradOutput умноженному на транспонированный инпут выходного слоя (т.е. аутпут первого).
                double[,] GradWeightsOutput = new double[OutputDimension, Layer_1_Dimension];
                for (int i = 0; i < OutputDimension; i++)
                {
                    for (int j = 0; j < Layer_1_Dimension; j++)
                    {
                        GradWeightsOutput[i, j] = GradOutput[i] * Layer_1_Output[j];
                    }
                }
                // Градиент Loss-функции по выходным значениям первого слоя.
                // Равен транспонированным весам выходного слоя, умноженным на GradOutput.
                double[] GradOutputLayer1 = new double[Layer_1_Dimension];
                for (int i = 0; i < Layer_1_Dimension; i++)
                {
                    for (int j = 0; j < OutputDimension; j++)
                    {
                        GradOutputLayer1[i] += OutputLayer.Neurons[j].Weights[i] * GradOutput[i];
                    }
                }
                // Градиент Loss-функции по Relu первого слоя.
                double[] GradReluLayer1 = new double[Layer_1_Dimension];
                for (int i = 0; i < Layer_1_Dimension; i++)
                {
                    double temp = Layer_1.Neurons[i].Output;
                    if (temp >= 0 && temp <= 100)
                    {
                        GradReluLayer1[i] = GradOutputLayer1[i];
                    }
                    else
                    {
                        GradReluLayer1[i] = 0.001 * GradOutputLayer1[i];
                    }
                }
                // Градиент Loss-функции по весам первого слоя.
                double[,] GradWeightsLayer1 = new double[Layer_1_Dimension, TaskDimension];
                for (int i = 0; i < Layer_1_Dimension; i++)
                {
                    for (int j = 0; j < TaskDimension; j++)
                    {
                        GradWeightsLayer1[i, j] = GradReluLayer1[i] * TrainCube.State[j];
                    }
                }
                // TODO: Перенести "обёрточные" циклы из 2х2. - Когда берётся набор сделанных ходов и проходим по кубу при обучении.


                // Корректируем веса выходного слоя.
                // Именно в таком порядке - не раньше, чем посчитаем все градиенты.
                for (int i = 0; i < OutputDimension; i++)
                {
                    for (int j = 0; j < Layer_1_Dimension; j++)
                    {
                        OutputLayer.Neurons[i].Weights[j] -= GradWeightsOutput[i, j] * LearningRate;
                    }
                    OutputLayer.Neurons[i].Bias -= GradOutput[i] * LearningRate;
                }
                // Корректируем веса первого слоя.
                for (int i = 0; i < Layer_1_Dimension; i++)
                {
                    for (int j = 0; j < TaskDimension; j++)
                    {
                        Layer_1.Neurons[i].Weights[j] -= GradWeightsLayer1[i, j] * LearningRate;
                    }
                }
            } // end for train.
        } // end Main
        // Метод, делающий заданный ход на кубе.
        static void MakeMove (Cube SomeCube, int MoveLabel)
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
        // Метод, скрамблящий куб заданным скрамблом.
        static void SetScramble (Cube SomeCube, int[] scramble)
        {
            int i = 0;
            while (scramble[i] > 0)
            {
                MakeMove(SomeCube, scramble[i]);
                i++;
            }
        }
        // Метод создания скрамбла.
        static int[] GetScramble (int scrambleLength)
        {
            int[] scrambleArray = new int[scrambleLength];
            for (byte i = 0; i < scrambleLength; i++)
            {
                if (i > 0)
                {
                    if (i <= 3)
                    {
                        scrambleArray[i] = Rnd.Next((int)MoveCode.U, (int)MoveCode.F2);
                    }
                    else if (i <= 6)
                    {
                        if (Rnd.NextDouble() < 0.5)
                        {
                            scrambleArray[i] = Rnd.Next((int)MoveCode.R, (int)MoveCode.R2);
                        }
                        else
                        {
                            scrambleArray[i] = Rnd.Next((int)MoveCode.F, (int)MoveCode.F2);
                        }
                        break;
                    }
                    else if (i <= 9)
                    {
                        scrambleArray[i] = Rnd.Next((int)MoveCode.R, (int)MoveCode.U2);
                    }
                    else
                    {
                        Console.WriteLine("Получен неверный номер хода! Номер должен быть от 1 до 9.");
                    }
                }
                else
                {
                    scrambleArray[i] = Rnd.Next(1, 9);
                }
            }
            return scrambleArray;
        }
        // Метод вывода скрамбла.
        static void WriteScramble(int[] scramble)
        {
            Console.WriteLine("Ваш скрамбл:");
            int i = 0;
            while (scramble[i] > 0)
            {
                Console.Write(scramble[i] + " ");
                i++;
            }
            Console.WriteLine();
        }
        // Перечисление ходов.
        enum MoveCode
        {
            O,
            R,
            Rp,
            R2,
            U,
            Up,
            U2,
            F,
            Fp,
            F2
        }
    }
}
