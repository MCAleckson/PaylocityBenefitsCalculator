using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DependentsController : ControllerBase
{
    private EmployeesController employeesController = new EmployeesController();

    internal List<GetDependentDto> GetDependentListFromDB()      // Mocking list of employees loaded from DB - declared internal so Swagger can't see the method but other classes in this assembly can see it.
    {
        List<GetDependentDto> dependentList = new List<GetDependentDto>();

        // MA - Mocked loading from DB
        List<GetEmployeeDto> employeeList = new List<GetEmployeeDto>();
        employeeList = employeesController.GetEmployeeListFromDB();
        // ***

        // Gather all dependents and place in dependentList.  Again, in the real world this messy list traversal would be replaced by REST API calls to a backend SQL database - never this...
        foreach (GetEmployeeDto employeeDto in employeeList)
        {
            foreach (GetDependentDto dependentDto in employeeDto.Dependents)
            {
                dependentList.Add(dependentDto);
            }
        }

        return dependentList;
    }

    [SwaggerOperation(Summary = "Get dependent by id")]
    [HttpGet("/GetDependent/{dependentID}")]
    public async Task<ActionResult<ApiResponse<GetDependentDto>>> GetDependent(int dependentID)
    {
        List<GetDependentDto> dependentList = new List<GetDependentDto>();
        GetDependentDto dependentDto = new GetDependentDto();
        ApiResponse<GetDependentDto> result = new ApiResponse<GetDependentDto>();
        ActionResult<ApiResponse<List<GetDependentDto>>> dependentResult;

        dependentResult = await GetAllDependents();
        dependentList = this.GetDependentListFromDB();


        if (dependentList.Count > 0)
        {
            dependentDto = dependentList.Find(item => item.Id == dependentID);


            if (dependentDto != null)
            {
                if (dependentDto.Relationship == Relationship.DomesticPartner || dependentDto.Relationship==Relationship.Spouse || dependentDto.Relationship == Relationship.Child)
                {
                    result.Success = true;
                    result.Data = dependentDto;
                }
                else
                {
                    result.Success = false;
                    result.Error = "Error:  Dependent relationship incorrectly specified.";
                    result.Data = dependentDto;
                }
            }
            else
            {
                result.Success = false;
                result.Error = "Error:  Dependent with supplied ID not found.";
            }
        }
        else
        {
            result.Success = false;
            result.Error = "Error:  No dependents exist.";
        }
        return result;
    }

    [SwaggerOperation(Summary = "Get all dependents")]
    [HttpGet("/GetAllDependents/")]
    public async Task<ActionResult<ApiResponse<List<GetDependentDto>>>> GetAllDependents()
    {
        ApiResponse<List<GetDependentDto>> result = new ApiResponse<List<GetDependentDto>>();
        List<GetDependentDto> dependentList = new List<GetDependentDto>();

        dependentList = this.GetDependentListFromDB();

        if (dependentList.Count > 0)
        {
            result.Success = true;
            result.Data = dependentList;
        }
        else
        {
            result.Success = false;
            result.Error = "Error:  No dependents found in database.";
        }
        return result;
    }
}
