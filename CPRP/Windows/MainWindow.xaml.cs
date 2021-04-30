using CPRP.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

namespace CPRP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<PriorityItem> priorityList = new List<PriorityItem>(6);
        private readonly ObservableCollection<string> extensionList = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();
            // window
            Title = App.Name;
            // priority
            priorityList.Add(new PriorityItem("Реального времени", ProcessPriorityClass.RealTime, "-rt"));
            priorityList.Add(new PriorityItem("Высокий", ProcessPriorityClass.High, "-h"));
            priorityList.Add(new PriorityItem("Выше среднего", ProcessPriorityClass.AboveNormal, "-an"));
            PriorityItem priorityNormal = new PriorityItem("Обычный", ProcessPriorityClass.Normal, "-n");
            priorityList.Add(priorityNormal);
            priorityList.Add(new PriorityItem("Ниже среднего", ProcessPriorityClass.BelowNormal, "-bn"));
            priorityList.Add(new PriorityItem("Низкий", ProcessPriorityClass.Idle, "-i"));
            comboBox_Priority.ItemsSource = priorityList;
            comboBox_Priority.DisplayMemberPath = "Title";
            comboBox_Priority.SelectedItem = priorityNormal;
        }

        private void Button_SelectApp_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Исполняемые файлы|*.exe",
                DefaultExt = "exe"
            })
            {
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textBox_App.Foreground = Brushes.Black;
                    textBox_App.Text = ofd.FileName;
                }
            }
        }

        private void Button_SelectOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                RootFolder = Environment.SpecialFolder.MyDocuments
            })
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textBox_OutputFolder.Foreground = Brushes.Black;
                    textBox_OutputFolder.Text = fbd.SelectedPath;
                }
            }
        }

        private void RadioButton_CreateShortcutOnDesktop_Checked(object sender, RoutedEventArgs e)
        {
            if (extensionList.Count == 0)
            {
                textBox_OutputFolder.IsEnabled = false;
                button_SelectOutputFolder.IsEnabled = false;
                checkBox_IsReplaceDesktopShortcut.IsEnabled = true;
            }
        }

        private void RadioButton_CreateShortcutOnDesktop_Unchecked(object sender, RoutedEventArgs e)
        {
            textBox_OutputFolder.IsEnabled = true;
            button_SelectOutputFolder.IsEnabled = true;
            checkBox_IsReplaceDesktopShortcut.IsEnabled = false;
        }

        private void Button_ClearExtensionList_Click(object sender, RoutedEventArgs e)
        {
            if (extensionList.Count != 0)
            {
                if (MessageBox.Show("Очистить список?", Title, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    extensionList.Clear();
                    if (radioButton_CreateShortcutOnDesktop.IsChecked.Value)
                    {
                        RadioButton_CreateShortcutOnDesktop_Checked(null, null);
                    }
                }
            }
        }

        private void Button_RemoveSelectedExtensions_Click(object sender, RoutedEventArgs e)
        {
            IList<object> selectedItems = (IList<object>) listBox_Extensions.SelectedItems;
            if (selectedItems.Count != 0)
            {
                if (MessageBox.Show("Удалить выбранные расширения из списка?", Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (object item in selectedItems)
                    {
                        extensionList.Remove(item.ToString());
                    }
                    if (radioButton_CreateShortcutOnDesktop.IsChecked.Value)
                    {
                        RadioButton_CreateShortcutOnDesktop_Checked(null, null);
                    }
                }
            }
        }
    }
}
