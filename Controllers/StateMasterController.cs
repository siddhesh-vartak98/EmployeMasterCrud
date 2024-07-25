using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeMasterCrud.Models;
using EmployeMasterCrud.Common;
using X.PagedList;
using NLog;
using Newtonsoft.Json;
using EmployeMasterCrud.ActionFilters;

namespace EmployeMasterCrud.Controllers
{
    [CheckSession]
    public class StateMasterController : Controller
    {
        private readonly EmployeeDemoDbContext _context;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public StateMasterController(EmployeeDemoDbContext context)
        {
            _context = context;
        }

        // GET: StateMaster
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string? ddlStatus,int? ddlContryID)
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

                var modal = from l in _context.StateMasters where l.IsDeleted == false select l;

                if (!String.IsNullOrEmpty(searchString))
                {
                    modal = modal.Where(x => x.StateName.Trim().Contains(searchString.Trim()));
                    ViewData["searchname"] = searchString;
                }

                if (ddlContryID > 0)
                {
                    modal = modal.Where(x => x.CountryId == ddlContryID);
                    ViewBag.searchCountryName = ddlContryID;
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
                        modal = modal.OrderBy(s => s.StateId);
                        break;
                    case "Name":
                        modal = modal.OrderBy(s => s.StateName);
                        break;
                    case "name_desc":
                        modal = modal.OrderByDescending(s => s.StateName);
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

                    default:
                        modal = modal.OrderByDescending(s => s.StateId);
                        break;
                }

                // <-- Count -->
                ViewBag.totalmodeldata = 0;

                if (modal != null && modal.Count() > 0)
                {
                    ViewBag.totalmodeldata = modal.Count();
                }

                ViewBag.ddlStatus = new SelectList(StaticData.IsPublishDD(), "value", "name");
                ViewBag.ddlContryID = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName");

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

        // GET: StateMaster/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stateMaster = await _context.StateMasters
                .FirstOrDefaultAsync(m => m.StateId == id);
            if (stateMaster == null)
            {
                return NotFound();
            }

