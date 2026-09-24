using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.Users.Application;

public class Errors: IErrorCodes
{    
    public static readonly Error UnknownError = new("200", "UnknownError");

    public static readonly Error InvalidRequest = new("201", "The request is invalid.");
    public static readonly Error UserAlreadyExists = new("202", "The user already exists.");
    public static readonly Error UserNotFound = new("203", "The user was not found.");

    /// <summary>
    /// La copropiedad del rol no se pudo resolver en el directorio.
    /// </summary>
    /// <remarks>
    /// El directorio no distingue «no existe» de «ahora mismo no puedo comprobarlo», asi que este error
    /// cubre los dos. Se reintenta: si el dato era malo acabara en la cola de descartes, donde se ve; si
    /// era una caida pasajera, el siguiente intento lo resuelve.
    /// <para>
    /// <b>No cubre</b> que el usuario todavia no pertenezca a la copropiedad: eso no es un error, es como
    /// se entra en ella, y el manejador lo resuelve anadiendosela.
    /// </para>
    /// </remarks>
    public static readonly Error TenantCouldNotBeResolved = new("204", "The tenant of the role could not be resolved.");
}
