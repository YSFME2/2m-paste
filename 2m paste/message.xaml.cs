using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace _2m_paste
{
    /// <summary>
    /// Interaction logic for message.xaml
    /// </summary>
    public partial class message : UserControl
    {
        private string title;
        private string text;
        private int mode;
        private RoutedEventHandler routed1;
        private RoutedEventHandler routed2;
        private RoutedEventHandler routed3;

        public string Title1 { get => title; set => title = value; }
        public string Text { get => text; set => text = value; }
        public int Mode { get => mode; set => mode = value; }
        public RoutedEventHandler Routed1 { get => routed1; set => routed1 = value; }
        public RoutedEventHandler Routed2 { get => routed2; set => routed2 = value; }
        public RoutedEventHandler Routed3 { get => routed3; set => routed3 = value; }

        public message()
        {
            InitializeComponent();
            btn1.Click += ( (sender, e) => { close_message(); });
            btn2.Click += ( (sender, e) => { close_message(); });
            btn3.Click += ( (sender, e) => { close_message(); });
        }

        public void preparing_message()
        {
            title_tb.Text = Title1;
            text_tb.Text = Text;
            switch (Mode)
            {
                case 0:
                    btn1.Visibility = Visibility.Hidden;
                    btn2.Visibility = Visibility.Visible;
                    btn3.Visibility = Visibility.Hidden;
                    btn2.Content = "Ok";
                    btn2.Focus();
                    break;
                case 1:
                    btn1.Visibility = Visibility.Visible;
                    btn2.Visibility = Visibility.Hidden;
                    btn3.Visibility = Visibility.Visible;
                    btn1.Content = "YES";
                    btn3.Content = "NO";
                    btn1.Focus();
                    break;
                case 2:
                    btn1.Visibility = Visibility.Visible;
                    btn2.Visibility = Visibility.Visible;
                    btn3.Visibility = Visibility.Visible;
                    btn1.Content = "YES";
                    btn2.Content = "NO";
                    btn3.Content = "Cancel";
                    btn1.Focus();
                    break;
            }
        }

        public void start_message(string title,string text ,RoutedEventHandler handler1,RoutedEventHandler handler2,RoutedEventHandler handler3, int mode = 0)
        {
            if(mode > 2 || mode < 0) { mode = 0; }
            Title1 = title;
            Text = text;
            Mode = mode;
            preparing_message();
            open_message();
            Routed1 = handler1;
            Routed2 = handler2;
            Routed3 = handler3;
            btn1.Click += Routed1;
            btn2.Click += Routed2;
            btn3.Click += Routed3;
        }

        public void open_message()
        {
            DoubleAnimation animation = new DoubleAnimation(1, TimeSpan.FromSeconds(1));
            BackEase ease = new BackEase();
            ease.EasingMode = EasingMode.EaseInOut;
            animation.EasingFunction = ease;
            message_grid.BeginAnimation(OpacityProperty, animation);
        }

        public void close_message()
        {
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(1));
            BackEase ease = new BackEase();
            ease.EasingMode = EasingMode.EaseInOut;
            animation.EasingFunction = ease;
            message_grid.BeginAnimation(OpacityProperty, animation);
        }
        public void reset()
        {
            btn1.Click -= Routed1;
            btn2.Click -= Routed2;
            btn3.Click -= Routed3;
            btn1.Visibility = Visibility.Visible;
            btn2.Visibility = Visibility.Visible;
            btn3.Visibility = Visibility.Visible;
        }
    }
}
