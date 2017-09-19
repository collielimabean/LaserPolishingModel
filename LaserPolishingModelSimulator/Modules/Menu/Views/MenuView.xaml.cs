using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace LaserPolishingModelSimulator.Modules.Menu.Views
{
    /// <summary>
    /// Interaction logic for MenuView.xaml
    /// </summary>
    public partial class MenuView : UserControl
    {
        const string DEFAULT_CULTURE_CODE = "en-US";

        IDictionary<string, string> languageCultureCodeMap = new Dictionary<string, string>
        {
            { "English (US)", "en-US" }
        };

        public MenuView()
        {
            SetLanguage(DEFAULT_CULTURE_CODE);
            InitializeComponent();
        }

        void OnExitMenuClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        void LanguageItemClick(object sender, RoutedEventArgs e)
        {
            // pull selected item's text contents
            var menu_item = sender as MenuItem;
            if (menu_item == null)
                return;

            // check the menuitem
            var parent_item = menu_item.Parent as MenuItem;
            if (parent_item == null)
                return;

            foreach (MenuItem item in parent_item.Items)
                item.IsChecked = item == menu_item;

            string language = menu_item.Header as string;
            if (language == null)
                return;

            // map text content to culture code
            string cultureCode;
            if (!languageCultureCodeMap.TryGetValue(language, out cultureCode))
                SetLanguage(DEFAULT_CULTURE_CODE);
            else
                SetLanguage(cultureCode);
        }

        public void SetLanguage(string cultureCode)
        {
            ResourceDictionary dict = new ResourceDictionary();
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureCode);

            string i18n_path = string.Format("i18n/Strings.{0}.xaml", cultureCode);

            if (Uri.IsWellFormedUriString(i18n_path, UriKind.Relative))
                dict.Source = new Uri(i18n_path, UriKind.Relative);
            else
                dict.Source = new Uri(string.Format("i18n/Strings.{0}.xaml", DEFAULT_CULTURE_CODE), UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
}
