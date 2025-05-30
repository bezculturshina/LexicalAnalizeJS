// Lex.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace LexicalAnalizer
{
    public enum Lex
    {
        BREAK, CASE, CATCH, CLASS, CONST, CONTINUE, DEBUGGER, DEFAULT, DELETE, DO, ELSE, EXPORT, EXTENDS, FINALLY, FOR,
        FUNCTION, IF, IMPORT, IN, INSTANCEOF, LET, NEW, RETURN, SUPER, SWITCH, THIS, THROW, TRY, TYPEOF, VAR, VOID, WHILE,
        WITH, YIELD, TRUE, FALSE, NULL, UNDEFINED, ABSTRACT, BOOLEAN, BYTE, CHAR, DOUBLE, FINAL, FLOAT, GOTO, INT, LONG,
        NATIVE, SHORT, SYNCHRONIZED, THROWS, TRANSIENT, VOLATILE,
        ENUM, IMPLEMENTS, INTERFACE, PACKAGE, PRIVATE, PROTECTED, PUBLIC, STATIC,
        CONSOLE, ARGUMENTS, EVAL, NAME, EOT,
        PLUS, MINUS, MULTIPLY, DIVIDE, MODULO, INCREMENT, DECREMENT, ASSIGN, PLUS_ASSIGN, MINUS_ASSIGN, MULTIPLY_ASSIGN, DIVIDE_ASSIGN, MODULO_ASSIGN,
        EQUAL, STRICT_EQUAL, NOT_EQUAL, STRICT_NOT_EQUAL, GREATER, LESS, GREATER_EQUAL, LESS_EQUAL,
        LOGICAL_AND, LOGICAL_OR, LOGICAL_NOT, BITWISE_AND, BITWISE_OR, BITWISE_XOR, BITWISE_NOT, LEFT_SHIFT, RIGHT_SHIFT, UNSIGNED_RIGHT_SHIFT,
        QUESTION, COLON, SEMICOLON, COMMA, DOT, ARROW,
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE, LEFT_BRACKET, RIGHT_BRACKET, STR, NUM
    }

    public static class Lexer
    {
        private static readonly Dictionary<string, Lex> _kw = new Dictionary<string, Lex>
    {
        {"break", Lex.BREAK},
        {"case", Lex.CASE},
        {"catch", Lex.CATCH},
        {"class", Lex.CLASS},
        {"const", Lex.CONST},
        {"continue", Lex.CONTINUE},
        {"debugger", Lex.DEBUGGER},
        {"default", Lex.DEFAULT},
        {"delete", Lex.DELETE},
        {"do", Lex.DO},
        {"else", Lex.ELSE},
        {"export", Lex.EXPORT},
        {"extends", Lex.EXTENDS},
        {"finally", Lex.FINALLY},
        {"for", Lex.FOR},
        {"function", Lex.FUNCTION},
        {"if", Lex.IF},
        {"import", Lex.IMPORT},
        {"in", Lex.IN},
        {"instanceof", Lex.INSTANCEOF},
        {"let", Lex.LET},
        {"new", Lex.NEW},
        {"return", Lex.RETURN},
        {"super", Lex.SUPER},
        {"switch", Lex.SWITCH},
        {"this", Lex.THIS},
        {"throw", Lex.THROW},
        {"try", Lex.TRY},
        {"typeof", Lex.TYPEOF},
        {"var", Lex.VAR},
        {"void", Lex.VOID},
        {"while", Lex.WHILE},
        {"with", Lex.WITH},
        {"yield", Lex.YIELD},
        {"true", Lex.TRUE},
        {"false", Lex.FALSE},
        {"null", Lex.NULL},
        {"undefined", Lex.UNDEFINED},
        {"abstract", Lex.ABSTRACT},
        {"boolean", Lex.BOOLEAN},
        {"byte", Lex.BYTE},
        {"char", Lex.CHAR},
        {"double", Lex.DOUBLE},
        {"final", Lex.FINAL},
        {"float", Lex.FLOAT},
        {"goto", Lex.GOTO},
        {"int", Lex.INT},
        {"long", Lex.LONG},
        {"native", Lex.NATIVE},
        {"short", Lex.SHORT},
        {"synchronized", Lex.SYNCHRONIZED},
        {"throws", Lex.THROWS},
        {"transient", Lex.TRANSIENT},
        {"volatile", Lex.VOLATILE},
        {"enum", Lex.ENUM},
        {"implements", Lex.IMPLEMENTS},
        {"interface", Lex.INTERFACE},
        {"package", Lex.PACKAGE},
        {"private", Lex.PRIVATE},
        {"protected", Lex.PROTECTED},
        {"public", Lex.PUBLIC},
        {"static", Lex.STATIC},
        {"console", Lex.CONSOLE},
        {"arguments", Lex.ARGUMENTS},
        {"eval", Lex.EVAL},
        {"+", Lex.PLUS},
        {"-", Lex.MINUS},
        {"*", Lex.MULTIPLY},
        {"/", Lex.DIVIDE},
        {"%", Lex.MODULO},
        {"++", Lex.INCREMENT},
        {"--", Lex.DECREMENT},
        {"=", Lex.ASSIGN},
        {"+=", Lex.PLUS_ASSIGN},
        {"-=", Lex.MINUS_ASSIGN},
        {"*=", Lex.MULTIPLY_ASSIGN},
        {"/=", Lex.DIVIDE_ASSIGN},
        {"%=", Lex.MODULO_ASSIGN},
        {"==", Lex.EQUAL},
        {"===", Lex.STRICT_EQUAL},
        {"!=", Lex.NOT_EQUAL},
        {"!==", Lex.STRICT_NOT_EQUAL},
        {">", Lex.GREATER},
        {"<", Lex.LESS},
        {">=", Lex.GREATER_EQUAL},
        {"<=", Lex.LESS_EQUAL},
        {"&&", Lex.LOGICAL_AND},
        {"||", Lex.LOGICAL_OR},
        {"!", Lex.LOGICAL_NOT},
        {"&", Lex.BITWISE_AND},
        {"|", Lex.BITWISE_OR},
        {"^", Lex.BITWISE_XOR},
        {"~", Lex.BITWISE_NOT},
        {"<<", Lex.LEFT_SHIFT},
        {">>", Lex.RIGHT_SHIFT},
        {">>>", Lex.UNSIGNED_RIGHT_SHIFT},
        {"?", Lex.QUESTION},
        {":", Lex.COLON},
        {";", Lex.SEMICOLON},
        {",", Lex.COMMA},
        {".", Lex.DOT},
        {"=>", Lex.ARROW},
        {"(", Lex.LEFT_PAREN},
        {")", Lex.RIGHT_PAREN},
        {"{", Lex.LEFT_BRACE},
        {"}", Lex.RIGHT_BRACE},
        {"[", Lex.LEFT_BRACKET},
        {"]", Lex.RIGHT_BRACKET}
    };

        public static Lex currentLex;
        public static double num;
        public static string name = "";
        public static readonly long maxint = (long)Math.Pow(2, 53) - 1;

        // Statistics
        public static Dictionary<Lex, int> lexemeCounts = new Dictionary<Lex, int>();
        public static int totalLexemes = 0;
        public static List<IdentifierInfo> identifiersInfo = new List<IdentifierInfo>();

        public class IdentifierInfo
        {
            public string Name { get; set; }
            public string File { get; set; }
            public int Line { get; set; }
        }

        public static void PrintStatistics()
        {
            Console.WriteLine("\nСтатистика лексем:");
            Console.WriteLine("{0,-30} {1,-15} {2,-15}", "Лексема", "Абсолютная частота", "Относительная частота");

            var sortedLexemes = lexemeCounts.OrderByDescending(x => x.Value);
            foreach (var kvp in sortedLexemes)
            {
                double relative = (totalLexemes > 0) ? (kvp.Value * 100.0 / totalLexemes) : 0;
                Console.WriteLine("{0,-30} {1,-15} {2,-15:F2}%", kvp.Key, kvp.Value, relative);
            }

            Console.WriteLine("\nСписок идентификаторов:");
            Console.WriteLine("{0,-20} {1,-20} {2,-10}", "Имя", "Файл", "Строка");

            var sortedIdentifiers = identifiersInfo.OrderBy(x => x.Name).ThenBy(x => x.File).ThenBy(x => x.Line);
            foreach (var info in sortedIdentifiers)
            {
                Console.WriteLine("{0,-20} {1,-20} {2,-10}", info.Name, info.File, info.Line);
            }
        }

        public static Lex LexName()
        {
            name = Text.ch.ToString();
            Text.NextCh();
            while (char.IsLetterOrDigit(Text.ch))
            {
                name += Text.ch;
                Text.NextCh();
            }

            if (_kw.TryGetValue(name, out Lex lexValue))
            {
                currentLex = lexValue;
            }
            else
            {
                currentLex = Lex.NAME;
            }
            return currentLex;
        }

        public static Lex ScanNumber()
        {
            num = 0;
            while (char.IsDigit(Text.ch) || Text.ch == '.')
            {
                int d = Text.ch - '0';
                if (num > (maxint - d) / 10)
                {
                    Error.LexError("Слишком большое число");
                }
                else
                {
                    num = num * 10 + d;
                }
                Text.NextCh();
            }
            currentLex = Lex.NUM;
            return currentLex;
        }

        public static void SingleComment()
        {
            Text.NextCh();
            while (Text.ch != Text.chEOL)
            {
                Text.NextCh();
                if (Text.ch == '/')
                {
                    Text.NextCh();
                    if (Text.ch == '*')
                    {
                        Comment();
                    }
                }
            }
            Text.NextCh();
        }

        public static void Comment()
        {
            Text.NextCh(); // Skip *
            while (true)
            {
                while (Text.ch != '*' && Text.ch != Text.chEOT)
                {
                    if (Text.ch == '/')
                    {
                        Text.NextCh();
                        if (Text.ch == '*')
                        {
                            Comment();
                        }
                    }
                    else
                    {
                        Text.NextCh();
                    }
                }
                if (Text.ch == Text.chEOT)
                {
                    Error.LexError("Не закончен комментарий");
                }
                else
                {
                    Text.NextCh();
                }
                if (Text.ch == '/') break;
            }
            Text.NextCh();
        }

        public static void JString(char simb)
        {
            Text.NextCh(); // Skip opening quote
            while (Text.ch != simb && Text.ch != Text.chEOT)
            {
                Text.NextCh();
            }
            if (Text.ch == simb)
            {
                Text.NextCh(); // Skip closing quote
            }
            else
            {
                Error.LexError("Незакрытая строка");
            }
        }

        public static Lex NextLex()
        {
            while (Text.ch == Text.chSPACE || Text.ch == Text.chTAB || Text.ch == Text.chEOL)
            {
                Text.NextCh();
            }

            if (char.IsLetter(Text.ch))
            {
                currentLex = LexName();
            }
            else if (char.IsDigit(Text.ch))
            {
                currentLex = ScanNumber();
            }
            else if (Text.ch == ';')
            {
                currentLex = Lex.SEMICOLON;
                Text.NextCh();
            }
            else if (Text.ch == ':')
            {
                currentLex = Lex.COLON;
                Text.NextCh();
            }
            else if (Text.ch == '.')
            {
                currentLex = Lex.DOT;
                Text.NextCh();
            }
            else if (Text.ch == ',')
            {
                currentLex = Lex.COMMA;
                Text.NextCh();
            }
            else if (Text.ch == '+')
            {
                Text.NextCh();
                if (Text.ch == '=')
                {
                    currentLex = Lex.PLUS_ASSIGN;
                    Text.NextCh();
                }
                else if (Text.ch == '+')
                {
                    currentLex = Lex.INCREMENT;
                    Text.NextCh();
                }
                else
                {
                    currentLex = Lex.PLUS;
                }
            }
            else if (Text.ch == '-')
            {
                Text.NextCh();
                if (Text.ch == '=')
                {
                    currentLex = Lex.MINUS_ASSIGN;
                    Text.NextCh();
                }
                else if (Text.ch == '-')
                {
                    currentLex = Lex.DECREMENT;
                    Text.NextCh();
                }
                else
                {
                    currentLex = Lex.MINUS;
                }
            }
            else if (Text.ch == '*')
            {
                Text.NextCh();
                if (Text.ch == '=')
                {
                    currentLex = Lex.MULTIPLY_ASSIGN;
                    Text.NextCh();
                }
                else
                {
                    currentLex = Lex.MULTIPLY;
                }
            }
            else if (Text.ch == '/')
            {
                Text.NextCh();
                if (Text.ch == '=')
                {
                    currentLex = Lex.DIVIDE_ASSIGN;
                    Text.NextCh();
                }
                else if (Text.ch == '/')
                {
                    SingleComment();
                    return NextLex();
                }
                else if (Text.ch == '*')
                {
                    Comment();
                    return NextLex();
                }
                else
                {
                    currentLex = Lex.DIVIDE;
                }
            }
            else if (Text.ch == '%')
            {
                Text.NextCh();
                if (Text.ch == '=')
                {
                    currentLex = Lex.MODULO_ASSIGN;
                    Text.NextCh();
                }
                else
                {
                    currentLex = Lex.MODULO;
                }
            }
            else if (Text.ch == '=')
            {
                Text.NextCh();
                if (Text.ch == '=')
                {
                    Text.NextCh();
                    if (Text.ch == '=')
                    {
                        currentLex = Lex.STRICT_EQUAL;
                        Text.NextCh();
                    }
                    else
                    {
                        currentLex = Lex.EQUAL;
                    }
                }
                else if (Text.ch == '>')
                {
                    currentLex = Lex.ARROW;
                }
                else
                {
                    currentLex = Lex.ASSIGN;
                }
            }
            else if (Text.ch == '!')
            {
                Text.NextCh();
                if (Text.ch == '=')
                {
                    Text.NextCh();
                    if (Text.ch == '=')
                    {
                        currentLex = Lex.STRICT_NOT_EQUAL;
                        Text.NextCh();
                    }
                    else
                    {
                        currentLex = Lex.NOT_EQUAL;
                    }
                }
                else
                {
                    currentLex = Lex.LOGICAL_NOT;
                }
            }
            else if (Text.ch == '>')
            {
                Text.NextCh();
                if (Text.ch == '=')
                {
                    currentLex = Lex.GREATER_EQUAL;
                    Text.NextCh();
                }
                else if (Text.ch == '>')
                {
                    Text.NextCh();
                    if (Text.ch == '>')
                    {
                        currentLex = Lex.UNSIGNED_RIGHT_SHIFT;
                        Text.NextCh();
                    }
                    else
                    {
                        currentLex = Lex.RIGHT_SHIFT;
                    }
                }
                else
                {
                    currentLex = Lex.GREATER;
                }
            }
            else if (Text.ch == '<')
            {
                Text.NextCh();
                if (Text.ch == '=')
                {
                    currentLex = Lex.LESS_EQUAL;
                    Text.NextCh();
                }
                else if (Text.ch == '<')
                {
                    currentLex = Lex.LEFT_SHIFT;
                    Text.NextCh();
                }
                else
                {
                    currentLex = Lex.LESS;
                }
            }
            else if (Text.ch == '&')
            {
                Text.NextCh();
                if (Text.ch == '&')
                {
                    currentLex = Lex.LOGICAL_AND;
                    Text.NextCh();
                }
                else
                {
                    currentLex = Lex.BITWISE_AND;
                }
            }
            else if (Text.ch == '|')
            {
                Text.NextCh();
                if (Text.ch == '|')
                {
                    currentLex = Lex.LOGICAL_OR;
                    Text.NextCh();
                }
                else
                {
                    currentLex = Lex.BITWISE_OR;
                }
            }
            else if (Text.ch == '^')
            {
                currentLex = Lex.BITWISE_XOR;
                Text.NextCh();
            }
            else if (Text.ch == '~')
            {
                currentLex = Lex.BITWISE_NOT;
                Text.NextCh();
            }
            else if (Text.ch == '?')
            {
                currentLex = Lex.QUESTION;
                Text.NextCh();
            }
            else if (Text.ch == '(')
            {
                currentLex = Lex.LEFT_PAREN;
                Text.NextCh();
            }
            else if (Text.ch == ')')
            {
                currentLex = Lex.RIGHT_PAREN;
                Text.NextCh();
            }
            else if (Text.ch == '{')
            {
                currentLex = Lex.LEFT_BRACE;
                Text.NextCh();
            }
            else if (Text.ch == '}')
            {
                currentLex = Lex.RIGHT_BRACE;
                Text.NextCh();
            }
            else if (Text.ch == '[')
            {
                currentLex = Lex.LEFT_BRACKET;
                Text.NextCh();
            }
            else if (Text.ch == ']')
            {
                currentLex = Lex.RIGHT_BRACKET;
                Text.NextCh();
            }
            else if (Text.ch == '"' || Text.ch == '\'' || Text.ch == '`')
            {
                JString(Text.ch);
                currentLex = Lex.STR;
            }
            else if (Text.ch == Text.chEOT)
            {
                currentLex = Lex.EOT;
            }
            else
            {
                Error.LexError("Недопустимый символ");
            }

            // Collect statistics
            if (currentLex != Lex.EOT)
            {
                if (lexemeCounts.ContainsKey(currentLex))
                {
                    lexemeCounts[currentLex]++;
                }
                else
                {
                    lexemeCounts[currentLex] = 1;
                }
                totalLexemes++;

                // If it's an identifier, save information about it
                if (currentLex == Lex.NAME)
                {
                    int lineNum = Text.GetCurrentLine();
                    string fileName = Environment.GetCommandLineArgs().Length > 1 ? Environment.GetCommandLineArgs()[1] : "unknown";
                    identifiersInfo.Add(new IdentifierInfo
                    {
                        Name = name,
                        File = fileName,
                        Line = lineNum
                    });
                }
                Console.WriteLine(currentLex);
            }
            
            return currentLex;
        }
    }
}