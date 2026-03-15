using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string code = textBox1.Text;

            DataTable table = new DataTable();
            table.Columns.Add("Lexeme");
            table.Columns.Add("Token");

            string keywords = @"\b(int|float|string|read|write|repeat|until|if|elseif|else|then|return|endl|main)\b";
            string identifiers = @"\b[a-zA-Z][a-zA-Z0-9]*\b";
            string numbers = @"\b\d+(\.\d+)?\b";
            string str = @"""([^""\\]|\\.)*""";
            string funcCall = @"[a-zA-Z][a-zA-Z0-9]*\(\s*([a-zA-Z][a-zA-Z0-9]*(\s*,\s*[a-zA-Z][a-zA-Z0-9]*)*)?\s*\)";
            string operators = @":=|<>|<|>|=|\+|\-|\*|\/|&&|\|\|";
            string symbols = @"[;{},()]";
            string comments = @"/\*([^*]|\*+[^*/])*\*/";

            string masterPattern = $"{comments}|{str}|{funcCall}|{keywords}|{numbers}|{operators}|{symbols}|{identifiers}";

            MatchCollection matches = Regex.Matches(code, masterPattern);

            foreach (Match match in matches)
            {
                string lex = match.Value;
                string type = "";

                if (Regex.IsMatch(lex, comments))
                    type = "Comment";

                else if (Regex.IsMatch(lex, str))
                    type = "String";

                else if (Regex.IsMatch(lex, funcCall))
                    type = "Function_Call";

                else if (Regex.IsMatch(lex, keywords))
                    type = "Keyword";

                else if (Regex.IsMatch(lex, numbers))
                    type = "Number";

                else if (lex == ":=")
                    type = "Assignment_Op";

                else if (lex == "=")
                    type = "Equal_Op";

                else if (lex == "<>")
                    type = "NotEqual_Op";

                else if (lex == "<")
                    type = "Less_Than_Op";

                else if (lex == ">")
                    type = "Greater_Than_Op";

                else if (lex == "+")
                    type = "Plus_Op";

                else if (lex == "-")
                    type = "Minus_Op";

                else if (lex == "*")
                    type = "Multiply_Op";

                else if (lex == "/")
                    type = "Divide_Op";

                else if (lex == "&&")
                    type = "AND_Op";

                else if (lex == "||")
                    type = "OR_Op";

                else if (lex == ";")
                    type = "Semicolon";

                else if (lex == ",")
                    type = "Comma";

                else if (lex == "(")
                    type = "Left_Paren";

                else if (lex == ")")
                    type = "Right_Paren";

                else if (lex == "{")
                    type = "Left_Brace";

                else if (lex == "}")
                    type = "Right_Brace";

                else if (Regex.IsMatch(lex, identifiers))
                    type = "Identifier";

                else
                    type = "Unknown";

                table.Rows.Add(lex, type);
            }

            dataGridView1.DataSource = table;
        }
    }
}