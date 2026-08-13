namespace TerraLink.Api.Common;

public class ForbidenException : Exception
{
    public ForbidenException(string message) :
        base(message)
    {        
    }
}