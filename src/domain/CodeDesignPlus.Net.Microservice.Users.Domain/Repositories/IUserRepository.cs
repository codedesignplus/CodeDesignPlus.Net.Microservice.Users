using CodeDesignPlus.Net.Microservice.Users.Domain.Entities;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.Repositories;

public interface IUserRepository : IRepositoryBase
{
    /// <summary>
    /// Anade un rol al usuario en una copropiedad, sin reescribir el documento entero.
    /// </summary>
    /// <remarks>
    /// El rol y la copropiedad los asignan procesos distintos que pueden coincidir en el tiempo: al registrar
    /// una titularidad y su residente, dos consumidores llaman a la vez. Leer el usuario, anadirle un rol y
    /// guardarlo entero hace que el ultimo en escribir borre lo del otro — <b>y ocurrio</b>: el 2026-08-19 un
    /// propietario se quedo sin su rol de residente porque los dos se crearon con 59 ms de diferencia.
    /// <para>
    /// Con <c>$addToSet</c> sobre el operador posicional la base anade dentro de la copropiedad que casa con
    /// el filtro, sobre el estado real y no sobre el que se leyo. Ademas no duplica, asi que repetir la
    /// operacion es inofensivo.
    /// </para>
    /// <para>
    /// La pertenencia se comprueba en el mismo filtro y no antes con una lectura: comprobarla aparte dejaria
    /// una ventana entre la comprobacion y la escritura.
    /// </para>
    /// </remarks>
    Task<RoleAssignmentResult> AddRoleAsync(Guid id, Guid tenantId, Guid role, Guid updatedBy, CancellationToken cancellationToken);

    /// <summary>
    /// Retira un rol del usuario en una copropiedad, sin reescribir el documento entero.
    /// </summary>
    /// <remarks>
    /// Misma razon que <see cref="AddRoleAsync"/>. Y retirarlo de una copropiedad no lo retira de las demas:
    /// quien consuma el evento tiene que comprobar que no le queda en ninguna otra antes de sacar al usuario
    /// del grupo del proveedor de identidad, que si es global.
    /// </remarks>
    Task<RoleAssignmentResult> RemoveRoleAsync(Guid id, Guid tenantId, Guid role, Guid updatedBy, CancellationToken cancellationToken);

    /// <summary>
    /// Anade una copropiedad al usuario sin reescribir el documento entero.
    /// </summary>
    /// <remarks>
    /// Misma razon que <see cref="AddRoleAsync"/>: un administrador que compra su segunda copropiedad tiene
    /// las dos escrituras compitiendo por el mismo documento.
    /// </remarks>
    /// <returns><c>true</c> si la copropiedad no estaba y se anadio.</returns>
    Task<bool> AddTenantAsync(Guid id, TenantEntity tenant, Guid updatedBy, CancellationToken cancellationToken);
}
