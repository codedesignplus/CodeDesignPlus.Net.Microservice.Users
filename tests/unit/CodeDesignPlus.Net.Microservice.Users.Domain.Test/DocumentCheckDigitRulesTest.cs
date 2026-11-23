using Xunit;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.Test;

/// <summary>
/// Un documento de un tipo que lleva dígito de verificación se escribe con guion y con el dígito que le corresponde, y el
/// rechazo dice cuál es.
/// </summary>
public class DocumentCheckDigitRulesTest
{
    private const string WithCheckDigit = "31";

    private static readonly DocumentCheckDigitRules Rules = new([WithCheckDigit], [3, 7, 13, 17, 19, 23, 29, 37, 41, 43, 47, 53, 59, 67, 71]);

    private static readonly DocumentType TaxId = new(Guid.NewGuid(), "Tax identification number", WithCheckDigit);

    private static readonly DocumentType NationalId = new(Guid.NewGuid(), "National identification", "13");

    [Theory]
    [InlineData("901888777", 3)]
    [InlineData("900765432", 6)]
    [InlineData("800197268", 4)]
    [InlineData("3", 2)]
    [InlineData("11", 1)]
    [InlineData("15", 0)]
    public void Compute_Number_ReturnsWeightedModulusElevenDigit(string number, int expected)
    {
        // Act
        var digit = Rules.Compute(number);

        // Assert
        Assert.Equal(expected, digit);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("90188877A")]
    [InlineData("1234567890123456")]
    public void Compute_NumberThatCannotBeWeighted_ReturnsNull(string? number)
    {
        // Act
        var digit = Rules.Compute(number);

        // Assert
        Assert.Null(digit);
    }

    [Fact]
    public void Compute_OtherWeights_ReturnsAnotherDigit()
    {
        // Arrange
        var rules = new DocumentCheckDigitRules([WithCheckDigit], [2, 3, 4, 5, 6, 7, 2, 3, 4]);

        // Act
        var digit = rules.Compute("901888777");

        // Assert
        Assert.NotEqual(Rules.Compute("901888777"), digit);
    }

    [Fact]
    public void Ensure_NumberWithItsCheckDigit_DoesNotThrow()
    {
        // Act
        var exception = Record.Exception(() => Rules.Ensure(WithCheckDigit, " 901888777-3 "));

        // Assert
        Assert.Null(exception);
    }

    [Theory]
    [InlineData("13")]
    [InlineData(null)]
    public void Ensure_TypeWithoutCheckDigit_AcceptsAnyNumber(string? documentTypeCode)
    {
        // Act
        var exception = Record.Exception(() => Rules.Ensure(documentTypeCode, "901888777"));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void Ensure_NoTypesConfigured_AcceptsAnyNumber()
    {
        // Act
        var exception = Record.Exception(() => DocumentCheckDigitRules.None.Ensure(WithCheckDigit, "901888777"));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void Ensure_NumberWithoutDash_ThrowsMissingWithTheDigit()
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Rules.Ensure(WithCheckDigit, "900765432"));

        // Assert
        Assert.Equal(Errors.DocumentCheckDigitIsMissing.GetCode(), exception.Code);
        Assert.Equal("Este tipo de documento se escribe con guion y dígito de verificación. Si 900765432 es el número sin el dígito, escríbelo así: 900765432-6.", exception.Error!.Value.GetMessage("es"));
    }

