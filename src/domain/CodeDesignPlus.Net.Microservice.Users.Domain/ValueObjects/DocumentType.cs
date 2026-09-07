namespace CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;

/// <summary>
/// Reference to an entry of the TypeDocument catalog owned by ms-catalogs.
///
/// Keeps the shape of <c>Item&lt;string&gt;</c> — <see cref="Id"/> plus the display snapshot in
/// <see cref="Value"/> — and adds <see cref="Code"/>, which is the interoperable half: it is what
/// the payment gateway expects and what travels to Entra as a directory extension attribute.
/// The identifier alone is useless outside this database.
/// </summary>
/// <param name="Id">Identifier of the catalog entry in ms-catalogs.</param>
/// <param name="Value">Display name at the time of selection, e.g. "Cédula de Ciudadanía".</param>
/// <param name="Code">Stable catalog code, e.g. "CC", "NIT", "PP".</param>
public record DocumentType(Guid Id, string Value, string Code);
