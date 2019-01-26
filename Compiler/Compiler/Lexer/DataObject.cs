using System.Collections.Generic;

namespace Compiler.Lexer
{
	public class DataObject
	{
		private int _countStates;
		private List<int> _finishStates;
		private Dictionary<string, List<int>> _automate;

		public DataObject(int countStates, List<int> finisStates, Dictionary<string, List<int>> automate)
		{
			_countStates = countStates;
			_finishStates = finisStates;
			_automate = automate;
		}

		public int CountStates
		{
			get => _countStates;
			set => _countStates = value;
		}

		public List<int> FinishStates
		{
			get => _finishStates;
			set => _finishStates = value;
		}

		public Dictionary<string, List<int>> Automate
		{
			get => _automate;
			set => _automate = value;
		}
	}
}