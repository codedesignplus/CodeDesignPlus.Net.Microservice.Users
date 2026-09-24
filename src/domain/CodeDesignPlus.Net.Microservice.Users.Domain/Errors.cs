using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.Users.Domain;

public class Errors: IErrorCodes
{    
    public static readonly Error UnknownError = new("100");

    public static readonly Error FirstNameRequired = new("101");
    public static readonly Error IdUserIsRequired = new("102");
    public static readonly Error LastNameRequired = new("103");
    public static readonly Error EmailRequired = new("104");
    public static readonly Error PhoneRequired = new("105");
    public static readonly Error RolesRequired = new("106");
    public static readonly Error UpdateByInvalid = new("107");
    public static readonly Error TenantAlreadyExists = new("108");
    public static readonly Error TenantNotFound = new("109");
    public static readonly Error RoleAlreadyExists = new("110");
    public static readonly Error AddressRequired = new("111");

    public static readonly Error ImageRequired = new("112");

    public static readonly Error RoleNotFound = new("113");
    public static readonly Error DocumentNumberRequired = new("114");
}
