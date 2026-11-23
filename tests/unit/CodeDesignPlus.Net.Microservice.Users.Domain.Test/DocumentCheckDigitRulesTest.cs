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

    /// <summary>
    /// Las mismas reglas con la longitud fija del número antes del guion para el tipo que lleva dígito.
    /// </summary>
    private static readonly DocumentCheckDigitRules FixedLength = Rules with { BaseLengths = new Dictionary<string, int> { [WithCheckDigit] = 9 } };

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
        var exception = Assert.Throws<CodeDesignPlusException>(() => Rules.Ensure(WithCheckDigit, "901888777"));

        // Assert
        Assert.Equal(Errors.DocumentCheckDigitIsMissing.GetCode(), exception.Code);
        Assert.Equal("Este tipo de documento se escribe con guion y dígito de verificación. Si 901888777 es el número sin el dígito, escríbelo así: 901888777-3.", exception.Error!.Value.GetMessage("es"));
    }

    [Fact]
    public void Ensure_NumberWithTheDigitAttached_DoesNotGuessWhichDigitItIs()
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => Rules.Ensure(WithCheckDigit, "9018887773"));

        // Assert
        Assert.Equal(Errors.DocumentCheckDigitIsMissing.GetCode(), exception.Code);
        Assert.StartsWith("Este tipo de documento se escribe con guion y dígito de verificación. Si 9018887773 es el número sin el dígito", exception.Error!.Value.GetMessage("es"));
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

    [Fact]
    public void Ensure_FixedLengthAndTheDigitAttached_ProposesSplittingTheLastDigit()
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => FixedLength.Ensure(WithCheckDigit, "9018887773"));

        // Assert
        Assert.Equal(Errors.DocumentCheckDigitIsNotSeparated.GetCode(), exception.Code);
        Assert.Equal("Este tipo de documento se escribe con guion y dígito de verificación. Escríbelo así: 901888777-3.", exception.Error!.Value.GetMessage("es"));
    }

    [Fact]
    public void Ensure_FixedLengthAndTheNumberWithoutItsDigit_ProposesTheNumberWithItsDigit()
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => FixedLength.Ensure(WithCheckDigit, "901888777"));

        // Assert
        Assert.Equal(Errors.DocumentCheckDigitIsNotSeparated.GetCode(), exception.Code);
        Assert.Equal("Este tipo de documento se escribe con guion y dígito de verificación. Escríbelo así: 901888777-3.", exception.Error!.Value.GetMessage("es"));
    }

    [Theory]
    [InlineData("9018887773-5", "Este tipo de documento lleva 9 dígitos antes del guion, y el número que escribiste tiene 10.")]
    [InlineData("90188877-1", "Este tipo de documento lleva 9 dígitos antes del guion, y el número que escribiste tiene 8.")]
    public void Ensure_FixedLengthAndAnotherLengthBeforeTheDash_ThrowsBaseLengthIsInvalid(string documentNumber, string expected)
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => FixedLength.Ensure(WithCheckDigit, documentNumber));

        // Assert
        Assert.Equal(Errors.DocumentBaseLengthIsInvalid.GetCode(), exception.Code);
        Assert.Equal(expected, exception.Error!.Value.GetMessage("es"));
    }

    [Theory]
    [InlineData("9018887775")]
    [InlineData("90188877")]
    [InlineData("90188877731")]
    public void Ensure_FixedLengthAndAnotherNumberWithoutDash_DoesNotGuess(string documentNumber)
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => FixedLength.Ensure(WithCheckDigit, documentNumber));

        // Assert
        Assert.Equal(Errors.DocumentCheckDigitIsMissing.GetCode(), exception.Code);
        Assert.StartsWith($"Este tipo de documento se escribe con guion y dígito de verificación. Si {documentNumber} es el número sin el dígito", exception.Error!.Value.GetMessage("es"));
    }

    [Fact]
    public void Ensure_FixedLengthAndTheNumberWithItsCheckDigit_DoesNotThrow()
    {
        // Act
        var exception = Record.Exception(() => FixedLength.Ensure(WithCheckDigit, "901888777-3"));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void Ensure_FixedLengthAndWrongCheckDigit_ThrowsDoesNotMatch()
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => FixedLength.Ensure(WithCheckDigit, "901888777-5"));

        // Assert
        Assert.Equal(Errors.DocumentCheckDigitDoesNotMatch.GetCode(), exception.Code);
    }

    [Fact]
    public void Ensure_FixedLengthAndNumberThatIsNotDigits_ThrowsFormatIsInvalid()
    {
        // Act
        var exception = Assert.Throws<CodeDesignPlusException>(() => FixedLength.Ensure(WithCheckDigit, "901.888.777-3"));

        // Assert
        Assert.Equal(Errors.DocumentNumberFormatIsInvalid.GetCode(), exception.Code);
    }

    [Fact]
    public void Ensure_FixedLengthOfAnotherType_ChangesNothing()
    {
        // Arrange
        var rules = Rules with { BaseLengths = new Dictionary<string, int> { ["13"] = 9 } };

        // Act
        var attached = Assert.Throws<CodeDesignPlusException>(() => rules.Ensure(WithCheckDigit, "9018887773"));
        var longer = Record.Exception(() => rules.Ensure(WithCheckDigit, "9018887773-5"));

        // Assert
        Assert.Equal(Errors.DocumentCheckDigitIsMissing.GetCode(), attached.Code);
        Assert.Null(longer);
    }

    [Theory]
    [InlineData("31", 9)]
    [InlineData(" 31 ", 9)]
    [InlineData("13", null)]
    [InlineData(null, null)]
    public void BaseLengthOf_DocumentType_ReturnsItsConfiguredLength(string? documentTypeCode, int? expected)
    {
        // Act
        var length = FixedLength.BaseLengthOf(documentTypeCode);

        // Assert
        Assert.Equal(expected, length);
    }

    [Theory]
    [InlineData("es")]
    [InlineData("en")]
    [InlineData("fr")]
    [InlineData("pt")]
    public void Errors_EveryLanguage_SaysHowToWriteAFixedLengthNumber(string language)
    {
        // Act
        var notSeparated = Errors.DocumentCheckDigitIsNotSeparated.With("901888777", 3).GetMessage(language);
        var length = Errors.DocumentBaseLengthIsInvalid.With(9, 10).GetMessage(language);

        // Assert
        Assert.Contains("901888777-3", notSeparated);
        Assert.DoesNotContain("{", notSeparated);
        Assert.Contains("9", length);
        Assert.Contains("10", length);
        Assert.DoesNotContain("{", length);
        Assert.Equal(language == "en", notSeparated == Errors.DocumentCheckDigitIsNotSeparated.With("901888777", 3).ToEnglish());
        Assert.Equal(language == "en", length == Errors.DocumentBaseLengthIsInvalid.With(9, 10).ToEnglish());
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
        var exception = Assert.Throws<CodeDesignPlusException>(() => UserAggregate.Create(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", "3105550131", null, "901888777", TaxId, true, Rules));

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
        var exception = Assert.Throws<CodeDesignPlusException>(() => user.UpdateProfile("John", "Doe", "john.doe@example.com", "3105550131", null, "901888777", TaxId, true, contact, job, Guid.NewGuid(), Rules));

        // Assert
        Assert.Equal(Errors.DocumentCheckDigitIsMissing.GetCode(), exception.Code);
        Assert.Equal("1023456781", user.DocumentNumber);
    }
}
