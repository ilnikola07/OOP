using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;

namespace Lab01OOP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 1. Заполняем ComboBox системными шрифтами
            FillFontComboBox();

            // 2. Заполняем таблицы оттенков для каждого цвета
            GenerateShades();

            // 3. Заполняем DataGrid на вкладке "Элементы управления" тестовыми данными
            FillSampleDataGrid();
        }

        // ====================================================================
        // Заполнение ComboBox шрифтами
        // ====================================================================
        private void FillFontComboBox()
        {
            foreach (var fontFamily in Fonts.SystemFontFamilies.OrderBy(f => f.Source))
            {
                FontFamilyCombo.Items.Add(fontFamily);
            }
            // По умолчанию выбираем Segoe UI
            var defaultFont = Fonts.SystemFontFamilies.FirstOrDefault(f => f.Source == "Segoe UI");
            FontFamilyCombo.SelectedItem = defaultFont ?? Fonts.SystemFontFamilies.FirstOrDefault();
        }

        // ====================================================================
        // Генерация 10 оттенков для каждого цвета палитры
        // ====================================================================
        private void GenerateShades()
        {
            // Основной — серый (#F3F3F3)
            ShadesMainDataGrid.ItemsSource = GenerateColorShades(243, 243, 243, "Основной");

            // Акцентный — синий (#0078D7)
            ShadesAccentDataGrid.ItemsSource = GenerateColorShades(0, 120, 215, "Акцентный");

            // Успех — зелёный (#107C10)
            ShadesSuccessDataGrid.ItemsSource = GenerateColorShades(16, 124, 16, "Успех");

            // Ошибка — красный (#D13438)
            ShadesErrorDataGrid.ItemsSource = GenerateColorShades(209, 52, 56, "Ошибка");

            // Информация — оранжевый (#FF8C00)
            ShadesInfoDataGrid.ItemsSource = GenerateColorShades(255, 140, 0, "Информация");
        }

        /// <summary>
        /// Генерирует 10 оттенков базового цвета: 5 светлых → базовый → 4 тёмных.
        /// Линейная интерполяция между белым и цветом, затем между цветом и чёрным.
        /// </summary>
        private List<ColorShadeItem> GenerateColorShades(int r, int g, int b, string baseName)
        {
            var shades = new List<ColorShadeItem>();
            var names = new[] { "100", "200", "300", "400", "500", "600", "700", "800", "900", "1000" };

            for (int i = 0; i < 10; i++)
            {
                int newR, newG, newB;

                if (i < 5)
                {
                    // От белого к базовому цвету: t = 1 - (i / 5.0)
                    double t = 1.0 - (i / 5.0);
                    newR = (int)(255 * t + r * (1 - t));
                    newG = (int)(255 * t + g * (1 - t));
                    newB = (int)(255 * t + b * (1 - t));
                }
                else
                {
                    // От базового к чёрному: t = (i - 5) / 5.0
                    double t = (i - 5.0) / 5.0;
                    newR = (int)(r * (1 - t));
                    newG = (int)(g * (1 - t));
                    newB = (int)(b * (1 - t));
                }

                // Корректируем базовый оттенок (i == 5 → должен быть точный цвет)
                if (i == 5)
                {
                    newR = r;
                    newG = g;
                    newB = b;
                }


                newR = Math.Min(Math.Max(newR, 0), 255);
                newG = Math.Min(Math.Max(newG, 0), 255);
                newB = Math.Min(Math.Max(newB, 0), 255);

                var brush = new SolidColorBrush(Color.FromRgb((byte)newR, (byte)newG, (byte)newB));
                string rgbCode = $"#{newR:X2}{newG:X2}{newB:X2}";
                string name = $"{baseName} {names[i]}";

                shades.Add(new ColorShadeItem { ColorBrush = brush, RgbCode = rgbCode, Name = name });
            }

            return shades;
        }

        // ====================================================================
        // Тестовые данные для DataGrid на вкладке "Элементы управления"
        // ====================================================================
        private void FillSampleDataGrid()
        {
            var data = new List<SampleDataItem>
            {
                new SampleDataItem { Id = 1, Title = "Первая запись", IsActive = true },
                new SampleDataItem { Id = 2, Title = "Вторая запись", IsActive = false },
                new SampleDataItem { Id = 3, Title = "Третья запись", IsActive = true },
                new SampleDataItem { Id = 4, Title = "Четвёртая запись", IsActive = false },
                new SampleDataItem { Id = 5, Title = "Пятая запись", IsActive = true },
            };

            SampleDataGrid.ItemsSource = data;
        }
    }

    // ========================================================================
    // Модель данных для таблиц оттенков
    // ========================================================================
    public class ColorShadeItem
    {
        public SolidColorBrush ColorBrush { get; set; }
        public string RgbCode { get; set; }
        public string Name { get; set; }
    }

    // ========================================================================
    // Модель данных для DataGrid на вкладке "Элементы управления"
    // ========================================================================
    public class SampleDataItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
    }
}