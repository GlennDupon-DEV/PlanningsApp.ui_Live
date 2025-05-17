using System.Reflection.Metadata.Ecma335;
using PlanningsApp.Ui.Models;
using PlanningsApp.Ui.Models;
using PlanningsApp.Ui.Models.PlanningModels;

namespace PlanningsApp.Ui.Mapper;

public static class PlanningMapper
{
    public static List<PlanningEmployee> Map(this List<Employee> employees)
    {
        return employees.Select(e => e.Map()).ToList();
    }

    public static PlanningEmployee Map(this Employee employee)
    {
        return new PlanningEmployee()
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Identifier = new Identifier(),
        };
    }

    public static List<PlanningLine> Map(this List<Line> lines)
    {
        return lines.Select(l => l.Map()).ToList();
    }

    public static PlanningLine Map(this Line line)
    {
        return new PlanningLine()
        {
            Id = line.Id,
            Name = line.Name,
            Shifts = new List<PlanningShift>(),
        };
    }

    public static List<PlanningShift> Map(this List<Shift> shifts)
    {
        return shifts.Select(s => s.Map()).ToList();
    }

    public static PlanningShift Map(this Shift shift)
    {
        return new PlanningShift()
        {
            Name = shift.Name,
            EndTime = shift.EndTime,
            StartTime = shift.StartTime,
            WorkDays = new List<PlanningWorkDay>(),
        };
    }

    public static List<PlanningWorkPost> Map(this List<LineWorkpost> lineWorkposts)
    {
        return lineWorkposts.Select(w => w.Map()).ToList();
    }

    public static PlanningWorkPost Map(this LineWorkpost lineWorkpost)
    {
        return new PlanningWorkPost()
        {
            LineId = lineWorkpost.LineId,
            Name = lineWorkpost.WorkPost.Name,
        };
    }
}
