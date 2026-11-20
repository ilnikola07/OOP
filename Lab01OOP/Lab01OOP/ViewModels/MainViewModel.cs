using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace Lab01OOP.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // === Вкладка "Основное" (ЛР1 п.1.a + ЛР2 п.3, п.7) ===

        private string _selectedTheme = "Светлая";
        public string SelectedTheme
        {
            get => _selectedTheme;
            set { _selectedTheme = value; OnPropertyChanged(); }
        }

        private string _selectedLanguage = "Русский";
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set { _selectedLanguage = value; OnPropertyChanged(); }
        }

        private string _selectedFont = "Segoe UI";
        public string SelectedFont
        {
            get => _selectedFont;
            set { _selectedFont = value; OnPropertyChanged(); }
        }

        private double _fontSize = 14;
        public double FontSize
        {
            get => _fontSize;
            set { _fontSize = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> Themes { get; } =
            new ObservableCollection<string> { "Светлая", "Темная", "Системная" };

        public ObservableCollection<string> Languages { get; } =
            new ObservableCollection<string> { "Русский", "English" };

        public ObservableCollection<string> Fonts { get; } =
            new ObservableCollection<string>(FontsHelper.GetSystemFontNames());

        // === Вкладка "Цветовая палитра" (ЛР1 п.1.b.iii) ===

        public ObservableCollection<ColorShade> ShadesMain { get; } = ShadeGenerator.Generate("#F3F3F3", "Основной");
        public ObservableCollection<ColorShade> ShadesAccent { get; } = ShadeGenerator.Generate("#0078D7", "Акцентный");
        public ObservableCollection<ColorShade> ShadesSuccess { get; } = ShadeGenerator.Generate("#107C10", "Успех");
        public ObservableCollection<ColorShade> ShadesError { get; } = ShadeGenerator.Generate("#D13438", "Ошибка");
        public ObservableCollection<ColorShade> ShadesInfo { get; } = ShadeGenerator.Generate("#FF8C00", "Информация");

        // === Вкладка "Элементы управления" (ЛР1 п.1.c) ===

        public ObservableCollection<SampleItem> SampleItems { get; } = new ObservableCollection<SampleItem>
        {
            new SampleItem { Id = 1, Title = "Первый элемент", IsActive = true  },
            new SampleItem { Id = 2, Title = "Второй элемент", IsActive = false },
            new SampleItem { Id = 3, Title = "Третий элемент", IsActive = true  },
        };

        // === Вкладка "Работа с данными" (ЛР2 п.5, п.6) ===

        public ObservableCollection<ColorShade> SourceShades { get; } = new ObservableCollection<ColorShade>();
        public ObservableCollection<ColorShade> TargetShades { get; } = new ObservableCollection<ColorShade>();

        private ColorShade _selectedSourceShade;
        public ColorShade SelectedSourceShade
        {
            get => _selectedSourceShade;
            set
            {
                _selectedSourceShade = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private ColorShade _clipboardShade;

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand ClearTargetCommand { get; }

        public MainViewModel()
        {
            // Тестовые данные для исходной таблицы
            SourceShades.Add(new ColorShade { ColorBrush = Brushes.White, RgbCode = "#FFFFFF", Name = "Белый" });
            SourceShades.Add(new ColorShade { ColorBrush = new SolidColorBrush(Color.FromRgb(0, 120, 215)), RgbCode = "#0078D7", Name = "Синий" });
            SourceShades.Add(new ColorShade { ColorBrush = new SolidColorBrush(Color.FromRgb(16, 124, 16)), RgbCode = "#107C10", Name = "Зелёный" });
            SourceShades.Add(new ColorShade { ColorBrush = new SolidColorBrush(Color.FromRgb(209, 52, 56)), RgbCode = "#D13438", Name = "Красный" });

            AddCommand = new RelayCommand(_ =>
            {
                SourceShades.Add(new ColorShade
                {
                    ColorBrush = new SolidColorBrush(Color.FromRgb(
                        (byte)(new Random().Next(256)),
                        (byte)(new Random().Next(256)),
                        (byte)(new Random().Next(256)))),
                    RgbCode = "#RANDOM",
                    Name = $"Новый цвет #{SourceShades.Count + 1}"
                });
            });

            DeleteCommand = new RelayCommand(_ =>
            {
                if (SelectedSourceShade != null)
                    SourceShades.Remove(SelectedSourceShade);
            }, _ => SelectedSourceShade != null);

            CopyCommand = new RelayCommand(_ =>
            {
                _clipboardShade = SelectedSourceShade;
            }, _ => SelectedSourceShade != null);

            PasteCommand = new RelayCommand(_ =>
            {
                if (_clipboardShade != null)
                {
                    TargetShades.Add(new ColorShade
                    {
                        ColorBrush = _clipboardShade.ColorBrush,
                        RgbCode = _clipboardShade.RgbCode,
                        Name = _clipboardShade.Name + " (копия)"
                    });
                }
            }, _ => _clipboardShade != null);

            ClearTargetCommand = new RelayCommand(_ => TargetShades.Clear());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // === Вспомогательные классы ===

    public class ColorShade
    {
        public Brush ColorBrush { get; set; }
        public string RgbCode { get; set; }
        public string Name { get; set; }
    }

    public class SampleItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
    }

    public static class FontsHelper
    {
        public static System.Collections.Generic.IEnumerable<string> GetSystemFontNames()
            => System.Windows.Media.Fonts.SystemFontFamilies
                .Select(f => f.Source)
                .OrderBy(s => s);
    }

    public static class ShadeGenerator
    {
        // Генерирует 10 оттенков от светлого к тёмному для заданного базового цвета
        public static ObservableCollection<ColorShade> Generate(string hex, string baseName)
        {
            var result = new ObservableCollection<ColorShade>();
            var rgb = (Color)ColorConverter.ConvertFromString(hex);

            for (int i = 0; i < 10; i++)
            {
                double factor = 1.0 - (i * 0.2); // 1.0, 0.8, 0.6 ... 0.0

                // Совместимо с .NET Framework 4.7.2 (вместо Math.Clamp)
                byte r = (byte)Math.Max(0, Math.Min(255, (int)(rgb.R * factor)));
                byte g = (byte)Math.Max(0, Math.Min(255, (int)(rgb.G * factor)));
                byte b = (byte)Math.Max(0, Math.Min(255, (int)(rgb.B * factor)));

                var c = Color.FromRgb(r, g, b);

                result.Add(new ColorShade
                {
                    ColorBrush = new SolidColorBrush(c),
                    RgbCode = $"#{r:X2}{g:X2}{b:X2}",
                    Name = $"{baseName} ({i + 1})"
                });
            }
            return result;
        }
    }
}