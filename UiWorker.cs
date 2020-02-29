using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Drawember
{
    public class UiWorker
    {
        public static UiWorker Instance { get; } = new UiWorker();

        public Dispatcher Dispatcher;

        public Label Percentage;
        public Label Score;

        private byte _percentageValue = 0;
        private uint _scoreValue = 0;


        public double PercentageValue 
        {
            get => _percentageValue;

            set
            {
                _percentageValue = (byte)Math.Round(value);

                Dispatcher.Invoke(() => UpdateGUI());
            }
        }
        public uint ScoreValue
        {
            get => _scoreValue;

            set
            {
                _scoreValue = value;

                Dispatcher.Invoke(() => UpdateGUI());
            }
        }

        public void UpdateGUI() 
        {
            Score.Content      = $"Score: {_scoreValue}";
            Percentage.Content = $"Last:  {_percentageValue} %";
        }
    }
}
