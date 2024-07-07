using Api.Dtos.Dependent;

namespace Api.Dtos.EmployeePaycheck;

public class GetEmployeePaycheckListDto
{
    public int EmployeeId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public decimal AnnualSalary { get; set; }
    public ICollection<GetEmployeePaycheckDto> Paychecks { get; set; } = new List<GetEmployeePaycheckDto>();
}
