using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearClassifier
{
    class Program
    {
        const int MoveQuantity = 128;                                               // Number of moves per rollout.
        const int MaxNodes = 1024;                                                  // Max number of nodes in MCTS.
        const int LayerDimension = 64;                                              // Размер первого слоя.
        public const int OutputDimension = 9;                                       // Размер выходного слоя = количество допустимых действий (ходов).
        const int BigData = 1024;                                                   // Объём запоминаемых данных для обучения сети.
        //const double LearningRate = 0.1;                                            // Коэффициент обучаемости.
        static Cube TrainCube = new Cube();
        public const int TaskDimension = 24;
        static Random Rnd = new Random();
        const double Gamma = 0.95;
        
        // Первый слой.
        static Layer Layer_1 = new Layer(LayerDimension, TaskDimension);
        // Выходной слой.
        static Layer OutputLayer = new Layer(OutputDimension, LayerDimension);

        static void Main(string[] args)
        {
            // Инициализация весов всех нейронов.
            Layer_1.WeightsInitialization();
            OutputLayer.WeightsInitialization();
            double[]MoveArray = new double[OutputDimension];
            //Данные для формирования датасета.
            double[,] Policy = new double[MoveQuantity, OutputDimension];
            double[] NetworkMove = new double[MoveQuantity];
            // Большой прогон.
            for (int Episode = 0; Episode < BigData; Episode++)
            {
                // Установка исходной позиции на кубе.
                TrainCube.SetSolved();
                // Создаём скрамбл с рандомной длиной от 1 до 10.
                int ScrLength = Rnd.Next(1, 10);
                int[] Scramble = new int[ScrLength + 1];
                Scramble = GetScramble(ScrLength);
                // WriteScramble(Scramble);
                SetScramble(TrainCube, Scramble);                                               // Скрамблим куб.
                // TrainCube.Print();
                // Rollout - Основной прогон, генерирующий набор из MoveQuantity ходов.
                int Rollout = 0;
                while (Rollout < MoveQuantity && !TrainCube.IsSolved())
                {
                    Cube MCTSCube = new Cube();
                    MCTSCube.SetState(TrainCube.GetState());                                    // Копия куба, чтобы не испортить TrainCube.
                    int UnicsCounter = 0;                                                       // Счётчик уникальных позиций.
                    // Дерево - массив нодов.
                    Node[] Tree = new Node[MaxNodes];
                    // Список, запоминающий пройденный "вниз" путь в виде пары (номер_позиции_в_дереве, номер_выбранного_хода_в_ней).
                    List<(int, int)> Way = new List<(int, int)>();
                    int NextMove = 0;                                                           // Указатель на следующий ход.
                    // MCTS
                    for (int MCTS = 0; MCTS < MaxNodes; MCTS++)
                    {
                        Position NewPos = new Position (MCTSCube.GetState().ToArray(), 0);     // Вместо нуля должен быть выход от второй "головы" сети.
                        int PosIndex = PositionIndex(NewPos.State, Tree);
                        if (PosIndex != -1)
                        {// Есть уже такая позиция в дереве.
                            // Запоминаем в предыдущей позиции индексы последующих позиций.
                            int prev = Way.Count;
                            Tree[Way[prev].Item1].Indexes[Way[prev].Item2] = Way[prev].Item2;
                            // Следующий ход берём как максимальный по (Q + U) см. Argmax.
                            NextMove = Argmax(Tree[PosIndex].Edges);
                            MakeMove(MCTSCube, NextMove);
                            // Добавляем в пройденный путь индексы позиции и хода.
                            Way.Add((UnicsCounter, NextMove));
                        }
                        else
                        {// Не было такой позиции в дереве (лист дерева).
                            // Добавляем позицию в дерево.
                            Tree[UnicsCounter] = new Node
                            {
                                Position = (Position)NewPos.Clone()
                            };
                            // Инициализация всех ходов для новой позиции. И запись ходов в дерево.
                            MoveArray = NeuralNetwork(MCTSCube);
                            //Tree[UnicsCounter].Item2 = Array.CreateInstance(Move, OutputDimension); //[OutputDimension];
                            for (int i = 0; i < OutputDimension; i++)
                            {
                                //Move NewMove = new Move(MoveArray[i], 0, 0);
                                Tree[UnicsCounter].Edges[i] = new Move(MoveArray[i], 0, 0);
                            }
                            // Обновление Visit и Quality для всех вышестоящих позиций в данном пути (Way).
                            if (Way.Count == 0)
                            {
                                Way.Add((0, 0));
                            }
                            for (int i = Way.Count - 1; i >= 0; i--)
                            {
                                int it1 = Way[i].Item1;
                                int it2 = Way[i].Item2;
                                Tree[it1].Edges[it2].Visit++;
                                double sum = 0;
                                foreach (int Index in Tree[Way[i].Item1].Indexes)
                                {
                                    if (Index != 0)
                                    {
                                        sum += Tree[Way[i].Item1].Edges[Index].Quality * Math.Pow(Gamma, i);
                                    }
                                }
                                Tree[Way[i].Item1].Edges[Way[i].Item2].Quality = sum / Tree[Way[i].Item1].Edges[Way[i].Item2].Visit;
                            }
                            Way.Clear();
                            //MCTSCube.SetState(TrainCube.GetState());
                            UnicsCounter++;
                        }
                    }
                    // По результатам MCTS делаем ход, по которому больше всего ходили.
                    for (int i = 0; i < OutputDimension; i++)
                    {
                        MoveArray[i] = Tree[0].Edges[i].Visit;
                        Policy[Rollout, i] = MoveArray[i];
                    }
                    NextMove = Argmax(MoveArray);
                    NetworkMove[Rollout] = NextMove;
                    MakeMove(TrainCube, NextMove);
                    //Console.Write($"Ход сети №{Rollout + 1}: {NextMove + 1}");
                    //Console.ReadLine();
                    Rollout++;
                }
        //        // TODO: Minibatch and GradientDescend.
        //        int[] BatchIndex = new int[MoveQuantity];                                              // Набор номеров ReplayMemory для минибатча.
        //        for (int i = 0; i < MoveQuantity; i++)
        //        {
        //            BatchIndex[i] = Rnd.Next(0, DataCounter - 1);
        //        }
        //        double[] YTarget = new double[MoveQuantity];
        //        double[] Loss = new double[MoveQuantity];
        //        double[] GradLoss = new double[MoveQuantity];
        //        double QValue;
        //        double QNewValue;
        //        int QIndex;
        //        int QMax;
        //        for (int i = 0; i < MoveQuantity; i++)
        //        {
        //            QIndex = ExistQIndex(ReplayMemory[BatchIndex[i]].NewState);
        //            QMax = Argmax(QFunction[QIndex].ActionReward);
        //            QNewValue = QFunction[QIndex].ActionReward[QMax];               // <----- Q-function для NewState из ReplayMemory.
        //            QIndex = ExistQIndex(ReplayMemory[BatchIndex[i]].State);
        //            QMax = Argmax(QFunction[QIndex].ActionReward);
        //            QValue = QFunction[QIndex].ActionReward[QMax];                  // <----- Q-function для State из ReplayMemory.
        //            if (ReplayMemory[BatchIndex[i]].NewState.SequenceEqual(TerminalState))
        //            {
        //                YTarget[i] = ReplayMemory[BatchIndex[i]].Reward;
        //            }
        //            else
        //            {
        //                YTarget[i] = ReplayMemory[BatchIndex[i]].Reward + Gamma * QNewValue;
        //            }
        //            Loss[i] = (YTarget[i] - QValue)* (YTarget[i] - QValue);
        //            //GradLoss[i]=
        //        }
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
        // Выдаёт номер под которым стоит новое состояние в Tree и (-1) если такого состояния нет.
        private static int PositionIndex(int[] state, Node[] tree)
        {
            int i = 0;
            int result = -1;
            if (tree[0] == null)
            {
                return result;
            }
            while (result == -1 && tree[i].Position.State != null)
            {
                if (state.SequenceEqual(tree[i].Position.State))
                {
                    result = i;
                }
                i++;
            }
            return result;
        }
        private static int Argmax(Move[] edge)
        {
            int result = 0;
            double max = double.MinValue;
            double sum;
            for (int i = 0; i < edge.Length; i++)
            {
                sum = edge[i].Policy + edge[i].UpperBound;
                if (max < sum)
                {
                    max = sum;
                    result = i;
                }
            }
            return result;
        }
        private static int Argmax(double[] arr)
        {
            int result = 0;
            double max = double.MinValue;
            for (int i = 0; i < arr.Length; i++)
            {
                if (max < arr[i])
                {
                    max = arr[i];
                    result = i;
                }
            }
            return result;
        }
        // Метод прогоняющий данные через нейронную сеть.                               // TODO: Написать две "головы" сети.
                                                                                        // Одна даёт Policy, другая - Value.
        private static double[] NeuralNetwork(Cube SomeCube)
        {            
            List<double> Layer_1_Output = new List<double>(LayerDimension);
            double[] resultPolicy = new double[OutputDimension];
            resultPolicy[0] = 0;
            // Передаём состояние куба на вход всем нейронам первого слоя.
            for (int i = 0; i < LayerDimension; i++)
            {
                Layer_1.Neurons[i].Inputs = SomeCube.GetStateToDouble();
            }
            for (int i = 0; i < LayerDimension; i++)
            {
                Layer_1_Output.Add(Layer_1.ReLu()[i]);                                    // Функция нелинейности.
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
                resultPolicy[i] = OutputLayer.Neurons[i].Output - Average;
            }
            return resultPolicy;
        }
        // Ход с некоторой вероятностью.
        //static void ProbabilityMove(Cube SomeCube, int SomeMove, double Probability, out int qmove)
        //{
        //    double Epsilon = Rnd.NextDouble();
        //    qmove = 0;
        //    if (Epsilon > Probability)
        //    {
        //        MakeMove(SomeCube, SomeMove);
        //    }
        //    else
        //    {
        //        int old = SomeMove;
        //        SomeMove = Rnd.Next(0, 8);
        //        while (SomeMove == old)
        //        {
        //            SomeMove = Rnd.Next(0, 8);
        //        }
        //        MakeMove(SomeCube, SomeMove + 1);
        //        qmove = SomeMove;
        //    }
        //}
        // Метод, делающий заданный ход на кубе.
        static void MakeMove (Cube SomeCube, int MoveLabel)
        {
            switch (MoveLabel)
            {
                case 0:
                    SomeCube.MoveR();
                    break;
                case 1:
                    SomeCube.MoveRp();
                    break;
                case 2:
                    SomeCube.MoveR2();
                    break;
                case 3:
                    SomeCube.MoveU();
                    break;
                case 4:
                    SomeCube.MoveUp();
                    break;
                case 5:
                    SomeCube.MoveU2();
                    break;
                case 6:
                    SomeCube.MoveF();
                    break;
                case 7:
                    SomeCube.MoveFp();
                    break;
                case 8:
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
                    if (prev == 0 || prev == 1 || prev == 2)
                    {
                        scArray[i] = Rnd.Next((int)Moves.U, (int)Moves.F2);
                    }
                    else if (prev == 3 || prev == 4 || prev == 5)
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
                    else if (prev == 6 || prev == 7 || prev == 8)
                    {
                        scArray[i] = Rnd.Next((int)Moves.R, (int)Moves.U2);
                    }
                    else
                    {
                        Console.WriteLine("Получен неверный номер хода! Номер должен быть от 0 до 8.");
                    }
                }
                else
                {
                    scArray[i] = Rnd.Next(0, 8);
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
            R, Rp, R2, U, Up, U2, F, Fp, F2
        }
    }
}