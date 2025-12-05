using BrainBox.Desktop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BrainBox.Desktop.Controls
{
    public partial class IdeaFilters : UserControl
    {
        public event EventHandler<FilterEventArgs>? FiltersApplied;
        private List<ThemeDto> _availableThemes = [];

        public IdeaFilters()
        {
            InitializeComponent();
        }

        public void LoadAvailableThemes(List<ThemeDto> themes)
        {
            //MessageBox.Show($"DEBUG: Filtri ha ricevuto {themes.Count} temi!");

            _availableThemes = themes;
            ThemeCheckBoxList.ItemsSource = themes;
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("DEBUG: Click su Applica Filtri rilevato! Invio evento...");
            var filters = new FilterEventArgs
            {
                CreatedFrom = CreatedFromDatePicker.SelectedDate,
                CreatedTo = CreatedToDatePicker.SelectedDate,
                ModifiedFrom = ModifiedFromDatePicker.SelectedDate,
                ModifiedTo = ModifiedToDatePicker.SelectedDate,
                SelectedThemeIds = GetSelectedThemeIds()
            };

            FiltersApplied?.Invoke(this, filters);
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            CreatedFromDatePicker.SelectedDate = null;
            CreatedToDatePicker.SelectedDate = null;
            ModifiedFromDatePicker.SelectedDate = null;
            ModifiedToDatePicker.SelectedDate = null;

            foreach (var theme in _availableThemes)
            {
                var checkbox = FindCheckBoxForTheme(theme.Id);
                if (checkbox != null)
                    checkbox.IsChecked = false;
            }

            var emptyFilters = new FilterEventArgs();
            FiltersApplied?.Invoke(this, emptyFilters);
        }

        private void ThemeCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            
        }

        private List<int> GetSelectedThemeIds()
        {
            var selectedIds = new List<int>();

            foreach (var item in ThemeCheckBoxList.Items)
            {
                var theme = item as ThemeDto;
                if (theme == null) continue;

                var checkbox = FindCheckBoxForTheme(theme.Id);
                if (checkbox != null && checkbox.IsChecked == true)
                {
                    selectedIds.Add(theme.Id);
                }
            }

            return selectedIds;
        }

        private CheckBox? FindCheckBoxForTheme(int themeId)
        {
            foreach (var item in ThemeCheckBoxList.Items)
            {
                var theme = item as ThemeDto;
                if (theme?.Id != themeId) continue;

                var container = ThemeCheckBoxList.ItemContainerGenerator
                    .ContainerFromItem(item);
                if (container == null) continue;

                return FindVisualChild<CheckBox>(container as DependencyObject);
            }
            return null;
        }

        private T? FindVisualChild<T>(DependencyObject? parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                    return typedChild;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }

    public class FilterEventArgs : EventArgs
    {
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public DateTime? ModifiedFrom { get; set; }
        public DateTime? ModifiedTo { get; set; }
        public List<int> SelectedThemeIds { get; set; } = new();
    }
}
