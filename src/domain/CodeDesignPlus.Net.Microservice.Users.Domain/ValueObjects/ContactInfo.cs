using System.Text.Json.Serialization;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;

public sealed partial class ContactInfo
{

    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? Country { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Phone { get; private set; }
    public string[] Email { get; private set; } = [];

    public ContactInfo()
    {
    }

    [JsonConstructor]
    public ContactInfo(string address, string city, string state, string country, string postalCode, string phone, string[] email)
    {
        Address = address;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        Phone = phone;
        Email = email;
    }

    public static ContactInfo Create(string address, string city, string state, string country, string postalCode, string phone, string[] email)
    {
        return new ContactInfo(address, city, state, country, postalCode, phone, email);
    }
}
