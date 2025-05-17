using MudBlazor;
using PlanningsApp.Ui.Models;
using PlanningsApp.Ui.Models.PlanningModels;

namespace PlanningsApp.Ui.Services.Interfaces;

public interface IEmployeeService
{
    Task<List<Employee>> GetEmployeesAsync();
    Task<List<PlanningEmployee>> GetEmployeesByDerpartmentNameAsync(string departmentName);

    Task<bool> CheckEmployeeAvailabilityByDate(MudItemDropInfo<PlanningEmployee> dropItem);
}
