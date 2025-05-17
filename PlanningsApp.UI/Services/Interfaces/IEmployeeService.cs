using MudBlazor;
using PlanningsApp.UI.Models;
using PlanningsApp.UI.Models.PlanningModels;

namespace PlanningsApp.UI.Services.Interfaces;

public interface IEmployeeService
{
    Task<List<Employee>> GetEmployeesAsync();
    Task<List<PlanningEmployee>> GetEmployeesByDerpartmentNameAsync(string departmentName);

    Task<bool> CheckEmployeeAvailabilityByDate(MudItemDropInfo<PlanningEmployee> dropItem);
}
