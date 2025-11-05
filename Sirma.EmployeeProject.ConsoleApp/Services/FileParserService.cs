using System.Globalization;

public class FileParserService : IFileParser
{
    public List<EmployeeProjectWorkPeriod> ParseEmployeeProjectWorkPeriods(string[] lines)
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

            if (!DateTime.TryParseExact(parts[2], formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            {
                if (!DateTime.TryParse(parts[2], out startDate))
                    continue;
            }

            if (string.IsNullOrWhiteSpace(parts[3]))
            {
                endDate = today;
            }
            else if (!DateTime.TryParseExact(parts[3], formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                if (!DateTime.TryParse(parts[3], out endDate))
                    endDate = today;
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