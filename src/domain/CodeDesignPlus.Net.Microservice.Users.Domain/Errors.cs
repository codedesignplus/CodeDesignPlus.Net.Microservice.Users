namespace CodeDesignPlus.Net.Microservice.Users.Domain;

public class Errors: IErrorCodes
{    
    public const string UnknownError = "100 : UnknownError";

    public static string FirstNameRequired { get; internal set; }
    public static string IdUserIsRequired { get; internal set; }
    public static string LastNameRequired { get; internal set; }
    public static string EmailRequired { get; internal set; }
    public static string PhoneRequired { get; internal set; }
    public static string RolesRequired { get; internal set; }
    public static string UpdateByInvalid { get; internal set; }
    public static string TenantAlreadyExists { get; internal set; }
    public static string TenantNotFound { get; internal set; }
    public static string RoleAlreadyExists { get; internal set; }
}
