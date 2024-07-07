using Api.Models;

namespace Api.Dtos.EmployeePaycheck;

public class GetEmployeePaycheckDto
{
    public int Id { get; set; }
    public decimal GrossSalaryForPeriod { get; set; }
    public decimal DeductionsForPeriod { get; set; }
    public decimal NetSalaryForPeriod { get; set; }
}
