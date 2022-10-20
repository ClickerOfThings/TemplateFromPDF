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
        private readonly List<PDFFile> pdfFilesList;
        private readonly List<Dictionary<string, string>> fieldsList = new List<Dictionary<string, string>>();

        public PDFFieldsWindow(List<PDFFile> pdfFilesList)
        {
            InitializeComponent();

            this.pdfFilesList = pdfFilesList;
            fieldsList.AddRange(pdfFilesList.Select(x => x.Fields));
        }
        
        // TODO организовать создание словарей и колонок в отдельный файл/класс
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<DataGridTextColumn> columnsOfPDFHeaders = Model.Helper.PDFFileColumnGenerator.GenerateColumnsFromPDFFiles(pdfFilesList);
            foreach(DataGridTextColumn column in columnsOfPDFHeaders)
            {
                PDFFilesFieldsDataGrid.Columns.Add(column);
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
