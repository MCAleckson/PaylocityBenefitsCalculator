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

    private List<GetEmployeeDto> GetEmployeeListFromDB()      // Mocking list of employees loaded from DB
    {
        var employees = new List<GetEmployeeDto>
        {
            new()
            {
                Id = 1,
                FirstName = "LeBron",
                LastName = "James",
                Salary = 26000,
                //Salary = 75420.99m,
                DateOfBirth = new DateTime(1984, 12, 30)
            },
            new()
            {
                Id = 2,
                FirstName = "Larry",
                LastName = "Bird",
                Salary = 260000,
                //Salary = 92365.22m,
                DateOfBirth = new DateTime(1999, 8, 10),
                Dependents = new List<GetDependentDto>
                {
                    new()
                    {
                        Id = 1,
                        FirstName = "Birdy",
                        LastName = "Bird",
                        Relationship = Relationship.Spouse,
                        DateOfBirth = new DateTime(1998, 3, 3)
                    },
                    new()
                    {
                        Id = 2,
                        FirstName = "Child1",
                        LastName = "Bird",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2020, 6, 23)
                    },
                    new()
                    {
                        Id = 3,
                        FirstName = "Child2",
                        LastName = "Bird",
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
                Salary = 2600000,
                //Salary = 143211.12m,
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
                        FirstName = "DomesticPartner",
                        LastName = "Jordan",
                        Relationship = Relationship.DomesticPartner,
                        DateOfBirth = new DateTime(1985, 3, 2)
                    }
                }
            }
        };

    return employees;
    }

    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("/GetEmployee/{id}")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> GetEmployee(int id)  // Returns an existing employee by ID, and runs spouse/domestic partner business rule.
    {
        // Mock retrieval of employee list from DB
        List<GetEmployeeDto> employeeList = new List<GetEmployeeDto>();
        employeeList = GetEmployeeListFromDB();
        // ***

        GetEmployeeDto employee = new GetEmployeeDto();
        var result = new ApiResponse<GetEmployeeDto>();

        if (employeeList.Count > 0)
        {
            employee = employeeList.Find(item => item.Id == id);

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
                    result.Error = "Employee data business rule failure:  Employee has more than 1 spouse or domestic partner";
                    result.Data = employee;
                }
            }
            else
            {
                result.Success = false;
                result.Error = "No employee with that ID exists.";
            }
        }
        else
        {
            result.Success = false;
            result.Error = "Employee list retrieval failed.";
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

        // Mock retrieval of employee list from DB
        //        List<GetEmployeeDto> employeeList = new List<GetEmployeeDto>();
        //        employeeList = GetEmployeeListFromDB();
        // ***

        //        employee = employeeList.Find(item => item.Id == employeeID);

        employeeResult = await GetEmployee(employeeID);

        if (employeeResult.Value.Success == true)
        {
            employee = employeeResult.Value.Data;
            if (employee != null)
            { 
                paycheckList.FirstName = employee.FirstName;
                paycheckList.LastName = employee.LastName;

                if (employeeResult.Value.Success != false)
                {
                    // Build out paycheck list

                    decimal defaultDeduction = 0;
                    decimal defaultDependentDeduction = 0;
                    decimal additionalDependentDeduction = 0;
                    decimal additionalDependentsOver50Deduction = 0;
                    decimal employeeSalaryOver80KDeduction = 0;

                    // Set default deductions

                    defaultDeduction = 1000 * 12 / payPeriodsPerYear;    // default employee deduction for benefits per pay period
                    defaultDependentDeduction = 600 * employee.Dependents.Count() * 12 / payPeriodsPerYear;  // default dependent deduction per pay period
                    if (employee.Salary > 80000) employeeSalaryOver80KDeduction = employee.Salary * (decimal).02 * 12 / payPeriodsPerYear;
                    
                    // Set dependent additional deductions
                    
                    // AGE CALCULATION !!!

                    foreach (GetDependentDto dependentDto in employee.Dependents)
                    {
                       //  if (dependentDto.)

                    }
                    
                    
                    
                    
                    
                    
                    for (int i = 1; i <= payPeriodsPerYear; i++)
                        {
                        GetEmployeePaycheckDto paycheck = new GetEmployeePaycheckDto();
                        
                         }

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
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll()
    {
        var result = new ApiResponse<List<GetEmployeeDto>>();

        //task: use a more realistic production approach
        // MA - Mocked loading from DB
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
            result.Error = "No employees found in database.";
        }
        return result;
    }


}
