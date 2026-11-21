using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows;
using CodingSeb.Localization;
using CodingSeb.Localization.Loaders;
using Lab01OOP.TranslateExt;
using Lab01OOP.ViewModels;

namespace Lab01OOP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            PreInitialize();
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private static void PreInitialize()
        {
            List<Assembly> assemblies = new List<Assembly>
            {
                typeof(MainWindow).Assembly
            };

            LocalizationLoader.Instance.FileLanguageLoaders
                .Add(new EmbeddedResourceFileLoader(assemblies));

            // ВАЖНО: имя файла должно совпадать с реальным
            LocalizationLoader.Instance.AddFile("Lab01OOP/Resources/translate.loc.json");

            JsonMissingTranslationsLogger.EnableLogFor(Loc.Instance);

            string locale = Thread.CurrentThread.CurrentUICulture.Parent.Name;
            if (Loc.Instance.AvailableLanguages.Contains(locale.ToLower()))
                Loc.Instance.CurrentLanguage = locale;
            else
                Loc.Instance.CurrentLanguage = "ru";
        }
    }
}