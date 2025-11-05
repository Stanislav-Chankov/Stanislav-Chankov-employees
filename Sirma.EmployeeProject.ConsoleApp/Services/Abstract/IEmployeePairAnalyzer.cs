public interface IEmployeePairAnalyzer
{
    EmployeePairWorkedTogether? FindLongestPair(Dictionary<(int, int), int> pairDurations);
}