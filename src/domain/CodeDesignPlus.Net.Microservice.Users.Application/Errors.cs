namespace CodeDesignPlus.Net.Microservice.Users.Application;

public class Errors: IErrorCodes
{    
    public const string UnknownError = "200 : UnknownError";

    public const string InvalidRequest = "201 : The request is invalid.";
    public const string UserAlreadyExists = "201 : The user already exists.";
    public const string UserNotFound = "201 : The user was not found.";

    /// <summary>
    /// Un rol sin copropiedad no existe, asi que no se puede dar uno en una a la que el usuario no
    /// pertenece.
    /// </summary>
    public const string TenantNotFound = "202 : The user does not belong to that tenant.";
}
