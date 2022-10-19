using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using TemplateFromPDF.Model;

namespace TemplateFromPDF.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class FileLoadWindow : Window
    {
        public ObservableCollection<string> pdfFilesPaths { get; set; }
            = new ObservableCollection<string>();

        public FileLoadWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
        }

        // TODO организовать извлечение путей файлов в отдельный файл/класс
        private void LoadPDFFilesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openPDFDialog = new OpenFileDialog();
            openPDFDialog.Filter = "PDF файлы (.pdf)|*.pdf";
            openPDFDialog.Multiselect = true;

            bool? result = openPDFDialog.ShowDialog();

            if (result is true)
            {
                string[] selectedFiles = openPDFDialog.FileNames;
                if (selectedFiles.Length == 0)
                    return;

                foreach(string selectedFile in selectedFiles)
                    if (!pdfFilesPaths.Contains(selectedFile))
                        pdfFilesPaths.Add(selectedFile);
            }
        }

        private void DeletePDFFilesFromListButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedPDFFiles = new List<string>(PDFFilesListBox.SelectedItems.Cast<string>());
            foreach (string selectedFile in selectedPDFFiles)
                pdfFilesPaths.Remove(selectedFile);
        }

        private void ProcessPDFFilesButton_Click(object sender, RoutedEventArgs e)
        {
            if (pdfFilesPaths.Count == 0)
            {
                MessageBox.Show("Не выбраны PDF файлы для обработки. Выберите хотя бы 1 файл", "Не выбраны файлы",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PDFFieldsWindow fieldsWindow = new PDFFieldsWindow(pdfFilesPaths.ToArray());

            this.Hide();
            fieldsWindow.ShowDialog();
            this.Show();
        }
    }
}
