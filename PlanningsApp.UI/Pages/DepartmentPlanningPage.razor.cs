using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PlanningsApp.UI.Models.PlanningModels;
using PlanningsApp.UI.Services.Interfaces;

namespace PlanningsApp.UI.Pages;

public partial class DepartmentPlanningPage : ComponentBase
{
    private bool _isLoading = true;
    private List<PlanningEmployee> _employeesOfDepartment = new();
    private List<PlanningLine> _linesOfDepartment;
    private List<PlanningShift> _shifts;
    private List<PlanningWorkDay> _workDaysOfWeek = new();

    // Mocked data
    private DepartmentPlanning _planning = new();

    [Parameter]
    public required string DepartmentName { get; set; }

    [Inject]
    public required ILineService LineService { get; set; }

    [Inject]
    public required ISnackbar Snackbar { get; set; }

    [Inject]
    protected IEmployeeService EmployeeService { get; set; }

    [Inject]
    protected IShiftService ShiftService { get; set; }

    [Inject]
    protected ILineWorkpostService LineWorkpostService { get; set; }

    [Inject]
    protected IPlanningService PlanningService { get; set; }
    public required int SelectedWeek { get; set; } = ISOWeek.GetWeekOfYear(DateTime.Today);
    public required int SelectedYear { get; set; } = DateTime.Today.Year;
    public required int ThisWeek { get; set; } = ISOWeek.GetWeekOfYear(DateTime.Today);
    public required int ThisYear { get; set; } = DateTime.Today.Year;
    public required bool IsSaved { get; set; } = true;
    public required bool IsNewPlanning { get; set; } = false;

    protected override async Task OnParametersSetAsync()
    {
        _isLoading = true;
        StateHasChanged();
        _linesOfDepartment = await LineService.GetLinesByDepartmentNameAsync(DepartmentName);
        _shifts = await ShiftService.GetShiftsAsync();
        await LoadPlanningForCurrentPeriodAsync();
        Console.Write(SelectedWeek);
        Console.Write(SelectedYear);
        Console.Write(IsNewPlanning);
        await InvokeAsync(StateHasChanged);
        _isLoading = false;
    }

    private async Task LoadPlanningForCurrentPeriodAsync()
    {
        try
        {
            _employeesOfDepartment = await EmployeeService.GetEmployeesByDerpartmentNameAsync(
                DepartmentName
            );

            //Leave Get for workdays here, used in check of rendered items!
            GetWorkdaysOfWeek(SelectedWeek, SelectedYear);
            _planning = await PlanningService.GetPlanningAsync(
                DepartmentName,
                SelectedYear,
                SelectedWeek
            );
            if (_planning == null)
            {
                // Static data is assumed to be already fetched at this point
                _planning = await BuildPlanning();
                IsNewPlanning = true;
                Console.WriteLine(IsNewPlanning);
            }
            else
            {
                RebindEmployeeReferences();
                IsNewPlanning = false;
            }
        }
        catch (Exception e)
        {
            Snackbar.Add(e.Message, Severity.Error);
        }
        finally
        {
            IsSaved = true;
            await InvokeAsync(StateHasChanged);
            ;
            Console.WriteLine(_employeesOfDepartment);
        }
    }

    private void RebindEmployeeReferences()
    {
        foreach (PlanningLine line in _planning.Lines)
        {
            foreach (PlanningShift shift in line.Shifts)
            {
                foreach (PlanningWorkDay workDay in shift.WorkDays)
                {
                    foreach (PlanningWorkPost workpost in workDay.Workposts)
                    {
                        for (int i = 0; i < workpost.Employee.Count; i++)
                        {
                            PlanningEmployee plannedEmployee = workpost.Employee[i];
                            Identifier identifier = new Identifier();
                            identifier.ParseIdentifier(plannedEmployee.Identifier.ToString());

                            PlanningEmployee departmentEmployee = _employeesOfDepartment
                                .Where(employee =>
                                    employee.Id == plannedEmployee.Id
                                    && employee.Identifier.LineName == "Not Planned"
                                )
                                .FirstOrDefault();
                            departmentEmployee.Identifier = identifier;
                            workpost.Employee.Remove(plannedEmployee);
                            workpost.Employee.Add(departmentEmployee);
                            CreateNewDropItem(departmentEmployee);
                        }
                    }
                }
            }
        }

        Console.WriteLine(_planning);
        Console.WriteLine(_employeesOfDepartment);
        StateHasChanged();
    }

