namespace CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;

/// <summary>
/// Qué tipos de documento se escriben con guion y dígito de verificación, y con qué pesos se calcula ese dígito.
/// </summary>
/// <remarks>
/// <para>
/// Las dos listas son configuración del despliegue (<c>UserDocument</c>), no de este micro, que es transversal y no lleva
/// un país dentro: el tipo es un código del catálogo de tipos de documento y los pesos los fija la autoridad tributaria de
/// cada país. Aquí solo vive el cálculo genérico, un módulo 11 ponderado: cada dígito del número, de derecha a izquierda,
/// se multiplica por su peso; la suma se divide entre 11, y si el residuo es 0 o 1 ese es el dígito, si no, 11 menos el
/// residuo.
/// </para>
/// <para>
/// El número y su dígito van en el mismo campo, «número-dígito». Sin tipos configurados, ningún documento lo exige.
/// </para>
/// <para>
/// Un tipo puede tener además una longitud fija antes del guion (<see cref="BaseLengths"/>), que también es
/// configuración. Con ella se sabe cuál es el número y cuál el dígito aunque falte el guion, y el rechazo propone la
/// escritura correcta en vez de una suposición. Sin longitud para un tipo, nada cambia.
/// </para>
/// </remarks>
/// <param name="DocumentTypes">Los códigos de los tipos de documento que llevan dígito de verificación.</param>
/// <param name="Weights">Los pesos, del dígito de las unidades hacia la izquierda.</param>
public sealed record DocumentCheckDigitRules(IReadOnlyCollection<string> DocumentTypes, IReadOnlyList<int> Weights)
{
    /// <summary>
    /// El divisor del cálculo. Es la definición del módulo 11, no un dato del país.
    /// </summary>
    private const int Modulus = 11;

    /// <summary>
    /// Las reglas de un despliegue que no configura ningún tipo con dígito de verificación.
    /// </summary>
    public static readonly DocumentCheckDigitRules None = new([], []);

    /// <summary>
    /// Cuántos dígitos lleva el número antes del guion, por código de tipo de documento, para los tipos que la tienen fija.
    /// </summary>
    /// <remarks>
    /// Va como propiedad y no en el constructor: es opcional y un tipo sin entrada se valida como antes de que existiera.
    /// </remarks>
    public IReadOnlyDictionary<string, int> BaseLengths { get; init; } = new Dictionary<string, int>();

    /// <summary>
    /// La longitud fija del número de un tipo de documento, sin el dígito de verificación.
    /// </summary>
    /// <param name="documentTypeCode">El código del tipo.</param>
    /// <returns>La longitud, o <c>null</c> si el tipo no la tiene configurada.</returns>
    public int? BaseLengthOf(string? documentTypeCode)
    {
        if (string.IsNullOrWhiteSpace(documentTypeCode))
        {
            return null;
        }

        foreach (var (code, length) in this.BaseLengths)
        {
            if (string.Equals(code.Trim(), documentTypeCode.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return length;
            }
        }

        return null;
    }

    /// <summary>
    /// Si un tipo de documento lleva dígito de verificación.
    /// </summary>
    /// <param name="documentTypeCode">El código del tipo.</param>
    /// <returns><c>true</c> si está entre los configurados.</returns>
    public bool Applies(string? documentTypeCode)
    {
        return !string.IsNullOrWhiteSpace(documentTypeCode) && this.DocumentTypes.Any(code => string.Equals(code.Trim(), documentTypeCode.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Calcula el dígito de verificación de un número.
    /// </summary>
    /// <param name="number">El número, solo dígitos y sin el dígito de verificación.</param>
    /// <returns>El dígito, o <c>null</c> si el número está vacío, no es solo dígitos o es más largo que la lista de pesos.</returns>
    public int? Compute(string? number)
    {
        if (string.IsNullOrEmpty(number) || number.Length > this.Weights.Count || !number.All(char.IsAsciiDigit))
        {
            return null;
        }

        var sum = 0;

        for (var i = 0; i < number.Length; i++)
        {
            sum += (number[number.Length - 1 - i] - '0') * this.Weights[i];
        }

        var remainder = sum % Modulus;

        return remainder > 1 ? Modulus - remainder : remainder;
    }

    /// <summary>
    /// Rechaza el número de un documento que lleva dígito de verificación si no viene con guion y con el dígito que le corresponde.
    /// </summary>
    /// <remarks>
    /// <para>
    /// El error dice qué corregir y cuál es el dígito. A un número sin guion no se le adivina si el último dígito es el de
    /// verificación pegado: el error dice cómo se escribe si lo recibido es el número sin el dígito.
    /// </para>
    /// <para>
    /// Cuando el tipo tiene longitud fija ya no hace falta adivinar. Sin guion, un número de esa longitud es el número sin
    /// su dígito, y uno con un dígito más cuyo último dígito es el de verificación de los anteriores trae el dígito pegado:
    /// en los dos casos el error propone la escritura completa. Con guion, el número tiene que tener esa longitud.
    /// </para>
    /// </remarks>
    /// <param name="documentTypeCode">El código del tipo de documento.</param>
    /// <param name="documentNumber">El número como se escribió.</param>
    /// <exception cref="CodeDesignPlus.Net.Exceptions.CodeDesignPlusException">Si falta el guion y el dígito, si el número no es solo dígitos, si no tiene la longitud de su tipo o si el dígito no corresponde.</exception>
    public void Ensure(string? documentTypeCode, string? documentNumber)
    {
        if (!this.Applies(documentTypeCode))
        {
            return;
        }

        var value = documentNumber?.Trim() ?? string.Empty;
        var dash = value.LastIndexOf('-');
        var isMissing = dash < 0;

        var number = isMissing ? value : value[..dash];
        var baseLength = this.BaseLengthOf(documentTypeCode);

        // Con longitud fija, un dígito de más que es el de verificación de los anteriores es el dígito pegado: el número
        // es lo que va antes, y eso es lo que se propone.
        if (isMissing && number.Length == baseLength + 1 && char.IsAsciiDigit(number[^1]) && this.Compute(number[..^1]) == number[^1] - '0')
        {
            number = number[..^1];
        }

        var checkDigit = this.Compute(number);

        var isWellFormed = checkDigit is not null && (isMissing || (dash == value.Length - 2 && char.IsAsciiDigit(value[^1])));

        DomainGuard.IsFalse(isWellFormed, Errors.DocumentNumberFormatIsInvalid);
        DomainGuard.IsTrue(isMissing && number.Length == baseLength, Errors.DocumentCheckDigitIsNotSeparated.With(number, checkDigit));
        DomainGuard.IsTrue(isMissing, Errors.DocumentCheckDigitIsMissing.With(number, checkDigit));
        DomainGuard.IsTrue(baseLength is not null && number.Length != baseLength, Errors.DocumentBaseLengthIsInvalid.With(baseLength, number.Length));
        DomainGuard.IsTrue(value[^1] - '0' != checkDigit, Errors.DocumentCheckDigitDoesNotMatch.With(number, checkDigit, value[^1]));
    }
}
