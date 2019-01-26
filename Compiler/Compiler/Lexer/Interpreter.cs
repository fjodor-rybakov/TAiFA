using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Compiler.Lexer
{
	public class Interpreter
	{
		private Dictionary<string, DataObject> _automates = new Dictionary<string, DataObject>();
		
		public Interpreter(params string[] paths)
		{
			try
			{
				foreach (string path in paths)
				{
					StreamReader dataReader = new StreamReader(path);
					int countStates = int.Parse(dataReader.ReadLine() ?? throw new Exception("Ошибка чтения числа"));
					List<int> finishStates = new List<int>();
					string[] lineFinishStates = dataReader.ReadLine()?.Split(' ');
					if (lineFinishStates != null)
					{
						foreach (string item in lineFinishStates)
						{
							finishStates.Add(int.Parse(item ?? throw new Exception("Ошибка чтения числа")));
						}
						
						string fileName = Path.GetFileName(path).Split('.')[0];
						Dictionary<string, List<int>> automate = GetAutomate(ref dataReader, countStates);
						DataObject dataObject = new DataObject(countStates, finishStates, automate);
						
						_automates.Add(fileName, dataObject);
					}
					else
					{
						Console.WriteLine("Нет финишных состояний.");
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		public Dictionary<string, DataObject> Automates
		{
			get => _automates;
			set => _automates = value;
		}

		private Dictionary<string, List<int>> GetAutomate(ref StreamReader dataReader, int countStates)
		{
			Dictionary<string, List<int>> table = new Dictionary<string, List<int>>();
			string line = dataReader.ReadLine();
			if (line != null)
			{
				string[] determinants = line.Split(' ');
				List<List<int>> items = new List<List<int>>();

				while ((line = dataReader.ReadLine()) != null)
				{
					List<int> item = Array.ConvertAll(line.Split(' '), int.Parse).ToList();
					items.Add(item);
				}

				for (int i = 0; i < determinants.Length; i++)
				{
					table.Add(determinants[i], items[i]);
				}
			}

			return table;
		}
	}
}