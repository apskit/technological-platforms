using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace lab8
{
    public partial class NewElementWindow : Window
    {
        private readonly string selectedFolderPath;

        public NewElementWindow(string selectedFolderPath)
        {
            InitializeComponent();
            this.selectedFolderPath = selectedFolderPath;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            string name = nameTextBox.Text.Trim();
            string path = Path.Combine(selectedFolderPath, name);

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Name:", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                if (directoryRadioButton.IsChecked == true)
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    File.Create(path).Close();
                }

                FileAttributes attributes = FileAttributes.Normal;
                if (readOnlyCheckBox.IsChecked == true)
                {
                    attributes |= FileAttributes.ReadOnly;
                }
                if (archiveCheckBox.IsChecked == true)
                {
                    attributes |= FileAttributes.Archive;
                }
                if (hiddenCheckBox.IsChecked == true)
                {
                    attributes |= FileAttributes.Hidden;
                }
                if (systemCheckBox.IsChecked == true)
                {
                    attributes |= FileAttributes.System;
                }

                File.SetAttributes(path, attributes);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}