            return View(stateMaster);
        }

        // GET: StateMaster/Create
        public IActionResult Create()
        {
            ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName");
            return View();
        }

        // POST: StateMaster/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StateMaster stateMaster)
        {
            if (ModelState.IsValid)
            {


                if(stateMaster != null)
                {
                    try
                    {
                        string jsonInput = JsonConvert.SerializeObject(stateMaster);
                        var isEntryInNlog = CommonFunctions.AddEntryOfLog(logger, null, jsonInput, "State create action input.");

                        stateMaster.StateName = stateMaster.StateName.Trim();

                        if (!_context.StateMasters.Any(x => (x.StateName == stateMaster.StateName && x.IsDeleted == false) && x.CountryId == stateMaster.CountryId))
                        {
                            await _context.AddAsync(stateMaster);
                            await _context.SaveChangesAsync();

                            TempData["Success"] = "State Successfully Created";

                            var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, null, null, "State successfully add in database.");

                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            TempData["show-modal"] = "showModal('Warning!','State with same name already in table!');";
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
            }

            #region Viewbag ddl list
            if (stateMaster != null)
            {
                if (stateMaster.CountryId > 0)
                {
                    ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName",stateMaster.CountryId);
                }
                else { 
                    ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName"); 
                }
            }
            else { 
                ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName"); 
            }
            #endregion

            return View(stateMaster);
        }

        // GET: StateMaster/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["show-modal"] = "showModal('Record Not Found Warning!',' We could not locate any data for the provided ID.');";

                return RedirectToAction(nameof(Index));
            }

            var stateMaster = await _context.StateMasters.FindAsync(id);

            if (stateMaster == null)
            {
                TempData["show-modal"] = "showModal('Record Not Found Warning!',' We could not locate any data for the provided ID.');";

                return RedirectToAction(nameof(Index));
            }

            if (stateMaster != null)
            {
                if (stateMaster.CountryId > 0)
                {
                    ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName", stateMaster.CountryId);
                }
                else
                {
                    ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName");
                }
            }
            else
            {
                ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName");
            }

            return View(stateMaster);
        }

        // POST: StateMaster/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StateMaster stateMaster)
        {
            if (id != stateMaster.StateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                string jsonInput = JsonConvert.SerializeObject(stateMaster);
                var isEntryInNlog = CommonFunctions.AddEntryOfLog(logger, null, jsonInput, "State edit action input.");

                try
                {
                    stateMaster.StateName = stateMaster.StateName.Trim();

                    if (!_context.StateMasters.Any(x => (x.StateName == stateMaster.StateName && x.IsDeleted == false && x.StateId != stateMaster.StateId) && x.CountryId == stateMaster.CountryId))
                    {
                        string jsonInputTwo = JsonConvert.SerializeObject(id);

                        var modal = _context.StateMasters.Find(id);

                        if (modal != null)
                        {
                            modal.CountryId = stateMaster.CountryId;
                            modal.StateName = stateMaster.StateName;

                            modal.WhenModified = DateTime.UtcNow;

                            await _context.SaveChangesAsync();

                            TempData["Success"] = "State Successfully Updated.";

                            var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, null, null, "State successfully update in database");

                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            TempData["show-modal"] = "showModal('Record Not Found Warning!',' We could not locate any data for the provided ID.');";

                            var isEntryInNlogTwo = CommonFunctions.AddEntryOfLog(logger, null, jsonInputTwo, "State data not found in given ID");
                        }
                    }
                    else
                    {
                        TempData["show-modal"] = "showModal('Warning!','State with same name already in table!');";
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!StateMasterExists(stateMaster.StateId))
                    {
                        string exMessage = CommonFunctions.getExceptionMessage(ex);

                        TempData["Warning"] = "showModal('Warning!','An error has occurred. Please try again. Error:- " + exMessage + " ');";

                        var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, ex, null, exMessage);

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        string exMessage = CommonFunctions.getExceptionMessage(ex);

                        TempData["Warning"] = "showModal('Warning!','An error has occurred. Please try again. Error:- " + exMessage + " ');";

                        var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, ex, null, exMessage);

                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            if (stateMaster != null)
            {
                if (stateMaster.CountryId > 0)
                {
                    ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName", stateMaster.CountryId);
                }
                else
                {
                    ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName");
                }
            }
            else
            {
                ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName");
            }

            return View(stateMaster);
        }

        // GET: StateMaster/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stateMaster = await _context.StateMasters
                .FirstOrDefaultAsync(m => m.StateId == id);
            if (stateMaster == null)
            {
                return NotFound();
            }

            return View(stateMaster);
        }

        // POST: StateMaster/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jsonObject = new { Id = id };
            string jsonInput = JsonConvert.SerializeObject(jsonObject);

            try
            {
                var stateMaster = await _context.StateMasters.FindAsync(id);

                if (stateMaster != null)
                {
                    if(_context.CityMasters.Any(x=>x.StateId == stateMaster.StateId))
                    {
                        TempData["show-modal"] = "showModal('Record Can Not Deleted!',' The current state is userd in city.');";

                        var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, null, jsonInput, "The current state is userd in city.");
                    }
                    else
                    {
                        stateMaster.IsDeleted = true;

                        await _context.SaveChangesAsync();

                        TempData["Success"] = "State Successfully Deleted.";

                        var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, null, jsonInput, "State successfully deleted.");
                    }

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["show-modal"] = "showModal('Record Not Found!',' We could not locate any data for the provided ID.');";

                    var isEntryInNlog = CommonFunctions.AddEntryOfLog(logger, null, jsonInput, "State data not found in given ID");
                }
                
            }
            catch(Exception ex)
            {
                string exMessage = CommonFunctions.getExceptionMessage(ex);

                TempData["Warning"] = "showModal('Warning!','An error has occurred. Please try again. Error:- " + exMessage + " ');";

                var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, ex, jsonInput, exMessage);
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool StateMasterExists(int id)
        {
            return _context.StateMasters.Any(e => e.StateId == id);
        }
    }
}
