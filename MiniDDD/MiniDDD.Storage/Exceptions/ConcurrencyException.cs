namespace MiniDDD.Storage.Exceptions
{
    public class ConcurrencyException : System.Exception
    {
        public ConcurrencyException(string message) : base(message) { }
    }
}