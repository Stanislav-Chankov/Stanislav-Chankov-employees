public class EmployeePairAnalyzerService : IEmployeePairAnalyzer
{
    public EmployeePairWorkedTogether? FindLongestPair(Dictionary<(int, int), int> pairDurations)
    {
        if (pairDurations == null || pairDurations.Count == 0)
        {
            return null;
        }

        var maxPair = pairDurations.OrderByDescending(p => p.Value).First();

        return new EmployeePairWorkedTogether
        {
            Employee1Id = maxPair.Key.Item1,
            Employee2Id = maxPair.Key.Item2,
            TotalDaysWorkedTogether = maxPair.Value
        };
    }
}