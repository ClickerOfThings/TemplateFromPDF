using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
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
        public ObservableCollection<string> PDFFilesPaths { get; set; }
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

            if (openPDFDialog.ShowDialog() is true)
            {
                string[] selectedFiles = openPDFDialog.FileNames;
                if (selectedFiles.Length == 0)
                    return;

                foreach(string selectedFile in selectedFiles)
                    if (!PDFFilesPaths.Contains(selectedFile))
                        PDFFilesPaths.Add(selectedFile);
            }
        }

        private void DeletePDFFilesFromListButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedPDFFiles = new List<string>(PDFFilesListBox.SelectedItems.Cast<string>());
            foreach (string selectedFile in selectedPDFFiles)
                PDFFilesPaths.Remove(selectedFile);
        }

        private void ProcessPDFFilesButton_Click(object sender, RoutedEventArgs e)
        {
            if (PDFFilesPaths.Count == 0)
            {
                MessageBox.Show("Не выбраны PDF файлы для обработки. Выберите хотя бы 1 файл", "Не выбраны файлы",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<PDFFile> parsedPDFFiles = PDFFile.ParsePDFFiles(PDFFilesPaths.ToArray());

            PDFFile? failedParsedPDF = parsedPDFFiles.FirstOrDefault(x => x.Fields.Count == 0);
            if (failedParsedPDF is not null)
            {
                MessageBox.Show($"Не удалось извлечь поля из файла {failedParsedPDF.FilePath}. " +
                    $"Проверьте файл на наличие ошибок.", "Ошибка извлечения данных",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PDFFieldsWindow fieldsWindow = new PDFFieldsWindow(parsedPDFFiles);

            this.Hide();
            fieldsWindow.ShowDialog();
            this.Show();
        }
    }
}
