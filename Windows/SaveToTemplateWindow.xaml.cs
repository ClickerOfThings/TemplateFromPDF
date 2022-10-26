using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for SaveToTemplateWindow.xaml
    /// </summary>
    public partial class SaveToTemplateWindow : Window
    {
        private readonly string templateExampleBaseText;
        private readonly List<PDFFile> pdfFilesToSave;
        private const int MAX_EXAMPLE_FILENAME_LENGTH = 30;

        public SaveToTemplateWindow(List<PDFFile> pdfFilesToSave)
        {
            InitializeComponent();

            this.pdfFilesToSave = pdfFilesToSave;
            
            templateExampleBaseText = TemplateNameExampleTextBlock.Text;
        }

        private void BackToPDFFieldsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveToTemplatesButton_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = SaveFolderPathTextBox.Text.Trim(),
                   baseFilename = TemplateBaseNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(folderPath) || string.IsNullOrEmpty(baseFilename))
            {
                MessageBox.Show("Путь к папке и/или базовое название файлов не введены. Введите оба значения.",
                    "Ошибка создания", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int fileCounter = 0;
            foreach (PDFFile file in pdfFilesToSave)
            {

                file.SaveFieldsToTemplate("сертификат.png",
                    PDFFile.DEFAULT_TOP_MARGIN, PDFFile.DEFAULT_SIDES_MARGIN,
                    PDFFile.DEFAULT_PARAGRAPH_WRAPS,
                    $"{folderPath}/{baseFilename}-{fileCounter++}.pdf");
            }

            MessageBox.Show("Файлы успешно созданы.", "Созданы файлы",
                MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }

        private void BrowseSaveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog folderChooseDialog = new CommonOpenFileDialog();
            folderChooseDialog.IsFolderPicker = true;
            folderChooseDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();

            if (folderChooseDialog.ShowDialog() == CommonFileDialogResult.Ok)
                SaveFolderPathTextBox.Text = folderChooseDialog.FileName;
        }

        private void TemplateBaseNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string templateBaseFileName = TemplateBaseNameTextBox.Text;
            if (templateBaseFileName.Length > MAX_EXAMPLE_FILENAME_LENGTH)
                templateBaseFileName = templateBaseFileName.Substring(0, MAX_EXAMPLE_FILENAME_LENGTH) + "...";
            TemplateNameExampleTextBlock.Text = templateExampleBaseText + $"{templateBaseFileName}-1.pdf, {templateBaseFileName}-2.pdf, ...";
        }
    }
}
