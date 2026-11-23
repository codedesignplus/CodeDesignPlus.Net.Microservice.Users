using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Microservice.Users.Application.Options;

/// <summary>
/// Qué tipos de documento de un usuario llevan dígito de verificación y con qué pesos se calcula.
/// </summary>
/// <remarks>
/// Es configuración del despliegue y no una lista en el código: este micro es transversal y no lleva un país dentro. El
/// tipo es un código del catálogo de tipos de documento, y cuál lleva dígito y con qué pesos lo decide la autoridad
/// tributaria del país del despliegue. Sin la sección, ningún tipo exige dígito. ms-microsoftgraph lleva la misma sección
/// con los mismos valores: lo que su registro acepta lo tiene que aceptar este micro.
/// </remarks>
public class UserDocumentOptions
{
    /// <summary>
    /// La sección de configuración.
    /// </summary>
    public static readonly string Section = "UserDocument";

    /// <summary>
    /// Los códigos de tipo de documento cuyo número se escribe con guion y dígito de verificación.
    /// </summary>
    public List<string> CheckDigitDocumentTypes { get; set; } = [];

    /// <summary>
    /// Los pesos del módulo 11 ponderado, del dígito de las unidades hacia la izquierda.
    /// </summary>
    public List<int> CheckDigitWeights { get; set; } = [];

    /// <summary>
    /// Cuántos dígitos lleva el número antes del guion, por código de tipo de documento, para los tipos que la tienen fija.
    /// </summary>
    /// <remarks>
    /// Es opcional. Con ella, el rechazo de un número sin guion propone la escritura correcta (separa el dígito que vino
    /// pegado) y un número con guion de otra longitud se rechaza. Un tipo sin entrada se valida solo por su dígito.
    /// </remarks>
    public Dictionary<string, int> CheckDigitBaseLengths { get; set; } = [];

    /// <summary>
    /// Las reglas del dígito de verificación como las entiende el dominio.
    /// </summary>
    /// <returns>Los tipos y los pesos configurados.</returns>
    public DocumentCheckDigitRules ToRules()
    {
        return new DocumentCheckDigitRules(this.CheckDigitDocumentTypes, this.CheckDigitWeights) { BaseLengths = this.CheckDigitBaseLengths };
    }
}

/// <summary>
/// Comprueba al arrancar que los tipos con dígito de verificación tienen con qué calcularlo.
/// </summary>
/// <remarks>
/// Se valida al arrancar y no al usarse: con tipos y sin pesos, todo documento de esos tipos se rechazaría sin que nadie
/// supiera por qué, y con un peso que no es positivo el dígito calculado sería otro. Una longitud de un tipo que no lleva
/// dígito no se aplicaría nunca, y una que no cabe en los pesos rechazaría todo documento de ese tipo.
/// </remarks>
public class UserDocumentOptionsValidator : IValidateOptions<UserDocumentOptions>
{
    /// <summary>
    /// Valida la sección.
    /// </summary>
    /// <param name="name">El nombre de la instancia de opciones.</param>
    /// <param name="options">La sección leída.</param>
    /// <returns>El resultado de la validación.</returns>
    public ValidateOptionsResult Validate(string? name, UserDocumentOptions options)
    {
        if (options.CheckDigitDocumentTypes.Exists(string.IsNullOrWhiteSpace))
        {
            return ValidateOptionsResult.Fail($"La seccion '{UserDocumentOptions.Section}' tiene un codigo vacio en {nameof(UserDocumentOptions.CheckDigitDocumentTypes)}.");
        }

        if (options.CheckDigitDocumentTypes.Count > 0 && options.CheckDigitWeights.Count == 0)
        {
            return ValidateOptionsResult.Fail($"La seccion '{UserDocumentOptions.Section}' define tipos con digito de verificacion pero no {nameof(UserDocumentOptions.CheckDigitWeights)}: sin pesos no hay con que calcularlo.");
        }

        if (options.CheckDigitWeights.Exists(weight => weight <= 0))
        {
            return ValidateOptionsResult.Fail($"La seccion '{UserDocumentOptions.Section}' tiene un peso que no es positivo en {nameof(UserDocumentOptions.CheckDigitWeights)}.");
        }

        foreach (var (code, length) in options.CheckDigitBaseLengths)
        {
            if (!options.CheckDigitDocumentTypes.Exists(type => string.Equals(type.Trim(), code.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                return ValidateOptionsResult.Fail($"La seccion '{UserDocumentOptions.Section}' define en {nameof(UserDocumentOptions.CheckDigitBaseLengths)} la longitud del tipo '{code}', que no esta en {nameof(UserDocumentOptions.CheckDigitDocumentTypes)}.");
            }

            if (length <= 0 || length > options.CheckDigitWeights.Count)
            {
                return ValidateOptionsResult.Fail($"La seccion '{UserDocumentOptions.Section}' define en {nameof(UserDocumentOptions.CheckDigitBaseLengths)} una longitud de {length} para el tipo '{code}': tiene que estar entre 1 y la cantidad de pesos ({options.CheckDigitWeights.Count}).");
            }
        }

        return ValidateOptionsResult.Success;
    }
}
