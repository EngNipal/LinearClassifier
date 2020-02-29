using System;
using System.Collections.Generic;

namespace LinearClassifier
{
    class Program
    {
        const int MoveQuantity = 30;                                                // Number of moves for one rollout.
        const int LayerDimension = 64;                                              // Размер первого слоя.
        const int OutputDimension = 9;                                              // Размер выходного слоя = количество допустимых действий (ходов).
        const int BigData = 1000;                                                   // Объём запоминаемых данных для обучения сети.
        const double LearningRate = 0.1;                                            // Коэффициент обучаемости.
        // ReplayMemory собирает стейты, ходы и реварды в большой массив данных (см лекция 13, 1ч11мин).
        (List<int> State, int Move, double Reward, List<int> NewState)[] ReplayMemory
            = new (List<int> State, int Move, double Reward, List<int> NewState)[BigData * MoveQuantity];
        static Random Rnd = new Random();
        static Cube TrainCube = new Cube();
        static int TaskDimension = TrainCube.State.Count;
        static double Gamma = 0.99;
        // Первый слой.
        static Layer Layer_1 = new Layer(LayerDimension, TaskDimension);
        // Выходной слой.
        static Layer OutputLayer = new Layer(OutputDimension, LayerDimension);

        void Main(string[] args)
        {
            TrainCube.SetSolved();                                                  // Установка исходной позиции на кубе.

            // Инициализация весов всех нейронов.
            Layer_1.WeightsInitialization();
            OutputLayer.WeightsInitialization();

            double Q_Function = 0;
            for (int Episode = 0; Episode < BigData; Episode++)
            {
                // Создаём скрамбл.
                int ScrLength = Rnd.Next(1, 10);                                        // Рандомная длина скрамбла от 1 до 10.
                int[] Scramble = new int[ScrLength];
                Scramble = GetScramble(ScrLength);
                WriteScramble(Scramble);
                SetScramble(TrainCube, Scramble);                                       // Скрамблим куб.
                // Rollout - Основной прогон, генерирующий набор из MoveQuantity ходов.
                int Rollout = 0;
                List<double> Policy = new List<double>();                               // Собирает все Policy за rollout.
                while (Rollout < MoveQuantity && !TrainCube.IsSolved())
                {
                    int DataCounter = Episode + Rollout;                                // Счётчик туплов в ReplayMemory.
                    ReplayMemory[DataCounter].State = TrainCube.State;                  // TODO: Выяснить передаётся по значению или по ссылке.
                    Policy = NeuralNetwork(TrainCube);                                  // TODO: Выяснить передаётся по значению или по ссылке.
                    int index = Argmax(Policy);
                    double CurrentReward = Policy[index];
                    Cube NextCube = new Cube();
                    NextCube = TrainCube;                                               // TODO: Выяснить передаётся по значению или по ссылке. Надо по значению.
                    Q_Function +=  QFunction(NextCube, Rollout);//!!!!!!!!!!!!!!!!!!!!!!!
                    int QMove = (int)Moves.O;
                    double Epsilon = Rnd.NextDouble();                                  // С некоторой вероятностью (0.1) делаем иной ход.
                    if (Epsilon > 0.1)
                    {
                        MakeMove(TrainCube, QMove);
                        ReplayMemory[DataCounter].Move = QMove;
                    }
                    else
                    {
                        QMove = Rnd.Next(1, 9);
                        ReplayMemory[DataCounter].Move = QMove;
                        MakeMove(TrainCube, QMove);
                    }
                    ReplayMemory[DataCounter].NewState = TrainCube.State;                               // TODO: Обдумать действия с reward-ом.
                    if (TrainCube.IsSolved())
                    {
                        ReplayMemory[DataCounter].Reward += 1;
                    }
                    else if (Rollout + 1 >= MoveQuantity)
                    {
                        ReplayMemory[DataCounter].Reward += -1;
                    }
                    else
                    {
                        ReplayMemory[DataCounter].Reward += 0;
                    }
                    //Console.WriteLine($"Ход сети №{RolloutCounter + 1}: {QMove}");
                    //Console.ReadLine();
                    Rollout++;
                }       // end Rollout.
            }
        }

