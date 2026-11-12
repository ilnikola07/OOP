using BusPark.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;                  
using System.Windows;            
namespace BusParkApp.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<Bus> AllBuses { get; set; } = new ObservableCollection<Bus>();
        public ObservableCollection<Bus> BusesInPark { get; set; } = new ObservableCollection<Bus>();
        public ObservableCollection<Bus> BusesOnRoute { get; set; } = new ObservableCollection<Bus>();
        private readonly string _fileName = "buses.txt";

        public MainViewModel()
        {
            LoadDataFromFile();
        }

        /// <summary>
        /// Метод чтения данных из текстового файла
        /// </summary>
        public void LoadDataFromFile()
        {
            try
            {
                if (!File.Exists(_fileName))
                    return;

                string[] lines = File.ReadAllLines(_fileName);
                System.Collections.Generic.List<BusPark.Models.Bus> temporaryList = new System.Collections.Generic.List<BusPark.Models.Bus>();

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(';');
                    
                    if (parts.Length != 3) 
                    {
                        MessageBox.Show($"Критическая ошибка в файле! Строка не разделена на 3 части: '{line}'. База данных не загружена.", "Ошибка файла", MessageBoxButton.OK, MessageBoxImage.Error);
                        return; 
                    }

                    string busNumber = parts[0].Trim().ToUpper();
                    string driverName = parts[1].Trim();
                    string routeNumber = parts[2].Trim();
                                        
                    bool isNumberValid = busNumber.Length == 6 // Проверка формата номера
                                         && char.IsLetter(busNumber[0])
                                         && char.IsDigit(busNumber[1]) && char.IsDigit(busNumber[2]) && char.IsDigit(busNumber[3])
                                         && char.IsLetter(busNumber[4]) && char.IsLetter(busNumber[5]);

                    if (!isNumberValid)
                    {
                        MessageBox.Show($"Критическая ошибка в файле! Неверный формат номера машины: '{busNumber}'. База данных не загружена.", "Ошибка файла", MessageBoxButton.OK, MessageBoxImage.Error);
                        return; 
                    }
                    if (!int.TryParse(routeNumber, out _))
                    {
                        MessageBox.Show($"Критическая ошибка в файле! Маршрут '{routeNumber}' должен содержать только цифры. База данных не загружена.", "Ошибка файла", MessageBoxButton.OK, MessageBoxImage.Error);
                        return; 
                    }
                    bool isDuplicate = false;
                    foreach (var tempBus in temporaryList)
                    {
                        if (tempBus.BusNumber == busNumber)
                        {
                            isDuplicate = true;
                            break;
                        }
                    }

                    if (isDuplicate)
                    {
                        throw new ArgumentException("");

                        MessageBox.Show($"Критическая ошибка в файле! Обнаружен дубликат номера машины: '{busNumber}'. База данных не загружена.", "Ошибка файла", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    BusPark.Models.Bus bus = new BusPark.Models.Bus(busNumber, driverName, routeNumber);
                    temporaryList.Add(bus);
                }

                // ЕСЛИ ЦИКЛ СУМЕЛ ЗАВЕРШИТЬСЯ ЗНАЧИТ ФАЙЛ ИДЕАЛЕН
                AllBuses.Clear();
                BusesInPark.Clear();
                BusesOnRoute.Clear();

                foreach (var approvedBus in temporaryList)
                {
                    AllBuses.Add(approvedBus);
                    BusesInPark.Add(approvedBus);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось прочитать файл данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        /// <summary>
        /// Метод добавления нового автобуса с клавиатуры
        /// </summary>
        public void AddBus(string number, string driver, string route)
        {
            string searchNumber = number.Trim().ToUpper();

            foreach (var existingBus in AllBuses)
            {
                if (existingBus.BusNumber.ToUpper() == searchNumber)
                {
                    MessageBox.Show($"Автобус с номером '{searchNumber}' уже зарегистрирован в системе!", "Дубликат данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; // Прерываем добавление
                }
            }

            Bus newBus = new Bus(number, driver, route)

            AllBuses.Add(newBus);
            BusesInPark.Add(newBus);
        }

        /// <summary>
        /// Метод удаления автобуса из системы
        /// </summary>
        public void DeleteBus(Bus bus)
        {
            if (bus != null)
            {
                if (BusesOnRoute.Contains(bus))
                {
                    MessageBox.Show("Нельзя удалить автобус, пока он находится на маршруте! Сначала верните его в парк.",
                                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                AllBuses.Remove(bus);
                BusesInPark.Remove(bus);
            }
        }

        /// <summary>
        /// Имитация выезда автобуса из парка на маршрут
        /// </summary>
        public void DepartBus(string busNumber)
        {            
            string searchNumber = busNumber.Trim().ToUpper(); // Ищется автобус в парке 
                        
            for (int i = 0; i < BusesInPark.Count; i++) // Ищется объект автобуса в коллекции BusesInPark
            {
                if (BusesInPark[i].BusNumber.ToUpper() == searchNumber)
                {
                    Bus bus = BusesInPark[i];
                    BusesInPark.RemoveAt(i); // Удалить из парка
                    BusesOnRoute.Add(bus);// Записасть на маршрут
                    return; 
                }
            }

            MessageBox.Show($"Автобус с номером '{searchNumber}' не найден в парке!", "Ошибка выезда", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Имитация въезда автобуса с маршрута в парк
        /// </summary>
        public void ArriveBus(string busNumber)
        {            
            string searchNumber = busNumber.Trim().ToUpper(); // Ищется автобус на маршруте 
            
            for (int i = 0; i < BusesOnRoute.Count; i++) // Ищется объект автобуса в коллекции BusesOnRoute
            {
                if (BusesOnRoute[i].BusNumber.ToUpper() == searchNumber)
                {
                    Bus bus = BusesOnRoute[i];
                    BusesOnRoute.RemoveAt(i); 
                    BusesInPark.Add(bus);
                    return;
                }
            }
            MessageBox.Show($"Автобус с номером '{searchNumber}' сейчас не находится на маршруте!", "Ошибка въезда", MessageBoxButton.OK, MessageBoxImage.Warning);
        }


    }
}
