using System.Globalization;

using static System.Runtime.InteropServices.JavaScript.JSType;

// Create an application that identifies the pair of employees who have worked
// together on common projects for the longest period of time.
// 1) DateTo can be NULL, equivalent to today.

// Read and parse the CSV file
var filePath = Path.Combine(Directory.GetCurrentDirectory(), "data", "EmployeeProjects.csv");
var lines = File.ReadAllLines(filePath);

var parsedData = FileParser.ParseEmployeeProjectWorkPeriods(lines);
var calculatedData = EmployeeOverlapCalculator.CalculateEmployeePairOverlaps(parsedData);
var longestPair = EmployeePairAnalyzer.FindLongestPair(calculatedData);
if (longestPair != null)
{
    Console.WriteLine($"Employees {longestPair.Employee1Id} and {longestPair.Employee2Id} worked together for {longestPair.TotalDaysWorkedTogether} days.");
}
else
{
    Console.WriteLine("No overlapping work periods found.");
}

var x = 5;  

static class FileParser
{
    public static List<EmployeeProjectWorkPeriod> ParseEmployeeProjectWorkPeriods(string[] lines)
    {
        var result = new List<EmployeeProjectWorkPeriod>();
        var today = DateTime.Now;
        var formats = new[] { "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy", "yyyy/MM/dd", "dd.MM.yyyy" };

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Split(',');
            if (parts.Length < 4) continue;

            int employeeId = int.Parse(parts[0]);
            int projectId = int.Parse(parts[1]);

            DateTime startDate;
            DateTime endDate;

            // Try to parse StartDate
            if (!DateTime.TryParseExact(parts[2], formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            {
                if (!DateTime.TryParse(parts[2], out startDate))
                    continue; // Skip if cannot parse
            }

            // Try to parse EndDate or use today
            if (string.IsNullOrWhiteSpace(parts[3]))
            {
                endDate = today;
            }
            else if (!DateTime.TryParseExact(parts[3], formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                if (!DateTime.TryParse(parts[3], out endDate))
                    endDate = today; // Fallback to today if cannot parse
            }

            result.Add(new EmployeeProjectWorkPeriod
            {
                EmployeeId = employeeId,
                ProjectId = projectId,
                StartDate = startDate,
                EndDate = endDate
            });
        }

        return result;
    }
}

static class EmployeeOverlapCalculator
{
    public static Dictionary<(int, int), int> CalculateEmployeePairOverlaps(List<EmployeeProjectWorkPeriod> workPeriods)
    {
        var pairDurations = new Dictionary<(int, int), int>();

        var projects = GroupByProject(workPeriods);
        foreach (var employees in projects)
        {
            var pairs = GetEmployeePairs(employees);
            foreach (var (emp1, emp2) in pairs)
            {
                int overlapDays = CalculateOverlapDays(emp1.StartDate, emp1.EndDate, emp2.StartDate, emp2.EndDate);
                if (overlapDays > 0)
                {
                    AddOrUpdatePairDuration(pairDurations, emp1.EmployeeId, emp2.EmployeeId, overlapDays);
                }
            }
        }

        return pairDurations;
    }

    // Groups work periods by project
    private static List<List<EmployeeProjectWorkPeriod>> GroupByProject(List<EmployeeProjectWorkPeriod> workPeriods)
        => workPeriods
            .GroupBy(wp => wp.ProjectId)
            .Select(g => g.ToList())
            .ToList();

    // Gets all unique employee pairs within a project
    private static List<(EmployeeProjectWorkPeriod, EmployeeProjectWorkPeriod)> GetEmployeePairs(List<EmployeeProjectWorkPeriod> employees)
    {
        var pairs = new List<(EmployeeProjectWorkPeriod, EmployeeProjectWorkPeriod)>();
        for (int i = 0; i < employees.Count; i++)
        {
            for (int j = i + 1; j < employees.Count; j++)
            {
                pairs.Add((employees[i], employees[j]));
            }
        }
        return pairs;
    }

    // Adds or updates the overlap duration for a pair
    private static void AddOrUpdatePairDuration(Dictionary<(int, int), int> pairDurations, int emp1, int emp2, int overlapDays)
    {
        var employeesPair = emp1 < emp2 ? (emp1, emp2) : (emp2, emp1);

        // Initialize if not present
        if (!pairDurations.ContainsKey(employeesPair))
        {
            pairDurations[employeesPair] = 0;
        }

        pairDurations[employeesPair] += overlapDays;
    }

    // Calculates the overlap in days between two date ranges (inclusive)
    public static int CalculateOverlapDays(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
    {
        var overlapStart = start1 > start2 ? start1 : start2;
        var overlapEnd = end1 < end2 ? end1 : end2;

        // If overlapStart is after overlapEnd, there is no overlap
        if (overlapStart > overlapEnd)
        {
            // No overlap
            return 0;
        }

        return (overlapEnd - overlapStart).Days + 1;
    }
}

static class EmployeePairAnalyzer
{
    // Finds the employee pair with the highest accumulated overlap
    public static EmployeePairWorkedTogether FindLongestPair(Dictionary<(int, int), int> pairDurations)
    {
        if (pairDurations == null || pairDurations.Count == 0)
            return null;

        var maxPair = pairDurations.OrderByDescending(p => p.Value).First();
        return new EmployeePairWorkedTogether
        {
            Employee1Id = maxPair.Key.Item1,
            Employee2Id = maxPair.Key.Item2,
            TotalDaysWorkedTogether = maxPair.Value
        };
    }
}

