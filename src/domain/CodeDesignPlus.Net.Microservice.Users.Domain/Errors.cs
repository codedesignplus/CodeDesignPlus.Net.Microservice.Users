namespace CodeDesignPlus.Net.Microservice.Users.Domain;

public class Errors: IErrorCodes
{    
    public const string UnknownError = "100 : UnknownError";

    public const string FirstNameRequired = "101 : The first name is required.";
    public const string IdUserIsRequired = "102 : The user ID is required.";
    public const string LastNameRequired = "103 : The last name is required.";
    public const string EmailRequired = "104 : The email is required.";
    public const string PhoneRequired = "105 : The phone number is required.";
    public const string RolesRequired = "106 : The roles are required.";
    public const string UpdateByInvalid = "107 : The updated by ID is invalid.";
    public const string TenantAlreadyExists = "108 : The tenant already exists.";
    public const string TenantNotFound = "109 : The tenant was not found.";
    public const string RoleAlreadyExists = "110 : The role already exists.";
    public const string AddressRequired = "111 : The address is required.";

    public const string ImageRequired = "112 : The image profile is required.";

    public const string RoleNotFound = "113 : The role was not found.";
}
