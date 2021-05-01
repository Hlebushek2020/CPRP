using CPRP.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
        private const string OutputFolderPlaceholder = "Папка выходных файлов";

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
            // TextBox OutputFolder
            textBox_OutputFolder.Text = OutputFolderPlaceholder;
            // extension list
            listBox_Extensions.ItemsSource = extensionList;
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
            List<string> selectedItems = new List<string>(listBox_Extensions.SelectedItems.Cast<string>());
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

        private void Button_AddExtension_Click(object sender, RoutedEventArgs e)
        {
            AddExtensionWindow addExtension = new AddExtensionWindow();
            addExtension.ShowDialog();
            if (!string.IsNullOrEmpty(addExtension.Extension))
            {
                if (!extensionList.Contains(addExtension.Extension))
                    extensionList.Add(addExtension.Extension);
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e) => Close();

        private void Button_StartProcess_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(textBox_App.Text))
            {
                MessageBox.Show("Выберите приложение!", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!radioButton_CreateShortcutOnDesktop.IsChecked.Value || extensionList.Count != 0)
            {
                if (textBox_OutputFolder.Text.Equals(OutputFolderPlaceholder))
                {
                    MessageBox.Show("Выберите папку для выходных файлов!", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            ProcessArgs processArgs = new ProcessArgs(
                textBox_App.Text,
                (PriorityItem)comboBox_Priority.SelectedItem,
                textBox_OutputFolder.Text,
                radioButton_CreateShortcutOnDesktop.IsChecked.Value,
                checkBox_IsReplaceDesktopShortcut.IsChecked.Value,
                extensionList);
            //
            // CODE
            //
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Закрыть программу?", Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                e.Cancel = true;
        }
    }
}
