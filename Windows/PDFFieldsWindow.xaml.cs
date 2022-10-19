using iText.Layout.Element;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
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
using System.Windows.Shapes;

using TemplateFromPDF.Model;

namespace TemplateFromPDF.Windows
{
    /// <summary>
    /// Interaction logic for PDFFieldsWindow.xaml
    /// </summary>
    public partial class PDFFieldsWindow : Window
    {
        private readonly string[] pdfFilesPaths;
        private readonly List<Dictionary<string, string>> fieldsList = new List<Dictionary<string, string>>();

        public PDFFieldsWindow(string[] pdfFilesPaths)
        {
            InitializeComponent();

            this.pdfFilesPaths = pdfFilesPaths;
        }
        
        // TODO организовать создание словарей и колонок в отдельный файл/класс
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(string pdfFilePath in pdfFilesPaths)
            {
                PDFFile currentPDFFile = PDFFile.LoadFromFile(pdfFilePath);

                fieldsList.Add(currentPDFFile.Fields);
            }

            var columns = new List<DataGridTextColumn>();
            foreach (Dictionary<string, string> dict in fieldsList)
            {
                foreach(string key in dict.Keys)
                {
                    if (columns.Any(x => (string)x.Header == key))
                        continue;
                    DataGridTextColumn column = new DataGridTextColumn();
                    column.Header = key;
                    column.Binding = new Binding($"[{key}]");
                    columns.Add(column);
                }
            }
            foreach(DataGridTextColumn col in columns)
            {
                PDFFilesFieldsDataGrid.Columns.Add(col);
            }

            PDFFilesFieldsDataGrid.ItemsSource = fieldsList;
        }

        private void BackToFileLoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show
                ("Все изменения в полях будут утеряны, точно вернуться к выбору файлов?", "Назад",
                MessageBoxButton.YesNo, MessageBoxImage.Question) 
                == MessageBoxResult.No)
                return;
            this.Close();
        }

        private void SaveNewPDFsToFolderButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog folderChooseDialog = new CommonOpenFileDialog();
            folderChooseDialog.IsFolderPicker = true;
            folderChooseDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();

            if (folderChooseDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                // TODO вставить сохранение в шаблон
                if (MessageBox.Show($"Файлы сохранены в папку {folderChooseDialog.FileName}. Продолжить редактирование полей?",
                    "Успешно сохранено", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    this.Close();
            }
        }
    }
}
