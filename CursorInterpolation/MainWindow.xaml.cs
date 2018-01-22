using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace CursorInterpolation
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        int x1, x2, y1, y2 = 0;
        Random rndX, rndY;

        //общие переменные, используемые в методах: move_cursor_directly, move_cursor_imitate_behaviour
        int x, y, beginCorn, eighth;
        double cosA, sinA;

        private void get_cursor_positions()
        {
            x1 = Int32.Parse(x1_text.Text);
            y1 = Int32.Parse(y1_text.Text);

            x2 = Int32.Parse(x2_text.Text);
            y2 = Int32.Parse(y2_text.Text);

            rndX = new Random();
            rndY = new Random();
        }

        private int getUpCosDirection(double cosAA) //возвращает начальное число для отсчета параметра
        {
            //int parts = 5; //количество частей, на которые разделется одна восьмая круга

            //определяем, в какой из угловых диапазонов мы попали
            int beginRand = 0;
            if (cosAA <= 1 && cosAA >= 0.996)
                beginRand = -1;
            else if (cosAA < 0.996 && cosAA >= 0.972)
                beginRand = 0;
            else if (cosAA < 0.972 && cosAA >= 0.923)
                beginRand = 1;
            else if (cosAA < 0.923 && cosAA >= 0.852)
                beginRand = 2;
            else if (cosAA < 0.852 && cosAA >= 0.76)
                beginRand = 3;
            else if (cosAA < 0.76 && cosAA >= 0.707)
                beginRand = 4;
            return beginRand;
        }

        private int getDownCosDirection(double cosAA)
        {
            //определяем, в какой из угловых диапазонов мы попали
            int beginRand = 0;
            if (cosAA < 0.707 && cosAA >= 0.649)
                beginRand = 4;
            else if (cosAA < 0.649 && cosAA >= 0.522)
                beginRand = 3;
            else if (cosAA < 0.522 && cosAA >= 0.382)
                beginRand = 2;
            else if (cosAA < 0.382 && cosAA >= 0.233)
                beginRand = 1;
            else if (cosAA < 0.233 && cosAA >= 0.078)
                beginRand = 0;
            else if (cosAA < 0.078 && cosAA >= 0)
                beginRand = -1;
            return beginRand;
        }

        //метод создан для вынесения общей логики, которая выполняется перед рассчетами точек движения курсора
        //это было сделано по той причине, что в не зависимости от метода передвижения курсора, изначально определяются: path, cosA, sinA
        //ощие переменные, используемые в обоих методах, вынесены за пределы методов

        //также важная функция, выполняемая методом - ответ на вопрос "нажно ли вообще передвигать курсор?". Если передвижение курсора не
        //требуется, то произойдет возврат из метода для предотвращения деления на 0
        private bool pre_quarter_calcucation()
        {
            get_cursor_positions();

            //определеяем, в какой четверти находится конечная точка по отношению к начальной
            x = x2 - x1; y = y2 - y1;

            cosA = 1; sinA = 1;
            double path = Math.Sqrt(x * x + y * y);

            beginCorn = -1;

            //определение ситуаций, когда вычисление пути приведет к делению на 0
            if (x == 0)
            {
                cosA = 0;
                if (y == 0) sinA = 0;
            }
            else if (y == 0)
            {
                sinA = 0;
                if (x == 0) cosA = 0;
            }

            //если cosA и sinA = 0, то движение курсора не требуется, выходим их метода
            if (cosA == 0 && sinA == 0) return false;
            else
            {
                cosA = x / path; //значение sinA нигде не используется в коде, т.к. sin и cos любого угла взимозаменяемы
                                 //поэтому было решено оставить sinA только в виде комментариев, чтобы не нарушать логику вычислении или будущего изменения
                                 //вычислений
                                 //sinA = y / path;
                eighth = 0;
                cosA = Math.Abs(cosA);

                return true;
            }
        }

        //отдельный расчет четверти, в которой находится конечная точка передвижения курсора
        private int determine_eighth()
        {
            //первая четверть - правая верхняя часть
            if (x >= 0 && y <= 0)
            {
                //определяем, в какаую из восьмых частей мы попали. Здесь выбираем из двух возможных. Используется свойство cos - sin
                //когда при значении угла в 45 град. значение тригонометрических функций одинаково и равно 0.70710678118 или просто 0.7
                if (cosA >= 0.707)
                    eighth = 1;
                else
                    eighth = 2;
            }

            if (x >= 0 && y >= 0)
            {
                if (cosA >= 0.707)
                    eighth = 8;
                else
                    eighth = 7;
            }

            if (x <= 0 && y >= 0)
            {
                if (cosA >= 0.707)
                    eighth = 5;
                else
                    eighth = 6;
            }

            if (x <= 0 && y <= 0)
            {
                if (cosA >= 0.707)
                    eighth = 4;
                else
                    eighth = 3;
            }

            return eighth;
        }

        //преимущественно данный метод планируется использовать в тех случаях, когда нужно точное перемещение курсора от начала до конца
        private void move_cursor_directly(object sender, RoutedEventArgs e)
        {
            if (!pre_quarter_calcucation())
                return;
            else
            {
                eighth = determine_eighth();
            }

            //часть по вычислению текущих координат курсора при движении по прямой. используется обычное уравнение прямой в прямоугольной системе
            //после того, как мы определили, в какой восьмой части находится точка назначения, мы понимаем, по какой из осей и в каком направлении
            //(знак) мы будем двигать значение на 1 с каждым шагом
            //но в отличие от случайной генерации чисел, теперь мы будем брать значение по другой оси из уравнениия прямой, это самое точное решение, которое только может быть
            double x3 = x1; double y3 = y1;
            switch (eighth)
            {
                case 2: case 3: case 6: case 7:
                    while (y2 > y3)
                    {
                        y3 += 1;
                        x3 = (x2 * (y3 - y1) + x1 * (y2 - y3)) / (y2 - y1);
                        x3 = Math.Round(x3);
                        SetCursorPos((int)x3, (int)y3);
                        Thread.Sleep(4);
                    }
                    while (y2 < y3)
                    {
                        y3 -= 1;
                        x3 = (x2 * (y3 - y1) + x1 * (y2 - y3)) / (y2 - y1);
                        x3 = Math.Round(x3);
                        SetCursorPos((int)x3, (int)y3);
                        Thread.Sleep(4);
                    }
                    //SetCursorPos(x2, y1); в принудительной подстройке координат в конце хода нет необходимости, т.к. при подставлении в уравнение прямой целых чисел, должны получиться целые координаты
                    break;

                case 1: case 4: case 5: case 8:
                    while (x2 > x3)
                    {
                        x3 += 1;
                        y3 = (y2 * (x3 - x1) + y1 * (x2 - x3)) / (x2 - x1);
                        y3 = Math.Round(y3);
                        SetCursorPos((int)x3, (int)y3);
                        Thread.Sleep(4);
                    }
                    while (x2 < x3)
                    {
                        x3 -= 1;
                        y3 = (y2 * (x3 - x1) + y1 * (x2 - x3)) / (x2 - x1);
                        y3 = Math.Round(y3);
                        SetCursorPos((int)x3, (int)y3);
                        Thread.Sleep(4);
                    }
                    //SetCursorPos(x1, y2);
                    break;
            }
        }

        private void move_cursor_imitate_behaviour(object sender, RoutedEventArgs e)
        {
            if (!pre_quarter_calcucation())
                return;
            else
            {
                eighth = determine_eighth();

                switch (eighth)
                {
                    case 1: case 4: case 5: case 8:
                        beginCorn = getUpCosDirection(cosA);
                        break;
                    case 2: case 3: case 6: case 7:
                        beginCorn = getDownCosDirection(cosA);
                        break;
                }
            }

            //передвигаем курсор, теперь мы знаем на какой угол нужно его развернуть
            switch (eighth)
            {
                case 1: case 8:
                    while (x1 < x2)
                    {
                        x1 += 5;
                        if (eighth == 1)
                            y1 -= rndY.Next(beginCorn, beginCorn + 3);
                        else
                            y1 += rndY.Next(beginCorn, beginCorn + 3);
                        SetCursorPos(x1, y1);
                        Thread.Sleep(10);
                    }
                    break;
                case 4: case 5:
                    while (x1 > x2)
                    {
                        x1 -= 5;
                        if (eighth == 4)
                            y1 -= rndY.Next(beginCorn, beginCorn + 3);
                        else
                            y1 += rndY.Next(beginCorn, beginCorn + 3);
                        SetCursorPos(x1, y1);
                        Thread.Sleep(10);
                    }
                    break;
                case 2: case 3:
                    while (y1 > y2)
                    {
                        y1 -= 5;
                        if (eighth == 2)
                            x1 += rndX.Next(beginCorn, beginCorn + 3);
                        else
                            x1 -= rndX.Next(beginCorn, beginCorn + 3);
                        SetCursorPos(x1, y1);
                        Thread.Sleep(10);
                    }
                    break;
                case 6: case 7:
                    while (y1 < y2)
                    {
                        y1 += 5;
                        if (eighth == 7)
                            x1 += rndX.Next(beginCorn, beginCorn + 3);
                        else
                            x1 -= rndX.Next(beginCorn, beginCorn + 3);
                        SetCursorPos(x1, y1);
                        Thread.Sleep(10);
                    }
                    break;
            }

            //логика прямоленейного довода курсора, чтобы избавиться от неточности при перемещении крусора на большое расстояние
            switch (eighth)
            {
                case 1: case 4: case 5: case 8:
                    while (y2 > y1)
                    {
                        y1 += 1;
                        SetCursorPos(rndX.Next(x2 - 2, x2 + 2), y1);
                        Thread.Sleep(4);
                    }
                    while (y2 < y1)
                    {
                        y1 -= 1;
                        SetCursorPos(rndX.Next(x2 - 2, x2 + 2), y1);
                        Thread.Sleep(4);
                    }
                    SetCursorPos(x2, y1);
                    break;

                case 2: case 3: case 6: case 7:
                    while (x2 > x1)
                    {
                        x1 += 1;
                        SetCursorPos(x1, rndY.Next(y2 - 2, y2 + 2));
                        Thread.Sleep(4);
                    }
                    while (x2 < x1)
                    {
                        x1 -= 1;
                        SetCursorPos(x1, rndY.Next(y2 - 2, y2 + 2));
                        Thread.Sleep(4);
                    }
                    SetCursorPos(x1, y2);
                    break;
            }
        }

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point p);

    }

}
