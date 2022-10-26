using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Converters;
using iText.Forms.Xfdf;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using Org.BouncyCastle.Asn1;

namespace TemplateFromPDF.Model
{
    /// <summary>
    /// Класс PDF файла с извлечёнными полями
    /// </summary>
    public class PDFFile
    {
        private const string DEFAULT_FONT = "/Fonts/FreeSans-LrmZ.ttf";

        public static readonly iText.Kernel.Colors.Color DEFAULT_COLOR = new iText.Kernel.Colors.DeviceRgb(138, 83, 10);
        public static readonly ParagraphWrap[] DEFAULT_PARAGRAPH_WRAPS = new ParagraphWrap[]
        {
            new ParagraphWrap{ TextWithFieldsFormatted = "ПОДТВЕРЖДАЕТ, ЧТО", Color = DEFAULT_COLOR, FontSize = 15},
            new ParagraphWrap{ TextWithFieldsFormatted = "{Фамилия Имя Отчество}", Color = DEFAULT_COLOR, IsBold = true},
            new ParagraphWrap{ TextWithFieldsFormatted = "{Должность}", Color = DEFAULT_COLOR, IsBold = true, FontSize = 15 },
            new ParagraphWrap{ TextWithFieldsFormatted = "{Учебное заведение} {Населённый пункт}", Color = DEFAULT_COLOR, FontSize = 15 },
            new ParagraphWrap{ TextWithFieldsFormatted = "ОПУБЛИКОВАЛ(А) СТАТЬЮ", Color = DEFAULT_COLOR, AfterSpacingMultiplier = 1.2f},
            new ParagraphWrap{ TextWithFieldsFormatted = "{Название статьи}", Color = DEFAULT_COLOR, IsBold = true, AfterSpacingMultiplier = 1.2f},
            new ParagraphWrap{ TextWithFieldsFormatted = "{Выберите издание}", Color = DEFAULT_COLOR, FontSize = 15},
        };
        //public const string DEFAULT_TEMPLATE_IMAGE_FILENAME = "сертификат.png";
        public const float DEFAULT_TOP_MARGIN = 220f;
        public const float DEFAULT_SIDES_MARGIN = 250f;

        /// <summary>
        /// Словарь полей, полученных из PDF файла
        /// </summary>
        public Dictionary<string, string> Fields { get; set; }

        /// <summary>
        /// Путь к файлу, из которого были извлечены поля
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Конструктор экземпляра PDFFile и заполнение словаря <see cref="Fields"/> напрямую из файла
        /// </summary>
        /// <param name="fileName">Путь к PDF файлу</param>
        public PDFFile(string fileName)
        {
            Fields = new Dictionary<string, string>();
            FilePath = fileName;

            PdfDocument docToParse;
            StringBuilder allTextFromPdf = new StringBuilder();
            using (PdfReader pdfReader = new PdfReader(fileName))
            {
                docToParse = new PdfDocument(pdfReader);

                int numOfPagesInDoc = docToParse.GetNumberOfPages();
                for (int i = 1; i <= numOfPagesInDoc; i++)
                {
                    PdfPage currentPage = docToParse.GetPage(i);

                    // TODO сделать тесты на PDF файлах, где есть более одной страницы,
                    // и решить по поводу использования Append или AppendLine
                    allTextFromPdf.Append(PdfTextExtractor.GetTextFromPage(currentPage));
                }
            }

            // блоки полей (заголовок и значение) в PDF файле, обычно разделяемые двойным newline-ом
            string[] fieldBlocks = allTextFromPdf.ToString().Split("\n \n");

            foreach (string fieldBlock in fieldBlocks)
            {
                // место в блоке поля, где заканчивается заголовок и начинается значение поля
                int fieldEndOfHeaderIndex = fieldBlock.IndexOf("\n");

                if (fieldEndOfHeaderIndex == -1)
                    continue;

                string fieldKey = fieldBlock.Substring(0, fieldEndOfHeaderIndex).Trim();
                string fieldValue = fieldBlock.Substring(fieldEndOfHeaderIndex + 1).Trim();
                // TODO решить, убирать newline-ы в значении поля, или нет

                if (string.IsNullOrEmpty(fieldKey) || string.IsNullOrEmpty(fieldValue))
                    continue;

                Fields.Add(fieldKey, fieldValue);
            }
        }

