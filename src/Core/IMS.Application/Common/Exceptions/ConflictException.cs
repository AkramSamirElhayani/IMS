namespace IMS.Application.Common.Exceptions
{
    public class ConflictException : ApplicationException
    {
        public ConflictException(string name, object key)
            : base("Conflict", $"Entity \"{name}\" ({key}) already exists.")
        {
            Name = name;
            Key = key;
        }

        public string Name { get; }
        public object Key { get; }
    }
}
