using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace _2m_paste
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public delegate void passing_methood();

    public partial class MainWindow : Window
    {
        private bool cancel_all_value = false;
        private string username;
        bool closing = false;
        string[] s1 = new string[] { "__*__" };
        bool scroll_left = false;
        bool scroll_right = false;
        bool reset = false;
        string[] s2 = new string[] { "--*--" };
        private Double margin_help = -600;
        public bool Cancel_all_value { get => cancel_all_value; set => cancel_all_value = value; }
        public double Margin_help { get => margin_help; set => margin_help = value; }
        public string Username { get => username; set => username = value; }

        public MainWindow()
        {
            InitializeComponent();
            loading_setting();

            ////initialize events
            get_start.Click += ((snder, e) => { get_started(); });
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            scrolling();
        }

        public void loading_setting()
        {
            string dirs = Properties.Settings.Default.DIRS;
            int using_count = Properties.Settings.Default.COUNT_OF_USE;
            string user = Properties.Settings.Default.USER_NAME;
            user_name.Text = user;


            if (dirs == "") { add_new(); }
            else { loading_dir_panels(); }
            if (using_count > 1 && user != string.Empty && !user.Contains("USER NAME"))
            {
                grid.Children.Remove(Help);
                body.Effect = null;
            }
            else
            {

            }
        }

        public void saving_setting()
        {
            if (!reset)
            {
                saving_dir_panels();
                string user = user_name.Text;
                var a = user.Split(" \n\t\r".ToCharArray());
                a = a.Where(item => item != "").ToArray();
                if (user_name.Text == string.Empty || a.Length == 0) { Properties.Settings.Default.USER_NAME = ""; } else { Properties.Settings.Default.USER_NAME = user_name.Text; }
                Properties.Settings.Default.COUNT_OF_USE++;
                Properties.Settings.Default.Save();
            }
        }

        public void loading_dir_panels()
        {
            string[] total_dirs = _2m_paste.Properties.Settings.Default.DIRS.Split(s1, StringSplitOptions.RemoveEmptyEntries);
            foreach (string items in total_dirs)
            {
                string[] item = items.Split(s2,StringSplitOptions.RemoveEmptyEntries);
                if (Directory.Exists(item[0]))
                {
                    add_new(item[1], item[0]);
                }
            }
            if(total_dirs.Length == 0) { add_new(); }
        }

        public void saving_dir_panels()
        {
            string dirs = "";
            foreach (dir_panel panel in Get_all_panels())
            {
                if (Directory.Exists(panel.dir_dir.Text))
                {
                    dirs += $"{panel.dir_dir.Text}{s2[0]}{panel.dir_title.Text}{s1[0]}";
                }
            }
            _2m_paste.Properties.Settings.Default.DIRS = dirs;
            _2m_paste.Properties.Settings.Default.Save();
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!closing)
            {
                e.Cancel = true;
                closing = true;
                message_box_show("EXIT", "DO YOU WANT TO EXIT NOW !!!",
                    async()=> {
                        cancel_all_prod();
                        bool f = true;
                        dir_panel[] panels = Get_all_panels();
                        while (f)
                        {
                            f = false;
                            foreach (dir_panel item in panels)
                            {
                                if (item.Processing) { f = true; }   
                            }
                            await Task.Run(()=> { Thread.Sleep(500); });
                        }
                        this.Close(); },
                    ()=> { },
                    () => { closing = false;},
                    1);
                saving_setting();
            }
        }
        
        public void add_new(string dir_title = "Dir Title", string dir_path = "Directory")
        {
            dir_panel new_dir = new dir_panel(dir_path, dir_title,this);
            try
            {
                new_dir.Height = this.ActualHeight - 160;
                new_dir.dir_scroller.MaxHeight = this.ActualHeight - 305;
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            new_dir.go.Click += ((sender, e2) => {
                message_box_show("GO", "DO YOU WANT TO GO NOW !!!",
                    new passing_methood(async() => {
                        new_dir.Excuting_task = true;
                        cancel_all_disable();
                        await new_dir.GO();
                        new_dir.Excuting_task = false;
                        cancel_all_disable();
                    }),
                    new passing_methood(() => { }),
                    new passing_methood(() => { }),
                    1);
            });
            Frame frame = new Frame();
            frame.Content = new_dir;
            StackPanel stack = new StackPanel();
            stack.Background = new SolidColorBrush(Color.FromArgb(25, 255, 255, 255));
            stack.Margin = new Thickness(5, 0, 5, 0);
            Button button = new Button();
            button.Style = Application.Current.FindResource("control_buttons") as Style;
            button.Content = "";
            button.FontSize = 50;
            button.VerticalAlignment = VerticalAlignment.Top;
            button.Width = 90;
            button.HorizontalAlignment = HorizontalAlignment.Right;
            button.Height = 90;
            button.Margin = new Thickness(-10, 8, 8, -100);
            button.Click += ((sed, e1) => { dir_list.Children.Remove(stack); });
            stack.Children.Add(button);
            stack.Children.Add(frame);
            dir_list.Children.Add(stack);
        }

        private void add_new_dir_Click(object sender, RoutedEventArgs e)
        {
            add_new();
            Scroller.ScrollToHorizontalOffset(Scroller.HorizontalOffset + 500);
        }

        public void message_box_show(string title, string text,passing_methood handler1, passing_methood handler2, passing_methood handler3, int mode = 0)
        {
            RoutedEventHandler h1 = (async (sender, e) => { await Task.Run(() => { Thread.Sleep(1000); }); Message.Visibility = Visibility.Hidden; Message.reset(); handler1.Invoke(); set_waiting(false); });
            RoutedEventHandler h2 = (async (sender, e) => { await Task.Run(() => { Thread.Sleep(1000); }); Message.Visibility = Visibility.Hidden; Message.reset(); handler2.Invoke(); set_waiting(false); });
            RoutedEventHandler h3 = (async (sender, e) => { await Task.Run(() => { Thread.Sleep(1000); }); Message.Visibility = Visibility.Hidden; Message.reset(); handler3.Invoke(); set_waiting(false); });
            Message.Visibility = Visibility.Visible;
            set_waiting(true);
            Message.start_message(title, text,
                 h1,
                 h2,
                 h3,
                 mode);
        }

        private void go_all_Click(object sender, RoutedEventArgs e)
        {
            message_box_show("GO ALL", "DO YOU WANT TO GO ALL NOW !!!",
                (() => { Go_all();}),
                (() => { }),
                (() => { }),
                1);
        }

        public dir_panel[] Get_all_panels()
        {
            int i = dir_list.Children.Count;
            dir_panel[] panel = new dir_panel[i];
            for(int n = 0; n < i;n++)
            {
                StackPanel stack1 = dir_list.Children[n] as StackPanel;
                Frame frm = stack1.Children[1] as Frame;
                panel[n] = frm.Content as dir_panel;
            }
            return panel;
        }

        public async void Go_all()
        {
            int n = dir_list.Children.Count;
            total_progress_control(true);
            dir_panel[] panels = Get_all_panels();
            foreach (dir_panel panel in panels)
            {
                panel.dis_enable_buttons(false);
            }
            for(int i = 0; i < panels.Length;i++)
            {
                StackPanel stack1 = dir_list.Children[i] as StackPanel;
                Frame frm = stack1.Children[1] as Frame;
                panels[i] = frm.Content as dir_panel;
                panels[i].go_progress.ValueChanged += ((send, e2) => { total_porgress.Value = (panels[i].go_progress.Value / n) + (100 * i / n); });
                panels[i].cancel_prog.Click += ((SEND, E3) => { this.Cancel_all_value = true; });
                if (Cancel_all_value) { break; }
                await panels[i].GO();
            }
            foreach (dir_panel panel in panels)
            {
                panel.dis_enable_buttons(true);
            }
            total_progress_control(false);
            cancel_all.IsEnabled = true;
            Cancel_all_value = false;
        }

        public void cancel_all_disable()
        {
            bool t = false;
            foreach (dir_panel item in Get_all_panels())
            {
                if (item.Excuting_task) { t = true; break; }
            }
            if (t) { go_all.IsEnabled = false; }
            else { go_all.IsEnabled = true; }
        }

        public void total_progress_control(bool open_close)
        {
            if (open_close)
            {
                ThicknessAnimation animation = new ThicknessAnimation(new Thickness(0),TimeSpan.FromSeconds(1));
                BackEase back = new BackEase();
                back.EasingMode = EasingMode.EaseInOut;
                animation.EasingFunction = back;
                fotter_slide1.BeginAnimation(StackPanel.MarginProperty, animation);
            }
            else
            {
                ThicknessAnimation animation = new ThicknessAnimation(new Thickness(((-1) * this.ActualWidth), 0, 0, 0), TimeSpan.FromSeconds(1));
                BackEase back = new BackEase();
                back.EasingMode = EasingMode.EaseInOut;
                animation.EasingFunction = back;
                fotter_slide1.BeginAnimation(StackPanel.MarginProperty, animation);
            }
        }

        public void set_waiting(bool wait)
        {
            foreach (dir_panel panel in Get_all_panels())
            {
                panel.waiting = wait;
            }
        }

        public void cancel_all_prod()
        {
            foreach (dir_panel panel in Get_all_panels())
            {
                panel.Cancel = true;
                panel.cancel_prog.IsEnabled = false;
            }
        }

        private void cancel_all_Click(object sender, RoutedEventArgs e)
        {
            message_box_show("CANCEL", "DO YOU WANT TO CANCEL ALL !!!",
                (() => {  cancel_all_prod(); Cancel_all_value = true; cancel_all.IsEnabled = false; }),
                (() => { }),
                (() => { }),
                1);
        }
        
        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            size_changing();
        }

        public void size_changing()
        {
            Scroller.Height = this.ActualHeight - 140;
            ThicknessAnimation animation = new ThicknessAnimation(new Thickness(((-1) * this.ActualWidth), 0, 0, 0), TimeSpan.FromSeconds(0));
            fotter_slide1.BeginAnimation(StackPanel.MarginProperty, animation);
            foreach (var item in dir_list.Children)
            {
                StackPanel stack1 = item as StackPanel;
                Frame frm = stack1.Children[1] as Frame;
                dir_panel panel = frm.Content as dir_panel;
                stack1.Height = this.ActualHeight - 160;
                panel.Height = this.ActualHeight - 160;
                panel.dir_scroller.MaxHeight = this.ActualHeight - 300;
            }
        }

        private async void skip_help(object sender, RoutedEventArgs e)
        {
            Storyboard storyboard = this.FindResource("close_help") as Storyboard;
            storyboard.Begin();
            await Task.Run(() => { Thread.Sleep(1000); });
            Help.Visibility = Visibility.Hidden;
            body.Effect = null;
        }
        
        private void more_scrolling(object sender, RoutedEventArgs e)
        {
            ThicknessAnimation animation = new ThicknessAnimation(new Thickness(Margin_help, 0, 0, 0), TimeSpan.FromSeconds(1));
            BackEase ease = new BackEase();
            ease.EasingMode = EasingMode.EaseInOut;
            animation.EasingFunction = ease;
            Button button = sender as Button;
            StackPanel panel1 = button.Parent as StackPanel;
            StackPanel panel2 = panel1.Parent as StackPanel;
            panel2.BeginAnimation(MarginProperty, animation);
        }

        private void exit_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mini_max_button_Click(object sender, RoutedEventArgs e)
        {
            if (window.WindowState == WindowState.Normal) { window.WindowState = WindowState.Maximized; } else { window.WindowState = WindowState.Normal; }
        }

        private void mini_button_Click(object sender, RoutedEventArgs e)
        {
            window.WindowState = WindowState.Minimized;
        }

        private void title_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        
        async void scrolling()
        {
            while (true)
            {
                if (scroll_left) { Scroller.ScrollToHorizontalOffset(Scroller.HorizontalOffset - 20); }
                else if (scroll_right) { Scroller.ScrollToHorizontalOffset(Scroller.HorizontalOffset + 20); }
                else { await Task.Run(() => { Thread.Sleep(1000); }); }
                await Task.Run(() => { Thread.Sleep(20); });
            }
        }

        private void mouse_scroller_left_MouseEnter(object sender, MouseEventArgs e)
        {
            scroll_left = true;
        }

        private void mouse_scroller_left_MouseLeave(object sender, MouseEventArgs e)
        {
            scroll_left = false;
        }

        private void mouse_scroller_right_MouseEnter(object sender, MouseEventArgs e)
        {
            scroll_right = true;
        }

        private void mouse_scroller_right_MouseLeave(object sender, MouseEventArgs e)
        {
            scroll_right = false;
        }

        private void window_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Home)
            {
                Properties.Settings.Default.USER_NAME = "";
                Properties.Settings.Default.DIRS = "";
                Properties.Settings.Default.COUNT_OF_USE = 0;
                Properties.Settings.Default.BG_COLOR = "#67ffffff";
                reset = true;
                Properties.Settings.Default.Save();
                Console.WriteLine("setting reseted....");
            }
        }

        public void get_started()
        {
            user_name.Text = user_name_input.Text;
            username = user_name_input.Text;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                get_started();
            }
        }
        
    }
}
