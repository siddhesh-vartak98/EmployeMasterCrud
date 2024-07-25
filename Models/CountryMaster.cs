using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeMasterCrud.Models;

public partial class CountryMaster
{
    [Key]
    [Display(Name = "Sr. No.")]
    public int CountryId { get; set; }

    [Required(ErrorMessage = "Please Enter Country Name")]
    [StringLength(200)]
    [Display(Name = "Country Name")]
    public string CountryName { get; set; } = null!;

    [Display(Name = "Date Entered")]
    public DateTime WhenEntered { get; set; } = DateTime.UtcNow;

    [Display(Name = "Last Updated")]
    public DateTime WhenModified { get; set; } = DateTime.UtcNow;

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Is Deleted")]
    public bool IsDeleted { get; set; } = false;
}
