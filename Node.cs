using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearClassifier
{
    class Node
    {
        public Node() : this(new Position())
        { }
        public Node (Position position) : this(position, new Move[Number], new int[Number])
        { }
        public Node (Position position, Move[] edges, int[] indexes)
        {
            Position = (Position) position.Clone();
            edges.CopyTo(Edges, 0);
            indexes.CopyTo(Indexes, 0);
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
                //else                                                                  // !!!!!!!!!!!!!!
                //{
                if (value.GetType() == _position.GetType())
                {
                    Position helper = value;
                    _position = (Position)helper.Clone();
                }
                //}
            }
        }
        // Набор ходов узла - рёбра графа.
        private Move[] _edges { get; set; }
        public Move[] Edges
        {
            get
            {
                Move[] result = new Move[Number];
                //if (_edges == null)
                //{
                //    _edges = new Move[Number];                                        // !!!!!!!!!!!!!!!!!!
                //}
                _edges.CopyTo(result, 0);
                return result;
            }
            set
            {
                if (_edges == null)
                {
                    _edges = new Move[Number];
                    for (int i = 0; i < Number; i++)
                    {
                        _edges[i] = new Move(0, 0, 0);                                  // !!!!!!!!!!!!!!!!!!!!!
                    }
                }
                //else
                //{
                if (value.Length == _edges.Length && value.GetType() == _edges.GetType())
                {
                    Move[] helper = value;
                    _edges = helper;
                }
                else
                {
                    Console.WriteLine("Несоответствие в Node.Edges");
                }
                //}
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
                _indexes.CopyTo(result, 0);
                return result;
            }
            set
            {
                if (_indexes == null)
                {
                    _indexes = new int[Number];
                }
                //else
                //{
                if (value.Length == _indexes.Length && value.GetType() == _indexes.GetType())
                {
                    int[] helper = value;
                    helper.CopyTo(_indexes, 0);
                }
                else
                {
                    Console.WriteLine("Несоответствие в Node.Indexes");
                }
                //}
            }
        }
    }
}
