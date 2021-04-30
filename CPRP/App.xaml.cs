using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace CPRP
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string Name { get; } = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyTitleAttribute>().Title;
    }
}
