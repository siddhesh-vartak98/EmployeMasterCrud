using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeMasterCrud.Models;

public partial class CityMaster
{
    [Key]
    [Display(Name = "Sr. No.")]
    public int CityId { get; set; }

    [Required(ErrorMessage = "Please Select Country")]
    [Display(Name = "Country Name")]
    public int CountryId { get; set; } = 0;

    [Required(ErrorMessage = "Please Select State")]
    [Display(Name = "State Name")]
    public int StateId { get; set; } = 0;

    [Required(ErrorMessage = "Please Enter City Name")]
    [StringLength(200)]
    [Display(Name = "City Name")]
    public string CityName { get; set; } = null!;

    [Display(Name = "Date Entered")]
    public DateTime WhenEntered { get; set; } = DateTime.UtcNow;

    [Display(Name = "Last Updated")]
    public DateTime WhenModified { get; set; } = DateTime.UtcNow;

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Is Deleted")]
    public bool IsDeleted { get; set; } = false;
}
