using BrainBox.Desktop.Model;
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
        private int? _selectedThemeId = null;
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

        // GESTIONE SELEZIONE LISTA
        private void ThemesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTheme = ThemesListBox.SelectedItem as ThemeDto;
            bool hasSelection = selectedTheme != null;

            // Abilita/disabilita pulsanti in base alla selezione
            DeleteThemeButton.IsEnabled = hasSelection;
            EditThemeButton.IsEnabled = hasSelection;

            // Memorizza ID del tema selezionato
            _selectedThemeId = selectedTheme?.Id;

            // Nascondi sezione edit se cambio selezione
            EditThemeSection.Visibility = Visibility.Collapsed;
        }

        // CREA NUOVO TEMA
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
                _loadedThemes.Add(newTheme);
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

        //  MOSTRA FORM MODIFICA TEMA
        private void EditTheme_Click(object sender, RoutedEventArgs e)
        {
            var selectedTheme = ThemesListBox.SelectedItem as ThemeDto;
            if (selectedTheme == null)
            {
                MessageBox.Show("Seleziona un tema dalla lista!", "Attenzione",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Popola la TextBox con il nome corrente
            EditThemeTextBox.Text = selectedTheme.Name;

            // Mostra la sezione edit
            EditThemeSection.Visibility = Visibility.Visible;

            // Focus sulla textbox e seleziona tutto il testo
            EditThemeTextBox.Focus();
            EditThemeTextBox.SelectAll();
        }

        private async void UpdateTheme_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedThemeId == null)
            {
                MessageBox.Show("Nessun tema selezionato!", "Errore",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string newName = EditThemeTextBox.Text.Trim();

            // Validazione: nome vuoto
            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Il nome del tema non può essere vuoto!", "Attenzione",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Trova tema corrente nella collezione
            var currentTheme = _loadedThemes.FirstOrDefault(t => t.Id == _selectedThemeId);
            if (currentTheme == null) return;

            // Validazione: duplicati
            var duplicate = _loadedThemes.Any(t =>
                t.Id != _selectedThemeId &&
                t.Name.Equals(newName, StringComparison.OrdinalIgnoreCase));

            if (duplicate)
            {
                MessageBox.Show($"Esiste già un tema con il nome '{newName}'!", "Attenzione",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Se il nome non è cambiato, non fare nulla
            if (currentTheme.Name.Equals(newName, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Il nome non è stato modificato!", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                EditThemeSection.Visibility = Visibility.Collapsed;
                return;
            }

            // Chiedi conferma
            var result = MessageBox.Show(
                $"Confermi la modifica del tema:\n\n'{currentTheme.Name}'\n\n→ '{newName}'?",
                "Conferma Modifica",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                // Chiamata API per aggiornare sul server
                await _apiService.UpdateThemeAsync(_selectedThemeId.Value, newName);

                // Aggiorna la collezione locale
                currentTheme.Name = newName;

                // Forza il refresh della ListBox per mostrare il nuovo nome
                ThemesListBox.Items.Refresh();

                EditThemeSection.Visibility = Visibility.Collapsed;

                MessageBox.Show($"✅ Tema aggiornato in '{newName}'!", "Successo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Errore nell'aggiornamento del tema:\n\n{ex.Message}",
                    "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //  ANNULLA MODIFICA TEMA
        private void CancelEditTheme_Click(object sender, RoutedEventArgs e)
        {
            // Nascondi la sezione edit e pulisci la textbox
            EditThemeSection.Visibility = Visibility.Collapsed;
            EditThemeTextBox.Clear();
        }

        // ELIMINA TEMA
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

        // TORNA ALLA HOME
        private void BackToHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
