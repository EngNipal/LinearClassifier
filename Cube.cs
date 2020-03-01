using System;
using System.Collections.Generic;

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
                if (value.Count == Elements && value.GetType() == _state.GetType())
                {
                    for (int i = 0; i < Elements; i++)
                    {
                        int element = value[i];
                        _state[i] = element;
                    }
                }
                else
                {
                    Console.WriteLine("А у вас ошибка! Количество или тип входных элементов не соответствует количеству или типу, установленным для куба");
                }
                
            }
        }
        // Далее методы, реализующие ходы (R, R' - Rp, R2, U, U', U2, F, F', F2).
        public void MoveR()
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
        public void MoveRp()
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
        public void MoveR2()
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
        public void MoveU()
        {
            int Changer;
            Changer = _state[4];
            _state[4] = _state[8];
            _state[8] = _state[12];
            _state[12] = _state[16];
            _state[16] = Changer;
            Changer = _state[5];
            _state[5] = _state[9];
            _state[9] = _state[13];
            _state[13] = _state[17];
            _state[17] = Changer;
            Changer = _state[0];
            _state[0] = _state[2];
            _state[2] = _state[3];
            _state[3] = _state[1];
            _state[1] = Changer;
        }
        public void MoveUp()
        {
            int Changer;
            Changer = _state[4];
            _state[4] = _state[16];
            _state[16] = _state[12];
            _state[12] = _state[8];
            _state[8] = Changer;
            Changer = _state[5];
            _state[5] = _state[17];
            _state[17] = _state[13];
            _state[13] = _state[9];
            _state[9] = Changer;
            Changer = _state[0];
            _state[0] = _state[1];
            _state[1] = _state[3];
            _state[3] = _state[2];
            _state[2] = Changer;
        }
        public void MoveU2()
        {
            int Changer;
            Changer = _state[4];
            _state[4] = _state[12];
            _state[12] = Changer;
            Changer = _state[8];
            _state[8] = _state[16];
            _state[16] = Changer;
            Changer = _state[5];
            _state[5] = _state[13];
            _state[13] = Changer;
            Changer = _state[9];
            _state[9] = _state[17];
            _state[17] = Changer;
            Changer = _state[0];
            _state[0] = _state[3];
            _state[3] = Changer;
            Changer = _state[1];
            _state[1] = _state[2];
            _state[2] = Changer;
        }
        public void MoveF()
        {
            int Changer;
            Changer = _state[2];
            _state[2] = _state[7];
            _state[7] = _state[21];
            _state[21] = _state[12];
            _state[12] = Changer;
            Changer = _state[3];
            _state[3] = _state[5];
            _state[5] = _state[20];
            _state[20] = _state[14];
            _state[14] = Changer;
            Changer = _state[8];
            _state[8] = _state[10];
            _state[10] = _state[11];
            _state[11] = _state[9];
            _state[9] = Changer;
        }
        public void MoveFp()
        {
            int Changer;
            Changer = _state[2];
            _state[2] = _state[12];
            _state[12] = _state[21];
            _state[21] = _state[11];
            _state[11] = Changer;
            Changer = _state[3];
            _state[3] = _state[14];
            _state[14] = _state[20];
            _state[20] = _state[5];
            _state[5] = Changer;
            Changer = _state[8];
            _state[8] = _state[9];
            _state[9] = _state[11];
            _state[11] = _state[10];
            _state[10] = Changer;
        }
        public void MoveF2()
        {
            int Changer;
            Changer = _state[2];
            _state[2] = _state[21];
            _state[21] = Changer;
            Changer = _state[7];
            _state[7] = _state[12];
            _state[12] = Changer;
            Changer = _state[3];
            _state[3] = _state[20];
            _state[20] = Changer;
            Changer = _state[5];
            _state[5] = _state[14];
            _state[14] = Changer;
            Changer = _state[8];
            _state[8] = _state[11];
            _state[11] = Changer;
            Changer = _state[8];
            _state[9] = _state[10];
            _state[10] = Changer;
        }
        //Метод задаёт решённое состояние куба.
        internal void SetSolved()
        {
            _state.Clear();
            for (int i = 0; i < _state.Capacity; i++)
            {
                switch (i + 1)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        _state.Add(1);
                    break;
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        _state.Add(2);
                    break;
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                        _state.Add(3);
                    break;
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                        _state.Add(4);
                    break;
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                        _state.Add(5);
                    break;
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                        _state.Add(6);
                    break;
                    default:
                        Console.WriteLine("Что-то пошло не так с кубом в методе SetSolved");
                    break;
                }
            }
        }
        //Метод проверяет, является ли текущее состояние куба решённым           TODO: Возможно стоит переделать метод.
        internal bool IsSolved()
        {
            bool flag = true;
            int i = 0;
            while (flag && (i < _state.Count))
            {
                int n = 1 + i / 4;
                if (_state[i] != n)
                {
                    flag = false;
                }
                i++;
            }
            return flag;
        }
        //Метод выводит текущее состояние куба в консоль
        internal void Print()
        {
            for (int i = 0; i < _state.Count; i++)
            {
                Console.Write(_state[i] + " ");
            }
            Console.WriteLine();
            //Console.ReadLine();
        }
    }
}