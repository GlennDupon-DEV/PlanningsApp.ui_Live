using PlanningsApp.UI.Models;

namespace PlanningsApp.UI.Services.Interfaces;

public interface IDepartmentService
{
    Task<List<Department>> GetDepartmentsAsync();
}
