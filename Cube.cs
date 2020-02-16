using System;
using System.Collections.Generic;
/*
using System.Linq;
using System.Text;
using System.Threading.Tasks;
*/
namespace LinearClassifier
{
    public class Cube
    {
        // Константа, определяющая количество "наклеек" куба
        private const byte Elements = 24;
        // Массив, содержащий состояние куба
        private List<int> _state = new List<int>(Elements);
        public List<int> State
        {
            get { return _state; }
            set
            {
                if (value.Capacity == Elements)
                {
                    for (int i = 0; i < Elements; i++)
                    {
                        int element = value[i];
                        _state[i] = element;
                    }
                }
                else
                {
                    Console.WriteLine("А у вас ошибка! Количество входных элементов не соответствует количеству, установленному для куба");
                }
                
            }
        }
        // Далее методы, реализующие ходы (R, R' - Rp, R2, U, U', U2, F, F', F2).
        public void R()
        {
            int Changer;
            Changer = _state[1];
            _state[1] = _state[9];
            _state[9] = _state[21];
            _state[21] = _state[18];
            _state[18] = Changer;
            Changer = _state[3];
            _state[3] = _state[11];
            _state[11] = _state[23];
            _state[23] = _state[16];
            _state[16] = Changer;
            Changer = _state[12];
            _state[12] = _state[14];
            _state[14] = _state[15];
            _state[15] = _state[13];
            _state[13] = Changer;
        }
        public void Rp()
        {
            int Changer;
            Changer = _state[1];
            _state[1] = _state[18];
            _state[18] = _state[21];
            _state[21] = _state[9];
            _state[9] = Changer;
            Changer = _state[3];
            _state[3] = _state[16];
            _state[16] = _state[23];
            _state[23] = _state[11];
            _state[11] = Changer;
            Changer = _state[12];
            _state[12] = _state[13];
            _state[13] = _state[15];
            _state[15] = _state[14];
            _state[14] = Changer;
        }
        public void R2()
        {
            int Changer;
            Changer = _state[1];
            _state[1] = _state[21];
            _state[21] = Changer;
            Changer = _state[3];
            _state[3] = _state[23];
            _state[23] = Changer;
            Changer = _state[9];
            _state[9] = _state[19];
            _state[19] = Changer;
            Changer = _state[11];
            _state[11] = _state[17];
            _state[17] = Changer;
            Changer = _state[12];
            _state[12] = _state[15];
            _state[15] = Changer;
            Changer = _state[13];
            _state[13] = _state[14];
            _state[14] = Changer;
        }
        //public void U()
        //{
        //    byte Changer;
        //    Changer = _state[1, 0];
        //    _state[1, 0] = _state[2, 0];
        //    _state[2, 0] = _state[3, 0];
        //    _state[3, 0] = _state[4, 0];
        //    _state[4, 0] = Changer;
        //    Changer = _state[1, 1];
        //    _state[1, 1] = _state[2, 1];
        //    _state[2, 1] = _state[3, 1];
        //    _state[3, 1] = _state[4, 1];
        //    _state[4, 1] = Changer;
        //    Changer = _state[0, 0];
        //    _state[0, 0] = _state[0, 2];
        //    _state[0, 2] = _state[0, 3];
        //    _state[0, 3] = _state[0, 1];
        //    _state[0, 1] = Changer;
        //}
        //public void Up()
        //{
        //    byte Changer;
        //    Changer = _state[1, 0];
        //    _state[1, 0] = _state[4, 0];
        //    _state[4, 0] = _state[3, 0];
        //    _state[3, 0] = _state[2, 0];
        //    _state[2, 0] = Changer;
        //    Changer = _state[1, 1];
        //    _state[1, 1] = _state[4, 1];
        //    _state[4, 1] = _state[3, 1];
        //    _state[3, 1] = _state[2, 1];
        //    _state[2, 1] = Changer;
        //    Changer = _state[0, 0];
        //    _state[0, 0] = _state[0, 1];
        //    _state[0, 1] = _state[0, 3];
        //    _state[0, 3] = _state[0, 2];
        //    _state[0, 2] = Changer;
        //}
        //public void U2()
        //{
        //    byte Changer;
        //    Changer = _state[1, 0];
        //    _state[1, 0] = _state[3, 0];
        //    _state[3, 0] = Changer;
        //    Changer = _state[2, 0];
        //    _state[2, 0] = _state[4, 0];
        //    _state[4, 0] = Changer;
        //    Changer = _state[1, 1];
        //    _state[1, 1] = _state[3, 1];
        //    _state[3, 1] = Changer;
        //    Changer = _state[2, 1];
        //    _state[2, 1] = _state[4, 1];
        //    _state[4, 1] = Changer;
        //    Changer = _state[0, 0];
        //    _state[0, 0] = _state[0, 3];
        //    _state[0, 3] = Changer;
        //    Changer = _state[0, 1];
        //    _state[0, 1] = _state[0, 2];
        //    _state[0, 2] = Changer;
        //}
        //public void F()
        //{
        //    byte Changer;
        //    Changer = _state[0, 2];
        //    _state[0, 2] = _state[1, 3];
        //    _state[1, 3] = _state[5, 1];
        //    _state[5, 1] = _state[3, 0];
        //    _state[3, 0] = Changer;
        //    Changer = _state[0, 3];
        //    _state[0, 3] = _state[1, 1];
        //    _state[1, 1] = _state[5, 0];
        //    _state[5, 0] = _state[3, 2];
        //    _state[3, 2] = Changer;
        //    Changer = _state[2, 0];
        //    _state[2, 0] = _state[2, 2];
        //    _state[2, 2] = _state[2, 3];
        //    _state[2, 3] = _state[2, 1];
        //    _state[2, 1] = Changer;
        //}
        //public void Fp()
        //{
        //    byte Changer;
        //    Changer = _state[0, 2];
        //    _state[0, 2] = _state[3, 0];
        //    _state[3, 0] = _state[5, 1];
        //    _state[5, 1] = _state[1, 3];
        //    _state[1, 3] = Changer;
        //    Changer = _state[0, 3];
        //    _state[0, 3] = _state[3, 2];
        //    _state[3, 2] = _state[5, 0];
        //    _state[5, 0] = _state[1, 1];
        //    _state[1, 1] = Changer;
        //    Changer = _state[2, 0];
        //    _state[2, 0] = _state[2, 1];
        //    _state[2, 1] = _state[2, 3];
        //    _state[2, 3] = _state[2, 2];
        //    _state[2, 2] = Changer;
        //}
        //public void F2()
        //{
        //    byte Changer;
        //    Changer = _state[0, 2];
        //    _state[0, 2] = _state[5, 1];
        //    _state[5, 1] = Changer;
        //    Changer = _state[1, 3];
        //    _state[1, 3] = _state[3, 0];
        //    _state[3, 0] = Changer;
        //    Changer = _state[0, 3];
        //    _state[0, 3] = _state[5, 0];
        //    _state[5, 0] = Changer;
        //    Changer = _state[1, 1];
        //    _state[1, 1] = _state[3, 2];
        //    _state[3, 2] = Changer;
        //    Changer = _state[2, 0];
        //    _state[2, 0] = _state[2, 3];
        //    _state[2, 3] = Changer;
        //    Changer = _state[2, 1];
        //    _state[2, 1] = _state[2, 2];
        //    _state[2, 2] = Changer;
        //}
        //Метод задаёт решённое состояние куба
        internal void SetSolved()
        {
            for (byte i = 0; i < _state.Capacity; i+=4)
            {
                _state[i] = 1 + i % 4;
                _state[i + 1] = 1 + i % 4;
                _state[i + 2] = 1 + i % 4;
                _state[i + 3] = 1 + i % 4;
            }
        }
        //Метод проверяет, является ли текущее состояние куба решённым
        internal bool IsSolved()
        {
            bool flag = true;
            int i = 0;
            while (flag && (i < _state.Capacity))
            {
                int n = 1 + i % 4;
                if (_state[i] != n || _state[i + 1] != n || _state[i +2] != n || _state[i + 3] != n )
                {
                    flag = false;
                }
                i+=4;
            }
            return flag;
        }
        //Метод выводит текущее состояние куба в консоль
        internal void Print()
        {
            for (int i = 0; i < _state.Capacity; i++)
            {
                Console.Write(_state[i] + " ");
            }
            Console.WriteLine();
            //Console.ReadLine();
        }
    }
}