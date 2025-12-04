using System.Windows;

namespace BrainBox.Desktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GoToIdeas_Click(object sender, RoutedEventArgs e)
        {
            // Apri finestra Gestione Idee
            IdeasWindow ideasWindow = new IdeasWindow();
            ideasWindow.Show();
        }

        private void GoToThemes_Click(object sender, RoutedEventArgs e)
        {
            // Apri finestra Gestione Temi
            ThemesWindow themesWindow = new ThemesWindow();
            themesWindow.Show();
        }
    }
}
