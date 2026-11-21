using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CodingSeb.Localization;
using CodingSeb.Localization.Loaders;


namespace Lab01OOP.TranslateExt
{
    public class EmbeddedResourceFileLoader : JsonFileLoader, ILocalizationFileLoader
    {
        private readonly List<Assembly> _assembly;

        public EmbeddedResourceFileLoader(List<Assembly> assembly) : base()
        {
            _assembly = assembly;
        }

        public new void LoadFile(string fileName, LocalizationLoader loader)
        {
            if (File.Exists(fileName))
            {
                LoadFromString(File.ReadAllText(fileName), loader, fileName);
            }
            else
            {
                foreach (Assembly assembly in _assembly)
                {
                    var resourceNames = assembly.GetManifestResourceNames();
                    var resource = fileName.Replace("/", ".");

                    foreach (var resourceName in resourceNames)
                    {
                        if (resourceName.EndsWith(resource))
                        {
                            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                            {
                                if (stream != null)
                                {
                                    using (StreamReader reader = new StreamReader(stream))
                                    {
                                        LoadFromString(reader.ReadToEnd(), loader, resourceName);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}