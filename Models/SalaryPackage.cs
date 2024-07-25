using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeMasterCrud.Models;

public partial class SalaryPackage
{
    [Key]
    public int SalaryPackageId { get; set; }

    public int EmployeeId { get; set; } = 0;

    public string PackageName { get; set; } = null!;

    public string PackageValue { get; set; } = null!;

    public string? PackageFile { get; set; }

    public DateTime WhenEntered { get; set; } = DateTime.UtcNow;
}
