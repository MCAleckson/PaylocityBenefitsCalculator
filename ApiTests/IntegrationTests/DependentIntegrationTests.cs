using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Newtonsoft.Json;
using Xunit;

namespace ApiTests.IntegrationTests;

public class DependentIntegrationTests : IntegrationTest
{
    [Fact]
    //task: make test pass
    public async Task WhenAskedForAllDependents_ShouldReturnAllDependents()
    {
        HttpResponseMessage response;
        string result;

        response = await HttpClient.GetAsync("/GetAllDependents");

        using (HttpContent content = response.Content)
        {
            result = await response.Content.ReadAsStringAsync();
        }

        // Load all dependents via the API
        ApiResponse<List<GetDependentDto>> apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<GetDependentDto>>>(result);
        List<GetDependentDto> loadedDependentDtoList = (List<GetDependentDto>)apiResponse.Data;

        // Manually-entered list of dependent data to compare with
        var testDependentDtoList = new List<GetDependentDto>
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
            },
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
            },
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
        };

        int listCount = testDependentDtoList.Count;

        Assert.True(testDependentDtoList.Count == loadedDependentDtoList.Count);  // Check that list count between loaded and test lists are the same

        for (int dependentNum = 0; dependentNum < listCount; dependentNum++)
        {
            Assert.True(testDependentDtoList[dependentNum].FirstName == loadedDependentDtoList[dependentNum].FirstName);
            Assert.True(testDependentDtoList[dependentNum].LastName == loadedDependentDtoList[dependentNum].LastName);
            Assert.True(testDependentDtoList[dependentNum].Age == loadedDependentDtoList[dependentNum].Age);
            Assert.True(testDependentDtoList[dependentNum].DateOfBirth == loadedDependentDtoList[dependentNum].DateOfBirth);
            Assert.True(testDependentDtoList[dependentNum].Relationship == loadedDependentDtoList[dependentNum].Relationship);
        }
    }

    [Fact]
    //task: make test pass
    public async Task WhenAskedForADependent_ShouldReturnCorrectDependent()
    {
        HttpResponseMessage response;
        string result;

        response = await HttpClient.GetAsync("/GetDependent/1");


        using (HttpContent content = response.Content)
        {
            result = await response.Content.ReadAsStringAsync();
        }

        ApiResponse<GetDependentDto> apiResponse = JsonConvert.DeserializeObject<ApiResponse<GetDependentDto>>(result);
        GetDependentDto loadedDependentDto = (GetDependentDto)apiResponse.Data;

        var testDependent = new GetDependentDto
        {
            Id = 1,
            FirstName = "Spouse",
            LastName = "Morant",
            Relationship = Relationship.Spouse,
            DateOfBirth = new DateTime(1998, 3, 3)
        };

        Assert.True(testDependent.Id == loadedDependentDto.Id);
        Assert.True(testDependent.FirstName == loadedDependentDto.FirstName);
        Assert.True(testDependent.LastName == loadedDependentDto.LastName);
        Assert.True(testDependent.Age == loadedDependentDto.Age);
        Assert.True(testDependent.DateOfBirth == loadedDependentDto.DateOfBirth);
    }

    [Fact]
    //task: make test pass
    public async Task WhenAskedForANonexistentDependent_ShouldReturnNotFound()
    {
        HttpResponseMessage response;
        string result;

        response = await HttpClient.GetAsync("/GetDependent/0");


        using (HttpContent content = response.Content)
        {
            result = await response.Content.ReadAsStringAsync();
        }

        ApiResponse<GetEmployeeDto> apiResponse = JsonConvert.DeserializeObject<ApiResponse<GetEmployeeDto>>(result);

        Assert.True(apiResponse.Success == false && apiResponse.Error.ToLower().Contains("not found"));
    }
}

