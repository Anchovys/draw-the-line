using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Drawember
{
    enum State
    {
        Remembering,
        Drawing,
        Result
    
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point currentPoint = new Point();
        State currentState = State.Drawing;
        bool startDrawing = false;

        List<Point> points = new List<Point>();

        Dispatcher dispatcherWindow;

        BackgroundWorker backgroundWorker = new BackgroundWorker();

        public MainWindow()
        {
            dispatcherWindow = Dispatcher;

            InitializeComponent();

            UiWorker.Instance.Dispatcher = Dispatcher;
            UiWorker.Instance.Percentage = last_l;
            UiWorker.Instance.Score      = score_l;
            UiWorker.Instance.UpdateGUI();

            DrawGrid();

            backgroundWorker.DoWork += BackgroundWorker_DoWork;

            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("Запущен "  + currentState.ToString());

            while (true)
            {
                switch (currentState)
                {
                    case State.Result:

                        System.Threading.Thread.Sleep(500);

                        dispatcherWindow.Invoke(() => paintSurface.Children.Clear());

                        currentState = State.Remembering;


                        break;

                    case State.Remembering:

                        dispatcherWindow.Invoke(() => RandomGenerate());

                        System.Threading.Thread.Sleep(200);

                        dispatcherWindow.Invoke(() => rememberSurface.Children.Clear());

                        currentState = State.Drawing;

                        startDrawing = true;

                        break;

                }
                System.Threading.Thread.Sleep(500);
            }
        }


        private void Canvas_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                if (!startDrawing && currentState == State.Drawing)
                {
                    startDrawing = true;
                }

                currentPoint = e.GetPosition(this);
            }
        }

        private void Canvas_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState != MouseButtonState.Pressed && currentState == State.Drawing)
            {
                startDrawing = false;

                double percent = PercentageOfComplete();

                UiWorker.Instance.PercentageValue = percent;

                if (percent < 50)
                {
                    UiWorker.Instance.ScoreValue = 0;
                }
                else 
                {
                    UiWorker.Instance.ScoreValue ++;
                }

                //MessageBox.Show($"Вы закончили рисование.\nПроцентов закрашено: {PercentageOfComplete()}%");
                currentState = State.Result;
            }
        }

        private void Canvas_MouseMove_1(object sender, MouseEventArgs e)
        {
            // ничего не нажато или рисование не запущено
            if (e.LeftButton != MouseButtonState.Pressed || !startDrawing || currentState != State.Drawing)
            {
                return;
            }

            // сдвинули курсор на слишком маленькое расстояние
            if (Math.Abs(currentPoint.X - e.GetPosition(this).X) < 4 &&
                Math.Abs(currentPoint.Y - e.GetPosition(this).Y) < 4)
            {
                return;
            }

            currentPoint = e.GetPosition(this);

            var line = new Ellipse
            {
                Fill = Brushes.Black,
                Height = 10,
                Width = 10,
            };

            paintSurface.Children.Add(line);

            Canvas.SetLeft(line, Math.Round(currentPoint.X));
            Canvas.SetTop(line, Math.Round(currentPoint.Y));

            Console.WriteLine($"{currentPoint.X},{currentPoint.Y}");

            Console.WriteLine(paintSurface.Children.Count);

        }

        private double PercentageOfComplete() 
        {
            int inRightPosition = 0;
            int counter = 0;

            bool[] checklist = new bool[points.Count];


            foreach (var item in paintSurface.Children)
            {
                (double x, double y) pos = (Canvas.GetLeft(item as UIElement), Canvas.GetTop(item as UIElement));

                bool check = false;

                for (int i = 0; i < points.Count; i++)
                {
                    Point point = points[i];

                    if (Math.Abs(pos.x - point.X) < 23 && Math.Abs(pos.y - point.Y) < 23)
                    {
                        if (!checklist[i])
                        {
                            inRightPosition++;
                            checklist[i] = true;
                            check = true;
                            break;
                        }
                    }

                }

                if (!check) counter++;


            }

           

            //foreach (var point in points)
            //{
            //    bool check = false;
            //    foreach (var item in paintSurface.Children)
            //    {
            //        (double x, double y) pos = (Canvas.GetLeft(item as UIElement), Canvas.GetTop(item as UIElement));

            //        if (Math.Abs(pos.x - point.X) < 15 && Math.Abs(pos.y - point.Y) < 15)
            //        {
            //            if (!check)
            //            {
            //                check = true;
            //            }
            //        }


            //        if (!check)
            //        {
            //            counter+=1;
            //        }
            //    }

            //    if (check)
            //    {
            //        inRightPosition ++;
            //    }

            //}

            var percent = points.Count == 0 || paintSurface.Children.Count == 0 ? 0.0 : ((inRightPosition * 100) / points.Count) * 0.5 + (((paintSurface.Children.Count - counter) * 100) / paintSurface.Children.Count) * 0.5;

            Console.WriteLine($"{inRightPosition} of {points.Count} ({percent}%) --- {counter}");

            return percent;
        }

        private void RandomGenerate() 
        {
            int width =  570;
            int height = 450;

            points.Clear();

            // начинаем где-то в центре экрана с рандомным смещением
            Point lastPoint = new Point(
                (width / 2) + new Random().Next(-250, 50), 
                new Random().Next(0, 50)
                );


            for (int i = 0; i < new Random().Next(50, 100); i++)
            {
                var point = new Ellipse
                {
                    Fill = Brushes.Gold,
                    Height = 30,
                    Width  = 30,
                };

                rememberSurface.Children.Add(point);

                Canvas.SetLeft (point, lastPoint.X);
                Canvas.SetTop  (point, lastPoint.Y);

                points.Add (new Point(lastPoint.X, lastPoint.Y));

                lastPoint = new Point(lastPoint.X + new Random().Next(2, 7), lastPoint.Y + new Random().Next(2, 7));
            }

        }

        private void DrawGrid() 
        {

            int width = 570;
            int height = 450;

            int countX = 19;
            int countY = 15;


            for (int x = 0; x < countX; x++)
            {
                gridSurface.Children.Add(new Line
                {
                    Stroke = Brushes.Black,
                    Opacity = 0.2,

                    X1 = (width / countX * (x + 1)),
                    X2 = (width / countX * (x + 1)),

                    Y1 = 0,
                    Y2 = height
                }) ;
            }

            for (int y = 0; y < countY; y++)
            {
                gridSurface.Children.Add(new Line
                {
                    Stroke = Brushes.Black,
                    Opacity = 0.2,

                    Y1 = (height / countY * (y + 1)),
                    Y2 = (height / countY * (y + 1)),

                    X1 = 0,
                    X2 = width
                });
            }
        }


    }
}
