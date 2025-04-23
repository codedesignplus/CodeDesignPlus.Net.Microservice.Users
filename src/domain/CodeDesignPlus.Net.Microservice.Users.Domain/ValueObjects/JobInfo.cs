using System.Text.Json.Serialization;

namespace CodeDesignPlus.Net.Microservice.Users.Domain.ValueObjects;

public sealed class JobInfo
{
    public string? JobTitle { get; private set; }
    public string? CompanyName { get; private set; }
    public string? Department { get; private set; }
    public string? EmployeeId { get; private set; }
    public string? EmployeeType { get; private set; }
    public Instant? EmployHireDate { get; private set; }
    public string? OfficeLocation { get; private set; }

    public JobInfo()
    {

    }

    [JsonConstructor]
    public JobInfo(string jobTitle, string companyName, string department, string employeeId, string employeeType, Instant? employHireDate, string officeLocation)
    {
        JobTitle = jobTitle;
        CompanyName = companyName;
        Department = department;
        EmployeeId = employeeId;
        EmployeeType = employeeType;
        EmployHireDate = employHireDate;
        OfficeLocation = officeLocation;
    }

    public static JobInfo Create(string jobTitle, string companyName, string department, string employeeId, string employeeType, Instant? employHireDate, string officeLocation)
    {
        return new JobInfo(jobTitle, companyName, department, employeeId, employeeType, employHireDate, officeLocation);
    }
}
