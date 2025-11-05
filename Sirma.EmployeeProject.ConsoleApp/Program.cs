using Microsoft.Extensions.DependencyInjection;

// Setup DI
var services = new ServiceCollection();
services.AddSingleton<IFileParser, FileParserService>();
services.AddSingleton<IEmployeeOverlapCalculator, EmployeeOverlapCalculatorService>();
services.AddSingleton<IEmployeePairAnalyzer, EmployeePairAnalyzerService>();
var provider = services.BuildServiceProvider();

// Resolve services
var fileParser = provider.GetRequiredService<IFileParser>();
var overlapCalculator = provider.GetRequiredService<IEmployeeOverlapCalculator>();
var pairAnalyzer = provider.GetRequiredService<IEmployeePairAnalyzer>();

// Step 1: Read the input file
var filePath = Path.Combine(Directory.GetCurrentDirectory(), "data", "EmployeeProjects.csv");
var lines = File.ReadAllLines(filePath);

// Step 2: Parse the file to get employee project work periods
var parsedData = fileParser.ParseEmployeeProjectWorkPeriods(lines);

// Step 3: Calculate overlaps and find the longest pair
var calculatedData = overlapCalculator.CalculateEmployeePairOverlaps(parsedData);

// Step 4: Find the pair with the longest total overlap
var longestPair = pairAnalyzer.FindLongestPair(calculatedData);

// Step 5: Output the result
if (longestPair is null)
{
    Console.WriteLine("No overlapping work periods found.");

    return;
}

Console.WriteLine($"Employees {longestPair.Employee1Id} and {longestPair.Employee2Id} worked together for {longestPair.TotalDaysWorkedTogether} days.");
