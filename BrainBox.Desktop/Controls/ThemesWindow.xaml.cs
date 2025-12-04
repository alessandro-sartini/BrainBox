using BrainBox.Desktop.Models;
using BrainBox.Desktop.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BrainBox.Desktop
{
    public partial class ThemesWindow : Window
    {
        private readonly BrainBoxApiService _apiService;
        private readonly ObservableCollection<ThemeDto> _loadedThemes = new();

        public ThemesWindow()
        {
            InitializeComponent();
            _apiService = (BrainBoxApiService)Application.Current.FindResource("ApiService");
            Loaded += ThemesWindow_Loaded;
            ThemesListBox.SelectionChanged += ThemesListBox_SelectionChanged;
        }

        private async void ThemesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadThemesAsync();
        }

        private async Task LoadThemesAsync()
        {
            try
            {
                var themes = await _apiService.GetThemesAsync();
                LoadThemes(themes);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore caricamento temi: {ex.Message}", "Errore",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadThemes(List<ThemeDto> themeDtos)
        {
            _loadedThemes.Clear();  
            foreach (var theme in themeDtos)
            {
                _loadedThemes.Add(theme);  
            }
            ThemesListBox.ItemsSource = _loadedThemes;  
        }

        private async void CreateTheme_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null) btn.IsEnabled = false;

            try
            {
                string themeName = NewThemeTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(themeName))
                {
                    MessageBox.Show("Inserisci un nome per il tema!", "Attenzione",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Controllo duplicati
                foreach (ThemeDto item in _loadedThemes)
                {
                    if (item.Name.Equals(themeName, StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Questo tema esiste già!", "Attenzione",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var newTheme = await _apiService.CreateThemeAsync(themeName);
                _loadedThemes.Add(newTheme);  // AGGIUNGE ALLA COLLEZIONE
                NewThemeTextBox.Clear();

                MessageBox.Show($"Tema '{themeName}' creato!", "Successo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore creazione tema: {ex.Message}", "Errore",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (btn != null) btn.IsEnabled = true;
            }
        }

        private async void DeleteTheme_Click(object sender, RoutedEventArgs e)
        {
            var selectedTheme = ThemesListBox.SelectedItem as ThemeDto;
            if (selectedTheme == null) return;

            var result = MessageBox.Show($"Eliminare '{selectedTheme.Name}'?", "Conferma",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                await _apiService.DeleteThemeAsync(selectedTheme.Id);
                _loadedThemes.Remove(selectedTheme);  
                MessageBox.Show("Tema eliminato!", "Successo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore: {ex.Message}");
            }
        }




        private void BackToHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void ThemesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DeleteThemeButton.IsEnabled = ThemesListBox.SelectedItem != null;
        }

    }
}
