using System.Windows;
using BrainBox.Desktop.Services;
using BrainBox.Desktop.Controls;
using BrainBox.Desktop.Model;

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
            _apiService = (BrainBoxApiService)Application.Current.FindResource("ApiService");
            this.Loaded += IdeasWindow_Loaded;
        }

        private async void IdeasWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadThemesAsync();

           
            FiltersControl.FiltersApplied += FiltersControl_FiltersApplied;

            LoadIdeas();

            IdeasListBox.SelectedIndex = -1;
            ClearForm();
        }

        private async Task LoadThemesAsync()
        {
            try
            {
                var themes = await _apiService.GetThemesAsync();

                // Carica nel form di creazione
                ThemeSelectorControl.LoadAvailableThemes(themes);

                FiltersControl.LoadAvailableThemes(themes);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Errore temi: {ex.Message}");
            }
        }

      
        private async void FiltersControl_FiltersApplied(object? sender, FilterEventArgs e)
        {

            try
            {
                IdeasListBox.Items.Clear();
                IdeasListBox.Items.Add("⏳ Filtraggio in corso...");

                // CHIAMA L'API CON I PARAMETRI
                _ideas = await _apiService.GetIdeasAsync(
                    createdFrom: e.CreatedFrom,
                    createdTo: e.CreatedTo,
                    modifiedFrom: e.ModifiedFrom,
                    modifiedTo: e.ModifiedTo,
                    themeIds: e.SelectedThemeIds
                );

                IdeasListBox.Items.Clear();

                if (_ideas.Count == 0)
                {
                    IdeasListBox.Items.Add("📭 Nessuna idea trovata con questi filtri");
                    return;
                }

                foreach (var idea in _ideas)
                {
                    IdeasListBox.Items.Add($"{idea.Id}. {idea.Title}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Errore Filtri: {ex.Message}");
                IdeasListBox.Items.Clear();
                IdeasListBox.Items.Add("❌ Errore");
            }
        }

        
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Errore LoadIdeas: {ex.Message}");
            }
        }

       
        private void IdeasListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
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

            int dotIndex = text.IndexOf('.');
            if (dotIndex == -1 || !int.TryParse(text.Substring(0, dotIndex), out int ideaId))
            {
                ClearForm();
                return;
            }

            var idea = _ideas.FirstOrDefault(i => i.Id == ideaId);
            if (idea == null) return;

            _selectedIdeaId = idea.Id;
            TitleTextBox.Text = idea.Title;
            DescriptionTextBox.Text = idea.Description;
            UpdateIdeaButton.IsEnabled = true;

            var themeIds = idea.Themes?.Select(t => t.Id).ToList() ?? new List<int>();
            ThemeSelectorControl.SetSelectedThemeIds(themeIds);
        }

     
        private async void CreateIdea_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedIdeaId != null)
            {
                ClearForm();
                IdeasListBox.SelectedIndex = -1;
                return;
            }

            if (string.IsNullOrWhiteSpace(TitleTextBox.Text)) return;

            try
            {
                var createDto = new CreateIdeaDto
                {
                    Title = TitleTextBox.Text.Trim(),
                    Description = DescriptionTextBox.Text.Trim(),
                    ThemeIds = ThemeSelectorControl.GetSelectedThemeIds()
                };

                await _apiService.CreateIdeaAsync(createDto);
                await ReloadIdeasAndSelectLast();
                MessageBox.Show("✅ Creata!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Errore Crea: {ex.Message}");
            }
        }

        private async void UpdateIdea_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedIdeaId == null) return;

            try
            {
                var updateDto = new UpdateIdeaDto
                {
                    Title = TitleTextBox.Text.Trim(),
                    Description = DescriptionTextBox.Text.Trim(),
                    ThemeIds = ThemeSelectorControl.GetSelectedThemeIds()
                };

                await _apiService.UpdateIdeaAsync(_selectedIdeaId.Value, updateDto);
                LoadIdeas();
                MessageBox.Show("✅ Aggiornata!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Errore Aggiorna: {ex.Message}");
            }
        }

        private async void DeleteIdea_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedIdeaId == null) return;

            if (MessageBox.Show("Sicuro?", "Elimina", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            try
            {
                await _apiService.DeleteIdeaAsync(_selectedIdeaId.Value);
                LoadIdeas();
                ClearForm();
                MessageBox.Show("✅ Eliminata!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Errore Elimina: {ex.Message}");
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
            foreach (var idea in _ideas) IdeasListBox.Items.Add($"{idea.Id}. {idea.Title}");
            if (IdeasListBox.Items.Count > 0) IdeasListBox.SelectedIndex = IdeasListBox.Items.Count - 1;
        }
    }
}
