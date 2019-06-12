using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Compiler.Generator
{
    public class Factorization
    {
        private readonly Dictionary<string, List<List<string>>> _ruleList = new Dictionary<string, List<List<string>>>();
        private readonly List<string> _alreadyExistKeyList = new List<string>();
        private string _filePath;
        
        public void Factor(string filePath)
        {
            _filePath = filePath;
            FileRead();
            Convert();
        }

        private void Convert()
        {
            for (var index = 0; index < _ruleList.Count; index++)
            {
                var item = _ruleList.ElementAtOrDefault(index);
                var grouping = item.Value.GroupBy(val => val.FirstOrDefault()).ToList();
                foreach (var group in grouping)
                {
                    if (group.ToList().Count < 2)
                    {
                        FileWrite($"{item.Key} -> {string.Join(" ", group.SelectMany(g => g).DefaultIfEmpty("E"))}");
                    }
                    else
                    {
                        var randString = RandomString();
                        FileWrite($"{item.Key} -> {group.Key} {randString}");
                        _ruleList.Add(randString, group.Select(i => i.Skip(1).ToList()).ToList());
                    }
                }
            }
        }

        public void ReplaceLR(string filePath)
        {
            _filePath = filePath;
            FileRead();
            Replace();
        }

        private void Replace()
        {
            var alreadyUsed = new List<string>();
            var tempList = new List<List<string>>();
            
            for (var index = 0; index < _ruleList.Count; index++)
            {
                var item = _ruleList.ElementAtOrDefault(index);

                foreach (var value in item.Value)
                {
                    if (item.Key == value.First() && value.Count < 2)
                    {
                        throw new Exception("Неверный формат правил");
                    }
                    
                    if (item.Key == value.First() && value.Count >= 2)
                    {
                        alreadyUsed.Add(item.Key);
                        var firstKey = RandomString();
                        var lastKey = RandomString();
                        FileWrite($"{item.Key} -> {firstKey} {lastKey}");
                        foreach (var exp in item.Value)
                        {
                            if (!exp.SequenceEqual(value))
                                FileWrite($"{firstKey} -> {string.Join(" ", exp)}");
                        }
                        FileWrite($"{lastKey} -> E");
                        FileWrite($"{lastKey} -> {string.Join(" ", value.Skip(1).SelectMany(g => g))} {lastKey}");
                        tempList.Clear();
                    }
                    else
                    {
                        tempList.Add(value);
                    }
                }

                if (tempList.Count == 0) continue;
                foreach (var value in tempList)
                {
                    if (!alreadyUsed.Contains(item.Key)) 
                        FileWrite($"{item.Key} -> {string.Join(" ", value)}");
                }
                tempList.Clear();
            }
        }

        private void FileWrite(string line)
        {
            File.AppendAllText(_filePath, line + "\n");
        }

        private void FileRead()
        {
            _alreadyExistKeyList.Clear();
            _ruleList.Clear();
            using (var reader = new StreamReader(_filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var lexems = line.Split(' ').ToList();
                    var key = lexems.First();
                    _alreadyExistKeyList.Add(key);
                    var values = lexems.Skip(2).ToList();
                    if (!_ruleList.ContainsKey(key)) _ruleList.Add(key, new List<List<string>> {values});
                    else _ruleList[key].Add(values);
                }
            }
            var fs = File.OpenWrite(_filePath);
            fs.SetLength(0);
            fs.Close();
        }

        private string RandomString(int length = 3)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string randString;
            do
            {
                randString = new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            } while (_alreadyExistKeyList.Contains(randString));
            _alreadyExistKeyList.Add(randString);

            return $"<{randString}>";
        }
    }
}