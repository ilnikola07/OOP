using CodingSeb.Localization;
using CodingSeb.Localization.Loaders;
using Lab01OOP.TranslateExt;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace Lab01OOP
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Регистрируем ваш загрузчик встроенных ресурсов
            LocalizationLoader.Instance.FileLanguageLoaders.Add(
                new EmbeddedResourceFileLoader(new List<Assembly> { Assembly.GetExecutingAssembly() })
            );

            // Загружаем файл (укажите точное имя вашего файла, например "lang.json")
            LocalizationLoader.Instance.AddFile("translations.json");

            // Устанавливаем язык по умолчанию
            Loc.Instance.CurrentLanguage = "ru";
        }
    }
}
