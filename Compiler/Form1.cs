using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
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

            string keywords = @"\b(num|text|check|otherwise|repeat|until)\b";
            string identifiers = @"\b[a-zA-Z][a-zA-Z0-9]*\b";
            string numbers = @"\b\d+(\.\d+)?\b";
            string str = @"""([^""\\]|\\.)*""";

            string operators = @":=|==|!=|<|>|\+|\-|\*|\/";
            string symbols = @"[;{},()]";
            string comments = @"/\*([^*]|\*+[^*/])*\*/";

            string masterPattern =
                $"{comments}|{str}|{keywords}|{numbers}|{operators}|{symbols}|{identifiers}";

            MatchCollection matches = Regex.Matches(code, masterPattern);

            List<Token> tokens = new List<Token>();

            int lastIndex = 0;

            foreach (Match match in matches)
            {
                if (match.Index > lastIndex)
                {
                    string garbage = code.Substring(lastIndex, match.Index - lastIndex).Trim();

                    if (!string.IsNullOrWhiteSpace(garbage))
                    {
                        throw new Exception("Invalid token: " + garbage);
                    }
                }

                lastIndex = match.Index + match.Length;

                string lex = match.Value;
                string type = "";

                if (Regex.IsMatch(lex, comments))
                    continue; 

                else if (Regex.IsMatch(lex, str))
                    type = "String";

                else if (Regex.IsMatch(lex, keywords))
                    type = "Keyword";

                else if (Regex.IsMatch(lex, numbers))
                    type = "Number";

                else if (lex == ":=")
                    type = "Assignment_Op";

                else if (lex == "==")
                    type = "Equal_Op";

                else if (lex == "!=")
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
                    throw new Exception("Unknown token: " + lex);

                table.Rows.Add(lex, type);

                tokens.Add(new Token { Value = lex, Type = type });
            }

            if (lastIndex < code.Length)
            {
                string garbage = code.Substring(lastIndex).Trim();

                if (!string.IsNullOrWhiteSpace(garbage))
                {
                    throw new Exception("Invalid token: " + garbage);
                }
            }

            dataGridView1.DataSource = table;

            try
            {
                if (tokens.Count == 0) return;

                MiniLParser parser = new MiniLParser(tokens);
                parser.ParseProgram();

                MessageBox.Show(
                    "Success: Your miniL code is correct",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Syntax Error:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}