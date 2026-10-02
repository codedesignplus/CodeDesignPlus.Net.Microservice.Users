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

    /// <summary>
    /// Los usuarios que pertenecen a una copropiedad, incluidos los dados de baja.
    /// </summary>
    /// <remarks>
    /// Se incluyen los dados de baja porque tambien guardan la membresia: al purgar la copropiedad no debe quedar
    /// ningun documento que la nombre.
    /// </remarks>
    Task<List<UserAggregate>> FindByTenantAsync(Guid tenantId, CancellationToken cancellationToken);

    /// <summary>
    /// Quita una copropiedad al usuario, con sus roles, sin reescribir el documento entero.
    /// </summary>
    /// <remarks>
    /// Misma razon que <see cref="AddRoleAsync"/>: un <c>$pull</c> sobre el estado real, que no pisa lo que otro
    /// proceso haya escrito en las demas copropiedades del usuario. Repetirlo es inofensivo.
    /// </remarks>
    /// <returns><c>true</c> si el usuario pertenecia a la copropiedad y se le quito.</returns>
    Task<bool> RemoveTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken);

    /// <summary>
    /// Marca la copropiedad del usuario como eliminada, con su fecha de purga, o le quita la marca si se restauró.
    /// </summary>
    /// <remarks>
    /// Misma razón que <see cref="AddRoleAsync"/>: se escribe solo ese campo de esa copropiedad, sin reescribir el
    /// documento. Repetirlo es inofensivo.
    /// </remarks>
    /// <param name="purgeAfter">La fecha de purga, o <c>null</c> si la copropiedad se restauró.</param>
    /// <returns><c>true</c> si el usuario pertenece a la copropiedad.</returns>
    Task<bool> SetTenantPurgeAfterAsync(Guid id, Guid tenantId, Instant? purgeAfter, CancellationToken cancellationToken);

    /// <summary>
    /// Si ya hay un usuario con ese correo, sin distinguir mayúsculas ni espacios alrededor (pendings/215).
    /// </summary>
    /// <remarks>
    /// La unicidad del correo es una invariante del caso de uso de crear, no de la base: no hay índice único, por
    /// decisión del usuario.
    /// </remarks>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
}
