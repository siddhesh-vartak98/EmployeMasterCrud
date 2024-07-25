using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeMasterCrud.Models;

public partial class StateMaster
{
    [Key]
    [Display(Name = "Sr. No.")]
    public int StateId { get; set; }

    [Required(ErrorMessage ="Please Select Country")]
    [Display(Name = "Country Name")]
    public int CountryId { get; set; }

    [Required(ErrorMessage = "Please Enter State Name")]
    [StringLength(200)]
    [Display(Name = "State Name")]
    public string StateName { get; set; } = null!;


    [Display(Name = "Date Entered")]
    public DateTime WhenEntered { get; set; } = DateTime.UtcNow;

    [Display(Name = "Last Updated")]
    public DateTime WhenModified { get; set; } = DateTime.UtcNow;

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Is Deleted")]
    public bool IsDeleted { get; set; } = false;
}
