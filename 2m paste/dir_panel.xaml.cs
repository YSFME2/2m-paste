using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Threading;

namespace _2m_paste
{
    
    public partial class dir_panel : Page
    {
        private string title ;
        private string dir;
        private bool processing;
        private bool excuting_task = false;
        file[] Files = new file[10000];
        public string Title1 { get => title; set => title = value; }
        public string Dir { get => dir; set => dir = value; }
        public bool Cancel { get => cancel; set => cancel = value; }
        public bool Excuting_task { get => excuting_task; set => excuting_task = value; }
        public bool Processing { get => processing; set => processing = value; }

        List<string> extantions = new List<string>() { "AI", "AVI","BAT","CMD","CSS","CVS","DLL" ,"DOC", "DOCX","ESP" ,"EXE","FLV","GIF","HTML","JPG","JPEG","JS","MKV","MOV","MP3","MP4","PDF", "PNG", "PPT", "PPTX", "PSD", "RAR", "TXT", "WAV", "WMA", "XLS", "ZIP" };
        string[] fils;
        private bool cancel;
        MainWindow Main;
        public bool waiting = false;
        

        public dir_panel(string dir,string title,MainWindow main)
        {
            InitializeComponent();
            Title1 = title;
            Dir = dir;
            dir_dir.Text = dir;
            dir_title.Text = title;
            this.Main = main;
            Cancel = false;
        }

