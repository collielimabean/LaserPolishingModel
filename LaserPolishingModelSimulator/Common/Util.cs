using System.Windows;


namespace LaserPolishingModelSimulator.Common
{
    static class Util
    {
        public static string GetLocalizedString(string key)
        {
            return Application.Current.Resources[key] as string;
        }
    }
}
