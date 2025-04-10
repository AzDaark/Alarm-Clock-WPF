using System;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Media;

namespace AlarmWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// Ajout d'une alarme personnalisée
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        //A circle
        private Ellipse ellipse;
        //3 hands
        private Line minutes;
        private Line hours;
        private Line seconds;

        private DispatcherTimer flashTimer;
        private bool isRed = true;
        private int flashCount = 0;
        public MainWindow()
        {
            InitializeComponent();

            
        }
        public static DateTime Now { get; }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            //Defines the number of seconds between each triggering of the Tick event 
            timer.Interval = TimeSpan.FromSeconds(1);
            //Associates an event procedure with the timer's Tick event, you'll need to write this event procedure.
            timer.Tick += timer_Tick;
            //Launches the timer, otherwise nothing happens
            timer.Start();

            ellipse = new Ellipse();
            CNVClock.Children.Add(ellipse);
            ellipse.Width = 300;
            ellipse.Height = 300;
            ellipse.Stroke = Brushes.Gray;
            ellipse.StrokeThickness = 1;

            seconds = new Line();
            CNVClock.Children.Add(seconds);
            seconds.Stroke = Brushes.Red;
            seconds.StrokeThickness = 1;
            //The point of origin is at the center of the circle
            seconds.X1 = ellipse.Width / 2;
            seconds.Y1 = ellipse.Height / 2;

            minutes = new Line();
            CNVClock.Children.Add(minutes);
            minutes.Stroke = Brushes.Blue;
            minutes.StrokeThickness = 1;
            //The point of origin is at the center of the circle
            minutes.X1 = ellipse.Width / 2;
            minutes.Y1 = ellipse.Height / 2;

            hours = new Line();
            CNVClock.Children.Add(hours);
            hours.Stroke = Brushes.Black;
            hours.StrokeThickness = 1;
            //The point of origin is at the center of the circle
            hours.X1 = ellipse.Width / 2;
            hours.Y1 = ellipse.Height / 2;


            

        }
        private void FlashTimer_Tick(object sender, EventArgs e)
        {
            if (isRed)
                mainGrid.Background = Brushes.White;
            else
                mainGrid.Background = Brushes.Red;

            isRed = !isRed;

            flashCount++;

            if (flashCount > 10) 
            {
                flashTimer.Stop();
                flashCount = 0;
                mainGrid.Background = Brushes.White; 
            }

        }
        void StartFlashing()
        {
            if (flashTimer == null)
            {
                flashTimer = new DispatcherTimer();
                flashTimer.Interval = TimeSpan.FromMilliseconds(500);
                flashTimer.Tick += FlashTimer_Tick;
            }

            flashTimer.Start();
        }
        void timer_Tick(object sender, EventArgs e) {
            //I define the length of the hand, I could set another value
            double lengthSecondsHand = ellipse.Width / 2;
            seconds.X2 = ellipse.Width / 2 + Math.Cos(15 * Math.PI / 30 - DateTime.Now.Second * Math.PI / 30) * lengthSecondsHand;
            seconds.Y2 = ellipse.Height / 2 + Math.Sin(-15 * Math.PI / 30 + DateTime.Now.Second * Math.PI / 30) * lengthSecondsHand;

            double lengthMinutesHand = ellipse.Width / 3;
            minutes.X2 = ellipse.Width / 2 + Math.Cos(15 * Math.PI / 30 - DateTime.Now.Minute * Math.PI / 30) * lengthMinutesHand;
            minutes.Y2 = ellipse.Height / 2 + Math.Sin(-15 * Math.PI / 30 + DateTime.Now.Minute * Math.PI / 30) * lengthMinutesHand;

            double lengthHoursHand = ellipse.Width / 4;

            hours.X2 = ellipse.Width / 2 + Math.Cos(15 * Math.PI / 6 - DateTime.Now.Hour * Math.PI / 6) * lengthHoursHand;
            hours.Y2 = ellipse.Height / 2 + Math.Sin(-15 * Math.PI / 6 + DateTime.Now.Hour * Math.PI / 6) * lengthHoursHand;
            triggerAlarm();
        }

   
        private void BTN_Add_Click(object sender, RoutedEventArgs e)
        {
            String total = TXB_hours.Text +  " : " + TXB_minutes.Text;
            LB_reveils.Items.Add(total);
        }

        private void BTN_Remove_Click(object sender, RoutedEventArgs e)
        {
            LB_reveils.Items.RemoveAt(LB_reveils.SelectedIndex);
        }

        private void TXB_hours_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TXB_hours.MaxLength = 2;
            if ((System.Text.RegularExpressions.Regex.IsMatch(TXB_hours.Text, "[^0-9]"))){
                TXB_hours.Clear();
            }
         
        }

        private void TXB_minutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            TXB_minutes.MaxLength = 2;
            if ((System.Text.RegularExpressions.Regex.IsMatch(TXB_minutes.Text, "[^0-9]")))
            {
                TXB_minutes.Clear();
            }

        }



        void triggerAlarm()
        {
            DateTime localdate = DateTime.Now;
            if(int.TryParse(TXB_hours.Text, out int targetHour) && int.TryParse(TXB_minutes.Text,out int targetMinute))
            {
                if (localdate.Hour == targetHour && localdate.Minute == targetMinute && localdate.Second == 0)
                {
                    StartFlashing();
                    MediaPlayer player = new MediaPlayer();
                    player.Open(new Uri("yourphonelinging.mp3", UriKind.Relative));
                    player.Play();
                        
                }
            }
        }
    }
}
