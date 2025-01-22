namespace IMS.Application.Common.Exceptions
{
    public class BadRequestException : ApplicationException
    {
        public BadRequestException(string message)
            : base("Bad Request", message)
        {
        }
    }
}
