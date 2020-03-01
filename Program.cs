using System;
using System.Collections.Generic;

namespace LinearClassifier
{
    class Program
    {
        const int MoveQuantity = 100;                                               // Number of moves for one rollout.
        const int LayerDimension = 64;                                              // Размер первого слоя.
        const int OutputDimension = 9;                                              // Размер выходного слоя = количество допустимых действий (ходов).
        const int BigData = 1000;                                                   // Объём запоминаемых данных для обучения сети.
        const double LearningRate = 0.1;                                            // Коэффициент обучаемости.
        static Cube TrainCube = new Cube();
        static int TaskDimension = TrainCube.State.Count;
        // ReplayMemory собирает стейты, ходы и реварды в большой массив данных (см лекция 13, 1ч11мин).
        (List<int> State, int Move, double Reward, List<int> NewState)[] ReplayMemory = new (List<int>, int, double, List<int>)[BigData * MoveQuantity];
        (List<int> UnicState, List<double> ActionReward)[] QFunction = new (List<int>, List<double>)[MoveQuantity];
        static Random Rnd = new Random();
        const double Gamma = 0.99;
        
        // Первый слой.
        static Layer Layer_1 = new Layer(LayerDimension, TaskDimension);
        // Выходной слой.
        static Layer OutputLayer = new Layer(OutputDimension, LayerDimension);

        void Main(string[] args)
        {
            TrainCube.SetSolved();                                                      // Установка исходной позиции на кубе.
            Layer_1.WeightsInitialization();                                            // Инициализация весов всех нейронов.
            OutputLayer.WeightsInitialization();
            int UnicsCounter = 0;                                                       // Счётчик уникальных состояний в Q-функции.
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
                while (Rollout < MoveQuantity)
                {
                    int DataCounter = Episode + Rollout;                                // Счётчик туплов в ReplayMemory.
                    int QMove = (int)Moves.O;                                           // Содержит ход, который будем делать.
                    int Discount = Rollout + 1;
                    int NextMove;
                    ReplayMemory[DataCounter].State = TrainCube.State;                  // TODO: Выяснить передаётся по значению или по ссылке.
                    int QIndex = ExistIndex(TrainCube.State);
                    int NextQIndex;
                    if (QIndex != -1)                                                   // Минус 1 - признак того, что нет такого стейта.
                    { // Есть уже такой текущий стейт.
                        QMove = Argmax(QFunction[QIndex].ActionReward);                 // Получаем топ ход из Q-функции
                        ProbabilityMove(TrainCube, QMove, 0.1, out QMove);              // Делаем ход на кубе с некоторой вероятностью.
                        ReplayMemory[DataCounter].Move = QMove;                         // Запоминаем сделанный ход.
                        ReplayMemory[DataCounter].NewState = TrainCube.State;           // Запоминаем новый стейт.
                        NextQIndex = ExistIndex(TrainCube.State);                       // Ищем новый стейт в Q-функции.
                        if (NextQIndex != -1)
                        { // Есть такой новый стейт.
                            NextMove = Argmax(QFunction[NextQIndex].ActionReward);
                            // Изменяем исходный Reward сделанного хода.
                            QFunction[QIndex].ActionReward[QMove] += Math.Pow(Gamma, Discount) * QFunction[NextQIndex].ActionReward[NextMove];
                            ReplayMemory[DataCounter].Reward = QFunction[QIndex].ActionReward[QMove];
                        }
                        else
                        { // Нет такого нового стейта.
                            QFunction[UnicsCounter].UnicState = TrainCube.State;
                            QFunction[UnicsCounter].ActionReward = NeuralNetwork(TrainCube);
                            NextMove = Argmax(QFunction[NextQIndex].ActionReward);
                            // Изменяем исходный Reward сделанного хода.
                            QFunction[QIndex].ActionReward[QMove] += Math.Pow(Gamma, Discount) * QFunction[NextQIndex].ActionReward[NextMove];
                            ReplayMemory[DataCounter].Reward = QFunction[QIndex].ActionReward[QMove];
                            UnicsCounter++;
                        }
                    }
                    else
                    { // Нет такого текущего стейта.
                        QFunction[UnicsCounter].UnicState = TrainCube.State;                // Прописываем новый стейт в Q-функцию.
                        QFunction[UnicsCounter].ActionReward = NeuralNetwork(TrainCube);    // TODO: Выяснить передаётся по значению или по ссылке.
                        QMove = Argmax(QFunction[UnicsCounter].ActionReward);               // Получаем топ ход из Q-функции.
                        ProbabilityMove(TrainCube, QMove, 0.1, out QMove);                  // Делаем ход на кубе с некоторой вероятностью.
                        ReplayMemory[DataCounter].Move = QMove;                             // Запоминаем сделанный ход.
                        ReplayMemory[DataCounter].NewState = TrainCube.State;               // Запоминаем новый стейт.
                        NextQIndex = ExistIndex(TrainCube.State);                           // Ищем новый стейт в Q-функции.
                        if (NextQIndex != -1)
                        { // Есть такой новый стейт.
                            NextMove = Argmax(QFunction[NextQIndex].ActionReward);
                            // Изменяем исходный Reward сделанного хода.
                            QFunction[UnicsCounter].ActionReward[NextMove] += Math.Pow(Gamma, Discount) * QFunction[NextQIndex].ActionReward[NextMove];
                            ReplayMemory[DataCounter].Reward = QFunction[UnicsCounter].ActionReward[NextMove];
                            UnicsCounter++;
                        }
                        else
                        { // Нет такого нового стейта.
                            QFunction[UnicsCounter + 1].UnicState = TrainCube.State;
                            QFunction[UnicsCounter + 1].ActionReward = NeuralNetwork(TrainCube);
                            NextMove = Argmax(QFunction[NextQIndex].ActionReward);
                            // Изменяем исходный Reward сделанного хода.
                            QFunction[UnicsCounter].ActionReward[QMove] += Math.Pow(Gamma, Discount) * QFunction[UnicsCounter + 1].ActionReward[NextMove];
                            ReplayMemory[DataCounter].Reward = QFunction[UnicsCounter].ActionReward[QMove];
                            UnicsCounter++;
                        }
                    }
                    // TODO: Minibatch and GradientDescend.

                    //Console.WriteLine($"Ход сети №{RolloutCounter + 1}: {QMove}");
                    //Console.ReadLine();
                    Rollout++;
                }
            }
        }
        
        // *******************************
        // ***** Используемые методы *****
        // *******************************
        
        // Проверяет есть ли новое состояние в Q-функции.
        private int ExistIndex(List<int> state)
        {
            int i = 0;
            int result = -1;
            while (result == -1 && QFunction[i].UnicState != null)
            {
                if (QFunction[i].UnicState == state)
                {
                    result = i;
                }
                i++;
            }
            return result;
        }
        private int Argmax(List<double> policy)
        {
            int result = 0;
            double max = double.MinValue;
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
        // Ход с некоторой вероятностью.
        static void ProbabilityMove(Cube SomeCube, int SomeMove, double Probability, out int qmove)
        {
            double Epsilon = Rnd.NextDouble();
            if (Epsilon > Probability)
            {
                MakeMove(TrainCube, SomeMove);
            }
            else
            {
                int old = SomeMove;
                SomeMove = Rnd.Next(1, 9);
                while (SomeMove == old)
                {
                    SomeMove = Rnd.Next(1, 9);
                }
                MakeMove(TrainCube, SomeMove);
            }
            qmove = SomeMove;
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