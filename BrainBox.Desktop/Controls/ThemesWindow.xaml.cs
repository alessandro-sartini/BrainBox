using System.Windows;

namespace BrainBox.Desktop
{
    public partial class ThemesWindow : Window
    {
        public ThemesWindow()
        {
            InitializeComponent();
            LoadThemes();
        }

        private void LoadThemes()
        {
            ThemesListBox.Items.Add("Mobile");
            ThemesListBox.Items.Add("Desktop");
            ThemesListBox.Items.Add("Web");
            ThemesListBox.Items.Add("Cibo");
            ThemesListBox.Items.Add("Gaming");
        }

        private void CreateTheme_Click(object sender, RoutedEventArgs e)
        {
            string themeName = NewThemeTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(themeName))
            {
                MessageBox.Show("Inserisci un nome per il tema!", "Attenzione",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (var item in ThemesListBox.Items)
            {
                if (item.ToString()!.Equals(themeName, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Questo tema esiste già!", "Attenzione",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            ThemesListBox.Items.Add(themeName);
            NewThemeTextBox.Clear();

            MessageBox.Show($"Tema '{themeName}' creato!", "Successo",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteTheme_Click(object sender, RoutedEventArgs e)
        {
            if (ThemesListBox.SelectedItem == null)
            {
                MessageBox.Show("Seleziona un tema da eliminare!", "Attenzione",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string themeName = ThemesListBox.SelectedItem.ToString()!;

            var result = MessageBox.Show($"Eliminare il tema '{themeName}'?",
                "Conferma", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ThemesListBox.Items.Remove(ThemesListBox.SelectedItem);
                MessageBox.Show("Tema eliminato!", "Successo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BackToHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
