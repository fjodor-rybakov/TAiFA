using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Compiler.LLSyntaxer
{
    public class Runner
    {
        private List<int> _indexStack = new List<int>();
        private List<NodeInfo> _table;
        private string _currLexem;
        private int _currentTableIndex;
        private int _currentWordIndex;
        private const string PATH_WORDS = @"../../../files/generator/sentence.txt";
        private readonly List<string> _words = new List<string>();

        public Runner(List<NodeInfo> table)
        {
            _table = table;
        }
        
        public bool Run()
        {
            _currentTableIndex = 0;
            _currentWordIndex = 0;
            ReadWords();
            _currLexem = _words[_currentWordIndex];
            return CheckWords();
        }

        private void ReadWords()
        {
            var reader = new StreamReader(PATH_WORDS);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var lexems = line.Split(' ');
                _words.AddRange(lexems);
            }
            _words.Add("[end]");
        }
        private bool CanProcessRow()
        {
            return _table[ _currentTableIndex ].DirSet.Contains(_currLexem) || ( _currLexem == "[end]" && _table[ _currentTableIndex ].DirSet.Count == 0 );
        }
        
        private bool CheckWords()
        {
            if ( CanProcessRow() )  // проверяем можно ли обрабатывать строку в таблице
            {
                ShiftIfEnabled();
                PushToStackIfEnabled();
                if ( _table[ _currentTableIndex ].GoTo == -1 && _indexStack.Count > 0 )  // переходим по стеку, если нельзя по goto
                {
                    _currentTableIndex = _indexStack.Last();
                    _indexStack.RemoveAt( _indexStack.Count - 1 );
                    return CheckWords();
                }
                if (_table[ _currentTableIndex ].GoTo != -1 )  // переходим по goto
                {
                    _currentTableIndex = _table[ _currentTableIndex ].GoTo;
                    return CheckWords();
                }
                return _indexStack.Count == 0 && _table[ _currentTableIndex ].IsEnd;
            }
            if (_table[ _currentTableIndex ].IfErrGoTo != -1 )  // переходим по onError, если возможно и нельзя обработать строку
            {
                _currentTableIndex = _table[ _currentTableIndex ].IfErrGoTo;
                return CheckWords();
            }
            return false;
        }
        
        private void ShiftIfEnabled()
        {
            if (!_table[ _currentTableIndex ].IsShift )
            {
                return;
            }

            _currentWordIndex++;
            if (_currentWordIndex > _words.Count - 1)
            {
                Console.WriteLine(_currentWordIndex);
                _currLexem = null;
                return;
            }
            _currLexem = _words[_currentWordIndex];
        }

        private void PushToStackIfEnabled()
        {
            if (!_table[ _currentTableIndex ].IsStack)
            {
                return;
            }
            _indexStack.Add( _currentTableIndex + 1 );
        }

    }
}