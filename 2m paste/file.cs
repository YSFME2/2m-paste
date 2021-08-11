using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace _2m_paste
{
    /// <summary>
    /// Interaction logic for dir_panel.xaml
    /// </summary>
    /// 
    public class file
    {
        private string dir;
        private string title;
        private string photo;
        private bool copy_cut;

        public string Dir { get => dir; set => dir = value; }
        public string Title { get => title; set => title = value; }
        public string Photo { get => photo; set => photo = value; }
        public bool Copy_cut { get => copy_cut; set => copy_cut = value; }

        public file(string dir, string title, string photo, bool copy_cut)
        {
            this.Dir = dir;
            this.Title = title;
            this.Photo = photo;
            this.Copy_cut = copy_cut;
        }

        public StackPanel Building()
        {
            StackPanel stack = new StackPanel();


            Grid grid = new Grid();
            grid.MaxWidth = 490;
            grid.Margin = new Thickness(-10, 5, 5, 5);


            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = new GridLength(60);
            grid.ColumnDefinitions.Add(column1);
            ColumnDefinition column2 = new ColumnDefinition();
            column2.Width = new GridLength(340);
            grid.ColumnDefinitions.Add(column2);
            ColumnDefinition column3 = new ColumnDefinition();
            column3.Width = new GridLength(40);
            grid.ColumnDefinitions.Add(column3);
            ColumnDefinition column4 = new ColumnDefinition();
            column4.Width = new GridLength(40);
            grid.ColumnDefinitions.Add(column4);


            RowDefinition row1 = new RowDefinition();
            row1.Height = new GridLength(50);
            grid.RowDefinitions.Add(row1);
            RowDefinition row2 = new RowDefinition();
            row2.Height = new GridLength(15);
            grid.RowDefinitions.Add(row2);


            Image image = new Image();
            try { image.Source = new BitmapImage(new Uri(Photo)); }
            catch { image.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/FILE.png")); }
            image.Height = 70;
            image.Width = 65;
            image.Margin = new Thickness(-5, -5, 0, 0);
            image.Stretch = Stretch.UniformToFill;
            Grid.SetColumn(image, 0);
            Grid.SetRow(image, 0);
            Grid.SetRowSpan(image, 2);
            grid.Children.Add(image);


            TextBlock text_title = new TextBlock();
            text_title.Text = Title;
            text_title.FontFamily = new FontFamily("/2m paste;component/Resources/#Neutra Text Alt");
            text_title.FontSize = 35;
            text_title.Background = null;
            text_title.Foreground = Brushes.White;
            Grid.SetColumn(text_title, 1);
            Grid.SetRow(text_title, 0);
            grid.Children.Add(text_title);


            TextBlock text_dir = new TextBlock();
            text_dir.Text = Dir;
            text_dir.FontFamily = new FontFamily("/2m paste;component/Resources/#Neutra Text Alt");
            text_dir.FontSize = 13;
            text_dir.Background = null;
            text_dir.Foreground = Brushes.White;
            Grid.SetColumn(text_dir, 1);
            Grid.SetRow(text_dir, 1);
            Grid.SetColumnSpan(text_dir, 3);
            grid.Children.Add(text_dir);

            Button copy_button = new Button();
            Button cut_button = new Button();

            copy_button.Style = Application.Current.FindResource("control_buttons") as Style;
            copy_button.Content = "";
            copy_button.Height = 30;
            copy_button.FontSize = 20;
            copy_button.Foreground = Brushes.Aqua;
            Copy_cut = true;
            cut_button.Foreground = Brushes.White;
            copy_button.Click += ((seder, e) => { copy_button.Foreground = Brushes.Aqua; Copy_cut = true; cut_button.Foreground = Brushes.White; });
            Grid.SetColumn(copy_button, 2);
            Grid.SetRow(copy_button, 0);
            grid.Children.Add(copy_button);

            cut_button.Style = Application.Current.FindResource("control_buttons") as Style;
            cut_button.Content = "";
            cut_button.Height = 30;
            cut_button.FontSize = 20;
            cut_button.Click += ((seder, e) => { cut_button.Foreground = Brushes.Aqua; Copy_cut = false; copy_button.Foreground = Brushes.White; });
            Grid.SetColumn(cut_button, 3);
            Grid.SetRow(cut_button, 0);
            grid.Children.Add(cut_button);
            stack.Children.Add(grid);

            return stack;
        }
    }
}
