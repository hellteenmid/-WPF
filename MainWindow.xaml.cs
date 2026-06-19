using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Text.RegularExpressions;

namespace AlarmApp
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer Timer;
        private DateTime alarmTime;
        private bool budilnik = false;
        private string lastVTime = "";

        public MainWindow()
        {
            InitializeComponent();
            InitializeClock();
        }

        private void InitializeClock()
        {
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += ClockTimer_Tick;
            Timer.Start();
            UpdateCurrentTime();
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            UpdateCurrentTime();
            if (budilnik)
            {
                ProverkaAlarma();
            }
        }

        private void UpdateCurrentTime()
        {
            TimeTextBox.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void AlarmTimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = AlarmTimeTextBox.Text;
            string formatted = FormatTimeInput(input);

            if (formatted != input)
            {
                int caretIndex = AlarmTimeTextBox.CaretIndex;
                AlarmTimeTextBox.Text = formatted;
                if (caretIndex <= formatted.Length)
                {
                    AlarmTimeTextBox.CaretIndex = caretIndex;
                }
            }

            PoprobUstanovitAlarm(AlarmTimeTextBox.Text);
        }

        private string FormatTimeInput(string input)
        {
            string digits = Regex.Replace(input, @"[^\d]", "");

            if (digits.Length == 0)
                return "";

            if (digits.Length == 1)
            {
                return digits + ":";
            }
            else if (digits.Length == 2)
            {
                return digits + ":";
            }
            else if (digits.Length >= 3)
            {
                string hours = digits.Substring(0, 2);
                string minutes = digits.Substring(2, Math.Min(2, digits.Length - 2));

                int h = int.Parse(hours);
                if (h > 23) hours = "23";

                if (minutes.Length == 2)
                {
                    int m = int.Parse(minutes);
                    if (m > 59) minutes = "59";
                    return $"{hours}:{minutes}";
                }
                else
                {
                    return $"{hours}:{minutes}";
                }
            }

            return input;
        }

        private void PoprobUstanovitAlarm(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                SbrosAlarma();
                AlarmTimeTextBox.Background = Brushes.White;
                return;
            }

            if (PoprobRazobratVremya(input, out TimeSpan vremyaAlarma))
            {
                UstanovitAlarm(vremyaAlarma);
                lastVTime = input;
                AlarmTimeTextBox.Background = Brushes.LightGreen;
            }
            else
            {
                if (input != lastVTime)
                {
                    SbrosAlarma();
                }
                AlarmTimeTextBox.Background = Brushes.LightYellow;
            }
        }

        private bool PoprobRazobratVremya(string input, out TimeSpan result)
        {
            result = TimeSpan.Zero;
            input = input.Trim();

            
            if (input.Contains(":"))
            {
                string[] parts = input.Split(':');

            
                if (parts.Length >= 2 && parts[0].Length > 0 && parts[1].Length > 0)
                {
                 
                    if (parts[1].Length == 2)
                    {
                        if (int.TryParse(parts[0], out int hours) &&
                            int.TryParse(parts[1], out int minutes))
                        {
                            if (hours >= 0 && hours <= 23 && minutes >= 0 && minutes <= 59)
                            {
                                result = new TimeSpan(hours, minutes, 0);
                                return true;
                            }
                        }
                    }
                }
            }

            // 4 цифры подряд (1430 -> 14:30)
            if (input.Length == 4 && int.TryParse(input, out int fourDigits))
            {
                int hours = fourDigits / 100;
                int minutes = fourDigits % 100;

                if (hours >= 0 && hours <= 23 && minutes >= 0 && minutes <= 59)
                {
                    result = new TimeSpan(hours, minutes, 0);
                    return true;
                }
            }

            // 3 цифры (830 -> 8:30)
            if (input.Length == 3 && int.TryParse(input, out int threeDigits))
            {
                int hours = threeDigits / 100;
                int minutes = threeDigits % 100;

                if (hours >= 0 && hours <= 23 && minutes >= 0 && minutes <= 59)
                {
                    result = new TimeSpan(hours, minutes, 0);
                    return true;
                }
            }

            return false;
        }

        private void UstanovitAlarm(TimeSpan vremyaAlarma)
        {
            DateTime now = DateTime.Now;
            alarmTime = new DateTime(now.Year, now.Month, now.Day,
                                     vremyaAlarma.Hours, vremyaAlarma.Minutes, 0);

            if (alarmTime <= now)
            {
                alarmTime = alarmTime.AddDays(1);
            }

            budilnik = true;
        }

        private void SbrosAlarma()
        {
            budilnik = false;
        }

        private void ProverkaAlarma()
        {
            DateTime now = DateTime.Now;
            if (now.Hour == alarmTime.Hour && now.Minute == alarmTime.Minute)
            {
                ZapuskAlarma();
            }
        }

        private void ZapuskAlarma()
        {
            budilnik = false;
            AlarmTimeTextBox.Background = Brushes.White;

            MessageBox.Show("Сработал будильник",
                          "Будильник",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);

            AlarmTimeTextBox.Text = "";
        }
    }
}