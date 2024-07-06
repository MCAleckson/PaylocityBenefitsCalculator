using Api.Dtos.Dependent;

namespace Api.Dtos.EmployeePaycheck;

public class GetEmployeePaycheckListDto
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public decimal Salary { get; set; }
    public ICollection<GetEmployeePaycheckDto> Paychecks { get; set; } = new List<GetEmployeePaycheckDto>();
}
