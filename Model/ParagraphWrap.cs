using iText.Kernel.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateFromPDF.Model
{
    /// <summary>
    /// Класс-обёртка для <see cref="iText.Layout.Element.Paragraph"/> с минимальной стилизацией
    /// </summary>
    public class ParagraphWrap
    {
        /// <summary>
        /// Текст, который будет вставлен напрямую в файл
        /// </summary>
        public string DirectText { get; set; } = "";

        /// <summary>
        /// Ключ поля из класса <see cref="PDFFile"/>, значение которого будет вставлено в файл
        /// </summary>
        public string FieldKey { get; set; } = "";

        /// <summary>
        /// Будет ли шрифт жирным
        /// </summary>
        public bool IsBold { get; set; } = false;

        /// <summary>
        /// Размер шрифта
        /// </summary>
        public float FontSize { get; set; } = 14;

        /// <summary>
        /// Цвет шрифта
        /// </summary>
        public iText.Kernel.Colors.Color Color { get; set; } = ColorConstants.BLACK;

        /// <summary>
        /// Множитель пространства между текущим и следующим параграфом
        /// </summary>
        public float AfterSpacingMultiplier { get; set; } = 1.0f; 
    }
}
