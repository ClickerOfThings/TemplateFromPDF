using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Forms.Xfdf;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Org.BouncyCastle.Asn1;

namespace TemplateFromPDF.Model
{
    public class PDFFile
    {
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
    }
}