    private async Task<DepartmentPlanning> BuildPlanning()
    {
        DepartmentPlanning planning = new DepartmentPlanning();
        planning.DepartmentName = DepartmentName;
        planning.Year = SelectedYear;
        planning.Week = SelectedWeek;
        planning.Lines = _linesOfDepartment;

        foreach (PlanningLine line in planning.Lines)
        {
            List<PlanningShift> newShiftsForLine = new List<PlanningShift>();

            foreach (PlanningShift shift in _shifts)
            {
                PlanningShift newShift = new PlanningShift()
                {
                    Name = shift.Name,
                    StartTime = shift.StartTime,
                    EndTime = shift.EndTime,
                };

                // Create a NEW list of WorkDays for EACH Shift
                List<PlanningWorkDay> workDaysForShift = new();
                List<PlanningWorkPost> workPostsOfLine =
                    await LineWorkpostService.GetWorkpostsByLineId(line.Id);
                DateTime dayofSelectedWeek = ISOWeek.ToDateTime(
                    SelectedYear,
                    SelectedWeek,
                    DayOfWeek.Monday
                );

                for (int j = 0; j < 5; j++)
                {
                    List<PlanningWorkPost> clonedWorkPosts = workPostsOfLine
                        .Select(post => new PlanningWorkPost
                        {
                            LineId = post.LineId,
                            Name = post.Name,
                            Employee = new List<PlanningEmployee>(),
                        })
                        .ToList();
                    workDaysForShift.Add(
                        new PlanningWorkDay()
                        {
                            Date = dayofSelectedWeek.AddDays(j),
                            Workposts = clonedWorkPosts,
                        }
                    );
                }

                newShift.WorkDays = workDaysForShift;
                newShiftsForLine.Add(newShift);
            }

            line.Shifts = newShiftsForLine;
        }
        return planning;
    }

    private void GetWorkdaysOfWeek(int week, int year)
    {
        _workDaysOfWeek.Clear();
        DateTime dayofSelectedWeek = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
        for (int i = 0; i < 5; i++)
        {
            _workDaysOfWeek.Add(new PlanningWorkDay() { Date = dayofSelectedWeek.AddDays(i) });
        }
    }

    public string GenerateIdentifier(
        string lineName,
        string shiftName,
        string shortWorkDate,
        string workpostName
    )
    {
        return new Identifier()
        {
            LineName = lineName,
            ShiftName = shiftName,
            WorkDate = shortWorkDate,
            WorkpostName = workpostName,
        }.ToString();
    }

    public async Task PlanEmployee(MudItemDropInfo<PlanningEmployee> dropItem)
    {
        bool isAvailable = await EmployeeService.CheckEmployeeAvailabilityByDate(dropItem);
        PlanningEmployee employeeClone = new PlanningEmployee(dropItem.Item);

        Identifier newIdentifier = new Identifier();
        if (dropItem.DropzoneIdentifier == "Not Planned" && !isAvailable)
        {
            dropItem.Item.Identifier.LineName = "Not Planned";
            Snackbar.Add("Employee is absent for this day.", Severity.Warning);
        }
        else
            newIdentifier.ParseIdentifier(dropItem.DropzoneIdentifier);

        dropItem.Item.Identifier = newIdentifier;
        UpdatePlanning(dropItem.Item, "add");
        CreateNewDropItem(dropItem.Item);
        await InvokeAsync(StateHasChanged);
        ;
    }

