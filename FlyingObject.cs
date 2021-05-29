using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using JR.Utils.GUI.Forms;

namespace WPF_Petzold
{
    public class FlyingObject : Window
    {
        [STAThread]
        public static void Main()
        {
            Application app = new Application();
            app.Run(new FlyingObject());
        }

        public FlyingObject()
        {
            Title = "Полёт тела в пространстве";
            SizeToContent = SizeToContent.WidthAndHeight;

            var grid = new Grid
            {
                Margin = new Thickness(16)
            };
            Content = grid;

            for (int i = 0; i < 6; i++)
            {
                var rowdef = new RowDefinition
                {
                    Height = GridLength.Auto
                };
                grid.RowDefinitions.Add(rowdef);
            }
            
            for (int i = 0; i < 2; i++)
            {
                var coldef = new ColumnDefinition();

                if (i == 1)
                    coldef.Width = new GridLength(128, GridUnitType.Star);
                else
                    coldef.Width = GridLength.Auto;

                grid.ColumnDefinitions.Add(coldef);
            }

            string[] labels = {
                "Изначальная скорость тела", "Угол полета", "Начальная высота тела",
                "Масса тела", "Коэффициент сопротивления воздуха"
            };

            for (int i = 0; i < labels.Length; i++)
            {
                var label = new Label
                {
                    Content = labels[i],
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                grid.Children.Add(label);
                Grid.SetRow(label, i);
                Grid.SetColumn(label, 0);

                var textbox = new TextBox
                {
                    Margin = new Thickness(16)
                };
                grid.Children.Add(textbox);
                
                Grid.SetRow(textbox, i);
                Grid.SetColumn(textbox, 1);
                Grid.SetColumnSpan(textbox, 3);
            }
            grid.Children[1].Focus();

            var btn = new Button
            {
                Content = "Рассчитать",
                Margin = new Thickness(5)
            };

            btn.Click += delegate { 
                ButtonOnClick(grid); 
            };

            grid.Children.Add(btn);
            Grid.SetRow(btn, 5);
            Grid.SetColumn(btn, 3);
        }

        void ButtonOnClick(Grid grid)
        {
            int[] int_input = new int[5];

            int c = -1;
            for (int i = 1; i < grid.Children.Count; i += 2)
            {
                c++;
                var str = Convert.ToString(grid.Children[i]);
                str = new string(str.Where( element => 
                char.IsDigit(element))
                    .ToArray());

                var parsedValue = int.Parse(str);

                int_input[c] = parsedValue;
            }

            const double g = 9.8;
            const double deltaTime = 0.05; //Шаг измерений
            double startHeight = Convert.ToInt64(int_input[2]);
            double startSpeed = Convert.ToInt64(int_input[0]);
            double time = 0;
            double k = Convert.ToInt64(int_input[3]);
            double m = Convert.ToInt64(int_input[4]);
            const int halfCircle = 180;

            double angle = Convert.ToInt64(int_input[1]);
            angle = Math.PI / halfCircle * angle;

            int N = 2000; //Ограничение по количеству операций

            double[] x = new double[N];
            double[] y = new double[N];

            double[] vx = new double[N];
            double[] vy = new double[N];

            vx[0] = startSpeed * Math.Cos(angle);
            vy[0] = startSpeed * Math.Sin(angle);
            x[0] = 0;
            y[0] = startHeight;

            string resultString = "Поехали! \n";

            resultString += $"time = 0:\t\t({x[0]} ; {y[0]})\n";

            for (int i = 1; i < N; i++)
            {
                time += deltaTime;

                vx[i] = vx[i - 1] - (k / m) * vx[i - 1] * deltaTime;
                vy[i] = vy[i - 1] - (g + k / m * vy[i - 1]) * deltaTime;

                x[i] = x[i - 1] + vx[i - 1] * deltaTime;
                y[i] = y[i - 1] + vy[i - 1] * deltaTime;

                if (y[i] < 0) break;

                resultString += $"time = {time:0.00}:\t({x[i]:0.000} ; {y[i]:0.000})\n";
                
            }

            resultString += "Приземлились!";


            FlexibleMessageBox.Show(resultString, "Отчёт");

        }
    }
        
}
