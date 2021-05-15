using CPRP.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
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
                ShowNewFolderButton = true
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

        #region Process
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
            Task.Run(() => Process(processArgs));
        }

        private void Process(ProcessArgs args)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    grid_Progress.Visibility = Visibility.Visible;
                    progressBar_Progress.IsIndeterminate = false;
                    textBlock_Progress.Text = "Создание ярлыка";
                });
                // shortcut
                string shortcutName = $"{Path.GetFileNameWithoutExtension(args.App)}.lnk";
                string shortcutPath = string.Empty;
                if (args.CreateShortcutOnDesktop)
                {
                    string shortcutPathTmp = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    shortcutPath = Path.Combine(shortcutPathTmp, shortcutName);
                    if (!args.IsReplaceDesktopShortcut)
                    {
                        int index = 0;
                        while (File.Exists(shortcutPath))
                        {
                            shortcutPath = Path.Combine(shortcutPathTmp, $"{index}-{shortcutName}");
                            index++;
                        }
                    }
                }
                else
                {
                    shortcutPath = Path.Combine(args.OutputFolder, shortcutName);
                }
                IWshRuntimeLibrary.WshShell wsh = new IWshRuntimeLibrary.WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)wsh.CreateShortcut(shortcutPath);
                shortcut.TargetPath = App.LocationRunner;
                shortcut.IconLocation = args.App;
                shortcut.Arguments = $"\"{args.App}\" {args.PriorityArg}";
                shortcut.WorkingDirectory = Path.GetDirectoryName(args.App);
                shortcut.Save();
                // registry
                string defaultProgram = string.Empty;
                string defaultIcon = args.App;
                RegistryKey keyClassesRoot = null;
                RegistryKey keyCurrentUserSoftware = null;
                RegistryKey keyLocalMachineClasses = null;
                RegistryRecovery registryRecovery = new RegistryRecovery();
                try
                {
                    Dispatcher.Invoke((Action<int>)((int max) =>
                    {
                        progressBar_Progress.IsIndeterminate = true;
                        progressBar_Progress.Value = 0;
                        progressBar_Progress.Maximum = max;
                        textBlock_Progress.Text = "Чтение параметров реестра";
                    }), args.ExtensionList.Count);
                    keyClassesRoot = Registry.ClassesRoot;
                    keyCurrentUserSoftware = Registry.CurrentUser.OpenSubKey("Software");
                    keyLocalMachineClasses = Registry.LocalMachine.OpenSubKey($@"Software\Classes");
                    foreach (string extension in args.ExtensionList)
                    {
                        RegistryRecoveryInfo registryRecoveryInfo = new RegistryRecoveryInfo();
                        // get default program and icon from HKEY_CURRENT_USER
                        Dispatcher.Invoke((Action<string>)((string argExtension) =>
                        {
                            textBlock_Progress.Text = $"Чтение параметров реестра для расширения {extension}";
                        }), extension);
                        using (RegistryKey keyCurrentUserClasses = keyCurrentUserSoftware.OpenSubKey("Classes"))
                        {
                            using (RegistryKey keyClass = keyCurrentUserClasses.OpenSubKey($".{extension}"))
                            {
                                if (keyClass != null)
                                {
                                    object defaultProgramObj = keyClass.GetValue(string.Empty);
                                    if (defaultProgramObj != null)
                                    {
                                        defaultProgram = (string)defaultProgramObj;
                                        registryRecoveryInfo.DefaultUser = defaultProgram;
                                        using (RegistryKey keyClassProg = keyCurrentUserClasses.OpenSubKey($@"{defaultProgram}\DefaultIcon"))
                                        {
                                            if (keyClassProg != null)
                                            {
                                                object defaultIconObj = keyClassProg.GetValue(string.Empty);
                                                if (defaultIconObj != null)
                                                {
                                                    defaultIcon = (string)defaultIconObj;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        // get default program and icon from HKEY_LOCAL_MACHINE
                        using (RegistryKey keyClass = keyLocalMachineClasses.OpenSubKey($".{extension}"))
                        {
                            if (keyClass != null)
                            {
                                object defaultProgramObj = keyClass.GetValue(string.Empty);
                                if (defaultProgramObj != null)
                                {
                                    if (string.IsNullOrEmpty(defaultProgram))
                                    {
                                        defaultProgram = (string)defaultProgramObj;
                                    }
                                    registryRecoveryInfo.DefaultMachine = (string)defaultProgramObj;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(defaultProgram) && defaultIcon.Equals(args.App))
                        {
                            using (RegistryKey keyClassProg = keyLocalMachineClasses.OpenSubKey($@"{defaultProgram}\DefaultIcon"))
                            {
                                if (keyClassProg != null)
                                {
                                    object defaultIconObj = keyClassProg.GetValue(string.Empty);
                                    if (defaultIconObj != null)
                                    {
                                        defaultIcon = (string)defaultIconObj;
                                    }
                                }
                            }
                        }
                        // read Explorer\FileExts\.{extension}\UserChoice
                        using (RegistryKey keyFileExts = keyCurrentUserSoftware.OpenSubKey($@"Microsoft\Windows\CurrentVersion\Explorer\FileExts\.{extension}"))
                        {
                            if (keyFileExts != null)
                            {
                                using (RegistryKey keyUserChoice = keyFileExts.OpenSubKey(RegistryRecoveryInfo.KeyUserChoice))
                                {
                                    if (keyUserChoice != null)
                                    {
                                        object valueObj = keyUserChoice.GetValue(RegistrySectionUserChoice.KeyHash);
                                        if (valueObj != null)
                                            registryRecoveryInfo.UserChoice.Hash = (string)valueObj;
                                        valueObj = keyUserChoice.GetValue(RegistrySectionUserChoice.KeyProgId);
                                        if (valueObj != null)
                                            registryRecoveryInfo.UserChoice.ProgId = (string)valueObj;
                                    }
                                }
                            }
                        }
                        // add recovery data
                        registryRecovery.DefaultValues.Add(extension, registryRecoveryInfo);
                        // create runner class for extension
                        Dispatcher.Invoke((Action<string>)((string argExtension) =>
                        {
                            textBlock_Progress.Text = $"Установка новых значений параметров реестра для расширения {extension}";
                        }), extension);
                        string runnerClassName = $"CPRP_{extension}";
                        using (RegistryKey keyRunnerClass = keyClassesRoot.CreateSubKey(runnerClassName, true))
                        {
                            using (RegistryKey keyDefaultIcon = keyRunnerClass.CreateSubKey("DefaultIcon"))
                            {
                                keyDefaultIcon.SetValue(string.Empty, defaultIcon);
                            }
                            using (RegistryKey keyOpenCommand = keyRunnerClass.CreateSubKey(@"Shell\Open\Command", true))
                            {
                                keyOpenCommand.SetValue(string.Empty, $"\"{App.LocationRunner}\" \"{args.App}\" \"%1\" {args.PriorityArg}");
                            }
                            using (RegistryKey keyApplication = keyRunnerClass.CreateSubKey("Application"))
                            {
                                keyApplication.SetValue("ApplicationName", $"CPRP for {extension} extension");
                                keyApplication.SetValue("ApplicationIcon", defaultIcon);
                            }
                        }
                        // delete selected user program and add runner class in program list
                        using (RegistryKey keyFileExts = keyCurrentUserSoftware.OpenSubKey($@"Microsoft\Windows\CurrentVersion\Explorer\FileExts\.{extension}", true))
                        {
                            if (keyFileExts != null)
                            {
                                keyFileExts.DeleteSubKey(RegistryRecoveryInfo.KeyUserChoice, false);
                                using (RegistryKey keyOpenWithProgids = keyFileExts.CreateSubKey(RegistryRecoveryInfo.KeyOpenWithProgids))
                                {
                                    keyOpenWithProgids.SetValue(runnerClassName, new byte[] { }, RegistryValueKind.None);
                                }
                            }
                        }
                        // set runner class
                        using (RegistryKey keyCurrentClass = keyClassesRoot.OpenSubKey($".{extension}", true))
                        {
                            keyCurrentClass.SetValue(string.Empty, runnerClassName);
                        }
                        Dispatcher.Invoke(() =>
                        {
                            progressBar_Progress.Value++;
                        });
                    }
                    Dispatcher.Invoke(() =>
                    {
                        progressBar_Progress.IsIndeterminate = true;
                        textBlock_Progress.Text = "Завершение";
                    });
                }
                finally
                {
                    // save recovery data
                    registryRecovery.Save(args.OutputFolder);
                    // dispose registry head key
                    if (keyLocalMachineClasses != null)
                        keyLocalMachineClasses.Dispose();
                    if (keyCurrentUserSoftware != null)
                        keyCurrentUserSoftware.Dispose();
                    if (keyClassesRoot != null)
                        keyClassesRoot.Dispose();
                }
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Готово!", Title, MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke((Action<string>)((string exMessage) =>
                {
                    MessageBox.Show(exMessage, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }), ex.Message);
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    progressBar_Progress.IsIndeterminate = true;
                    grid_Progress.Visibility = Visibility.Hidden;
                });
            }
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Закрыть программу?", Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                e.Cancel = true;
        }
    }
}
