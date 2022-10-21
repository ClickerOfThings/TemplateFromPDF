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
    public partial class SaveToTemplateFIOWindow : Window
    {
        private readonly List<PDFFile> pdfFilesToSave;

        private static readonly iText.Kernel.Colors.Color DEFAULT_COLOR = new iText.Kernel.Colors.DeviceRgb(138, 83, 10);
        private static readonly ParagraphWrap[] DEFAULT_PARAGRAPH_WRAPS = new ParagraphWrap[]
        {
            new ParagraphWrap{ TextWithFieldsFormatted = "О ПУБЛИКАЦИИ СТАТЬИ", Color = DEFAULT_COLOR, FontSize = 15},
            new ParagraphWrap{ TextWithFieldsFormatted = "{Название статьи}", Color = DEFAULT_COLOR, IsBold = true},
            new ParagraphWrap{ TextWithFieldsFormatted = "{Выберите издание}", Color = DEFAULT_COLOR, FontSize = 15},
            new ParagraphWrap{ TextWithFieldsFormatted = "выдано", Color = DEFAULT_COLOR},
            new ParagraphWrap{ TextWithFieldsFormatted = "{Фамилия Имя Отчество}", Color = DEFAULT_COLOR, IsBold = true },
            new ParagraphWrap{ TextWithFieldsFormatted = "{Должность}", Color = DEFAULT_COLOR, IsBold = true, FontSize = 15 },
            new ParagraphWrap{ TextWithFieldsFormatted = "{Учебное заведение} {Населённый пункт}", Color = DEFAULT_COLOR, FontSize = 15 },
        };
        private const string DEFAULT_TEMPLATE_IMAGE_FILENAME = "сертификат.png";
        private const float DEFAULT_TOP_MARGIN = 220f;
        private const float DEFAULT_SIDES_MARGIN = 250f;

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
            string folderPath = SaveFolderPathTextBox.Text.Trim();

            if (string.IsNullOrEmpty(folderPath))
            {
                MessageBox.Show("Не введен путь к папке.",
                    "Ошибка создания", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int fileCounter = 0;
            foreach (PDFFile file in pdfFilesToSave)
            {
                file.SaveFieldsToTemplate(DEFAULT_TEMPLATE_IMAGE_FILENAME,
                    DEFAULT_TOP_MARGIN, DEFAULT_SIDES_MARGIN,
                    DEFAULT_PARAGRAPH_WRAPS,
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
    }
}
