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
    public static readonly Error DocumentTypeRequired = new("115");

    /// <summary>
    /// El tipo de documento lleva dígito de verificación y el número vino sin guion. Argumentos: el número y su dígito.
    /// </summary>
    public static readonly Error DocumentCheckDigitIsMissing = new("116");

    /// <summary>
    /// El dígito de verificación no es el del número. Argumentos: el número, el dígito que le corresponde y el escrito.
    /// </summary>
    public static readonly Error DocumentCheckDigitDoesNotMatch = new("117");

    /// <summary>
    /// El número de un documento con dígito de verificación no es «solo dígitos, guion y un dígito».
    /// </summary>
    public static readonly Error DocumentNumberFormatIsInvalid = new("118");

    /// <summary>
    /// El agregado recibió el documento sin las reglas del dígito de verificación: es un error de programación.
    /// </summary>
    public static readonly Error DocumentRulesRequired = new("119");

    /// <summary>
    /// El número vino sin guion y su tipo tiene longitud fija, así que se sabe cuál es el dígito. Argumentos: el número
    /// sin el dígito y su dígito.
    /// </summary>
    public static readonly Error DocumentCheckDigitIsNotSeparated = new("120");

    /// <summary>
    /// El número antes del guion no tiene la longitud de su tipo. Argumentos: la longitud del tipo y la escrita.
    /// </summary>
    public static readonly Error DocumentBaseLengthIsInvalid = new("121");
}
