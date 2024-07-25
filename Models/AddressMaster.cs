using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeMasterCrud.Models;

public partial class AddressMaster
{
    [Key]
    public int AddressId { get; set; }

    [Display(Name = "Employee Name")]
    public int EmployeeId { get; set; } = 0;

    [Display(Name = "Is Primary")]
    public bool IsPrimary { get; set; } = false;

    [Display(Name = "Address Line One")]
    public string AddressLineOne { get; set; } = null!;

    [Display(Name = "Address Line Two")]
    public string AddressLineTwo { get; set; } = null!;

    [Display(Name = "Country Name")]
    public int CountryId { get; set; } = 0;

    [Display(Name = "State Name")]
    public int StateId { get; set; } = 0;

    [Display(Name = "City Name")]
    public int CityId { get; set; } = 0;

    [Display(Name = "Date Entered")]
    public DateTime WhenEntered { get; set; } = DateTime.UtcNow;

    [Display(Name = "Last Updated")]
    public DateTime WhenModified { get; set; } = DateTime.UtcNow;

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Is Deleted")]
    public bool IsDeleted { get; set; } = false;
}