    private void CreateNewDropItem(PlanningEmployee dropItem)
    {
        PlanningEmployee newEmployee = new PlanningEmployee(dropItem);
        _employeesOfDepartment.Add(newEmployee);
        newEmployee.TimesGenerated++;
        StateHasChanged();
    }

    private async Task SavePlanning(DepartmentPlanning planning)
    {
        try
        {
            Snackbar.Add("Saving...", Severity.Warning);
            if (IsNewPlanning)
                await PlanningService.CreatePlanningAsync(planning);
            else
                await PlanningService.UpdatePlanningAsync(planning);
            IsNewPlanning = false;
            IsSaved = true;
            await InvokeAsync(StateHasChanged);
            ;
            Snackbar.Add("Planning saved.", Severity.Success);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void UpdatePlanning(PlanningEmployee employee, string method)
    {
        PlanningLine plannedLine = _planning
            .Lines.Where(line => line.Name == employee.Identifier.LineName)
            .First();
        PlanningShift plannedShift = plannedLine
            .Shifts.Where(shift => shift.Name == employee.Identifier.ShiftName)
            .First();
        PlanningWorkDay plannedWorkDay = plannedShift
            .WorkDays.Where(workday => workday.Date.ToString("d") == employee.Identifier.WorkDate)
            .First();
        PlanningWorkPost plannedWorkPost = plannedWorkDay
            .Workposts.Where(workpost => workpost.Name == employee.Identifier.WorkpostName)
            .First();
        switch (method)
        {
            case "remove":
                plannedWorkPost.Employee.Remove(employee);
                PlanningEmployee notPlannedEmployee = _employeesOfDepartment
                    .Where(e => e.Id == employee.Id && e.Identifier.LineName == "Not Planned")
                    .First();
                notPlannedEmployee.TimesGenerated--;
                break;
            case "add":
                plannedWorkPost.Employee.Add(employee);
                break;
        }
        IsSaved = false;
        Console.Write(_planning);
        StateHasChanged();
    }

    public void RemoveFromPlanning(PlanningEmployee employee)
    {
        _employeesOfDepartment.Remove(employee);
        UpdatePlanning(employee, "remove");
        StateHasChanged();
    }

    public bool EmployeeCanBePlanned(
        PlanningEmployee employee,
        PlanningWorkPost workpost,
        PlanningWorkDay workday,
        DepartmentPlanning departmentPlanning
    )
    {
        List<PlanningLine> planningLines = _planning.Lines;
        List<PlanningShift> shifts = planningLines.SelectMany(line => line.Shifts).ToList();
        List<PlanningWorkDay> workDays = shifts
            .SelectMany(shift => shift.WorkDays)
            .Where(w => w.Date == workday.Date)
            .ToList();
        List<PlanningWorkPost> workPosts = workDays.SelectMany(w => w.Workposts).ToList();
        List<PlanningEmployee> plannedEmployees = workPosts.SelectMany(w => w.Employee).ToList();
        bool employeeAlreadyPlanned = false;
        if (plannedEmployees.Any(e => e.Id == employee.Id))
        {
            employeeAlreadyPlanned = true;
        }
        bool workpostIsEmpty = workpost.Employee.Count < 1;
        if (employeeAlreadyPlanned)
        {
            Snackbar.Add(
                $"{employee.FirstName} {employee.LastName[0]}. is already planned for this day.",
                Severity.Warning
            );
        }
        return workpostIsEmpty && !employeeAlreadyPlanned;
    }

    private async Task OnYearChanged(int newYear)
    {
        if (SelectedYear != newYear)
        {
            SelectedYear = newYear;
            await LoadPlanningForCurrentPeriodAsync();
        }
    }

    private async Task OnWeekChanged(int newWeek)
    {
        if (SelectedWeek != newWeek)
        {
            SelectedWeek = newWeek;
            await LoadPlanningForCurrentPeriodAsync();
        }
    }
}
