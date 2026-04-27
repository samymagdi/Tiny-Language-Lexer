using System;
using System.Collections.Generic;

namespace Compiler
{
    public class MiniLParser
    {
        private List<Token> tokens;
        private int index = 0;
        private Token currentToken;

        public MiniLParser(List<Token> tokens)
        {
            this.tokens = tokens;

            if (tokens.Count > 0)
                currentToken = tokens[index];
            else
                throw new Exception("Empty program");
        }

        public int GetCurrentIndex()
        {
            return index;
        }

        private void Next()
        {
            index++;
            if (index < tokens.Count)
                currentToken = tokens[index];
        }

        private void Match(string expected)
        {
            if (index < tokens.Count &&
                (currentToken.Value == expected || currentToken.Type == expected))
            {
                Next();
            }
            else
            {
                throw new Exception($"Expected '{expected}' but found '{currentToken?.Value}'");
            }
        }

        public void ParseProgram()
        {
            ParseStatements();

            if (index < tokens.Count)
            {
                throw new Exception($"Unexpected token '{currentToken.Value}' after end of program");
            }
        }

        private void ParseStatements()
        {
            ParseStatement();

            if (index < tokens.Count)
            {
                if (currentToken.Value == ";")
                {
                    Match(";");

                    if (index < tokens.Count &&
                        currentToken.Value != "}" &&
                        currentToken.Value != "until")
                    {
                        ParseStatements();
                    }
                }
                else
                {
                    throw new Exception(
                        $"Missing semicolon after '{tokens[index - 1].Value}'"
                    );
                }
            }
        }

        private void ParseStatement()
        {
            if (index >= tokens.Count)
                throw new Exception("Unexpected end of input");

            if (currentToken.Value == "num" || currentToken.Value == "text")
            {
                ParseDeclaration();
            }
            else if (currentToken.Type == "Identifier")
            {
                ParseAssignment();
            }
            else if (currentToken.Value == "check")
            {
                ParseCheck();
            }
            else if (currentToken.Value == "repeat")
            {
                ParseRepeat();
            }
            else if (currentToken.Value == "{")
            {
                ParseBlock();
            }
            else
            {
                throw new Exception($"Invalid start of statement: '{currentToken.Value}'");
            }
        }

        private void ParseDeclaration()
        {
            if (currentToken.Value == "num") Match("num");
            else Match("text");

            ParseAssignment();
        }

        private void ParseAssignment()
        {
            Match("Identifier");
            Match(":=");
            ParseExpression();
        }

        private void ParseCheck()
        {
            Match("check");
            Match("(");
            ParseCondition();
            Match(")");
            ParseBlock();

            if (index < tokens.Count && currentToken.Value == "otherwise")
            {
                Match("otherwise");
                ParseBlock();
            }
        }

        private void ParseRepeat()
        {
            Match("repeat");
            ParseBlock();
            Match("until");
            Match("(");
            ParseCondition();
            Match(")");
        }

        private void ParseBlock()
        {
            if (currentToken.Value == "{")
            {
                Match("{");

                if (index < tokens.Count && currentToken.Value != "}")
                    ParseStatements();

                Match("}");
            }
            else
            {
                ParseStatement();
            }
        }

        private void ParseCondition()
        {
            ParseExpression();

            if (currentToken.Value == "<" || currentToken.Value == ">" ||
                currentToken.Value == "==" || currentToken.Value == "!=")
            {
                Match(currentToken.Value);
            }
            else
            {
                throw new Exception("Expected relational operator (<, >, ==, !=)");
            }

            ParseExpression();
        }

        private void ParseExpression()
        {
            ParseTerm();

            while (index < tokens.Count &&
                  (currentToken.Value == "+" || currentToken.Value == "-"))
            {
                Match(currentToken.Value);
                ParseTerm();
            }
        }

        private void ParseTerm()
        {
            ParseFactor();

            while (index < tokens.Count &&
                  (currentToken.Value == "*" || currentToken.Value == "/"))
            {
                Match(currentToken.Value);
                ParseFactor();
            }
        }

        private void ParseFactor()
        {
            if (currentToken.Type == "Identifier")
            {
                Match("Identifier");
            }
            else if (currentToken.Type == "Number")
            {
                Match("Number");
            }
            else if (currentToken.Value == "(")
            {
                Match("(");
                ParseExpression();
                Match(")");
            }
            else
            {
                throw new Exception($"Expected Identifier or Number but found '{currentToken.Value}'");
            }
        }
    }
}