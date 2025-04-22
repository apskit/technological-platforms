using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TreeView = System.Windows.Controls.TreeView;

namespace lab8
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Obsługa zdarzenia dla opcji "Open"
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog() { Description = "Wybierz folder do otwarcia:" };
            dlg.ShowDialog();

            // pobranie wybranej ścieżki
            string path = dlg.SelectedPath;
            DisplayTreeView(path);

        }

        private void DisplayTreeView(string selectedPath, TreeViewItem parentItem)
        {
            // dodawanie elementów do drzewa dla każdego pliku
            foreach (string file in System.IO.Directory.GetFiles(selectedPath))
            {
                TreeViewItem fileItem = new TreeViewItem();
                fileItem.Header = System.IO.Path.GetFileName(file);
                fileItem.Tag = file;
                parentItem.Items.Add(fileItem);
            }

            // dodawanie elementów do drzewa dla każdego folderu
            foreach (string directory in System.IO.Directory.GetDirectories(selectedPath))
            {
                TreeViewItem folderItem = new TreeViewItem();
                folderItem.Header = System.IO.Path.GetFileName(directory);
                folderItem.Tag = directory;
                parentItem.Items.Add(folderItem);

                DisplayTreeView(directory, folderItem);
            }
        }

        private void DisplayTreeView(string selectedPath)
        {
            // element TreeViewItem dla folderu głównego
            TreeViewItem rootItem = new TreeViewItem();
            rootItem.Header = System.IO.Path.GetFileName(selectedPath); // nazwa folderu głównego
            rootItem.Tag = selectedPath; // ścieżka folderu głównego

            // wyświetlenie drzewa dla folderu głównego
            DisplayTreeView(selectedPath, rootItem);

            // dodanie folderu głównego do drzewa
            treeView.Items.Add(rootItem);
        }


        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = treeView.SelectedItem as TreeViewItem;

            if (selectedItem != null)
            {
                string path = selectedItem.Tag as string;

                if (path != null)
                {
                    if (File.Exists(path))
                    {
                        FileAttributes attributes = File.GetAttributes(path);

                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            attributes &= ~FileAttributes.ReadOnly; // and  
                            File.SetAttributes(path, attributes);
                        }

                        File.Delete(path);
                    }
                    else if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }

                    // najwyższy poziom w drzewie
                    if (selectedItem.Parent is TreeView)
                    {
                        treeView.Items.Remove(selectedItem);
                    }
                    else
                    {
                        // usuwanie elementu z drzewa
                        TreeViewItem parentItem = selectedItem.Parent as TreeViewItem;
                        if (parentItem != null)
                        {
                            parentItem.Items.Remove(selectedItem);
                        }
                    }
                }
            }
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = treeView.SelectedItem as TreeViewItem;

            if (selectedItem != null)
            {
                string path = selectedItem.Tag as string;

                if (File.Exists(path))
                {
                    // zawartość pliku do ciągu tekstowego
                    string content = File.ReadAllText(path);

                    // zawartość pliku w polu tekstowym
                    textBlock.Text = content;
                }
            }
        }

        private void CreateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = treeView.SelectedItem as TreeViewItem;
            string path = selectedItem.Tag as string;

            NewElementWindow createWindow = new NewElementWindow(path);
            createWindow.ShowDialog();

            // odświeżenie drzewa
            treeView.Items.Clear();
            DisplayTreeView(path);
        }



        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
            if (menuItem != null)
            {
                System.Windows.MessageBox.Show($"Kliknąłeś: {menuItem.Header}");
            }
        }

        private void DOSAttributes(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = treeView.SelectedItem as TreeViewItem;
            if (item != null)
            {
                string path = item.Tag as string;
                FileAttributes attributes = File.GetAttributes(path);
                string fileStatus = "";

                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    fileStatus += 'r';
                else
                    fileStatus += '-';

                if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
                    fileStatus += 'a';
                else
                    fileStatus += '-';

                if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    fileStatus += 'h';
                else
                    fileStatus += '-';

                if ((attributes & FileAttributes.System) == FileAttributes.System)
                    fileStatus += 's';
                else
                    fileStatus += '-';

                statusTextBlock.Text = fileStatus;
            }
            else
            {
                statusTextBlock.Text = string.Empty;
            }
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }
    }
}
