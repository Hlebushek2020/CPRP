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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CPRP
{
    /// <summary>
    /// Логика взаимодействия для AddExtensionWindow.xaml
    /// </summary>
    public partial class AddExtensionWindow : Window
    {
        public string Extension { get; private set; } = null;

        private const string ExtensionPlaceholder = "Расширение (Пример: jpg)";

        public AddExtensionWindow()
        {
            InitializeComponent();
            // window
            Title = App.Name;
            // TextBox Extension
            TextBox_Extension_LostFocus(null, null);
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e) => Close();

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (ExtensionPlaceholder.Equals(textBox_Extension.Text))
            {
                MessageBox.Show("Поле \"Расширение\" не может быть пустым!", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Extension = textBox_Extension.Text;
            Close();
        }

        private void TextBox_Extension_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBox_Extension.Text.Equals(ExtensionPlaceholder))
            {
                textBox_Extension.Foreground = Brushes.Black;
                textBox_Extension.Text = string.Empty;
            }
        }

        private void TextBox_Extension_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_Extension.Text))
            {
                textBox_Extension.Foreground = Brushes.Gray;
                textBox_Extension.Text = ExtensionPlaceholder;
            }
        }
    }
}
