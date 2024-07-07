using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Dtos.EmployeePaycheck;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{

    internal List<GetEmployeeDto> GetEmployeeListFromDB()      // Mocking list of employees loaded from DB - declared internal so Swagger can't see the methold but other classes in this assembly can see it.
    {
        var employees = new List<GetEmployeeDto>
        {
            new()
            {
                Id = 1,
                FirstName = "LeBron",
                LastName = "James",
                Salary = 75420.99m,
                DateOfBirth = new DateTime(1984, 12, 30)
            },
            new()
            {
                Id = 2,
                FirstName = "Ja",
                LastName = "Morant",
                Salary = 92365.22m,
                DateOfBirth = new DateTime(1999, 8, 10),
                Dependents = new List<GetDependentDto>
                {
                    new()
                    {
                        Id = 1,
                        FirstName = "Spouse",
                        LastName = "Morant",
                        Relationship = Relationship.Spouse,
                        DateOfBirth = new DateTime(1998, 3, 3)
                    },
                    new()
                    {
                        Id = 2,
                        FirstName = "Child1",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2020, 6, 23)
                    },
                    new()
                    {
                        Id = 3,
                        FirstName = "Child2",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2021, 5, 18)
                    }
                }
            },
            new()
            {
                Id = 3,
                FirstName = "Michael",
                LastName = "Jordan",
                Salary = 143211.12m,
                DateOfBirth = new DateTime(1963, 2, 17),
                Dependents = new List<GetDependentDto>
                {
                    new()
                    {
                        Id = 4,
                        FirstName = "Spouse",
                        LastName = "Jordan",
                        Relationship = Relationship.Spouse,
                        DateOfBirth = new DateTime(1974, 1, 2)
                    },
                    new()
                    {
                        Id = 5,
                        FirstName = "DP",
                        LastName = "Jordan",
                        Relationship = Relationship.DomesticPartner,
                        DateOfBirth = new DateTime(1970, 10, 2)
                    }
                }
            },
            new()
            {
                Id = 4,
                FirstName = "Shaquille",
                LastName = "O'Neal",
                Salary = 2080000.00m,
                //Salary = 143211.12m,
                DateOfBirth = new DateTime(1963, 2, 17),
                Dependents = new List<GetDependentDto>
                {
                    new()
                    {
                        Id = 6,
                        FirstName = "Spouse",
                        LastName = "O'Neal",
                        Relationship = Relationship.Spouse,
                        DateOfBirth = new DateTime(1974, 1, 2)
                    },
                    new()
                    {
                        Id = 7,
                        FirstName = "Child1",
                        LastName = "O'Neal",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(1985, 3, 2)
                    },
                    new()
                    {
                        Id = 8,
                        FirstName = "Child2",
                        LastName = "O'Neal",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(1989, 5, 2)
                    }
                }
            }
        };

    return employees;
    }

    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("/GetEmployee/{employeeID}")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> GetEmployee(int employeeID)  // Returns an existing employee by ID, and runs spouse/domestic partner business rule.
    {
        // Though not required here, chose async task because in live system, async httpclient post calls would likely be being made, requiring this type of method.

        // Mock retrieval of employee list from DB
        List<GetEmployeeDto> employeeList = new List<GetEmployeeDto>();
        employeeList = GetEmployeeListFromDB();
        // ***

        GetEmployeeDto employee = new GetEmployeeDto();
        ApiResponse<GetEmployeeDto> result = new ApiResponse<GetEmployeeDto>();

        if (employeeList.Count > 0)
        {
            employee = employeeList.Find(item => item.Id == employeeID);

            if (employee != null)
            {
                int dpOrSpouseCount = 0;
                foreach (var dependent in employee.Dependents)
                {
                    if (dependent.Relationship == Relationship.DomesticPartner || dependent.Relationship == Relationship.Spouse) dpOrSpouseCount++;
                }

                if (dpOrSpouseCount < 2)
                {
                    result.Success = true;
                    result.Data = employee;
                }
                else
                {
                    result.Success = false;
                    result.Error = "Error:  Employee data business rule exception - Employee has more than one spouse or domestic partner.";
                    result.Data = employee;
                }
            }
            else
            {
                result.Success = false;
                result.Error = "Error:  Employee with supplied ID not found.";
            }
        }
        else
        {
            result.Success = false;
            result.Error = "Error:  Employee list retrieval failed.";
        }
        return result;
    }

    [SwaggerOperation(Summary = "Get an employee's paycheck list")]
    [HttpGet("/GetPaycheckList/{employeeID}")]
    public async Task<ActionResult<ApiResponse<GetEmployeePaycheckListDto>>> GetPaycheckList(int employeeID)
    {
        int payPeriodsPerYear = 26;

        GetEmployeeDto employee = new GetEmployeeDto();
        ActionResult<ApiResponse<GetEmployeeDto>> employeeResult;

        GetEmployeePaycheckListDto paycheckList = new GetEmployeePaycheckListDto();
        ApiResponse<GetEmployeePaycheckListDto> paycheckListResult = new ApiResponse<GetEmployeePaycheckListDto>
        {
            Data = paycheckList,
            Success = false
        };

        employeeResult = await GetEmployee(employeeID);

        if (employeeResult.Value.Success == true)
        {
            employee = employeeResult.Value.Data;
            if (employee != null)
            { 
                paycheckList.EmployeeId = employee.Id;
                paycheckList.FirstName = employee.FirstName;
                paycheckList.LastName = employee.LastName;

                if (employeeResult.Value.Success != false)   // Build paycheck list
                {
                    // Initialize annual variables
                    decimal grossAnnualSalary = 0;
                    decimal grossAnnualDeductions = 0;

                    // Initialize monthly variables
                    decimal monthlyEmployeeSalary = 0;
                    decimal monthlyDefaultDeduction = 0;
                    decimal monthlyDependentsDeduction = 0;
                    decimal monthlyAdditionalDependentsOver50Deduction = 0;
                    decimal monthlyEmployeeSalaryOver80KDeduction = 0;

                    // Initialize per pay period variables
                    decimal grossSalaryPerPeriod = 0;
                    decimal totalDeductionsPerPeriod = 0;

                    // Set monthly variables
                    monthlyEmployeeSalary = employee.Salary / 12;
                    monthlyDefaultDeduction = 1000;                                     // Set default employee deduction for benefits per month
                    monthlyDependentsDeduction = 600 * employee.Dependents.Count();     // Set default dependent deduction per dependent per month
                    if (employee.Salary >= 80000)                                       // Set over $80K salary additional deduction per month (ON MONTHLY SALARY)
                    {
                        monthlyEmployeeSalaryOver80KDeduction = monthlyEmployeeSalary * .02m;  
                    }
                    foreach (GetDependentDto dependentDto in employee.Dependents)       // Set dependents over 50 additional deductions
                    {
                        if (dependentDto.Age >= 50) monthlyAdditionalDependentsOver50Deduction += 200;
                    }

                    // Set annual variables (to calculate rounding error and adjust during first pay period)
                    grossAnnualSalary = employee.Salary;
                    grossAnnualDeductions = (monthlyDefaultDeduction + monthlyDependentsDeduction + monthlyAdditionalDependentsOver50Deduction + monthlyEmployeeSalaryOver80KDeduction) * 12;

                    // Set per pay period variables
                    grossSalaryPerPeriod = monthlyEmployeeSalary * 12 / payPeriodsPerYear;
                    totalDeductionsPerPeriod = grossAnnualDeductions / payPeriodsPerYear;
                    
                    // Create pay check list
                    paycheckList.FirstName = employee.FirstName;
                    paycheckList.LastName = employee.LastName;
                    paycheckList.AnnualSalary = String.Format("{0:C}",grossAnnualSalary);
                    paycheckList.AnnualDeductions = String.Format("{0:C}",grossAnnualDeductions);

                    for (int i = 1; i <= payPeriodsPerYear; i++)
                    {
                        GetEmployeePaycheckDto paycheck = new GetEmployeePaycheckDto();

                        paycheck.Id = i;
                        if (i==1)  // resolve rounding anomalies during first pay period of the year
                        {
                            decimal grossSalaryForPeriodRoundingError = grossAnnualSalary - (Math.Round(grossSalaryPerPeriod, 2, MidpointRounding.ToEven) * payPeriodsPerYear);
                            decimal grossAnnualDeductionsRoundingError = grossAnnualDeductions - (Math.Round(totalDeductionsPerPeriod, 2, MidpointRounding.ToEven) * payPeriodsPerYear);

                            paycheck.GrossSalaryForPeriod = String.Format("{0:C}", Math.Round(grossSalaryPerPeriod,2,MidpointRounding.ToEven) + grossSalaryForPeriodRoundingError);
                            paycheck.DeductionsForPeriod = String.Format("{0:C}", Math.Round(totalDeductionsPerPeriod,2,MidpointRounding.ToEven) + grossAnnualDeductionsRoundingError);
                            paycheck.NetSalaryForPeriod = String.Format("{0:C}", (Math.Round(grossSalaryPerPeriod, 2, MidpointRounding.ToEven) + grossSalaryForPeriodRoundingError) - (Math.Round(totalDeductionsPerPeriod, 2, MidpointRounding.ToEven) + grossAnnualDeductionsRoundingError));
                        }
                        else
                        {
                            paycheck.GrossSalaryForPeriod = String.Format("{0:C}", Math.Round(grossSalaryPerPeriod, 2, MidpointRounding.ToEven));
                            paycheck.DeductionsForPeriod = String.Format("{0:C}", Math.Round(totalDeductionsPerPeriod, 2, MidpointRounding.ToEven));
                            paycheck.NetSalaryForPeriod = String.Format("{0:C}", Math.Round(grossSalaryPerPeriod, 2, MidpointRounding.ToEven) - Math.Round(totalDeductionsPerPeriod, 2, MidpointRounding.ToEven));
                        }



                        paycheckList.Paychecks.Add(paycheck);
                    }

                    // Load result 
                    paycheckListResult.Data = paycheckList;
                    paycheckListResult.Success = true;
                    paycheckListResult.Data = paycheckList;
                }
                else
                {
                    paycheckListResult.Success = false;
                    paycheckListResult.Error = employeeResult.Value.Error; ;
                }
            }
            else
            {
                paycheckListResult.Success = false;
                paycheckListResult.Error = employeeResult.Value.Error;
            }
        }
        else
        {
            paycheckListResult.Success = false;
            paycheckListResult.Error = employeeResult.Value.Error;
        }
 
        return paycheckListResult;
    }

    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet("/GetAllEmployees/")]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAllEmployees()
    {
        ApiResponse<List<GetEmployeeDto>> result = new ApiResponse<List<GetEmployeeDto>>();

        //task: use a more realistic production approach
        //response:  I mocked a function "GetEmployeeListFromDB" which, in a real scenario, would load structured data from a SQL database.  Here, it merely supplies a hard-coded list of employees and dependents.
        List<GetEmployeeDto> employeeList = new List<GetEmployeeDto>();
        employeeList = GetEmployeeListFromDB();
        // ***

        if (employeeList.Count > 0)
        {
            result.Success = true;
            result.Data = employeeList;
        }
        else
        {
            result.Success = false;
            result.Error = "Error:  No employees found in database.";
        }
        return result;
    }


}
