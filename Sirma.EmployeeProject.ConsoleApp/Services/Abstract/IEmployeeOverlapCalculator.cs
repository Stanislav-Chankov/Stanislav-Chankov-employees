public interface IEmployeeOverlapCalculator
{
    Dictionary<(int, int), int> CalculateEmployeePairOverlaps(List<EmployeeProjectWorkPeriod> workPeriods);
}