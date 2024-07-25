using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeMasterCrud.Models;
using EmployeMasterCrud.Common;
using NLog;
using X.PagedList;
using Newtonsoft.Json;
using EmployeMasterCrud.ViewModels;
using EmployeMasterCrud.ActionFilters;
using Microsoft.CodeAnalysis;
using OfficeOpenXml;

namespace EmployeMasterCrud.Controllers
{
    [CheckSession]
    public class EmployeeMasterController : Controller
    {
        private readonly EmployeeDemoDbContext _context;
        public readonly IWebHostEnvironment webHostEnvironment;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public EmployeeMasterController(EmployeeDemoDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        // GET: EmployeeMaster
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string? ddlStatus,string? emailID)
        {
            try
            {
                if (page != null && page < 1)
                {
                    page = 1;
                }

                ViewData["CurrentSort"] = sortOrder;
                ViewData["IDSortParm"] = string.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
                ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
                ViewData["DateEntered"] = sortOrder == "DateEntered" ? "de_desc" : "DateEntered";
                ViewData["DateModified"] = sortOrder == "DateModified" ? "dm_desc" : "DateModified";

                ViewData["SequenceSort"] = sortOrder == "SequenceSort" ? "desc_sequence" : "SequenceSort";

                if (searchString != null)
                {

                }
                else
                {
                    searchString = currentFilter;
                }

                ViewData["CurrentFilter"] = searchString;
                ViewBag.SearchStatus = ddlStatus;

                var modal = from l in _context.EmployeeMasters where l.IsDeleted == false select l;

                if (!String.IsNullOrEmpty(searchString))
                {
                    modal = modal.Where(x => x.Name.Trim().ToLower().Contains(searchString.Trim().ToLower()));
                    ViewData["searchname"] = searchString;
                }

                if (!String.IsNullOrEmpty(emailID))
                {
                    modal = modal.Where(x => x.EmailId.Trim().ToLower().Contains(emailID.Trim().ToLower()));
                    ViewBag.searchEmail = emailID;
                }

                if (!string.IsNullOrEmpty(ddlStatus))
                {
                    if (ddlStatus == "True")
                    {
                        modal = modal.Where(x => x.IsActive == true);
                    }
                    else
                    {
                        modal = modal.Where(x => x.IsActive == false);
                    }

                    ViewBag.SearchStatus = ddlStatus;
                }

                switch (sortOrder)
                {
                    case "id_desc":
                        modal = modal.OrderBy(s => s.EmployeeId);
                        break;
                    case "Name":
                        modal = modal.OrderBy(s => s.Name);
                        break;
                    case "name_desc":
                        modal = modal.OrderByDescending(s => s.Name);
                        break;
                    case "DateEntered":
                        modal = modal.OrderBy(s => s.WhenEntered);
                        break;
                    case "de_desc":
                        modal = modal.OrderByDescending(s => s.WhenEntered);
                        break;
                    case "DateModified":
                        modal = modal.OrderBy(s => s.WhenModified);
                        break;
                    case "dm_desc":
                        modal = modal.OrderByDescending(s => s.WhenModified);
                        break;
                    //case "SequenceSort":
                    //    modal = modal.OrderBy(s => s.BannerSequence);
                    //    break;
                    //case "desc_sequence":
                    //    modal = modal.OrderByDescending(s => s.BannerSequence);
                    //    break;

                    default:
                        modal = modal.OrderByDescending(s => s.EmployeeId);
                        break;
                }

                // <-- Count -->
                ViewBag.totalmodeldata = 0;

                if (modal != null && modal.Count() > 0)
                {
                    ViewBag.totalmodeldata = modal.Count();
                }

                ViewBag.ddlStatus = new SelectList(StaticData.IsPublishDD(), "value", "name");

                var listModel = modal!.ToPagedList(page ?? 1, Config.pageSize);

                return View(listModel);
            }
            catch (Exception ex)
            {
                string exMessage = CommonFunctions.getExceptionMessage(ex);

                TempData["Warning"] = "showModal('Warning!','An error has occurred. Please try again. Error:- " + exMessage + " ');";

                var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, ex, null, exMessage);

                return RedirectToAction("Index", "Home");
            }
        }

