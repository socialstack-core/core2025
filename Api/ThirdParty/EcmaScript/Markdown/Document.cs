using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Api.EcmaScript.Markdown
{
    /// <summary>
    /// Represents a Markdown document.
    /// </summary>
    public class Document
    {
        /// <summary>
        /// The document filename.
        /// </summary>
        public string FileName;

        /// <summary>
        /// The document content builder.
        /// </summary>
        private readonly StringBuilder DocumentBuilder = new();

        /// <summary>
        /// Adds a heading to the document (level 1 to 6).
        /// </summary>
        public void AddHeading(string text, int level = 1)
        {
            level = Math.Clamp(level, 1, 6);
            DocumentBuilder.AppendLine($"{new string('#', level)} {text}");
            DocumentBuilder.AppendLine();
        }

        /// <summary>
        /// Adds a paragraph to the document.
        /// </summary>
        public void AddParagraph(string text)
        {
            DocumentBuilder.AppendLine(text);
            DocumentBuilder.AppendLine();
        }

        /// <summary>
        /// Adds an inline code snippet.
        /// </summary>
        public void AddInlineCode(string code)
        {
            DocumentBuilder.Append($"`{code}`");
        }

        /// <summary>
        /// Adds a code block.
        /// </summary>
        public void AddCodeBlock(string code, string language = "")
        {
            DocumentBuilder.AppendLine($"```{language}");
            DocumentBuilder.AppendLine(code);
            DocumentBuilder.AppendLine("```");
            DocumentBuilder.AppendLine();
        }

        /// <summary>
        /// Adds a bullet list to the document.
        /// </summary>
        public void AddList(params string[] items)
        {
            foreach (var item in items)
            {
                DocumentBuilder.AppendLine($"- {item}");
            }
            DocumentBuilder.AppendLine();
        }

        /// <summary>
        /// Adds a horizontal rule.
        /// </summary>
        public void AddHorizontalRule()
        {
            DocumentBuilder.AppendLine("---");
            DocumentBuilder.AppendLine();
        }

        /// <summary>
        /// Appends raw markdown directly.
        /// </summary>
        public void AddRaw(string markdown)
        {
            DocumentBuilder.AppendLine(markdown);
        }

        /// <summary>
        /// Adds a markdown table.
        /// </summary>
        public void AddTable(string[] headers, List<string[]> rows)
        {
            int[] columnWidths = new int[headers.Length];

            for (int i = 0; i < headers.Length; i++)
            {
                columnWidths[i] = headers[i].Length;
            }

            foreach (var row in rows)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    columnWidths[i] = Math.Max(columnWidths[i], row[i]?.Length ?? 0);
                }
            }

            // Header
            for (int i = 0; i < headers.Length; i++)
            {
                DocumentBuilder.Append("| ");
                DocumentBuilder.Append(headers[i].PadRight(columnWidths[i]));
                DocumentBuilder.Append(" ");
            }
            DocumentBuilder.AppendLine("|");

            // Separator
            for (int i = 0; i < headers.Length; i++)
            {
                DocumentBuilder.Append("| ");
                DocumentBuilder.Append(new string('-', columnWidths[i]));
                DocumentBuilder.Append(" ");
            }
            DocumentBuilder.AppendLine("|");

            // Rows
            foreach (var row in rows)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    DocumentBuilder.Append("| ");
                    DocumentBuilder.Append((row[i] ?? "").PadRight(columnWidths[i]));
                    DocumentBuilder.Append(" ");
                }
                DocumentBuilder.AppendLine("|");
            }

            DocumentBuilder.AppendLine();
        }

        /// <summary>
        /// Returns the full markdown document as a string.
        /// </summary>
        public string GetMarkdown()
        {
            return DocumentBuilder.ToString();
        }

        /// <summary>
        /// Saves the markdown to the specified file.
        /// </summary>
        public void SaveToFile()
        {
            if (string.IsNullOrEmpty(FileName))
            {
                throw new InvalidOperationException("Filename not set.");
            }

            File.WriteAllText(FileName, GetMarkdown());
        }
    }
}