        /// <summary>
        /// Сохранение извлечённых полей в новый PDF файл
        /// </summary>
        /// <param name="imageTemplatePath">Путь к изображению, поверх которого будет наложен текст</param>
        /// <param name="marginTop">Отступ начала текста от верхнего края документа</param>
        /// <param name="marginRightAndLeft">Отступы от левого и правого краёв документа, ограничивает ширину текста</param>
        /// <param name="paragraphsToWrite">Массив объектов <see cref="ParagraphWrap"/> для описания стиля параграфа текста</param>
        /// <param name="outputFileName">Выходное название файла</param>
        public void SaveFieldsToTemplate(string imageTemplatePath,
                                         float marginTop, float marginRightAndLeft,
                                         ParagraphWrap[] paragraphsToWrite,
                                         string outputFileName)
        {
            using (PdfWriter pdfWriter = new PdfWriter(outputFileName))
            using (PdfDocument resultDoc = new PdfDocument(pdfWriter))
            {

                Document docToWrite = new Document(resultDoc, PageSize.A4);
                docToWrite.SetMargins(0, 0, 0, 0);

                ImageData templateImgData = ImageDataFactory.Create(imageTemplatePath);
                Image templateImage = new Image(templateImgData);
                templateImage.ScaleToFit(PageSize.A4.GetWidth(), PageSize.A4.GetHeight());

                docToWrite.Add(templateImage);

                PdfCanvas pdfCanvas = new PdfCanvas(resultDoc.GetFirstPage());
                var rectangle = resultDoc.GetFirstPage().GetPageSize();
                rectangle.DecreaseHeight(marginTop)
                    .DecreaseWidth(marginRightAndLeft)
                    .MoveRight(marginRightAndLeft / 2);
                Canvas canvas = new Canvas(pdfCanvas, rectangle);

                System.Windows.Resources.StreamResourceInfo fontStreamInfo = 
                    Application.GetResourceStream(new Uri(DEFAULT_FONT, UriKind.Relative));
                System.IO.Stream fontStream = fontStreamInfo.Stream;

                byte[] fontBytes = new byte[fontStream.Length];
                fontStream.Read(fontBytes, 0, fontBytes.Length);

                PdfFont fontUnicode = PdfFontFactory.CreateFont(fontBytes, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

                foreach (ParagraphWrap paragraph in paragraphsToWrite)
                {
                    StringBuilder fieldsInsertedStringBuilder = new StringBuilder(paragraph.TextWithFieldsFormatted);
                    MatchCollection fieldsMatches =
                        Regex.Matches(paragraph.TextWithFieldsFormatted, @"{.+?}");
                    foreach (Match fieldMatch in fieldsMatches)
                    {
                        string fieldHeader = fieldMatch.Value.Substring(1, fieldMatch.Value.Length - 2);
                        if (Fields.ContainsKey(fieldHeader))
                            fieldsInsertedStringBuilder.Replace(fieldMatch.Value, Fields[fieldHeader]);
                    }
                    string textToWrite = fieldsInsertedStringBuilder.ToString();

                    Paragraph templateFieldText = new Paragraph(textToWrite);
                    templateFieldText.SetFont(fontUnicode)
                        .SetFontSize(paragraph.FontSize)
                        .SetFontColor(paragraph.Color)
                        .SetMultipliedLeading(paragraph.AfterSpacingMultiplier);
                    if (paragraph.IsBold)
                        templateFieldText.SetBold();
                    templateFieldText.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);

                    canvas.Add(templateFieldText);
                }
                canvas.Close();
                docToWrite.Close();
            }
        }

        /// <summary>
        /// Создание нескольких экзепляров <see cref="PDFFile"/> из массива строк с путями
        /// </summary>
        /// <param name="pdfFilesPaths">Массив строк с путями PDF файлов</param>
        /// <returns>Список созданных экземпляров <see cref="PDFFile"/></returns>
        public static List<PDFFile> ParsePDFFiles(string[] pdfFilesPaths)
        {
            List<PDFFile> parsedPDFFiles = new List<PDFFile>();
            foreach (string pdfFilePath in pdfFilesPaths)
            {
                parsedPDFFiles.Add(new PDFFile(pdfFilePath));
            }
            return parsedPDFFiles;
        }
    }
}
