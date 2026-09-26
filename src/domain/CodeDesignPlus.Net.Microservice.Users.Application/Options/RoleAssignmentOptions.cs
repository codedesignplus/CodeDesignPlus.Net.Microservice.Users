using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Options;

/// <summary>
/// Los roles que este microservicio asigna por su cuenta.
/// </summary>
/// <remarks>
/// <b>Un rol se identifica por su id en el catalogo de ms-roles (<c>20000000-…</c>), no por su nombre ni por el id
/// de su grupo en el proveedor de identidad.</b> El id del catalogo es el mismo en todos los entornos, asi que vive en
/// el <c>appsettings</c> base; el del grupo de Entra cambia por entorno y solo lo usan el token (claim <c>groups</c>,
/// para el frontend) y ms-microsoftgraph, que traduce entre los dos. Es tambien el id que llevan los permisos de
/// ms-rbac y el que compara el SDK al autorizar.
/// <para>
/// Si falta, <see cref="RoleAssignmentOptionsValidator"/> impide que el micro arranque: el comprador quedaria con un
/// rol que no existe y nada fallaria.
/// </para>
/// </remarks>
public class RoleAssignmentOptions
{
    public const string Section = "RoleAssignment";

    /// <summary>
    /// El rol con el que se da de alta al comprador de una copropiedad.
    /// </summary>
    public Guid AdministrationRole { get; set; }
}

/// <summary>
/// Comprueba al arrancar que el entorno definio los identificadores de grupo.
/// </summary>
/// <remarks>
/// Se valida al arrancar y no al usarse: si faltara, el alta por compra terminaria bien y el comprador
/// entraria sin poder hacer nada, sin un solo error en el log. Un micro que no levanta se ve en el primer
/// despliegue.
/// </remarks>
public class RoleAssignmentOptionsValidator : IValidateOptions<RoleAssignmentOptions>
{
    public ValidateOptionsResult Validate(string? name, RoleAssignmentOptions options)
    {
        if (options.AdministrationRole != Guid.Empty)
            return ValidateOptionsResult.Success;

        return ValidateOptionsResult.Fail(
            $"La seccion '{RoleAssignmentOptions.Section}' no define el identificador del rol de " +
            $"{nameof(RoleAssignmentOptions.AdministrationRole)}. Es el mismo en todos los entornos y vive en el appsettings base.");
    }
}
