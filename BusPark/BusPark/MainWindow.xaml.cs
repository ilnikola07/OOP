using BusPark.Models;
using BusParkApp.ViewModels; 
using System.Windows;

namespace BusPark
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            this.DataContext = _viewModel;

            // События для вкладки 1 
            AddBusButton.Click += AddBusButton_Click;
            DeleteBusButton.Click += DeleteBusButton_Click;

            // Событи для кнопок вкладки 2
            DepartButton.Click += DepartButton_Click;
            ArriveButton.Click += ArriveButton_Click;

            AvailableNumbersList.SelectionChanged += AvailableNumbersList_SelectionChanged;
        }

        private void AvailableNumbersList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (AvailableNumbersList.SelectedItem is BusPark.Models.Bus selectedBus)
            {
                BusNumberInput.Text = selectedBus.BusNumber;
            }
        }
        private void DepartButton_Click(object sender, RoutedEventArgs e)
        {
            string number = BusNumberInput.Text.Trim();

            if (string.IsNullOrEmpty(number))
            {
                MessageBox.Show("Пожалуйста, введите номер автобуса для выезда!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            _viewModel.DepartBus(number);

            BusNumberInput.Clear();
        }
        private void ArriveButton_Click(object sender, RoutedEventArgs e)
        {
            string number = BusNumberInput.Text.Trim();

            if (string.IsNullOrEmpty(number))
            {
                MessageBox.Show("Пожалуйста, введите номер автобуса для въезда!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            _viewModel.ArriveBus(number);

            BusNumberInput.Clear();
        }
        private void AddBusButton_Click(object sender, RoutedEventArgs e)
        {
            string number = NewBusNumber.Text.Trim().ToUpper();
            string driver = NewDriverName.Text.Trim();
            string route = NewRouteNumber.Text.Trim();

            if (string.IsNullOrEmpty(number) || string.IsNullOrEmpty(driver) || string.IsNullOrEmpty(route))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isNumberValid = number.Length == 6
                                 && char.IsLetter(number[0])
                                 && char.IsDigit(number[1]) && char.IsDigit(number[2]) && char.IsDigit(number[3])
                                 && char.IsLetter(number[4]) && char.IsLetter(number[5]);

            if (!isNumberValid)
            {
                MessageBox.Show("Неверный формат номера! Используйте формат А123АА (например, В456ВВ).", "Ошибка формата", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(route, out _))
            {
                MessageBox.Show("Номер маршрута должен состоять только из цифр!", "Ошибка формата", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _viewModel.AddBus(number, driver, route);

            NewBusNumber.Clear();
            NewDriverName.Clear();
            NewRouteNumber.Clear();
        }


        private void DeleteBusButton_Click(object sender, RoutedEventArgs e)
        {            
            Bus selectedBus = DataGridAllBuses.SelectedItem as Bus; // Какая строка выделена пользователем в таблице
            if (selectedBus != null)
            {
                _viewModel.DeleteBus(selectedBus);
            }
            else
            {
                MessageBox.Show("Выберите автобус в таблице для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}
