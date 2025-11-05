public class EmployeeOverlapCalculatorService : IEmployeeOverlapCalculator
{
    public Dictionary<(int, int), int> CalculateEmployeePairOverlaps(List<EmployeeProjectWorkPeriod> workPeriods)
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

    private List<List<EmployeeProjectWorkPeriod>> GroupByProject(List<EmployeeProjectWorkPeriod> workPeriods)
        => workPeriods.GroupBy(wp => wp.ProjectId).Select(g => g.ToList()).ToList();

    private List<(EmployeeProjectWorkPeriod, EmployeeProjectWorkPeriod)> GetEmployeePairs(List<EmployeeProjectWorkPeriod> employees)
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

    private void AddOrUpdatePairDuration(Dictionary<(int, int), int> pairDurations, int emp1, int emp2, int overlapDays)
    {
        var employeesPair = emp1 < emp2 ? (emp1, emp2) : (emp2, emp1);
        if (!pairDurations.ContainsKey(employeesPair))
        {
            pairDurations[employeesPair] = 0;
        }

        pairDurations[employeesPair] += overlapDays;
    }

    public int CalculateOverlapDays(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
    {
        var overlapStart = start1 > start2 ? start1 : start2;
        var overlapEnd = end1 < end2 ? end1 : end2;

        if (overlapStart > overlapEnd)
        {
            return 0;
        }
        // Both start and end dates are inclusive
        return (overlapEnd - overlapStart).Days + 1;
    }
}