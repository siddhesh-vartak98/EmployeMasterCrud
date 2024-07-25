using EmployeMasterCrud.Models;
using System.ComponentModel.DataAnnotations;

namespace EmployeMasterCrud.ViewModels
{
    public class EmployeeViewModel
    {
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

        [Required(ErrorMessage = "Please Upload Thumbnail Image")]
        [StringLength(500)]
        [Display(Name = "Thumbnail Image")]
        public string ThumbnailImage { get; set; } = null!;

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

        public List<EmployeeFileData>? listEmployeeFilesData { get; set; }

        public List<EmployeePackageData>? listEmployeePackageData { get; set; }

    }

    public class EmployeeFileData
    {
        public int EmployeeFilesId { get; set; }

        public string? FileName { get; set; }
    }

    public class EmployeePackageData
    {
        public int SalaryPackageId { get; set; }

        public string? PackageName { get; set; }

        public string? PackageValue { get; set; }

        public string? PackageFile { get; set; }
    }
}
