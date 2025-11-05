public interface IFileParser
{
    List<EmployeeProjectWorkPeriod> ParseEmployeeProjectWorkPeriods(string[] lines);
}