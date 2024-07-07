using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Dtos.EmployeePaycheck;
using Api.Models;
using Newtonsoft.Json;
using Xunit;

namespace ApiTests.IntegrationTests;

public class EmployeeIntegrationTests : IntegrationTest
{
    [Fact]
    public async Task WhenAskedForAllEmployees_ShouldReturnAllEmployees()
    {
        HttpResponseMessage response;
        string result;

        response = await HttpClient.GetAsync("/GetAllEmployees");

        using (HttpContent content = response.Content)
        {
            result = await response.Content.ReadAsStringAsync();
        }

        // Load all employees via the API
        ApiResponse<List<GetEmployeeDto>> apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<GetEmployeeDto>>>(result);
        List<GetEmployeeDto> loadedEmployeeDtoList = (List<GetEmployeeDto>)apiResponse.Data;

        // Manually-entered list of employee data to compare with
        var testEmployeeDtoList = new List<GetEmployeeDto>
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


        int listCount = testEmployeeDtoList.Count;

        Assert.True(testEmployeeDtoList.Count == loadedEmployeeDtoList.Count);  // Check that list count between loaded and test lists are the same

        for (int employeeNum=0; employeeNum < listCount; employeeNum++)
        {
            Assert.True(testEmployeeDtoList[employeeNum].FirstName == loadedEmployeeDtoList[employeeNum].FirstName);
            Assert.True(testEmployeeDtoList[employeeNum].LastName == loadedEmployeeDtoList[employeeNum].LastName);
            Assert.True(testEmployeeDtoList[employeeNum].Salary == loadedEmployeeDtoList[employeeNum].Salary);
            Assert.True(testEmployeeDtoList[employeeNum].DateOfBirth == loadedEmployeeDtoList[employeeNum].DateOfBirth);
            Assert.True(testEmployeeDtoList[employeeNum].Dependents.Count == loadedEmployeeDtoList[employeeNum].Dependents.Count);
        }
    }

    [Fact]
    //task: make test pass
    public async Task WhenAskedForAnEmployee_ShouldReturnCorrectEmployee()
    {
        HttpResponseMessage response;
        string result;

        response = await HttpClient.GetAsync("/GetEmployee/1");


        using (HttpContent content = response.Content)
        {
            result = await response.Content.ReadAsStringAsync();
        }

        ApiResponse<GetEmployeeDto> apiResponse = JsonConvert.DeserializeObject<ApiResponse<GetEmployeeDto>>(result);
        GetEmployeeDto loadedEmployeeDto = (GetEmployeeDto)apiResponse.Data;

        var testEmployee = new GetEmployeeDto
        {
            Id = 1,
            FirstName = "LeBron",
            LastName = "James",
            Salary = 75420.99m,
            DateOfBirth = new DateTime(1984, 12, 30)
        };

        Assert.True(testEmployee.Id == loadedEmployeeDto.Id);
        Assert.True(testEmployee.FirstName == loadedEmployeeDto.FirstName);
        Assert.True(testEmployee.LastName == loadedEmployeeDto.LastName);
        Assert.True(testEmployee.Salary == loadedEmployeeDto.Salary);
        Assert.True(testEmployee.DateOfBirth == loadedEmployeeDto.DateOfBirth);

    }
    
    [Fact]
    //task: make test pass
    public async Task WhenAskedForANonexistentEmployee_ShouldReturnNotFound()
    {
        HttpResponseMessage response;
        string result;

        response = await HttpClient.GetAsync("/GetEmployee/0");


        using (HttpContent content = response.Content)
        {
            result = await response.Content.ReadAsStringAsync();
        }

        ApiResponse<GetEmployeeDto> apiResponse = JsonConvert.DeserializeObject<ApiResponse<GetEmployeeDto>>(result);

        Assert.True(apiResponse.Success == false && apiResponse.Error.ToLower().Contains("not found"));
    }

    [Fact]
    public async Task WhenAskedForAPaycheckList_ShouldReturnCorrect()
    {
        HttpResponseMessage response;
        string result;

        response = await HttpClient.GetAsync("/GetPaycheckList/4");


        using (HttpContent content = response.Content)
        {
            result = await response.Content.ReadAsStringAsync();
        }

        ApiResponse<GetEmployeePaycheckListDto> apiResponse = JsonConvert.DeserializeObject<ApiResponse<GetEmployeePaycheckListDto>>(result);
        GetEmployeePaycheckListDto loadedEPaycheckListDto = (GetEmployeePaycheckListDto)apiResponse.Data;

        // For employee 4 - Shaq
        decimal testAnnualSalary = 2080000m;
        decimal testMonthlySalary = testAnnualSalary / 12;
        decimal testGrossSalaryForPeriod = Math.Round(testAnnualSalary / 26,2);

        // Monthly cost: (1000 base + 600*3 dependents + 200 for a dependent over 50 + 2% of (annual salary / 12 months)) * 12 months / 26 pay periods per year
        decimal testDeductionsForPeriod = Math.Round(((1000) + (600*3) + (1 * 200) + (testMonthlySalary * .02m)) * 12 / 26, 2);
        decimal testNetSalaryForPeriod = Math.Round(testGrossSalaryForPeriod - testDeductionsForPeriod,2);

        foreach (GetEmployeePaycheckDto paycheckDto in loadedEPaycheckListDto.Paychecks)
        {
            Assert.True(paycheckDto.GrossSalaryForPeriod == testGrossSalaryForPeriod);
            Assert.True(paycheckDto.DeductionsForPeriod == testDeductionsForPeriod);
            Assert.True(paycheckDto.NetSalaryForPeriod == testNetSalaryForPeriod);
        }
    }
}

