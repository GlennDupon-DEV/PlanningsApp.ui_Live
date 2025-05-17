using PlanningsApp.Ui.Models;

namespace PlanningsApp.Ui.Services.Interfaces;

public interface IDepartmentService
{
    Task<List<Department>> GetDepartmentsAsync();
}
