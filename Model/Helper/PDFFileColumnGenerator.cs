using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using TemplateFromPDF.Model;

namespace TemplateFromPDF.Model.Helper
{
    internal static class PDFFileColumnGenerator
    {
        public static List<DataGridTextColumn> GenerateColumnsFromPDFFiles(IEnumerable<PDFFile> pdfFilesToUse)
        {
            List<DataGridTextColumn> fieldsColumns = new List<DataGridTextColumn>();

            foreach (PDFFile pdfFile in pdfFilesToUse)
            {
                foreach (string fieldHeader in pdfFile.Fields.Keys)
                {
                    if (fieldsColumns.Any(x => (string)x.Header == fieldHeader))
                        continue;
                    DataGridTextColumn newFieldColumn = new DataGridTextColumn();
                    newFieldColumn.Header = fieldHeader;
                    newFieldColumn.Binding = new Binding($"[{fieldHeader}]");
                    fieldsColumns.Add(newFieldColumn);
                }
            }

            return fieldsColumns;
        }
    }
}
