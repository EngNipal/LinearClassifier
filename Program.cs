using System;
using System.Collections.Generic;
using System.Linq;

namespace LinearClassifier
{
    class Program
    {
        const int MoveQuantity = 128;                                               // Number of moves for one rollout.
        const int LayerDimension = 64;                                              // Размер первого слоя.
        const int OutputDimension = 9;                                              // Размер выходного слоя = количество допустимых действий (ходов).
        const int BigData = 1024;                                                   // Объём запоминаемых данных для обучения сети.
        const double LearningRate = 0.1;                                            // Коэффициент обучаемости.
        static Cube TrainCube = new Cube();
        static int TaskDimension = 24;
        // ReplayMemory собирает стейты, ходы и реварды в большой массив данных (см лекция 13, 1ч11мин).
        static (List<int> State, int Move, double Reward, List<int> NewState)[] ReplayMemory = new (List<int>, int, double, List<int>)[BigData * MoveQuantity];
        private static (List<int> UnicState, List<double> ActionReward)[] QFunction = new (List<int>, List<double>)[MoveQuantity];
        static Random Rnd = new Random();
        const double Gamma = 0.99;
        
        // Первый слой.
        static Layer Layer_1 = new Layer(LayerDimension, TaskDimension);
        // Выходной слой.
        static Layer OutputLayer = new Layer(OutputDimension, LayerDimension);

