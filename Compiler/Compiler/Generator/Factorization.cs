using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Compiler.Generator
{
    public class Factorization
    {
        private readonly Dictionary<string, List<List<string>>> _ruleList = new Dictionary<string, List<List<string>>>();
        private string _filePath;
        private readonly List<string> _alreadyExistKeyList = new List<string>();
        
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
                        var randString = RandomString(3);
                        FileWrite($"{item.Key} -> {group.Key} {randString}");
                        _ruleList.Add(randString, group.Select(i => i.Skip(1).ToList()).ToList());
                    }
                }
            }
        }

        private void FileWrite(string line)
        {
            File.AppendAllText(_filePath, line + "\n");
        }

        private void FileRead()
        {
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

        private string RandomString(int length)
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