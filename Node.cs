using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearClassifier
{
    class Node
    {
        public Node()
        {
            Position = new Position();
            Edges = new Move[Number];
            for (int i = 0; i < Number; i++)
            {
                Edges[i] = new Move();
            }
            Indexes = new int[Number];
            Indexes.Initialize();
        }
        public Node (Position position, Move[] edges, int[] indexes)
        {
            Position = position;
            Edges = edges;
            Indexes = indexes;
        }
        // Размер массивов ходов и индексов.
        const int Number = Program.OutputDimension;
        // Позиция узла.
        private Position _position { get; set; }
        public Position Position
        {
            get
            {
                Position result = new Position();
                result = (Position) _position.Clone();
                return result;
            }
            set
            {
                if (_position == null)
                {
                    _position = new Position();
                }
                if (value.GetType () == _position.GetType())
                {
                    Position helper = value;
                    _position = (Position) helper.Clone();
                }
            }
        }
        // Набор ходов узла - рёбра графа.
        private Move[] _edges { get; set; }
        public Move[] Edges
        {
            get
            {
                if (_edges == null)
                {
                    _edges = new Move[Number];
                    _edges.Initialize();
                }
                Move[] result = new Move[Number];
                for (int i = 0; i < Number; i++)
                {
                    result[i] = _edges[i];
                }
                return result;
            }
            set
            {
                if (_edges == null)
                {
                    _edges = new Move[Number];
                    _edges.Initialize();
                }
                if (value.Length == _edges.Length && value.GetType() == _edges.GetType())
                {
                    Move[] helper = value;
                    for (int i = 0; i < Number; i++)
                    {
                        _edges[i] = helper[i];
                    }
                }
                else
                {
                    Console.WriteLine("Несоответствие в Node.Edges");
                }
            }
        }
        // Набор индексов, указывающих на номер позиции в дереве, согласно тому,
        // куда ведёт соответствующий ход из массива ходов (Edges).
        // Индекс ноль означает, что такой ход не делали, т.к. ноль в дереве зарезервирован для корневой позиции (стартовый скрамбл).
        private int[] _indexes { get; set; }
        public int[] Indexes
        {
            get
            {
                int[] result = new int[Number];
                for (int i = 0; i < Number; i++)
                {
                    result[i] = _indexes[i];
                }
                return result;
            }
            set
            {
                if (_indexes == null)
                {
                    _indexes = new int[Number];
                    _indexes.Initialize();
                }
                if (value.Length == _indexes.Length && value.GetType() == _indexes.GetType())
                {
                    int[] helper = value;
                    for (int i = 0; i < Number; i++)
                    {
                        _indexes[i] = helper[i];
                    }
                }
                else
                {
                    Console.WriteLine("Несоответствие в Node.Indexes");
                }
            }
        }
    }
}
