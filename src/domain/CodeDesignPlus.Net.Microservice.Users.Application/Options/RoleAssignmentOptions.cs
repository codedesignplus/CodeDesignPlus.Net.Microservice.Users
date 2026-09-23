using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Options;

/// <summary>
/// Los roles que este microservicio asigna por su cuenta.
/// </summary>
/// <remarks>
/// <b>Un rol se identifica por el id de su grupo en el proveedor de identidad, no por su nombre.</b> Eso
/// es lo que trae el token en el claim <c>groups</c>, y comparar nombres contra identificadores no casa
/// nunca.
/// <para>
/// Los identificadores <b>cambian por entorno</b>, porque cada entorno es un directorio distinto. Por eso
/// viven en el <c>appsettings</c> de cada uno y el base los deja en blanco: si un entorno no los define,
/// <see cref="RoleAssignmentOptionsValidator"/> impide que el micro arranque. Arrancar con un valor por
/// defecto seria peor, porque el comprador quedaria con un rol que no existe y nada fallaria.
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
            $"La seccion '{RoleAssignmentOptions.Section}' no define el identificador del grupo de " +
            $"{nameof(RoleAssignmentOptions.AdministrationRole)}. Cada entorno tiene su propio directorio, " +
            "asi que este valor va en el appsettings del entorno o como variable de entorno en el chart.");
    }
}
