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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TemplateFromPDF.Model;
using MessageBox = System.Windows.MessageBox;

namespace TemplateFromPDF.Windows
{
    /// <summary>
    /// Interaction logic for SaveToTemplateWindow.xaml
    /// </summary>
    public partial class SaveToTemplateFIOWindow : Window
    {
        private readonly List<PDFFile> pdfFilesToSave;

        public SaveToTemplateFIOWindow(List<PDFFile> pdfFilesToSave)
        {
            InitializeComponent();

            this.pdfFilesToSave = pdfFilesToSave;
        }

        private void BackToPDFFieldsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveToTemplatesButton_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = SaveFolderPathTextBox.Text.Trim(),
                   templateImgFilename = TemplateImageFileTextBox.Text.Trim();

            if (string.IsNullOrEmpty(folderPath)
                || string.IsNullOrEmpty(templateImgFilename))
            {
                MessageBox.Show("Введены не все данные. Для работы программы необходимо внести все данные в окне.",
                    "Ошибка создания", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!System.IO.File.Exists(templateImgFilename))
            {
                MessageBox.Show("Указанный файл изображения-шаблона не найден.",
                    "Ошибка создания", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int fileCounter = 0;
            foreach (PDFFile file in pdfFilesToSave)
            {

                file.SaveFieldsToTemplate(templateImgFilename,
                    PDFFile.DEFAULT_TOP_MARGIN, PDFFile.DEFAULT_SIDES_MARGIN,
                    PDFFile.DEFAULT_PARAGRAPH_WRAPS,
                    $"{folderPath}/{file.Fields["Фамилия Имя Отчество"]}-{fileCounter++}.pdf");
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

        private void BrowseImageTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openImgDialog = new OpenFileDialog();
            openImgDialog.Filter = "Файлы изображений (.png;.jpg;.jpeg)|*.png;*.jpg;*.jpeg|Все файлы|*.*";
            openImgDialog.Multiselect = false;

            if (openImgDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TemplateImageFileTextBox.Text = openImgDialog.FileName;
            }
        }
    }
}
