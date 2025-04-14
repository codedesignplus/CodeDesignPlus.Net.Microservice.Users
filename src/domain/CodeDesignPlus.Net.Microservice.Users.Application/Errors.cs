namespace CodeDesignPlus.Net.Microservice.Users.Application;

public class Errors: IErrorCodes
{    
    public const string UnknownError = "200 : UnknownError";

    public static string InvalidRequest { get; internal set; }
    public static string UserAlreadyExists { get; internal set; }
    public static string UserNotFound { get; internal set; }
}
