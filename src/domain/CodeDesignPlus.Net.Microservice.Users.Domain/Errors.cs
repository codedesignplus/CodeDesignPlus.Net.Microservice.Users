using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.Users.Domain;

public class Errors: IErrorCodes
{    
    public static readonly Error UnknownError = new("100", "UnknownError");

    public static readonly Error FirstNameRequired = new("101", "The first name is required.");
    public static readonly Error IdUserIsRequired = new("102", "The user ID is required.");
    public static readonly Error LastNameRequired = new("103", "The last name is required.");
    public static readonly Error EmailRequired = new("104", "The email is required.");
    public static readonly Error PhoneRequired = new("105", "The phone number is required.");
    public static readonly Error RolesRequired = new("106", "The roles are required.");
    public static readonly Error UpdateByInvalid = new("107", "The updated by ID is invalid.");
    public static readonly Error TenantAlreadyExists = new("108", "The tenant already exists.");
    public static readonly Error TenantNotFound = new("109", "The tenant was not found.");
    public static readonly Error RoleAlreadyExists = new("110", "The role already exists.");
    public static readonly Error AddressRequired = new("111", "The address is required.");

    public static readonly Error ImageRequired = new("112", "The image profile is required.");

    public static readonly Error RoleNotFound = new("113", "The role was not found.");
    public static readonly Error DocumentNumberRequired = new("114", "The document number is required.");
}
