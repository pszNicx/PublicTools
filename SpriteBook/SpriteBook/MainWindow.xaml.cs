﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using WinForms = System.Windows.Forms;

namespace SpriteBook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var openFolderDialog = new WinForms.FolderBrowserDialog())
            {
                var result = openFolderDialog.ShowDialog();
                if (result == WinForms.DialogResult.OK)
                {
                    var folderPath = openFolderDialog.SelectedPath + "\\";
                    var files = this.GetFiles(folderPath);
                    foreach (var file in files)
                        this._imageList.Items.Add(file);
                }
            }
        }

        private void AddImages_Click(object sender, RoutedEventArgs e)
        {
            // Restrict the input types to Text, Data, and Log files
            var openFileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "PNG Image (*.png)|*.png|All files (*.*)|*.*"
            };

            openFileDialog.ShowDialog();
            foreach (var filepath in openFileDialog.FileNames)
                this._imageList.Items.Add(filepath);
        }

        private void ClearList_Click(object sender, RoutedEventArgs e)
        {
            this._imageList.Items.Clear();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private List<string> GetFiles(string folder)
        {
            var filePaths = new List<string>();

            // Add this folder's files
            var files = Directory.GetFiles(folder, "*.png");
            foreach (string file in files)
                filePaths.Add(file);

            // Add subfolder files
            var subfolders = Directory.GetDirectories(folder);
            foreach (var subfolder in subfolders)
                filePaths.AddRange(this.GetFiles(subfolder));

            return filePaths;
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = this._imageList.SelectedIndex;
            if (selectedIndex < this._imageList.Items.Count - 1)
            {
                this._imageList.SelectedIndex = -1;
                var lowerPath = this._imageList.Items[selectedIndex + 1] as string;
                this._imageList.Items[selectedIndex + 1] = this._imageList.Items[selectedIndex] as string;
                this._imageList.Items[selectedIndex] = lowerPath;
                this._imageList.SelectedIndex = selectedIndex + 1;
            }
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = this._imageList.SelectedIndex;
            if (selectedIndex > 0)
            {
                this._imageList.SelectedIndex = -1;
                var upperPath = this._imageList.Items[selectedIndex - 1] as string;
                this._imageList.Items[selectedIndex - 1] = this._imageList.Items[selectedIndex] as string;
                this._imageList.Items[selectedIndex] = upperPath;
                this._imageList.SelectedIndex = selectedIndex - 1;
            }
        }

        private void RemoveImage_Click(object sender, RoutedEventArgs e)
        {
            this._imageList.Items.RemoveAt(this._imageList.SelectedIndex);
        }

        private void SaveSpriteSheet_Click(object sender, RoutedEventArgs e)
        {
            // Restrict the input types to Text, Data, and Log files
            var saveFileDialog = new SaveFileDialog()
            {
                Filter = "PNG Image (*.png)|*.png|All files (*.*)|*.*"
            };
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName == null)
            {
                MessageBox.Show("Must provide a valid save path.", "Error");
                return;
            }

            try
            {
                SpriteGenerator.GenerateSpriteSheet(this._imageList.Items.OfType<string>().ToArray<string>(), saveFileDialog.FileName, 1.0f);
            }
            catch (SpriteSizeException)
            {
                MessageBox.Show("Image sizes do not match!", "Error");
            }
        }
    }
}