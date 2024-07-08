using Api.Models;

namespace Api.Dtos.EmployeePaycheck;
public class GetEmployeePaycheckDto
{
    public int Id { get; set; }
    public string GrossSalaryForPeriod { get; set; }
    public string DeductionsForPeriod { get; set; }
    public string NetSalaryForPeriod { get; set; }
}
