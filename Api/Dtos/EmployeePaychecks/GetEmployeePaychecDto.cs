using Api.Models;

namespace Api.Dtos.EmployeePaycheck;

public class GetEmployeePaycheckDto
{
    public int Id { get; set; }
    public decimal PeriodGrossSalary { get; set; }
    public decimal PeriodDeductions { get; set; }
    public decimal PeriodNetSalary { get; set; }
}
