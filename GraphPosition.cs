using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearClassifier
{/// <summary>Клас для Графа Поизиций с одним корневым узлоь</summary>
    class GraphPosition
    {
        /// <summary>Корневой узел.
        /// Задаётся один раз при создании Графа</summary>
        public Node Root { get; }

        public GraphPosition(Position position)
        {
            NodeFromPosition(position, out Node root);
            Root = root;
        }

        /// <summary>Словарь содержащий ссылки на узлы по их позициям</summary>
        private Dictionary<Position, Node> PositionsNodes
            = new Dictionary<Position, Node>();

        /// <summary>Метод получения узла в дереве по позиции</summary>
        /// <param name="position">Позиция для узла</param>
        /// <param name="node">Возвращает узел содержащий позицию</param>
        /// <returns><see langword="true"/> - если возвращён новый узел,
        /// <see langword="false"/> - если возвращён уже имеющийся узел</returns>
        private bool NodeFromPosition(Position position, out Node node)
        {
            // Проверяется наличие розиции в уже созданных
            // если нет, то создаётся новый узел с этой позицией
            if (!PositionsNodes.ContainsKey(position))
            {
                PositionsNodes.Add(position, node = new Node(position));
                return true;
            }

            // Возврат узла содеожащего позицию
            node = PositionsNodes[position];
            return false;
        }
        
    }
}
