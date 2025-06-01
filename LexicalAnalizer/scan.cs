// Scanner.cs
using System;

namespace LexicalAnalizer
{
    public static class Scanner
    {
        public static void ScanText()
        {
            Text.NextCh();
            Lexer.NextLex();
            int n = 0;
            while (Lexer.currentLex != Lex.EOT)
            {
                Loc.lexPos = Loc.pos; // <-- сохраняем позицию начала лексемы
                Lexer.NextLex();
                n++;
            }

            Console.WriteLine();
            Console.WriteLine("Число лексем " + n);

            // Print statistics
            Lexer.PrintStatistics();
        }
    }
}