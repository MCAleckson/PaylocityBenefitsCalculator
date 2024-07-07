using Api.Models;

namespace Api.Dtos.Dependent;

public class GetDependentDto
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int Age
    {
        get
        {
            int ageInt = (int)((DateTime.Now - DateOfBirth).TotalDays / 365.242199);
            return ageInt;
        }
        set
        {
        }
    }
    public Relationship Relationship { get; set; }
}
