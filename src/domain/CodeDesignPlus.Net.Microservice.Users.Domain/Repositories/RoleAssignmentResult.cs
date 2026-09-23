namespace CodeDesignPlus.Net.Microservice.Users.Domain.Repositories;

/// <summary>
/// En que quedo un intento de asignar o retirar un rol.
/// </summary>
/// <remarks>
/// Son tres desenlaces y no un booleano porque dos de ellos no significan lo mismo: que el rol ya
/// estuviera es no tener nada que hacer, y que la copropiedad no sea del usuario es un error de quien
/// llamo. Con un booleano los dos devolvian <c>false</c> y el segundo pasaba desapercibido.
/// </remarks>
public enum RoleAssignmentResult
{
    /// <summary>
    /// El rol no estaba y quedo asignado, o estaba y quedo retirado.
    /// </summary>
    Applied,

    /// <summary>
    /// No habia nada que hacer: el rol ya estaba, o no estaba y se pedia retirarlo.
    /// </summary>
    NothingToDo,

    /// <summary>
    /// El usuario no pertenece a esa copropiedad, asi que no puede tener un rol en ella.
    /// </summary>
    TenantNotFound,
}
