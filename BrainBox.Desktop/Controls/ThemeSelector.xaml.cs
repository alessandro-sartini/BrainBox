
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BrainBox.Desktop.Models;

namespace BrainBox.Desktop.Controls
{
    public partial class ThemeSelector : UserControl
    {
        private List<ThemeDto> _availableThemes = [];
        private List<int> _selectedThemeIds = [];

        public ThemeSelector()
        {
            InitializeComponent();
        }

        // Carica tutti i temi disponibili (checkbox)
        public void LoadAvailableThemes(List<ThemeDto> themes)
        {
            _availableThemes = themes ?? new List<ThemeDto>();

            AvailableThemesListBox.Items.Clear();

            foreach (var theme in _availableThemes)
            {
                var checkBox = new CheckBox
                {
                    Content = theme.Name,
                    Tag = theme.Id,
                    Margin = new Thickness(5, 3, 5, 3),
                     Foreground = new SolidColorBrush(Colors.Black)
                };

                checkBox.Checked += ThemeCheckBox_Checked;
                checkBox.Unchecked += ThemeCheckBox_Unchecked;

                AvailableThemesListBox.Items.Add(checkBox);
            }

            UpdateCheckboxStates();
            UpdateChipsDisplay();
        }

      
        public void SetSelectedThemeIds(List<int> themeIds)
        {
            _selectedThemeIds.Clear();

            if (themeIds != null && themeIds.Count > 0)
                _selectedThemeIds.AddRange(themeIds);

            UpdateCheckboxStates();
            UpdateChipsDisplay();
        }

       
        public void SetSelectedThemes(List<ThemeDto> selectedThemes)
        {
            var themeIds = selectedThemes?.Select(t => t.Id).ToList() ?? new List<int>();
            SetSelectedThemeIds(themeIds);
        }

        public List<int> GetSelectedThemeIds()
        {
            return [.. _selectedThemeIds];
        }

        public void Clear()
        {
            _selectedThemeIds.Clear();
            UpdateCheckboxStates();
            UpdateChipsDisplay();
        }

        private void ThemeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Tag is int themeId)
            {
                if (!_selectedThemeIds.Contains(themeId))
                {
                    _selectedThemeIds.Add(themeId);
                    UpdateChipsDisplay();
                }
            }
        }

        private void ThemeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.Tag is int themeId)
            {
                _selectedThemeIds.Remove(themeId);
                UpdateChipsDisplay();
            }
        }

        private void UpdateChipsDisplay()
        {
            SelectedThemesPanel.Children.Clear();

            if (_selectedThemeIds.Count == 0)
            {
                var noThemesText = new TextBlock
                {
                    Text = "Nessun tema selezionato",
                    Foreground = new SolidColorBrush(Colors.Gray),
                    FontStyle = FontStyles.Italic,
                    FontSize = 12
                };
                SelectedThemesPanel.Children.Add(noThemesText);
                return;
            }

            foreach (var themeId in _selectedThemeIds)
            {
                var theme = _availableThemes.Find(t => t.Id == themeId);
                if (theme == null) continue;

                var chip = CreateChip(theme);
                SelectedThemesPanel.Children.Add(chip);
            }
        }

        private Border CreateChip(ThemeDto theme)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(3, 2, 3, 2)
            };

            var textBlock = new TextBlock
            {
                Text = theme.Name,
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 12
            };

            border.Child = textBlock;
            return border;
        }

        private void UpdateCheckboxStates()
        {
            foreach (var item in AvailableThemesListBox.Items)
            {
                if (item is CheckBox checkBox && checkBox.Tag is int themeId)
                {
                    checkBox.IsChecked = _selectedThemeIds.Contains(themeId);
                }
            }
        }
    }
}
