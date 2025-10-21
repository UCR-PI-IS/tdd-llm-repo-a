using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.ValueObjects.AccountManagement;
namespace UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Entities.AccountManagement;

public class Student : Person
{
    public string StudentId { get; set; }
    public Email InstitutionalEmail { get; set; }

    /// <summary>
    /// Constructor for the Student entity
    /// </summary>
    /// <param name="email"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="phone"></param>
    /// <param name="birthDate"></param>
    /// <param name="identityNumber"></param>
    /// <param name="studentId"></param>
    /// <param name="institutionalEmail"></param>
    public Student(Email email, string firstName, string lastName, Phone phone, BirthDate birthDate, IdentityNumber identityNumber, string studentId, Email institutionalEmail)
        : base(email, firstName, lastName, phone, birthDate, identityNumber)
    {
        StudentId = studentId;
        InstitutionalEmail = institutionalEmail;
    }

    /// <summary>
    /// Private parameterless constructor to the entity
    /// </summary>
    protected Student() : base()
    {
        StudentId = null!;
        InstitutionalEmail = null!;
    }
}