        static void Main(string[] args)
        {
            Layer_1.WeightsInitialization();                                            // Инициализация весов всех нейронов.
            OutputLayer.WeightsInitialization();
            int UnicsCounter = 0;                                                       // Счётчик уникальных состояний в Q-функции.
            int DataCounter = 0;                                                        // Счётчик туплов в ReplayMemory.
            List<int> TerminalState = new List<int> { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6 };
            for (int Episode = 0; Episode < BigData; Episode++)
            {
                TrainCube.SetSolved();                                                  // Установка исходной позиции на кубе.
                // Создаём скрамбл.
                int ScrLength = Rnd.Next(1, 10);                                        // Рандомная длина скрамбла от 1 до 10.
                int[] Scramble = new int[ScrLength + 1];
                Scramble = GetScramble(ScrLength);
                WriteScramble(Scramble);
                SetScramble(TrainCube, Scramble);                                       // Скрамблим куб.
                TrainCube.Print();
                // Rollout - Основной прогон, генерирующий набор из MoveQuantity ходов.
                int Rollout = 0;
                while (Rollout < MoveQuantity && !TrainCube.IsSolved())
                {
                    int QAction = (int)Moves.O;                                         // Содержит ход, который будем делать.
                    int Discount = Rollout + 1;
                    int NextMove;
                    ReplayMemory[DataCounter].State = TrainCube.GetState();
                    int QIndex = ExistQIndex(TrainCube.GetState());
                    int NextQIndex;
                    if (QIndex != -1)                                                           // Минус 1 - признак того, что нет такого стейта.
                    { // Есть уже такой текущий стейт.
                        QAction = Argmax(QFunction[QIndex].ActionReward);                       // Получаем топ ход из Q-функции
                        ProbabilityMove(TrainCube, QAction + 1, 0.1, out QAction);              // Делаем ход на кубе с некоторой вероятностью (0.1).
                        ReplayMemory[DataCounter].Move = QAction;                               // Запоминаем сделанный ход.
                        ReplayMemory[DataCounter].NewState = TrainCube.GetState();              // Запоминаем новый стейт.
                        NextQIndex = ExistQIndex(TrainCube.GetState());                         // Ищем новый стейт в Q-функции.
                        if (NextQIndex != -1)
                        { // Есть такой новый стейт.
                            NextMove = Argmax(QFunction[NextQIndex].ActionReward);
                            // Запоминаем Reward, полученный от Environment после сделанного хода.
                            ReplayMemory[DataCounter].Reward = EnviornmentReward(Rollout);
                        }
                        else
                        { // Нет такого нового стейта.
                            QFunction[UnicsCounter].UnicState = TrainCube.State;
                            QFunction[UnicsCounter].ActionReward = NeuralNetwork(TrainCube);
                            NextMove = Argmax(QFunction[UnicsCounter].ActionReward);
                            UnicsCounter++;
                            // Изменяем исходный Reward сделанного хода.
                            ReplayMemory[DataCounter].Reward = EnviornmentReward(Rollout);
                        }
                    }
                    else
                    { // Нет такого текущего стейта.
                        QFunction[UnicsCounter].UnicState = TrainCube.GetState();               // Прописываем новый стейт в Q-функцию.
                        QFunction[UnicsCounter].ActionReward = NeuralNetwork(TrainCube);        // TODO: Выяснить передаётся по значению или по ссылке. - Создать интерфейс (стр. 182 Троелсен).
                        QAction = Argmax(QFunction[UnicsCounter].ActionReward);                 // Получаем топ ход из Q-функции.
                        UnicsCounter++;
                        ProbabilityMove(TrainCube, QAction + 1, 0.1, out QAction);              // Делаем ход на кубе с некоторой вероятностью (0.1).
                        ReplayMemory[DataCounter].Move = QAction;                               // Запоминаем сделанный ход.
                        ReplayMemory[DataCounter].NewState = TrainCube.GetState();              // Запоминаем новый стейт.
                        NextQIndex = ExistQIndex(TrainCube.GetState());                         // Ищем новый стейт в Q-функции.
                        if (NextQIndex != -1)
                        { // Есть такой новый стейт.
                            NextMove = Argmax(QFunction[NextQIndex].ActionReward);
                            // Изменяем исходный Reward сделанного хода.
                            ReplayMemory[DataCounter].Reward = EnviornmentReward(Rollout);
                        }
                        else
                        { // Нет такого нового стейта.
                            QFunction[UnicsCounter].UnicState = TrainCube.GetState();
                            QFunction[UnicsCounter].ActionReward = NeuralNetwork(TrainCube);
                            NextMove = Argmax(QFunction[UnicsCounter].ActionReward);
                            UnicsCounter++;
                            // Изменяем исходный Reward сделанного хода.
                            ReplayMemory[DataCounter].Reward = EnviornmentReward(Rollout);
                        }
                    }
                    //Console.Write($"Ход сети №{Rollout + 1}: {QAction + 1}");
                    //Console.ReadLine();
                    Rollout++;
                    DataCounter++;
                }
                // TODO: Minibatch and GradientDescend.
                int[] BatchIndex = new int[MoveQuantity];                                              // Набор номеров ReplayMemory для минибатча.
                for (int i = 0; i < MoveQuantity; i++)
                {
                    BatchIndex[i] = Rnd.Next(0, DataCounter - 1);
                }
                double[] YTarget = new double[MoveQuantity];
                double[] Loss = new double[MoveQuantity];
                double[] GradLoss = new double[MoveQuantity];
                double QValue;
                double QNewValue;
                int QIndex;
                int QMax;
                for (int i = 0; i < MoveQuantity; i++)
                {
                    QIndex = ExistQIndex(ReplayMemory[BatchIndex[i]].NewState);
                    QMax = Argmax(QFunction[QIndex].ActionReward);
                    QNewValue = QFunction[QIndex].ActionReward[QMax];               // <----- Q-function для NewState из ReplayMemory.
                    QIndex = ExistQIndex(ReplayMemory[BatchIndex[i]].State);
                    QMax = Argmax(QFunction[QIndex].ActionReward);
                    QValue = QFunction[QIndex].ActionReward[QMax];                  // <----- Q-function для State из ReplayMemory.
                    if (ReplayMemory[BatchIndex[i]].NewState.SequenceEqual(TerminalState))
                    {
                        YTarget[i] = ReplayMemory[BatchIndex[i]].Reward;
                    }
                    else
                    {
                        YTarget[i] = ReplayMemory[BatchIndex[i]].Reward + Gamma * QNewValue;
                    }
                    Loss[i] = (YTarget[i] - QValue)* (YTarget[i] - QValue);
                    //GradLoss[i]=
                }
            }
        }

        // *******************************
        // ***** Используемые методы *****
        // *******************************

