using System.Windows;
using Lab01OOP.ViewModels;

namespace Lab01OOP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}