        private void dir_browes_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                dir_dir.Text = dialog.SelectedPath;
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Dir = dir_dir.Text;
                Title1 = dir_title.Text;
            }
            catch (Exception e0) { Console.WriteLine(e0.Message); }
        }

        private void add_files_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = true;
            if(open.ShowDialog() == true){ initialize(open.FileNames); }
        }

        public void initialize(string[] files)
        {
            bool existance = true;
            foreach (var item in files)
            {
                string item1 = item;
                if (!File.Exists(item))
                {
                    if (item.Length > 50) { item1 = item.Substring(item.Length - 50, 50); }
                    Main.message_box_show("ERROR", $"FILE : {item1}  ISN'T EXIST!!!", new passing_methood(() => { }), new passing_methood(() => { }), new passing_methood(() => { }));
                    existance = false;
                    break;
                }
            }
            if (existance) {
                file_list.Items.Clear();
                if (fils == null) { fils = files; }
                else { fils = fils.Union(files).ToArray(); }
                for (int i = 0; i < fils.Length; i++)
                {
                    string spic_file = fils[i];
                    string img = image_path(spic_file);
                    bool button = false;
                    var f = spic_file.Split((char)string.Format(@"\")[0]);
                    string ti = f[f.Length - 1].Substring(0, f[f.Length - 1].Length);
                    Files[i] = new file(spic_file, ti, img, true);
                    StackPanel stack = Files[i].Building();
                    stack.MouseLeftButtonDown += ((sender, e2) => { button = true; });
                    stack.MouseLeftButtonUp += ((sender, e2) => { button = false; });
                    stack.MouseMove += ((sender, e2) => {
                        if (button)
                        {
                            DataObject data = new DataObject();
                            string[] dirs = gs_items_dir();
                            if (dirs != null)
                            {
                                data = new DataObject(dirs);
                            }
                            else
                            {
                                data = new DataObject(spic_file);
                            }
                            DragDrop.DoDragDrop(stack, data, DragDropEffects.All);
                        }
                    });
                    file_list.Items.Add(stack);
                }
            }
        }

        public string image_path(string file)
        {
            string[] y = file.Split((char)string.Format(@".")[0]);
            string x = y[y.Length - 1].ToUpper();
            string ext = "FILE";
            if (extantions.Contains(x)) { ext = x; }
            string img = $"pack://application:,,,/Resources/{ext}.png";
            return img;
        }

        private void file_list_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                initialize(files);
            }
            if (e.Data.GetDataPresent(typeof(string[])))
            {
                initialize((string[])e.Data.GetData(typeof(string[])));
            }
            if (e.Data.GetDataPresent(typeof(string)))
            {
                string[] item = new string[1];
                item[0] = (string)e.Data.GetData(typeof(string));
                initialize(item);
            }
        }
        
        private void file_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(file_list.SelectedItems.Count == 1) { System.Diagnostics.Process.Start(fils[file_list.SelectedIndex]); }
        }

        private void dir_scroller_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void delete_files_Click(object sender, RoutedEventArgs e)
        {
            string[] dirs = gs_items_dir();
            if (dirs != null)
            {
                fils = fils.ToList().Except(dirs).ToArray();
                initialize(fils);
            }
            
        }

        public int[] gs_items_index()
        {
            if (file_list.SelectedItems.Count > 0)
            {
                int[] i = new int[file_list.SelectedItems.Count];
                for (int n = 0; n < file_list.SelectedItems.Count; n++)
                {
                    i[n] = file_list.Items.IndexOf(file_list.SelectedItems[n]);
                }
                return i;
            }
            return null;
        }

        public string[] gs_items_dir()
        {
            int[] i = gs_items_index();
            if (i != null)
            {
                string[] temp = new string[i.Length];
                temp = fils.Where(item => i.Contains(fils.ToList().IndexOf(item))).ToArray();
                return temp;
            }
            return null;
        }

        public int gs_item(object item)
        { return file_list.Items.IndexOf(item); }

        private void copy_files_Click(object sender, RoutedEventArgs e)
        {
            int[] indecies = gs_items_index();
            if(indecies != null)
            {
                foreach (var item in indecies)
                {
                    StackPanel panel = file_list.Items[item] as StackPanel;
                    Grid grid = panel.Children[0] as Grid;
                    Button copy_button = grid.Children[3] as Button;
                    Button cut_button = grid.Children[4] as Button;
                    copy_button.Foreground = Brushes.Aqua;
                    cut_button.Foreground = Brushes.White;
                    Files[item].Copy_cut = true;
                }
            }
        }

        private void cut_files_Click(object sender, RoutedEventArgs e)
        {
            int[] indecies = gs_items_index();
            if (indecies != null)
            {
                foreach (var item in indecies)
                {
                    StackPanel panel = file_list.Items[item] as StackPanel;
                    Grid grid = panel.Children[0] as Grid;
                    Button copy_button = grid.Children[3] as Button;
                    Button cut_button = grid.Children[4] as Button;
                    copy_button.Foreground = Brushes.White;
                    cut_button.Foreground = Brushes.Aqua;
                    Files[item].Copy_cut = false;
                }
            }
        }

        public async Task GO()
        {
            string folder = dir_dir.Text;
            if (Directory.Exists(folder))
            {
                processing = true;
                dis_enable_buttons(false);
                file_list.AllowDrop = false;
                var sb = this.FindResource("open_prog") as Storyboard;
                sb.Begin();
                int n = file_list.Items.Count;
                for (int i = 0; i < n; i++)
                {
                    await Task.Run(() => { while (waiting) { Thread.Sleep(1000); } });
                    if (Cancel) { break; }
                    go_progress.Value = (i + 1) * 100 / n;

                    StackPanel panel = file_list.Items[0] as StackPanel;
                    Grid grid = panel.Children[0] as Grid;
                    Button copy_button = grid.Children[3] as Button;
                    TextBlock file_title = grid.Children[1] as TextBlock;
                    TextBlock file_dir = grid.Children[2] as TextBlock; ;
                    bool copy_cut = (copy_button.Foreground == Brushes.Aqua);
                    string new_file = dir_dir.Text + "\\" + file_title.Text;

                    try {
                        using (FileStream SourceStream = File.Open(file_dir.Text, FileMode.Open))
                        {
                            using (FileStream DestinationStream = File.Create(new_file))
                            {
                                await SourceStream.CopyToAsync(DestinationStream);
                            }
                        }
                        if (!copy_cut) { File.Delete(file_dir.Text); }
                    }
                    catch(Exception e2) { Main.message_box_show("ERROR", e2.Message.ToUpper(), (() => { }),(() => { }),(() => { })); break; }
                    

                    file_list.Items.RemoveAt(0);
                    var temp = Files.ToList();
                    temp.RemoveAt(0);
                    Files = temp.ToArray();
                    var temp1 = fils.ToList();
                    temp1.RemoveAt(0);
                    fils = temp1.ToArray();
                }
                var sb1 = this.FindResource("close_prog") as Storyboard;
                sb1.Begin();
                Cancel = false;
                dis_enable_buttons(true);
                file_list.AllowDrop = true;
                processing = false;
            }
            else { Main.message_box_show("ERROR", "INVALID DIRECTORY PATH !!!!\nPLEASE CHECK YOUR DIRECTORY.",(() => { }), (() => { }),(() => { })); }
        }

        public void dis_enable_buttons(bool true_false)
        {
            dir_browes.IsEnabled = true_false;
            cancel_prog.IsEnabled = true;
            if (true_false) { cancel_prog.IsEnabled = false; }
            int n = file_list.Items.Count;
            for (int i = 0; i < n; i++)
            {
                StackPanel panel = file_list.Items[i] as StackPanel;
                Grid grid = panel.Children[0] as Grid;
                Button copy_button = grid.Children[3] as Button;
                Button cut_button = grid.Children[4] as Button;
                copy_button.IsEnabled = true_false;
                cut_button.IsEnabled = true_false;
            }
        }

        private void cancel_prog_Click(object sender, RoutedEventArgs e)
        {
            waiting = true;
            Main.message_box_show("CANCEL", "DO YOU WANT TO CANCEL !!!\n(YOU CAN CONTAINUE ANOTHER TIME)",
                (() => { waiting = false; cancel_prog.IsEnabled = false; Cancel = true; }),
                (() => { }),
                (() => {  waiting = false; Cancel = false; cancel_prog.IsEnabled = true; }),
                1);
        }
    }
}