        // Выдаёт Reward от внешней среды.
        private static int EnviornmentReward(int rollout)
        {
            if (TrainCube.IsSolved())
            {
                return 1;
            }
            else if (rollout + 1 == MoveQuantity)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        // Выдаёт номер под которым стоит новое состояние в Q-функции и (-1) если такого состояния нет.
        private static int ExistQIndex(List<int> state)
        {
            int i = 0;
            int result = -1;
            while (result == -1 && QFunction[i].UnicState != null)
            {
                if (state.SequenceEqual(QFunction[i].UnicState))
                {
                    result = i;
                }
                i++;
            }
            return result;
        }
        private static int Argmax(List<double> policy)
        {
            int result = 0;
            double max = double.MinValue;
            for (int i = 0; i < policy.Count; i++)
            {
                if (max < policy[i])
                {
                    max = policy[i];
                    result = i;
                }
            }
            return result;
        }
        // Метод прогоняющий данные через нейронную сеть.
        private static List<double> NeuralNetwork(Cube SomeCube)
        {            
            List<double> Layer_1_Output = new List<double>(LayerDimension);
            List<double> resultPolicy = new List<double>(OutputDimension);
            // Передаём состояние куба на вход всем нейронам первого слоя.
            for (int i = 0; i < LayerDimension; i++)
            {
                Layer_1.Neurons[i].Inputs = SomeCube.GetStateToDouble();
            }
            for (int i = 0; i < LayerDimension; i++)
            {
                Layer_1_Output[i] = Layer_1.ReLu()[i];                                    // Функция нелинейности.
                //Layer_1_Output[i] = Layer_1.Sigmoid()[i];                               // - Другая функция нелинейности.
            }
            // Передаём выход первого слоя на вход нейронам выходного слоя.
            // Считаем выходное значение каждого нейрона (метод SetOutput).
            // И записываем это значение в выход неросети.
            double Average = 0;
            foreach (Neuron neuron in OutputLayer.Neurons)
            {
                for (int i = 0; i < LayerDimension; i++)
                {
                    neuron.Inputs[i] = Layer_1_Output[i];
                }
                neuron.SetOutput();
                Average += neuron.Output;
            }
            Average /= OutputDimension;
            for (int i = 0; i < OutputDimension; i++)
            {
                resultPolicy.Add(OutputLayer.Neurons[i].Output - Average);
            }
            return resultPolicy;
        }
        // Ход с некоторой вероятностью.
        static void ProbabilityMove(Cube SomeCube, int SomeMove, double Probability, out int qmove)
        {
            double Epsilon = Rnd.NextDouble();
            qmove = 0;
            if (Epsilon > Probability)
            {
                MakeMove(SomeCube, SomeMove);
            }
            else
            {
                int old = SomeMove;
                SomeMove = Rnd.Next(0, 8);
                while (SomeMove == old)
                {
                    SomeMove = Rnd.Next(0, 8);
                }
                MakeMove(SomeCube, SomeMove + 1);
                qmove = SomeMove;
            }
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
            while ( scramble[i] > 0)
            {
                MakeMove(SomeCube, scramble[i]);
                i++;
            }
        }
        // Метод создания скрамбла.
        private static int[] GetScramble (int scrambleLength)
        {
            int[] scArray = new int[scrambleLength + 1];
            for (byte i = 0; i < scrambleLength; i++)
            {
                if (i > 0)
                {
                    int prev = scArray[i - 1];
                    if (prev == 1 || prev == 2 || prev == 3)
                    {
                        scArray[i] = Rnd.Next((int)Moves.U, (int)Moves.F2);
                    }
                    else if (prev == 4 || prev == 5 || prev == 6)
                    {
                        if (Rnd.NextDouble() < 0.5)
                        {
                            scArray[i] = Rnd.Next((int)Moves.R, (int)Moves.R2);
                        }
                        else
                        {
                            scArray[i] = Rnd.Next((int)Moves.F, (int)Moves.F2);
                        }
                        break;
                    }
                    else if (prev == 7 || prev == 8 || prev == 9)
                    {
                        scArray[i] = Rnd.Next((int)Moves.R, (int)Moves.U2);
                    }
                    else
                    {
                        Console.WriteLine("Получен неверный номер хода! Номер должен быть от 1 до 9.");
                    }
                }
                else
                {
                    scArray[i] = Rnd.Next(1, 9);
                }
            }
            return scArray;
        }
        // Метод вывода скрамбла.
        static void WriteScramble(int[] scramble)
        {
            Console.WriteLine("Ваш скрамбл:");
            int i = 0;
            while ( scramble[i] > 0)
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