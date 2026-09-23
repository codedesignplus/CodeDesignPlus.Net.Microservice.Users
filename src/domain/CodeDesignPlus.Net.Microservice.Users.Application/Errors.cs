namespace CodeDesignPlus.Net.Microservice.Users.Application;

public class Errors: IErrorCodes
{    
    public const string UnknownError = "200 : UnknownError";

    public const string InvalidRequest = "201 : The request is invalid.";
    public const string UserAlreadyExists = "201 : The user already exists.";
    public const string UserNotFound = "201 : The user was not found.";

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
    public const string TenantCouldNotBeResolved = "202 : The tenant of the role could not be resolved.";
}
