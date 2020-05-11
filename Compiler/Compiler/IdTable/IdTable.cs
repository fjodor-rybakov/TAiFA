using System;
using System.Collections.Generic;

namespace Compiler.IdTable
{
    public class IdTable
    {
        private Stack<List<IdTableItem>> _table= new Stack<List<IdTableItem>>();

        public void CreateBlock()
        {
            _table.Push(new List<IdTableItem>());
        }

        public void RemoveBlock()
        {
            _table.Pop();
        }

        public IdTableItem GetInfoByName(string name)
        {
            var currentBlockTable = _table.Peek();

            foreach (var tableItem in currentBlockTable)
            {
                if (tableItem.Name == name)
                {
                    return tableItem;
                }
            }
            
            throw new Exception("Variable " + name + " doesn't exist");
        }

        public void AddItems(IEnumerable<IdTableItem> idList)
        {
            var currentBlockTable = _table.Peek();
            foreach (var tableItem in idList)
            {
                foreach (var item in currentBlockTable)
                {
                    if (item.Name == tableItem.Name)
                    {
                        throw new Exception("Variable name " + tableItem.Name + " is already exist");
                    }
                }
                _table.Peek().Add(tableItem);
            }
            
        }

        public void SetValueByName(string name, string value)
        {
            var currentBlockTable = _table.Peek();
            
            foreach (var tableItem in currentBlockTable)
            {
                if (tableItem.Name == name)
                {
                    tableItem.Value = value;
                    return;
                }
            }
            
            throw new Exception("Variable " + name + " doesn't exist");
        }
    }
}