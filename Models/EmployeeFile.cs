using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeMasterCrud.Models;

public partial class EmployeeFile
{
    [Key]
    public int EmployeeFilesId { get; set; }

    public int EmployeeId { get; set; } = 0;

    public string FileName { get; set; } = null!;

    public DateTime WhenEntered { get; set; } = DateTime.UtcNow;
}
