using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BrainBox.Desktop.Services;
using BrainBox.Desktop.Models;

namespace BrainBox.Desktop
{
    public partial class IdeasWindow : Window
    {
        private readonly BrainBoxApiService _apiService;
        private List<IdeaDto> _ideas = new();
        private int? _selectedIdeaId = null;

        public IdeasWindow()
        {
            InitializeComponent();
            _apiService = new BrainBoxApiService();

            this.Loaded += IdeasWindow_Loaded;
        }

        private async void IdeasWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadThemesAsync();
            LoadIdeas();

            // Modalità CREAZIONE all’avvio: nessuna idea selezionata, form vuoto
            IdeasListBox.SelectedIndex = -1;
            ClearForm();
        }

        // ════════════════════════════════════════
        // CARICA TEMI DAL SERVER
        // ════════════════════════════════════════
        private async Task LoadThemesAsync()
        {
            try
            {
                var themes = await _apiService.GetThemesAsync();
                ThemeSelectorControl.LoadAvailableThemes(themes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Errore nel caricamento dei temi:\n\n{ex.Message}",
                    "Errore",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ════════════════════════════════════════
        // CARICA IDEE DAL SERVER
        // ════════════════════════════════════════
        private async void LoadIdeas()
        {
            try
            {
                IdeasListBox.Items.Clear();
                IdeasListBox.Items.Add("⏳ Caricamento...");

                _ideas = await _apiService.GetIdeasAsync();

                IdeasListBox.Items.Clear();

                if (_ideas.Count == 0)
                {
                    IdeasListBox.Items.Add("📭 Nessuna idea trovata");
                    return;
                }

                foreach (var idea in _ideas)
                {
                    IdeasListBox.Items.Add($"{idea.Id}. {idea.Title}");
                }

                // Nessuna selezione automatica: restiamo in modalità creazione
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Errore nel caricamento delle idee:\n\n{ex.Message}\n\nVerifica che il backend sia avviato!",
                    "Errore",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                IdeasListBox.Items.Clear();
                IdeasListBox.Items.Add("❌ Errore caricamento");
            }
        }

        private void IdeasListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Nessuna selezione → torna in modalità nuova idea
            if (IdeasListBox.SelectedItem == null)
            {
                ClearForm();
                return;
            }

            var text = IdeasListBox.SelectedItem.ToString()!;
            if (text.StartsWith("⏳") || text.StartsWith("📭") || text.StartsWith("❌"))
            {
                ClearForm();
                return;
            }

            string selectedText = text;
            int dotIndex = selectedText.IndexOf('.');

            if (dotIndex == -1)
            {
                ClearForm();
                return;
            }

            if (!int.TryParse(selectedText.Substring(0, dotIndex), out int ideaId))
            {
                ClearForm();
                return;
            }

            var idea = _ideas.FirstOrDefault(i => i.Id == ideaId);

            if (idea == null)
            {
                ClearForm();
                return;
            }

            _selectedIdeaId = idea.Id;
            TitleTextBox.Text = idea.Title;
            DescriptionTextBox.Text = idea.Description;
            UpdateIdeaButton.IsEnabled = true;

            // Evidenzia solo i temi dell’idea (le checkbox esistono già)
            var themeIds = idea.Themes?.Select(t => t.Id).ToList() ?? new List<int>();
            ThemeSelectorControl.SetSelectedThemeIds(themeIds);
        }

        // ════════════════════════════════════════
        // CREA NUOVA IDEA
        // ════════════════════════════════════════
        private async void CreateIdea_Click(object sender, RoutedEventArgs e)
        {
            // Se stai modificando un’idea, il primo click passa in modalità creazione
            if (_selectedIdeaId != null)
            {
                ClearForm();
                IdeasListBox.SelectedIndex = -1;
                return;
            }

            string title = TitleTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Inserisci un titolo!", "Attenzione",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Inserisci una descrione per la tua idea!", "Attenzione",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var selectedThemeIds = ThemeSelectorControl.GetSelectedThemeIds();

                var createDto = new CreateIdeaDto
                {
                    Title = title,
                    Description = description,
                    ThemeIds = selectedThemeIds
                };

                var createdIdea = await _apiService.CreateIdeaAsync(createDto);

                await ReloadIdeasAndSelectLast();

                MessageBox.Show(
                    $"✅ Idea '{createdIdea.Title}' creata con successo!\n\nID: {createdIdea.Id}",
                    "Successo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Errore nella creazione dell'idea:\n\n{ex.Message}",
                    "Errore",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ════════════════════════════════════════
        // AGGIORNA IDEA ESISTENTE
        // ════════════════════════════════════════
        private async void UpdateIdea_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedIdeaId == null)
            {
                MessageBox.Show("Seleziona un'idea dalla lista!", "Attenzione",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string title = TitleTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Il titolo non può essere vuoto!", "Attenzione",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var idea = _ideas.FirstOrDefault(i => i.Id == _selectedIdeaId);
            if (idea == null) return;

            var result = MessageBox.Show(
                $"Confermi la modifica dell'idea:\n\n'{idea.Title}'\n\n→ '{title}'?",
                "Conferma Modifica",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                var selectedThemeIds = ThemeSelectorControl.GetSelectedThemeIds();

                var updateDto = new UpdateIdeaDto
                {
                    Title = title,
                    Description = description,
                    ThemeIds = selectedThemeIds
                };

                await _apiService.UpdateIdeaAsync(_selectedIdeaId.Value, updateDto);

                int currentSelectedId = _selectedIdeaId.Value;
                LoadIdeas();
                SelectIdeaById(currentSelectedId);

                MessageBox.Show(
                    $"✅ Idea '{title}' aggiornata con successo!",
                    "Successo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Errore nell'aggiornamento dell'idea:\n\n{ex.Message}",
                    "Errore",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async void DeleteIdea_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedIdeaId == null)
            {
                MessageBox.Show("Seleziona un'idea dalla lista!", "Attenzione",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var idea = _ideas.FirstOrDefault(i => i.Id == _selectedIdeaId);
            if (idea == null) return;

            var result = MessageBox.Show(
                $"Sei sicuro di voler eliminare l'idea:\n\n'{idea.Title}'?\n\nQuesta azione non può essere annullata.",
                "Conferma Eliminazione",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                await _apiService.DeleteIdeaAsync(_selectedIdeaId.Value);
                LoadIdeas();

                MessageBox.Show(
                    $"✅ Idea '{idea.Title}' eliminata con successo!",
                    "Successo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Errore nell'eliminazione dell'idea:\n\n{ex.Message}",
                    "Errore",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            ThemeSelectorControl.Clear();
            _selectedIdeaId = null;
            UpdateIdeaButton.IsEnabled = false;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadIdeas();
        }

        private void BackToHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async Task ReloadIdeasAndSelectLast()
        {
            _ideas = await _apiService.GetIdeasAsync();
            IdeasListBox.Items.Clear();

            foreach (var idea in _ideas)
            {
                IdeasListBox.Items.Add($"{idea.Id}. {idea.Title}");
            }

            if (IdeasListBox.Items.Count > 0)
            {
                IdeasListBox.SelectedIndex = IdeasListBox.Items.Count - 1;
            }
        }

        private void SelectIdeaById(int ideaId)
        {
            for (int i = 0; i < IdeasListBox.Items.Count; i++)
            {
                string item = IdeasListBox.Items[i].ToString()!;
                if (item.StartsWith($"{ideaId}."))
                {
                    IdeasListBox.SelectedIndex = i;
                    break;
                }
            }
        }
    }
}