        // *******************************
        // ***** Используемые методы *****
        // *******************************
        

        private double QFunction(Cube nextCube, int discount)
        {
            // Делаем каждый ход по отдельности и смотрим какая в новом состоянии функция Q.
            // Усредняем все значения этой функции по всем ходам.
            double result = 0;
            int index = 0;
            List<double> nextPolicy = new List<double>(OutputDimension);
            for (int i = 1; i <= OutputDimension; i++)
            {
                MakeMove(nextCube, i);
                nextPolicy = NeuralNetwork(nextCube);
                index = Argmax(nextPolicy);
                result += (Math.Pow(Gamma, discount) + 1) * nextPolicy[index];
                // Возвращаем предыдущее состояние куба - 
                // Делаем противоположный ход в зависимости от того, какой ход был сделан перед этим.
                if (Math.IEEERemainder(i, 3) == 1)
                {
                    MakeMove(nextCube, i + 1);
                }
                else if (Math.IEEERemainder(i, 3) == 2)
                {
                    MakeMove(nextCube, i - 1);
                }
                else
                {
                    MakeMove(nextCube, i);
                }
            }
            result /= OutputDimension;
            return result;
        }
        private int Argmax(List<double> policy)
        {
            int result = 0;
            double max = 0.0;
            for (int i = 0; i < policy.Count; i++)
            {
                if (max < policy[i])
                {
                    max = policy[i];
                    result = i + 1;
                }
            }
            return result;
        }
        // Метод прогоняющий данные через нейронную сеть.
        private List<double> NeuralNetwork(Cube SomeCube)
        {            
            List<double> Layer_1_Output = new List<double>(LayerDimension);
            List<double> resultPolicy = new List<double>(OutputDimension);
            // Передаём состояние куба на вход всем нейронам первого слоя.
            for (int i = 0; i < LayerDimension; i++)
            {
                for (int j = 0; j < TaskDimension; j++)
                {
                    Layer_1.Neurons[i].Inputs[j] = SomeCube.State[j];
                }
            }
            Layer_1_Output = Layer_1.ReLu();                                    // Функция нелинейности.
            //Layer_1_Output = Layer_1.Sigmoid();                               // - Другая функция нелинейности.
            // Передаём выход первого слоя на вход нейронам выходного слоя.
            // Считаем выходное значение каждого нейрона (метод SetOutput).
            double SumOfExponents = 0.0;
            double Average = 0.0;                                               // Усреднение выходных значений выходного слоя.
            foreach (Neuron neuron in OutputLayer.Neurons)
            {
                neuron.Inputs = Layer_1_Output;
                neuron.SetOutput();
                Average += neuron.Output;
            }
            Average /= OutputDimension;
            foreach (Neuron neuron in OutputLayer.Neurons)
            {
                SumOfExponents += Math.Exp(neuron.Output - Average);
            }
            for (int i = 0; i < OutputDimension; i++)
            {
                resultPolicy.Add(Math.Exp(OutputLayer.Neurons[i].Output - Average) / SumOfExponents);
            }
            return resultPolicy;
        }
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
        private static int[] GetScramble (int scrambleLength)
        {
            int[] scrambleArray = new int[scrambleLength];
            for (byte i = 0; i < scrambleLength; i++)
            {
                if (i > 0)
                {
                    if (i <= 3)
                    {
                        scrambleArray[i] = Rnd.Next((int)Moves.U, (int)Moves.F2);
                    }
                    else if (i <= 6)
                    {
                        if (Rnd.NextDouble() < 0.5)
                        {
                            scrambleArray[i] = Rnd.Next((int)Moves.R, (int)Moves.R2);
                        }
                        else
                        {
                            scrambleArray[i] = Rnd.Next((int)Moves.F, (int)Moves.F2);
                        }
                        break;
                    }
                    else if (i <= 9)
                    {
                        scrambleArray[i] = Rnd.Next((int)Moves.R, (int)Moves.U2);
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
        enum Moves
        {
            O, R, Rp, R2, U, Up, U2, F, Fp, F2
        }
    }
}