    [Theory]
    [InlineData("9018887773", "Este tipo de documento se escribe con guion y dígito de verificación. Si 9018887773 ya incluye el dígito, escríbelo así: 901888777-3. Si no lo incluye, así: 9018887773-5.")]
    [InlineData("9007654326", "Este tipo de documento se escribe con guion y dígito de verificación. Si 9007654326 ya incluye el dígito, escríbelo así: 900765432-6. Si no lo incluye, así: 9007654326-6.")]
    [InlineData("901888777", "Este tipo de documento se escribe con guion y dígito de verificación. Si 901888777 ya incluye el dígito, escríbelo así: 90188877-7. Si no lo incluye, así: 901888777-3.")]
    [InlineData("32", "Este tipo de documento se escribe con guion y dígito de verificación. Si 32 ya incluye el dígito, escríbelo así: 3-2. Si no lo incluye, así: 32-6.")]
    public void Ensure_LastDigitIsTheCheckDigitOfTheRest_GivesBothReadings(string documentNumber, string expected)
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Rules.Ensure(WithCheckDigit, documentNumber));

        // Assert
        Assert.Equal(Errors.DocumentCheckDigitMayBeAttached.GetCode(), exception.Code);
        Assert.Equal(expected, exception.Error!.Value.GetMessage("es"));
    }

    [Theory]
    [InlineData("9018887775")]
    [InlineData("3")]
    public void Ensure_LastDigitIsNotTheCheckDigitOfTheRest_DoesNotGuess(string documentNumber)
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Rules.Ensure(WithCheckDigit, documentNumber));

        // Assert
        Assert.Equal(Errors.DocumentCheckDigitIsMissing.GetCode(), exception.Code);
        Assert.StartsWith($"Este tipo de documento se escribe con guion y dígito de verificación. Si {documentNumber} es el número sin el dígito, escríbelo así: {documentNumber}-", exception.Error!.Value.GetMessage("es"));
    }

    [Fact]
    public void Ensure_NumberWithDash_IsNeverReadAsAttached()
    {
        // Act
        var exception = Record.Exception(() => Rules.Ensure(WithCheckDigit, "9018887773-5"));

        // Assert
        Assert.Null(exception);
    }

    [Theory]
    [InlineData("es")]
    [InlineData("en")]
    [InlineData("fr")]
    [InlineData("pt")]
    public void Errors_EveryLanguage_GivesBothReadings(string language)
    {
        // Act
        var message = Errors.DocumentCheckDigitMayBeAttached.With("9018887773", "901888777", '3', 5).GetMessage(language);

        // Assert
        Assert.Contains("901888777-3", message);
        Assert.Contains("9018887773-5", message);
        Assert.True(message.IndexOf("901888777-3", StringComparison.Ordinal) < message.IndexOf("9018887773-5", StringComparison.Ordinal));
        Assert.DoesNotContain("{", message);
        Assert.Equal(language == "en", message == Errors.DocumentCheckDigitMayBeAttached.With("9018887773", "901888777", '3', 5).ToEnglish());
    }

    [Fact]
    public void Ensure_WrongCheckDigit_ThrowsDoesNotMatchWithTheDigit()
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Rules.Ensure(WithCheckDigit, "901888777-5"));

        // Assert
        Assert.Equal(Errors.DocumentCheckDigitDoesNotMatch.GetCode(), exception.Code);
        Assert.Equal("El dígito de verificación no corresponde: el de 901888777 es 3, no 5. Escríbelo así: 901888777-3.", exception.Error!.Value.GetMessage("es"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("ABC")]
    [InlineData("-3")]
    [InlineData("901888777-")]
    [InlineData("901888777-33")]
    [InlineData("901888777-A")]
    [InlineData("901.888.777-3")]
    [InlineData("901 888 777-3")]
    [InlineData("1234567890123456-1")]
    public void Ensure_NumberThatIsNotDigitsDashDigit_ThrowsFormatIsInvalid(string documentNumber)
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Rules.Ensure(WithCheckDigit, documentNumber));

        // Assert
        Assert.Equal(Errors.DocumentNumberFormatIsInvalid.GetCode(), exception.Code);
    }

    [Theory]
    [InlineData("es")]
    [InlineData("en")]
    [InlineData("fr")]
    [InlineData("pt")]
    public void Errors_EveryLanguage_SaysTheDigit(string language)
    {
        // Act
        var missing = Errors.DocumentCheckDigitIsMissing.With("901888777", 3).GetMessage(language);
        var wrong = Errors.DocumentCheckDigitDoesNotMatch.With("901888777", 3, '5').GetMessage(language);
        var format = Errors.DocumentNumberFormatIsInvalid.GetMessage(language);

        // Assert
        Assert.Contains("901888777-3", missing);
        Assert.Contains("901888777-3", wrong);
        Assert.NotEqual(Errors.DocumentNumberFormatIsInvalid.GetCode(), format);
        Assert.Equal(language == "en", missing == Errors.DocumentCheckDigitIsMissing.With("901888777", 3).ToEnglish());
    }

    [Fact]
    public void Create_TaxIdWithoutCheckDigit_Throws()
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "3105550131", null, "900765432", TaxId, true, Rules));

        // Assert
        Assert.Equal(Errors.DocumentCheckDigitIsMissing.GetCode(), exception.Code);
    }

    [Fact]
    public void Create_TaxIdWithItsCheckDigit_KeepsTheNumberAsWritten()
    {
        // Act
        var user = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "3105550131", null, "901888777-3", TaxId, true, Rules);

        // Assert
        Assert.Equal("901888777-3", user.DocumentNumber);
    }

    [Fact]
    public void Create_WithoutRules_Throws()
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "3105550131", null, "901888777-3", TaxId, true, null!));

        // Assert
        Assert.Equal(Errors.DocumentRulesRequired.GetCode(), exception.Code);
    }

    [Fact]
    public void Update_TaxIdWithWrongCheckDigit_ThrowsAndKeepsTheDocument()
    {
        // Arrange
        var user = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "3105550131", null, "1023456781", NationalId, true, Rules);

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => user.Update("John", "Doe", "john.doe@example.com", "3105550131", null, "901888777-5", TaxId, true, Guid.NewGuid(), Rules));

        // Assert
        Assert.Equal(Errors.DocumentCheckDigitDoesNotMatch.GetCode(), exception.Code);
        Assert.Equal("1023456781", user.DocumentNumber);
    }

    [Fact]
    public void UpdateProfile_TaxIdWithoutCheckDigit_ThrowsAndKeepsTheDocument()
    {
        // Arrange
        var user = UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "3105550131", null, "1023456781", NationalId, true, Rules);
        var contact = ContactInfo.Create("Street 1", "City", "State", "Country", "110111", "3105550131", ["john.doe@example.com"]);
        var job = JobInfo.Create("Title", "Company", "Department", "1", "Type", SystemClock.Instance.GetCurrentInstant(), "Office");

        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => user.UpdateProfile("John", "Doe", "john.doe@example.com", "3105550131", null, "900765432", TaxId, true, contact, job, Guid.NewGuid(), Rules));

        // Assert
        Assert.Equal(Errors.DocumentCheckDigitIsMissing.GetCode(), exception.Code);
        Assert.Equal("1023456781", user.DocumentNumber);
    }
}
