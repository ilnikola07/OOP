using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Lab01OOP.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private double _fontSize = 14;
        public double FontSize
        {
            get => _fontSize;
            set { _fontSize = value; OnPropertyChanged(); }
        }

        // Коллекция для DataGrid
        public ObservableCollection<ColorShade> Shades { get; set; } = new ObservableCollection<ColorShade>();

        // Команды
        public ICommand AddShadeCommand { get; }
        public ICommand DeleteShadeCommand { get; }

        public MainViewModel()
        {
            // Инициализация тестовыми данными
            Shades.Add(new ColorShade { RgbCode = "#FFFFFF", Name = "Белый" });

            AddShadeCommand = new RelayCommand(_ => Shades.Add(new ColorShade { RgbCode = "#000000", Name = "Новый цвет" }));
            DeleteShadeCommand = new RelayCommand(_ =>
            {
                // Удаление выбранного элемента (реализуется через привязку SelectedItem)
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ColorShade
    {
        public string RgbCode { get; set; }
        public string Name { get; set; }
    }
}