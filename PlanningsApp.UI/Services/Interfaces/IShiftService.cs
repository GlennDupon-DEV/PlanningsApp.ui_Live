using PlanningsApp.Ui.Models.PlanningModels;

namespace PlanningsApp.Ui.Services.Interfaces;

public interface IShiftService
{
    Task<List<PlanningShift>> GetShiftsAsync();
}
