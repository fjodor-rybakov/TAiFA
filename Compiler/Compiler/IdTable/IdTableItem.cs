namespace Compiler.IdTable
{
    public class IdTableItem
    {
        public IdTableItem(string name, string type, string value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public string Name { get; }
        public string Type { get; }
        public string Value { get; set; }
    }
}