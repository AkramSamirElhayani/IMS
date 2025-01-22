using System;

namespace IMS.Application.Common.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string name, object key)
            : base("Not Found", $"Entity \"{name}\" ({key}) was not found.")
        {
            Name = name;
            Key = key;
        }

        public string Name { get; }
        public object Key { get; }
    }
}
