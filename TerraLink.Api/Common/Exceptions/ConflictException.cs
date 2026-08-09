namespace TerraLink.Api.Common;

public class ConflictException : Exception
{
    public ConflictException(string message) :
        base(message)
    {        
    }
}