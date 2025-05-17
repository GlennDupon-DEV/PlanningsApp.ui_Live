using PlanningsApp.UI.Models.PlanningModels;

namespace PlanningsApp.UI.Services.Interfaces;

public interface IShiftService
{
    Task<List<PlanningShift>> GetShiftsAsync();
}
