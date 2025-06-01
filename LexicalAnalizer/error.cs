// Error.cs
using LexicalAnalizer;
using System;

namespace LexicalAnalizer
{
    public static class Error
    {
        public static void _Error(string msg, int p)
        {
            while (Text.ch != Text.chEOL && Text.ch != Text.chEOT)
            {
                Text.NextCh();
            }
            Console.WriteLine();
            Console.WriteLine(new string(' ', p) + "^");

            Console.WriteLine(msg);
            Console.WriteLine("\n\n\n\n\n");
            Environment.Exit(1);
        }

        public static void LexError(string msg)
        {
            _Error(msg, Loc.pos);
        }

        public static void Expect(string msg)
        {
            _Error("Ожидается " + msg, Loc.lexPos);
        }

        public static void CtxError(string msg)
        {
            _Error(msg, Loc.lexPos);
        }

        public static void Error1(string msg)
        {
            Console.WriteLine();
            Console.WriteLine(msg);
            Environment.Exit(2);
        }

        public static void Warning(string msg)
        {
            Console.WriteLine();
            Console.WriteLine(msg);
        }
    }
}