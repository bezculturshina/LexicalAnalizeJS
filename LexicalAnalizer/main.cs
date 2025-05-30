// Program.cs
using System;

namespace LexicalAnalizer
{
    class Program
    {
        static void Init()
        {
            Text.Reset();
            // Clear statistics on each new run
            Lexer.lexemeCounts.Clear();
            Lexer.totalLexemes = 0;
            Lexer.identifiersInfo.Clear();
        }

        static void Done()
        {
        }

        static void Main(string[] args)
        {
            Init();
            Scanner.ScanText();
            Done();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Done");
        }
    }
}
