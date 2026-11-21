using CodingSeb.Localization;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections;
using System.Collections.Generic;

namespace Lab01OOP.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Вкладка "Основное" 

        public ObservableCollection<ThemeOption> Themes { get; } =
            new ObservableCollection<ThemeOption>
            {
                new ThemeOption("Theme.Light"),
                new ThemeOption("Theme.Dark"),
                new ThemeOption("Theme.System")
            };

        private ThemeOption _selectedTheme;
        public ThemeOption SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                OnPropertyChanged();
                if (value != null)
                    ApplyTheme(value.Key);
            }
        }

        private void ApplyTheme(string themeKey)
        {
            var res = System.Windows.Application.Current.Resources;

            if (themeKey == "Theme.Dark")
            {
                res["BackgroundBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                res["SurfaceBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D30"));
                res["BorderBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3F3F46"));
                res["TextPrimaryBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F1F1F1"));
                res["TextSecondaryBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A0A0A0"));
            }
            else // "Theme.Light" (или "Theme.System")
            {
                res["BackgroundBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F3F3"));
                res["SurfaceBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                res["BorderBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0"));
                res["TextPrimaryBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#202020"));
                res["TextSecondaryBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#606060"));
            }
        }

        private string _selectedLanguage = "Русский";
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                OnPropertyChanged();
                Loc.Instance.CurrentLanguage = (value == "English") ? "en" : "ru";
            }
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

        public ObservableCollection<string> Languages { get; } =
            new ObservableCollection<string> { "Русский", "English" };

        public ObservableCollection<string> Fonts { get; } =
            new ObservableCollection<string>(FontsHelper.GetSystemFontNames());

        // Вкладка "Цветовая палитра" 

        public ObservableCollection<ColorShade> ShadesMain { get; } = ShadeGenerator.Generate("#F3F3F3", "Palette.Main");
        public ObservableCollection<ColorShade> ShadesAccent { get; } = ShadeGenerator.Generate("#0078D7", "Palette.Accent");
        public ObservableCollection<ColorShade> ShadesSuccess { get; } = ShadeGenerator.Generate("#107C10", "Palette.Success");
        public ObservableCollection<ColorShade> ShadesError { get; } = ShadeGenerator.Generate("#D13438", "Palette.Error");
        public ObservableCollection<ColorShade> ShadesInfo { get; } = ShadeGenerator.Generate("#FF8C00", "Palette.Info");

        // Вкладка "Элементы управления" 

        public ObservableCollection<SampleItem> SampleItems { get; } = new ObservableCollection<SampleItem>
        {
            new SampleItem { Id = 1, TitleKey = "Sample.Item1", IsActive = true  },
            new SampleItem { Id = 2, TitleKey = "Sample.Item2", IsActive = false },
            new SampleItem { Id = 3, TitleKey = "Sample.Item3", IsActive = true  },
        };


        // Вкладка "Работа с данными" 

        public ObservableCollection<ColorShade> SourceShades { get; } = new ObservableCollection<ColorShade>();
        public ObservableCollection<ColorShade> TargetShades { get; } = new ObservableCollection<ColorShade>();

        private List<ColorShade> _clipboardShades = new List<ColorShade>();

        public ICommand AddCommand { get; }
        public ICommand DeleteSourceCommand { get; }
        public ICommand DeleteTargetCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand ClearTargetCommand { get; }

        public MainViewModel()
        {
            // Устанавливаем Светлую тему по умолчанию при старте
            _selectedTheme = Themes[0];

            // Тестовые данные для исходной таблицы
            SourceShades.Add(new ColorShade { ColorBrush = Brushes.White, RgbCode = "#FFFFFF", NameKey = "Color.White" });
            SourceShades.Add(new ColorShade { ColorBrush = new SolidColorBrush(Color.FromRgb(0, 120, 215)), RgbCode = "#0078D7", NameKey = "Color.Blue" });
            SourceShades.Add(new ColorShade { ColorBrush = new SolidColorBrush(Color.FromRgb(16, 124, 16)), RgbCode = "#107C10", NameKey = "Color.Green" });
            SourceShades.Add(new ColorShade { ColorBrush = new SolidColorBrush(Color.FromRgb(209, 52, 56)), RgbCode = "#D13438", NameKey = "Color.Red" });

            AddCommand = new RelayCommand(_ =>
            {
                SourceShades.Add(new ColorShade
                {
                    ColorBrush = new SolidColorBrush(Color.FromRgb(
                        (byte)(new Random().Next(256)),
                        (byte)(new Random().Next(256)),
                        (byte)(new Random().Next(256)))),
                    RgbCode = "#RANDOM",
                    NameKey = "Color.New",
                    IsNewColor = true,
                    Index = SourceShades.Count + 1
                });
            });


            DeleteSourceCommand = new RelayCommand(param =>
            {
                if (param is IList selectedItems && selectedItems.Count > 0)
                {
                    foreach (var item in selectedItems.Cast<ColorShade>().ToList())
                    {
                        SourceShades.Remove(item);
                    }
                }
            }, param => param is IList list && list.Count > 0);

            DeleteTargetCommand = new RelayCommand(param =>
            {
                if (param is IList selectedItems && selectedItems.Count > 0)
                {
                    foreach (var item in selectedItems.Cast<ColorShade>().ToList())
                    {
                        TargetShades.Remove(item);
                    }
                }
            }, param => param is IList list && list.Count > 0);

            CopyCommand = new RelayCommand(param =>
            {
                if (param is IList selectedItems && selectedItems.Count > 0)
                {
                    _clipboardShades = selectedItems.Cast<ColorShade>().ToList();
                    CommandManager.InvalidateRequerySuggested();
                }
            }, param => param is IList list && list.Count > 0);

            PasteCommand = new RelayCommand(_ =>
            {
                foreach (var item in _clipboardShades)
                {
                    TargetShades.Add(new ColorShade
                    {
                        ColorBrush = item.ColorBrush,
                        RgbCode = item.RgbCode,
                        NameKey = item.NameKey,
                        Index = item.Index,
                        IsNewColor = item.IsNewColor,
                        IsCopy = true
                    });
                }
            }, _ => _clipboardShades != null && _clipboardShades.Count > 0);


            ClearTargetCommand = new RelayCommand(_ => TargetShades.Clear());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


    public class ThemeOption : INotifyPropertyChanged
    {
        public string Key { get; }
        public string Title => Loc.Instance.Translate(Key);

        public ThemeOption(string key)
        {
            Key = key;
            Loc.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Loc.CurrentLanguage) || string.IsNullOrEmpty(e.PropertyName))
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ColorShade : INotifyPropertyChanged
    {
        public Brush ColorBrush { get; set; }
        public string RgbCode { get; set; }

        public string NameKey { get; set; }
        public int Index { get; set; }
        public bool IsNewColor { get; set; }
        public bool IsCopy { get; set; }
        private string _customName;

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(NameKey))
                    return _customName;

                string baseName = Loc.Instance.Translate(NameKey);

                if (Index > 0)
                    baseName += IsNewColor ? $" #{Index}" : $" ({Index})";

                if (IsCopy)
                    baseName += $" {Loc.Instance.Translate("Color.Copy")}";

                return baseName;
            }
            set => _customName = value;
        }

        public ColorShade()
        {
            Loc.Instance.PropertyChanged += (s, e) =>
            {
                if (!string.IsNullOrEmpty(NameKey) &&
                    (e.PropertyName == nameof(Loc.CurrentLanguage) || string.IsNullOrEmpty(e.PropertyName)))
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }


    public static class ShadeGenerator
    {
        public static ObservableCollection<ColorShade> Generate(string hex, string baseNameKey)
        {
            var result = new ObservableCollection<ColorShade>();
            var rgb = (Color)ColorConverter.ConvertFromString(hex);

            for (int i = 0; i < 10; i++)
            {
                double factor = 1.0 - (i * 0.2);

                byte r = (byte)Math.Max(0, Math.Min(255, (int)(rgb.R * factor)));
                byte g = (byte)Math.Max(0, Math.Min(255, (int)(rgb.G * factor)));
                byte b = (byte)Math.Max(0, Math.Min(255, (int)(rgb.B * factor)));

                var c = Color.FromRgb(r, g, b);

                result.Add(new ColorShade
                {
                    ColorBrush = new SolidColorBrush(c),
                    RgbCode = $"#{r:X2}{g:X2}{b:X2}",
                    NameKey = baseNameKey,
                    Index = i + 1
                });
            }
            return result;
        }
    }
    public class SampleItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string TitleKey { get; set; }
        public bool IsActive { get; set; }

        public string Title => !string.IsNullOrEmpty(TitleKey)
            ? Loc.Instance.Translate(TitleKey)
            : string.Empty;

        public SampleItem()
        {
            Loc.Instance.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Loc.CurrentLanguage) || string.IsNullOrEmpty(e.PropertyName))
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public static class FontsHelper
    {
        public static System.Collections.Generic.IEnumerable<string> GetSystemFontNames()
            => System.Windows.Media.Fonts.SystemFontFamilies
                .Select(f => f.Source)
                .OrderBy(s => s);
    }


}