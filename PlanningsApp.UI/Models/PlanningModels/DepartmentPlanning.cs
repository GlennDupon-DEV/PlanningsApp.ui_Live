using System.Globalization;

namespace PlanningsApp.Ui.Models.PlanningModels;

// Class used for building the planning
// Data structure for storing in NoSQL DB

public class DepartmentPlanning
{
    public string? Id { get; set; } // Dep(artment) - Year - Week
    public string DepartmentName { get; set; }
    public int Year { get; set; } = ISOWeek.GetYear(DateTime.Today);
    public int Week { get; set; } = ISOWeek.GetWeekOfYear(DateTime.Today);
    public List<PlanningLine> Lines { get; set; } = new();
}

public class PlanningLine
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<PlanningShift> Shifts { get; set; } //  5 standard Workdays
}

public class PlanningShift
{
    public string Name { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public List<PlanningWorkDay> WorkDays { get; set; } = new();
}

public class PlanningWorkDay
{
    public DateTime Date { get; set; }
    public List<PlanningWorkPost> Workposts { get; set; } = new();
}

public class PlanningWorkPost
{
    public int LineId { get; set; }
    public string Name { get; set; }
    public List<PlanningEmployee> Employee { get; set; } = new(); // List for MUDBLAZOR (drop zones) , only 1 employee allowed per workpost
}

public class PlanningEmployee
{
    public PlanningEmployee() { }

    public PlanningEmployee(PlanningEmployee otherEmployee)
    {
        Id = otherEmployee.Id;
        FirstName = otherEmployee.FirstName;
        LastName = otherEmployee.LastName;
        Identifier = new Identifier();
        TimesGenerated = otherEmployee.TimesGenerated;
    }

    public int Id { get; set; }
    public int TimesGenerated { get; set; } = 1;
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Identifier Identifier { get; set; } = new();
}

public class Identifier
{
    public string LineName { get; set; } = "Not Planned";
    public string? ShiftName { get; set; }
    public string? WorkDate { get; set; }
    public string? WorkpostName { get; set; }

    public override string ToString()
    {
        if (
            string.IsNullOrWhiteSpace(ShiftName)
            || string.IsNullOrWhiteSpace(WorkDate)
            || string.IsNullOrWhiteSpace(WorkpostName)
        )
            return LineName;
        return $"{LineName}|{ShiftName}|{WorkDate}|{WorkpostName}";
    }

    public void ParseIdentifier(string identifierString)
    {
        string[] splitIdentifierString = identifierString.Split(
            "|",
            StringSplitOptions.RemoveEmptyEntries
        );
        this.LineName = splitIdentifierString[0];
        this.ShiftName = splitIdentifierString[1];
        this.WorkDate = splitIdentifierString[2];
        this.WorkpostName = splitIdentifierString[3];
    }
}
