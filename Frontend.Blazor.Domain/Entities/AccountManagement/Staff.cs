using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.ValueObjects.AccountManagement;
namespace UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Entities.AccountManagement;

public class Staff : Person
{
    // Relationship between Type and Staff Member
    public string Type { get; set; }
    public Email InstitutionalEmail { get; set; }

    /// <summary> 
    /// Constructor for the Staff entity
    /// </summary>
    /// <param name="id"></param>
    /// <param name="email"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="phone"></param>
    /// <param name="birthDate"></param>
    /// <param name="identityNumber"></param>
    /// <param name="type"></param>
    /// <param name="institutionalEmail"></param>
    public Staff(Email email, string firstName, string lastName, Phone phone, BirthDate birthDate, IdentityNumber identityNumber, string type, Email institutionalEmail)
        : base(email, firstName, lastName, phone, birthDate, identityNumber)
    {
        Type = type;
        InstitutionalEmail = institutionalEmail;
    }

    protected Staff() : base()
    {
        Type = string.Empty;
        InstitutionalEmail = null!;
    }
}