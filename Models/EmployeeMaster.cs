using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeMasterCrud.Models;

public partial class EmployeeMaster
{
    [Key]
    [Display(Name = "Sr. No.")]
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Please Enter Name")]
    [StringLength(200)]
    [Display(Name = "Name")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Please Enter Email ID")]
    [StringLength(200)]
    [EmailAddress]
    [Display(Name = "Email ID")]
    public string EmailId { get; set; } = null!;

    [Required(ErrorMessage = "Please Enter Mobile No.")]
    [StringLength(10)]
    [Display(Name = "Mobile No.")]
    public string MobileNo { get; set; } = null!;

    [StringLength(20)]
    public string? CountryCode { get; set; }

    [StringLength(20)]
    public string? CountryFlag { get; set; }

    [StringLength(10)]
    public string? Otp { get; set; }

    [Required(ErrorMessage = "Please Upload Thumbnail Image")]
    [StringLength(500)]
    [Display(Name = "Thumbnail Image")]
    public string ThumbnailImage { get; set; } = null!;

    [Display(Name = "Date Entered")]
    public DateTime WhenEntered { get; set; } = DateTime.UtcNow;

    [Display(Name = "Last Updated")]
    public DateTime WhenModified { get; set; } = DateTime.UtcNow;

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Is Deleted")]
    public bool IsDeleted { get; set; } = false;
}
