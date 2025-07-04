﻿// Text.cs
using System;
using System.Data.Common;
using System.IO;

namespace LexicalAnalizer
{
    public static class Text
    {
        public const char chEOT = '\0';
        public const char chEOL = '\n';
        public const char chSPACE = ' ';
        public const char chTAB = '\t';

        private static string _scr = "";
        private static int _i = 0;
        public static char ch = '\0';
        private static int _currentLine = 1;
        private static int _column = 1;
        private static int _carriageReturns = 0;
        public static int GetCurrentColumn() => _column;

        public static void Reset()
        {
            _currentLine = 1;
            var args = Environment.GetCommandLineArgs();
            if (args.Length < 2)
            {
                Error.Error1("Запуск: Lexer <файл программы>");
            }
            else
            {
                try
                {
                    //_scr = File.ReadAllText(args[1]);
                    _scr = File.ReadAllText(args[1]).Replace("\r", "");

                }
                catch
                {
                    Error.Error1("Ошибка открытия файла");
                }
            }
        }

        
        public static void NextCh()
        {
            if (_i < _scr.Length)
            {
                ch = _scr[_i];
                Console.Write(ch);
                Loc.pos++;
                _i++;
                if (ch == '\n' || ch == '\r')
                {
                    ch = chEOL;
                    Loc.pos = 0;
                    _currentLine++;
                    _column = 1;
                }
                else
                {
                    _column++;
                }

            }
            else
            {
                ch = chEOT;
            }
        }

        public static int GetCurrentLine()
        {
            return _currentLine - _carriageReturns;           
        }
    }
}