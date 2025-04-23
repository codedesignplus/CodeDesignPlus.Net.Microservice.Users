namespace CodeDesignPlus.Net.Microservice.Users.Application;

public class Errors: IErrorCodes
{    
    public const string UnknownError = "200 : UnknownError";

    public const string InvalidRequest = "201 : The request is invalid.";
    public const string UserAlreadyExists = "201 : The user already exists.";
    public const string UserNotFound = "201 : The user was not found.";
}