        // GET: EmployeeMaster/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeMaster = await _context.EmployeeMasters
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employeeMaster == null)
            {
                return NotFound();
            }

            return View(employeeMaster);
        }

        // GET: EmployeeMaster/Create
        public IActionResult Create()
        {
            ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x=>x.IsDeleted == false), "CountryId", "CountryName");
            ViewBag.StateId = new List<SelectListItem>();
            ViewBag.CityId = new List<SelectListItem>();

            return View();
        }

        // POST: EmployeeMaster/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel employeeViewModel, IFormCollection formData)
        {
            if (ModelState.IsValid)
            {
                string jsonInput = JsonConvert.SerializeObject(employeeViewModel);
                var isEntryInNlog = CommonFunctions.AddEntryOfLog(logger, null, jsonInput, "Employee create action input.");

                try
                {
                    employeeViewModel.EmailId = employeeViewModel.EmailId.Trim();
                    employeeViewModel.Name = employeeViewModel.Name.Trim();

                    if (!_context.EmployeeMasters.Any(x => x.EmailId.Trim().ToLower() == employeeViewModel.EmailId.Trim().ToLower() && x.IsDeleted == false))
                    {
                        #region Add employee  
                        EmployeeMaster employeeMaster = new();

                        employeeMaster.Name = employeeViewModel.Name;  
                        employeeMaster.EmailId = employeeViewModel.EmailId;
                        employeeMaster.MobileNo = employeeViewModel.MobileNo;
                        employeeMaster.CountryCode = employeeViewModel.CountryCode;
                        employeeMaster.CountryFlag = employeeViewModel.CountryFlag;
                        employeeMaster.ThumbnailImage = employeeViewModel.ThumbnailImage;

                        await _context.EmployeeMasters.AddAsync(employeeMaster);
                        await _context.SaveChangesAsync();

                        #endregion

                        int employeeID = employeeMaster.EmployeeId;

                        TempData["Success"] = "Employee Successfully Created";

                        if (employeeID > 0)
                        {
                            #region Add employee address
                            AddressMaster addressMaster = new();

                            addressMaster.EmployeeId = employeeID;
                            addressMaster.AddressLineOne = employeeViewModel.AddressLineOne;
                            addressMaster.AddressLineTwo = employeeViewModel.AddressLineTwo;
                            addressMaster.CountryId = employeeViewModel.CountryId;
                            addressMaster.StateId = employeeViewModel.StateId;
                            addressMaster.CityId = employeeViewModel.CityId;

                            await _context.AddressMasters.AddAsync(addressMaster);
                            await _context.SaveChangesAsync();

                            var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, null, null, "Employee address successfully add in database.");

                            #endregion

                            #region Add Multiple images
                            int fileCount = Convert.ToInt32(formData["hfGalleryFileCount"]);
                            string fileName = string.Empty;
                            List<EmployeeFile> lists = new();

                            for (int i = 0; i < fileCount; i++)
                            {
                                try
                                {
                                    fileName = formData["hf_image_" + (i + 1)].ToString();

                                    if (!string.IsNullOrEmpty(fileName))
                                    {
                                        lists.Add(new EmployeeFile
                                        {
                                            EmployeeId = employeeID,
                                            FileName = fileName,
                                        });
                                    }
                                }
                                catch { }
                                //catch for delete image remove
                            }

                            if (lists != null && lists.Any())
                            {
                                string jsonInputTwo = JsonConvert.SerializeObject(lists);
                                var isEntryInNlogTwo = CommonFunctions.AddEntryOfLog(logger, null, jsonInputTwo, "Employee files create action input.");

                                await _context.EmployeeFiles.AddRangeAsync(lists);
                                await _context.SaveChangesAsync();

                                var isEntryInNlogThree = CommonFunctions.AddEntryOfLog(logger, null, null, "Employee files successfully add in database.");
                            }

                            #endregion

                            #region Add Document
                            int documentFileCount = Convert.ToInt32(formData["hfNewsFileCount"]);

                            if (documentFileCount > 0)
                            {
                                List<SalaryPackage> listPartyDocuments = new();

                                for (int i = 0; i < documentFileCount; i++)
                                {
                                    try
                                    {
                                        string packageName = formData["txtDocumentName_" + (i + 1)].ToString();
                                        string packageValue = formData["txtDocumentNumber_" + (i + 1)].ToString();
                                        string packageFile = formData["hf_documentFile_" + (i + 1)].ToString();

                                        if (!string.IsNullOrEmpty(packageName))
                                        {
                                            if (!string.IsNullOrEmpty(packageValue))
                                            {
                                                if (!string.IsNullOrEmpty(packageFile))
                                                {
                                                    listPartyDocuments.Add(new SalaryPackage
                                                    {
                                                        EmployeeId = employeeID,
                                                        PackageName = packageName,
                                                        PackageValue = packageValue,
                                                        PackageFile = packageFile
                                                    });
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }

                                if (listPartyDocuments != null)
                                {
                                    if (listPartyDocuments.Any())
                                    {
                                        #region NLog entry of party documents 

                                        string jsonPartyDocuments = JsonConvert.SerializeObject(listPartyDocuments);
                                        var isEntryInNlogFour = CommonFunctions.AddEntryOfLog(logger, null, jsonPartyDocuments, "Employee salary files create action input.");

                                        #endregion

                                        await _context.SalaryPackages.AddRangeAsync(listPartyDocuments);
                                        await _context.SaveChangesAsync();

                                        logger.Debug("Employee salary files successfully add in database.");

                                    }
                                }
                            }
                            #endregion
                        }
                        
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["show-modal"] = "showModal('Warning!','Employee with same email ID already in table!');";

                        //return View(employeeViewModel);
                    }
                }
                catch (Exception ex)
                {
                    string exMessage = CommonFunctions.getExceptionMessage(ex);

                    TempData["Warning"] = "showModal('Warning!','An error has occurred. Please try again. Error:- " + exMessage + " ');";

                    var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, ex, jsonInput, exMessage);

                    return RedirectToAction(nameof(Index));
                }
            }

            #region viewbag ddl list 
            if (employeeViewModel != null)
            {
                if(employeeViewModel.CountryId > 0)
                {
                    ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "CountryId", "CountryName",employeeViewModel.CountryId);

                    if(employeeViewModel.StateId > 0)
                    {
                        ViewBag.StateId = new SelectList(_context.StateMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "StateId", "StateName",employeeViewModel.StateId);

                        if(employeeViewModel.CityId > 0)
                        {
                            ViewBag.CityId = new SelectList(_context.CityMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "CityId", "CityName",employeeViewModel.CityId);
                        }
                        else
                        {
                            ViewBag.CityId = new SelectList(_context.CityMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "CityId", "CityName");
                        }
                    }
                    else
                    {
                        ViewBag.StateId = new SelectList(_context.StateMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "StateId", "StateName");
                        ViewBag.CityId = new List<SelectListItem>();
                    }
                }
                else
                {
                    ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "CountryId", "CountryName");
                    ViewBag.StateId = new List<SelectListItem>();
                    ViewBag.CityId = new List<SelectListItem>();
                }
            }
            else
            {
                ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "CountryId", "CountryName");
                ViewBag.StateId = new List<SelectListItem>();
                ViewBag.CityId = new List<SelectListItem>();
            }
            #endregion


            return View(employeeViewModel);
        }

        // GET: EmployeeMaster/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, null, null, "The input ID of employee edit is null.");

                TempData["show-modal"] = "showModal('Warning!','Opps! Somthing went wrong.');";

                return RedirectToAction(nameof(Index));

                //return NotFound();
            }

            var employeeMaster = await _context.EmployeeMasters.FindAsync(id);

            if (employeeMaster == null)
            {
                var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, null, null, "Employee data not found in given ID.");

                TempData["show-modal"] = "showModal('Warning!','Data not found.');";

                return RedirectToAction(nameof(Index));

                //return NotFound();
            }

            EmployeeViewModel employeeViewModel = new();
            List<EmployeeFileData> listEmployeeFileData = new();
            List<EmployeePackageData> listPackageData = new();

            int employeeFileCount = 0;
            int packageCount = 0;

            if(employeeMaster != null)
            {
                employeeViewModel.Name = employeeMaster.Name;
                employeeViewModel.EmailId = employeeMaster.EmailId;
                employeeViewModel.MobileNo = employeeMaster.MobileNo;
                employeeViewModel.EmployeeId = employeeMaster.EmployeeId;
                employeeViewModel.ThumbnailImage = employeeMaster.ThumbnailImage;
                employeeViewModel.CountryCode = employeeMaster.CountryCode;
                employeeViewModel.CountryFlag = employeeMaster.CountryFlag;

                #region Get employee address
                var addressModel = from l in _context.AddressMasters where l.EmployeeId == employeeMaster.EmployeeId select l;

                if(addressModel != null)
                {
                    if (addressModel.Any())
                    {
                        var employeeAddress = addressModel.FirstOrDefault();

                        if(employeeAddress != null)
                        {
                            employeeViewModel.AddressLineOne = employeeAddress.AddressLineOne;
                            employeeViewModel.AddressLineTwo = employeeAddress.AddressLineTwo;
                            employeeViewModel.CountryId = employeeAddress.CountryId;
                            employeeViewModel.StateId = employeeAddress.StateId;
                            employeeViewModel.CityId = employeeAddress.CityId;
                        }
                    }
                }
                #endregion

                #region Get files 
                var employeeFileModel = from l in _context.EmployeeFiles where l.EmployeeId == employeeMaster.EmployeeId select l;

                if(employeeFileModel != null)
                {
                    if (employeeFileModel.Any())
                    {
                        employeeFileCount = employeeFileModel.Count();

                        foreach(var item in employeeFileModel)
                        {
                            if(item != null)
                            {
                                listEmployeeFileData.Add(new EmployeeFileData
                                {
                                    EmployeeFilesId = item.EmployeeFilesId,
                                    FileName = item.FileName,
                                });
                            }
                        }

                    }
                }

                if(listEmployeeFileData != null && listEmployeeFileData.Any())
                {
                    employeeViewModel.listEmployeeFilesData = listEmployeeFileData;
                }
                #endregion

                #region Get packages

                var packageModel = from l in _context.SalaryPackages where l.EmployeeId == employeeMaster.EmployeeId select l;

                if(packageModel != null)
                {
                    if (packageModel.Any())
                    {
                        packageCount = packageModel.Count();

                        foreach(var item in packageModel)
                        {
                            if(item != null)
                            {
                                listPackageData.Add(new EmployeePackageData
                                {
                                    SalaryPackageId = item.SalaryPackageId,
                                    PackageName = item.PackageName,
                                    PackageValue = item.PackageValue,
                                    PackageFile = item.PackageFile,
                                });
                            }
                        }
                    }
                }

                if(listPackageData != null && listPackageData.Any())
                {
                    employeeViewModel.listEmployeePackageData = listPackageData;
                }
                #endregion
            }

            #region viewbag ddl list 
            if (employeeViewModel != null)
            {
                if (employeeViewModel.CountryId > 0)
                {
                    ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "CountryId", "CountryName", employeeViewModel.CountryId);

                    if (employeeViewModel.StateId > 0)
                    {
                        ViewBag.StateId = new SelectList(_context.StateMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "StateId", "StateName", employeeViewModel.StateId);

                        if (employeeViewModel.CityId > 0)
                        {
                            ViewBag.CityId = new SelectList(_context.CityMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "CityId", "CityName", employeeViewModel.CityId);
                        }
                        else
                        {
                            ViewBag.CityId = new SelectList(_context.CityMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "CityId", "CityName");
                        }
                    }
                    else
                    {
                        ViewBag.StateId = new SelectList(_context.StateMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "StateId", "StateName");
                        ViewBag.CityId = new List<SelectListItem>();
                    }
                }
                else
                {
                    ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "CountryId", "CountryName");
                    ViewBag.StateId = new List<SelectListItem>();
                    ViewBag.CityId = new List<SelectListItem>();
                }
            }
            else
            {
                ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "CountryId", "CountryName");
                ViewBag.StateId = new List<SelectListItem>();
                ViewBag.CityId = new List<SelectListItem>();
            }
            #endregion

            ViewBag.EmployeeFileCount = employeeFileCount;
            ViewBag.PackageCount = packageCount;

            return View(employeeViewModel);
        }

        // POST: EmployeeMaster/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel employeeViewModel, IFormCollection formData)
        {
            string jsonInput = JsonConvert.SerializeObject(employeeViewModel);
            var isEntryInNlog = CommonFunctions.AddEntryOfLog(logger, null, jsonInput, "Employee edit action input.");

            if (id != employeeViewModel.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    employeeViewModel.EmailId = employeeViewModel.EmailId.Trim();
                    employeeViewModel.Name = employeeViewModel.Name.Trim();

                    if (!_context.EmployeeMasters.Any(x => x.EmailId.Trim().ToLower() == employeeViewModel.EmailId.Trim().ToLower() && x.IsDeleted == false && x.EmployeeId == employeeViewModel.EmployeeId))
                    {
                        #region Add employee  
                        EmployeeMaster employeeMaster = new();

                        employeeMaster.Name = employeeViewModel.Name;
                        employeeMaster.EmailId = employeeViewModel.EmailId;
                        employeeMaster.MobileNo = employeeViewModel.MobileNo;
                        employeeMaster.CountryCode = employeeViewModel.CountryCode;
                        employeeMaster.CountryFlag = employeeViewModel.CountryFlag;
                        employeeMaster.ThumbnailImage = employeeViewModel.ThumbnailImage;

                        await _context.EmployeeMasters.AddAsync(employeeMaster);
                        await _context.SaveChangesAsync();

                        #endregion

                        int employeeID = employeeMaster.EmployeeId;

                        TempData["Success"] = "Employee Successfully Created";

                        if (employeeID > 0)
                        {
                            #region Remove employee address
                            _context.AddressMasters.RemoveRange(_context.AddressMasters.Where(x => x.EmployeeId == employeeID));
                            await _context.SaveChangesAsync();
                            #endregion

                            #region Add employee address
                            AddressMaster addressMaster = new();

                            addressMaster.EmployeeId = employeeID;
                            addressMaster.AddressLineOne = employeeViewModel.AddressLineOne;
                            addressMaster.AddressLineTwo = employeeViewModel.AddressLineTwo;
                            addressMaster.CountryId = employeeViewModel.CountryId;
                            addressMaster.StateId = employeeViewModel.StateId;
                            addressMaster.CityId = employeeViewModel.CityId;

                            await _context.AddressMasters.AddAsync(addressMaster);
                            await _context.SaveChangesAsync();

                            var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, null, null, "Employee address successfully add in database.");

                            #endregion

                            #region Remove multiple images 
                            _context.EmployeeFiles.RemoveRange(_context.EmployeeFiles.Where(x => x.EmployeeId == employeeID));
                            await _context.SaveChangesAsync();
                            #endregion

                            #region Add Multiple images
                            int fileCount = Convert.ToInt32(formData["hfGalleryFileCount"]);
                            string fileName = string.Empty;
                            List<EmployeeFile> lists = new();

                            for (int i = 0; i < fileCount; i++)
                            {
                                try
                                {
                                    fileName = formData["hf_image_" + (i + 1)].ToString();

                                    if (!string.IsNullOrEmpty(fileName))
                                    {
                                        lists.Add(new EmployeeFile
                                        {
                                            EmployeeId = employeeID,
                                            FileName = fileName,
                                        });
                                    }
                                }
                                catch { }
                                //catch for delete image remove
                            }

                            if (lists != null && lists.Any())
                            {
                                string jsonInputTwo = JsonConvert.SerializeObject(lists);
                                var isEntryInNlogTwo = CommonFunctions.AddEntryOfLog(logger, null, jsonInputTwo, "Employee files create action input.");

                                await _context.EmployeeFiles.AddRangeAsync(lists);
                                await _context.SaveChangesAsync();

                                var isEntryInNlogThree = CommonFunctions.AddEntryOfLog(logger, null, null, "Employee files successfully add in database.");
                            }

                            #endregion

                            #region Remove document
                            _context.SalaryPackages.RemoveRange(_context.SalaryPackages.Where(x=>x.EmployeeId == employeeID));
                            await _context.SaveChangesAsync();

                            #endregion

                            #region Add Document
                            int documentFileCount = Convert.ToInt32(formData["hfNewsFileCount"]);

                            if (documentFileCount > 0)
                            {
                                List<SalaryPackage> listPartyDocuments = new();

                                for (int i = 0; i < documentFileCount; i++)
                                {
                                    try
                                    {
                                        string packageName = formData["txtDocumentName_" + (i + 1)].ToString();
                                        string packageValue = formData["txtDocumentNumber_" + (i + 1)].ToString();
                                        string packageFile = formData["hf_documentFile_" + (i + 1)].ToString();

                                        if (!string.IsNullOrEmpty(packageName))
                                        {
                                            if (!string.IsNullOrEmpty(packageValue))
                                            {
                                                if (!string.IsNullOrEmpty(packageFile))
                                                {
                                                    listPartyDocuments.Add(new SalaryPackage
                                                    {
                                                        EmployeeId = employeeID,
                                                        PackageName = packageName,
                                                        PackageValue = packageValue,
                                                        PackageFile = packageFile
                                                    });
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }

                                if (listPartyDocuments != null)
                                {
                                    if (listPartyDocuments.Any())
                                    {
                                        #region NLog entry of party documents 

                                        string jsonPartyDocuments = JsonConvert.SerializeObject(listPartyDocuments);
                                        var isEntryInNlogFour = CommonFunctions.AddEntryOfLog(logger, null, jsonPartyDocuments, "Employee salary files create action input.");

                                        #endregion

                                        await _context.SalaryPackages.AddRangeAsync(listPartyDocuments);
                                        await _context.SaveChangesAsync();

                                        logger.Debug("Employee salary files successfully add in database.");

                                    }
                                }
                            }
                            #endregion
                        }

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["show-modal"] = "showModal('Warning!','Employee with same email ID already in table!');";

                        //return View(employeeViewModel);
                    }
                    
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!EmployeeMasterExists(employeeViewModel.EmployeeId))
                    {
                        string exMessage = CommonFunctions.getExceptionMessage(ex);

                        TempData["Warning"] = "showModal('Warning!','An error has occurred. Please try again. Error:- " + exMessage + " ');";

                        var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, ex, jsonInput, exMessage);

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        string exMessage = CommonFunctions.getExceptionMessage(ex);

                        TempData["Warning"] = "showModal('Warning!','An error has occurred. Please try again. Error:- " + exMessage + " ');";

                        var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, ex, jsonInput, exMessage);

                        return RedirectToAction(nameof(Index));
                    }
                }
                //return RedirectToAction(nameof(Index));
            }

            #region viewbag ddl list 
            if (employeeViewModel != null)
            {
                if (employeeViewModel.CountryId > 0)
                {
                    ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "CountryId", "CountryName", employeeViewModel.CountryId);

                    if (employeeViewModel.StateId > 0)
                    {
                        ViewBag.StateId = new SelectList(_context.StateMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "StateId", "StateName", employeeViewModel.StateId);

                        if (employeeViewModel.CityId > 0)
                        {
                            ViewBag.CityId = new SelectList(_context.CityMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "CityId", "CityName", employeeViewModel.CityId);
                        }
                        else
                        {
                            ViewBag.CityId = new SelectList(_context.CityMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "CityId", "CityName");
                        }
                    }
                    else
                    {
                        ViewBag.StateId = new SelectList(_context.StateMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "StateId", "StateName");
                        ViewBag.CityId = new List<SelectListItem>();
                    }
                }
                else
                {
                    ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "CountryId", "CountryName");
                    ViewBag.StateId = new List<SelectListItem>();
                    ViewBag.CityId = new List<SelectListItem>();
                }
            }
            else
            {
                ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false && x.IsActive == true), "CountryId", "CountryName");
                ViewBag.StateId = new List<SelectListItem>();
                ViewBag.CityId = new List<SelectListItem>();
            }
            #endregion

            return View(employeeViewModel);
        }

        // GET: EmployeeMaster/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeMaster = await _context.EmployeeMasters
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employeeMaster == null)
            {
                return NotFound();
            }

            return View(employeeMaster);
        }

        // POST: EmployeeMaster/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jsonObject = new { Id = id };
            string json = JsonConvert.SerializeObject(jsonObject);

            try
            {
                var model = await _context.EmployeeMasters.FindAsync(id);

                if (model != null)
                {
                    model.IsDeleted = true;

                    await _context.SaveChangesAsync();

                    var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, null, json, "Employee successfully deleted.");

                    TempData["Success"] = "Employee Successfully Deleted";
                }
                else
                {
                    TempData["show-modal"] = "showModal('Record Not Found Warning!',' We could not locate any data for the provided ID.');";

                    var isEntryInNlog = CommonFunctions.AddEntryOfLog(logger, null, json, "Employee data not found in given ID");
                }
            }
            catch (Exception ex)
            {
                string exMessage = CommonFunctions.getExceptionMessage(ex);

                TempData["Warning"] = "showModal('Warning!','An error has occurred. Please try again. Error:- " + exMessage + " ');";

                var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, ex, json, exMessage);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<string> SingleUploadImage(IFormFile file)
        {
            string newFileName = string.Empty;
            var request = HttpContext.Request;
            file = request.Form.Files[0];

            if (file != null)
            {
                newFileName = CommonFunctions.CreateImageFileName(file);

                //Save File
                string filepath = Path.Combine(webHostEnvironment.ContentRootPath + Config.ThumbnailImagePath + "/", newFileName);

                Stream fileStream = new FileStream(filepath, FileMode.Create);

                await file.CopyToAsync(fileStream);
                await fileStream.DisposeAsync();
            }

            return newFileName;
        }

        public async Task<string> SingleUploadFile(IFormFile file)
        {
            string newFileName = string.Empty;
            var request = HttpContext.Request;
            file = request.Form.Files[0];

            if (file != null)
            {
                newFileName = CommonFunctions.CreateImageFileName(file);

                //Save File
                string filepath = Path.Combine(webHostEnvironment.ContentRootPath + Config.DocumentFilePath + "/", newFileName);

                Stream fileStream = new FileStream(filepath, FileMode.Create);

                await file.CopyToAsync(fileStream);
                await fileStream.DisposeAsync();
            }

            return newFileName;
        }

        [HttpPost]
        public string MultiUploadFile()
        {
            var request = HttpContext.Request;

            List<IFormFile> files = new List<IFormFile>();

            files = request.Form.Files.ToList();

            List<string> images = new List<string>();

            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        //Getting FileName
                        var fileName = Path.GetFileName(file.FileName);

                        //Assigning Unique Filename (Guid)
                        var myUniqueFileName = Convert.ToString(Guid.NewGuid());

                        //Getting file Extension
                        var fileExtension = Path.GetExtension(fileName);

                        // concatenating  FileName + FileExtension
                        var newFileName = String.Concat(myUniqueFileName, fileExtension);

                        // Combines two strings into a path.
                        string filepath = Path.Combine(webHostEnvironment.ContentRootPath + Config.EmpFilePath, newFileName);

                        //var filepath = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images")).Root + $@"\{newFileName}";

                        using FileStream fs = System.IO.File.Create(filepath);
                        file.CopyTo(fs);
                        fs.Flush();

                        images.Add(newFileName);

                    }
                }
            }

            return string.Join(",", images);
        }

        // @Html.ActionLink("Download To Excel", "AllNominationsExcelDownload", new { id = emmaEventID, ddlSearchEventCategory = ViewBag.SearchEventCategory, SearchString = ViewData["searchname"], ddlSearchApproveStatus = ViewBag.SearchApproveStatus, fromDate = ViewBag.fromDate, toDate = ViewBag.toDate }, htmlAttributes: new { @class = "btn btn-success btn-sm" })
        public IActionResult AllNominationsExcelDownload(string sortOrder, string currentFilter, string searchString,
            int? ddlSearchEventCategory, bool? ddlSearchApproveStatus, string fromDate, string toDate)
        {

                var stream = new MemoryStream();

                #region get data 
                var modal = from ee in _context.EmployeeMasters where ee.IsDeleted == false select ee;

                #region search 
                //if (ddlSearchEventCategory > 0)
                //{
                //    modal = modal.Where(x => x.emmaNominationCatogoryID == ddlSearchEventCategory);

                //    ViewBag.SearchEventCategory = ddlSearchEventCategory;
                //    ViewData["SearchEventCategory"] = ddlSearchEventCategory;
                //}

                if (!String.IsNullOrEmpty(searchString))
                {
                    modal = modal.Where(x => x.Name.Trim().ToLower().Contains(searchString.Trim().ToLower()));
                    ViewData["searchname"] = searchString;
                }

                //if (ddlSearchApproveStatus != null && !string.IsNullOrEmpty(ddlSearchApproveStatus.ToString()!))
                //{
                //    modal = modal.Where(x => x.isApprove == ddlSearchApproveStatus);

                //    ViewBag.SearchApproveStatus = ddlSearchApproveStatus;
                //    ViewData["SearchApproveStatus"] = ddlSearchApproveStatus;
                //}

                if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                {
                    DateTime startDate = Convert.ToDateTime(fromDate);
                    DateTime endDate = Convert.ToDateTime(toDate);

                    modal = modal.Where(c => c.WhenEntered >= startDate && c.WhenEntered <= endDate);

                    ViewBag.fromDate = fromDate;
                    ViewBag.toDate = toDate;

                    ViewData["fromDate"] = fromDate;
                    ViewData["toDate"] = toDate;
                }
                #endregion
                #endregion

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var xls = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet;

                    try
                    {
                        worksheet = xls.Workbook.Worksheets.Add("Sheet1");
                    }
                    catch (Exception)
                    {
                        xls.Workbook.Worksheets.Delete("Sheet1");
                        worksheet = xls.Workbook.Worksheets.Add("Sheet1");
                    }

                    worksheet.SelectedRange["A1:N1"].Style.Font.Bold = true;
                    worksheet.SelectedRange["A1:N1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.SelectedRange["A1:N1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);

                    worksheet.Cells[1, 1].Value = "Sr. No.";
                    worksheet.Cells[1, 2].Value = "Name";
                    worksheet.Cells[1, 3].Value = "Email ID";
                    worksheet.Cells[1, 4].Value = "Country Code";
                    worksheet.Cells[1, 5].Value = "Mobile No.";
                    worksheet.Cells[1, 6].Value = "Date Entered";
                    worksheet.Cells[1, 7].Value = "Date Modified";

                    if (modal != null && modal.Any())
                    {
                        int j = 0;

                        foreach (var item in modal)
                        {
                            if (item != null)
                            {
                                #region status and Date - Entered,Modified
                                string whenEntered = CommonFunctions.ConvertToIST(item.WhenEntered).ToString(Config.dateTimeFormat);
                                string whenModified = CommonFunctions.ConvertToIST(item.WhenModified).ToString(Config.dateTimeFormat);
                                #endregion

                                worksheet.Cells[j + 2, 0 + 1].Value = (j + 1);
                                worksheet.Cells[j + 2, 0 + 2].Value = item.Name;
                                worksheet.Cells[j + 2, 0 + 3].Value = item.EmailId;
                                worksheet.Cells[j + 2, 0 + 4].Value = (!string.IsNullOrEmpty(item.CountryCode)) ? item.CountryCode : "-";
                                worksheet.Cells[j + 2, 0 + 5].Value = item.MobileNo;

                                worksheet.Cells[j + 2, 0 + 6].Value = whenEntered;
                                worksheet.Cells[j + 2, 0 + 7].Value = whenModified;

                                j++;
                            }

                        }

                        xls.Save();
                    }
                }

                stream.Position = 0;
                string excelName = $"All_Nominations" + CommonFunctions.ConvertToIST(DateTime.UtcNow).ToString(Config.dateTimeFormat) + ".xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            
        }

        //<a asp-action="UploadNomination" asp-route-id="@emmaEventID" class="btn btn-primary btn-sm">Upload Nomination</a>
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UploadNomination(int id, EmmaNominationViewModel emmaNominationViewModel)
        //{
        //    if (id == 0)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        if (emmaNominationViewModel != null)
        //        {
        //            #region save data of emmaNomination 

        //            EmmaNomination emmaNomination = new EmmaNomination();

        //            emmaNomination.emmaEventID = id;
        //            emmaNomination.emmaNominationCatogoryID = (int)emmaNominationViewModel.emmaNominationCatogoryID;

        //            emmaNomination.firstName = emmaNominationViewModel.firstName;
        //            emmaNomination.lastName = emmaNominationViewModel.lastName;
        //            emmaNomination.emailID = emmaNominationViewModel.emailID;
        //            emmaNomination.componyName = emmaNominationViewModel.componyName;

        //            emmaNomination.nominationTitle = emmaNominationViewModel.nominationTitle;
        //            emmaNomination.nominationShortDescription = emmaNominationViewModel.nominationShortDescription;
        //            emmaNomination.nominationLongDescription = emmaNominationViewModel.nominationLongDescription;
        //            emmaNomination.nominationThumbnail = emmaNominationViewModel.nominationThumbnail;

        //            emmaNomination.uploadFileOne = emmaNominationViewModel.uploadFileOne;
        //            emmaNomination.uploadFileTwo = emmaNominationViewModel.uploadFileTwo;
        //            emmaNomination.uploadLinkOne = emmaNominationViewModel.uploadLinkOne;
        //            emmaNomination.uploadLinkTwo = emmaNominationViewModel.uploadLinkTwo;

        //            _context.Add(emmaNomination);
        //            await _context.SaveChangesAsync();
        //            #endregion

        //            TempData["Success"] = "Emma Nominations Successfully Created";

        //            return RedirectToAction(nameof(Index));
        //        }

        //    }

        //    var listEmmaEvents = from emmaev in _context.EmmaEvent
        //                         join ev in _context.Events on emmaev.EventId equals ev.EventId
        //                         where ev.isDeleted == false && emmaev.IsDeleted == false
        //                         orderby ev.EventId
        //                         select new
        //                         {
        //                             ev.EventId,
        //                             ev.Title,
        //                             emmaev.EmmaEventId,
        //                         };

        //    if (emmaNominationViewModel != null)
        //    {
        //        if (emmaNominationViewModel.emmaEventID > 0)
        //        {
        //            if (id > 0)
        //            {
        //                listEmmaEvents = listEmmaEvents.Where(x => x.EmmaEventId == id);

        //                if (listEmmaEvents != null && listEmmaEvents.Any())
        //                {
        //                    foreach (var item in listEmmaEvents)
        //                    {
        //                        if (item != null)
        //                        {
        //                            if (item.EmmaEventId == id)
        //                            {
        //                                emmaNominationViewModel.emmaEventID = id;
        //                                emmaNominationViewModel.emmaEventName = item.Title;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                ViewBag.emmaEventID = new SelectList(listEmmaEvents.ToList(), "EmmaEventId", "Title", emmaNominationViewModel.emmaEventID);
        //            }

        //        }
        //        else
        //        {
        //            ViewBag.emmaEventID = new SelectList(listEmmaEvents.ToList(), "EmmaEventId", "Title");
        //        }

        //        if (emmaNominationViewModel.emmaNominationCatogoryID > 0)
        //        {
        //            ViewBag.emmaNominationCatogoryID = new SelectList(_context.EmmaNominationCatogories.Where(x => x.IsDeleted == false), "EmmaNominationCatogoryId", "Title", emmaNominationViewModel.emmaNominationCatogoryID);
        //        }
        //        else
        //        {
        //            ViewBag.emmaNominationCatogoryID = new SelectList(_context.EmmaNominationCatogories.Where(x => x.IsDeleted == false), "EmmaNominationCatogoryId", "Title");
        //        }
        //    }
        //    else
        //    {
        //        if (id > 0)
        //        {
        //            listEmmaEvents = listEmmaEvents.Where(x => x.EmmaEventId == id);

        //            if (listEmmaEvents != null && listEmmaEvents.Any())
        //            {
        //                foreach (var item in listEmmaEvents)
        //                {
        //                    if (item != null)
        //                    {
        //                        if (item.EmmaEventId == id)
        //                        {
        //                            emmaNominationViewModel.emmaEventID = id;
        //                            emmaNominationViewModel.emmaEventName = item.Title;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            ViewBag.emmaEventID = new SelectList(listEmmaEvents.ToList(), "EmmaEventId", "Title");
        //        }

        //        ViewBag.emmaNominationCatogoryID = new SelectList(_context.EmmaNominationCatogories.Where(x => x.IsDeleted == false), "EmmaNominationCatogoryId", "Title");
        //    }


        //    ViewBag.emmaEventID = id;

        //    return View(emmaNominationViewModel);
        //}



        private bool EmployeeMasterExists(int id)
        {
            return _context.EmployeeMasters.Any(e => e.EmployeeId == id);
        }
    }
}
