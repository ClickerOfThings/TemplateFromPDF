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
        /// Форматированная строка с ключами полей класса <see cref="PDFFile"/>, которые должны быть заменены.
        /// </summary>
        /// <remarks>
        /// Формат строки: {Название-Поля}.
        /// В фигурных скобках вводится заголовок поля, который будет заменён значением из PDF файла, извлечённого 
        /// классом <see cref="PDFFile"/>. Данных конструкций в строке может быть несколько штук.
        /// <para/>
        /// Если не добавлять фигурные скобки, то текст из свойства вставится напрямую в шаблон.
        /// </remarks>
        /// <example>
        /// '{ФИО}' - будет заменено на 'Иванов Иван Иванович'
        /// '{ФИО} {Должность}' - будет заменено на 'Иванов Иван Иванович Директор'
        /// 'FooBar' - будет напрямую всталвен в шаблон (отсутствуют фигурные скобки)
        /// </example>
        public string TextWithFieldsFormatted { get; set; } = "";

        /// <summary>
        /// Будет ли шрифт жирным
        /// </summary>
        public bool IsBold { get; set; } = false;

        /// <summary>
        /// Размер шрифта
        /// </summary>
        public float FontSize { get; set; } = 18;

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
