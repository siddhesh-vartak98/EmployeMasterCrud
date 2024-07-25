using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeMasterCrud.Models;
using Azure;
using EmployeMasterCrud.Common;
using Microsoft.Data.SqlClient;
using X.PagedList;
using NLog;
using Newtonsoft.Json;
using EmployeMasterCrud.ActionFilters;

namespace EmployeMasterCrud.Controllers
{
    [CheckSession]
    public class CountryMasterController : Controller
    {
        private readonly EmployeeDemoDbContext _context;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public CountryMasterController(EmployeeDemoDbContext context)
        {
            _context = context;
        }

        // GET: CountryMaster
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string? ddlStatus)
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

                var modal = from l in _context.CountryMasters where l.IsDeleted == false select l;

                if (!String.IsNullOrEmpty(searchString))
                {
                    modal = modal.Where(x => x.CountryName.Trim().Contains(searchString.Trim()));
                    ViewData["searchname"] = searchString;
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
                        modal = modal.OrderBy(s => s.CountryId);
                        break;
                    case "Name":
                        modal = modal.OrderBy(s => s.CountryName);
                        break;
                    case "name_desc":
                        modal = modal.OrderByDescending(s => s.CountryName);
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
                    //    modal = modal.OrderBy(s => s.sequence);
                    //    break;
                    //case "desc_sequence":
                    //    modal = modal.OrderByDescending(s => s.sequence);
                    //    break;

                    default:
                        modal = modal.OrderByDescending(s => s.CountryId);
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

        // GET: CountryMaster/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var countryMaster = await _context.CountryMasters
                .FirstOrDefaultAsync(m => m.CountryId == id);
            if (countryMaster == null)
            {
                return NotFound();
            }

            return View(countryMaster);
        }

        // GET: CountryMaster/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CountryMaster/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CountryMaster countryMaster)
        {
            if (ModelState.IsValid)
            {
                if (countryMaster != null)
                {
                    try
                    {
                        string jsonInput = JsonConvert.SerializeObject(countryMaster);
                        var isEntryInNlog = CommonFunctions.AddEntryOfLog(logger, null, jsonInput, "Country create action input.");

                        countryMaster.CountryName = countryMaster.CountryName.Trim();

                        if (!_context.CountryMasters.Any(x => x.CountryName == countryMaster.CountryName && x.IsDeleted == false))
                        {
                            await _context.AddAsync(countryMaster);
                            await _context.SaveChangesAsync();

                            TempData["Success"] = "Country Successfully Created";

                            var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, null, null, "Country successfully add in database.");

                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            TempData["show-modal"] = "showModal('Warning!','Country with same name already in table!');";

                            return View(countryMaster);
                        }
                    }
                    catch (Exception ex)
                    {
                        string exMessage = CommonFunctions.getExceptionMessage(ex);

                        TempData["Warning"] = "showModal('Warning!','An error has occurred. Please try again. Error:- " + exMessage + " ');";

                        var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, ex, null, exMessage);

                        return RedirectToAction(nameof(Index));
                    }
                }

                return View(countryMaster);
            }

            return View(countryMaster);
        }

        // GET: CountryMaster/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["show-modal"] = "showModal('Record Not Found Warning!',' We could not locate any data for the provided ID.');";

                return RedirectToAction(nameof(Index));
            }

            var countryMaster = await _context.CountryMasters.FindAsync(id);

            if (countryMaster == null)
            {
                TempData["show-modal"] = "showModal('Record Not Found Warning!',' We could not locate any data for the provided ID.');";

                return RedirectToAction(nameof(Index));
            }

            return View(countryMaster);
        }

        // POST: CountryMaster/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,CountryMaster countryMaster)
        {
            if (id != countryMaster.CountryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string jsonInput = JsonConvert.SerializeObject(countryMaster);
                var isEntryInNlog = CommonFunctions.AddEntryOfLog(logger, null, jsonInput, "Country edit action input.");

                if (!_context.CountryMasters.Any(x => x.CountryName == countryMaster.CountryName && x.IsDeleted == false && x.CountryId != id))
                {
                    try
                    {
                        string jsonInputTwo = JsonConvert.SerializeObject(id);

                        var modal = _context.CountryMasters.Find(id);

                        if (modal != null)
                        {
                            modal.CountryName = countryMaster.CountryName;

                            modal.WhenModified = DateTime.UtcNow;

                            await _context.SaveChangesAsync();

                            TempData["Success"] = "Country Successfully Updated.";

                            var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, null, null, "Country successfully update in database");

                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            TempData["show-modal"] = "showModal('Record Not Found Warning!',' We could not locate any data for the provided ID.');";

                            var isEntryInNlogTwo = CommonFunctions.AddEntryOfLog(logger, null, jsonInputTwo, "Country data not found in given ID");
                        }
                    }
                    catch(Exception ex)
                    {
                        if (!CountryMasterExists(countryMaster.CountryId))
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
                }
                else
                {
                    TempData["show-modal"] = "showModal('Warning!','Country with same name already in table!');";

                    return View(countryMaster);
                }
               
                return RedirectToAction(nameof(Index));
            }
            return View(countryMaster);
        }

        // GET: CountryMaster/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var countryMaster = await _context.CountryMasters
                .FirstOrDefaultAsync(m => m.CountryId == id);
            if (countryMaster == null)
            {
                return NotFound();
            }

            return View(countryMaster);
        }

        // POST: CountryMaster/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jsonObject = new { Id = id };
            string jsonInput = JsonConvert.SerializeObject(jsonObject);

            try
            {
                var countryMaster = await _context.CountryMasters.FindAsync(id);

                if (countryMaster != null)
                {
                    if(_context.StateMasters.Any(x=>x.CountryId  == countryMaster.CountryId))
                    {
                        TempData["show-modal"] = "showModal('Record Can Not Deleted!',' The current country is userd in state.');";

                        var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, null, jsonInput, "The current country is userd in state.");
                    }
                    else
                    {

                        countryMaster.IsDeleted = true;

                        await _context.SaveChangesAsync();

                        TempData["Success"] = "Country Successfully Deleted.";

                        var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, null, jsonInput, "Country successfully deleted.");
                    }

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["show-modal"] = "showModal('Record Not Found!',' We could not locate any data for the provided ID.');";

                    var isEntryInNlog = CommonFunctions.AddEntryOfLog(logger, null, jsonInput, "Country data not found in given ID");
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string exMessage = CommonFunctions.getExceptionMessage(ex);

                TempData["Warning"] = "showModal('Warning!','An error has occurred. Please try again. Error:- " + exMessage + " ');";

                var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, ex, jsonInput, exMessage);
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool CountryMasterExists(int id)
        {
            return _context.CountryMasters.Any(e => e.CountryId == id);
        }
    }
}
