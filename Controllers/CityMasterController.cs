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
    public class CityMasterController : Controller
    {
        private readonly EmployeeDemoDbContext _context;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public CityMasterController(EmployeeDemoDbContext context)
        {
            _context = context;
        }

        // GET: CityMaster
        public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string? ddlStatus, int? ddlContryID,int? ddlStateID)
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

                var modal = from l in _context.CityMasters where l.IsDeleted == false select l;

                if (!String.IsNullOrEmpty(searchString))
                {
                    modal = modal.Where(x => x.CityName.Trim().Contains(searchString.Trim()));
                    ViewData["searchname"] = searchString;
                }

                if (ddlContryID > 0)
                {
                    modal = modal.Where(x => x.CountryId == ddlContryID);
                    ViewBag.searchCountryName = ddlContryID;
                }

                if(ddlStateID > 0)
                {
                    modal = modal.Where(x=>x.StateId == ddlStateID);
                    ViewBag.searchStateName = ddlStateID;
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
                        modal = modal.OrderBy(s => s.CityId);
                        break;
                    case "Name":
                        modal = modal.OrderBy(s => s.CityName);
                        break;
                    case "name_desc":
                        modal = modal.OrderByDescending(s => s.CityName);
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
                        modal = modal.OrderByDescending(s => s.CityId);
                        break;
                }

                // <-- Count -->
                ViewBag.totalmodeldata = 0;

                if (modal != null && modal.Count() > 0)
                {
                    ViewBag.totalmodeldata = modal.Count();
                }

                ViewBag.ddlStatus = new SelectList(StaticData.IsPublishDD(), "value", "name");
                ViewBag.countryID = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName");

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

        // GET: CityMaster/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cityMaster = await _context.CityMasters
                .FirstOrDefaultAsync(m => m.CityId == id);
            if (cityMaster == null)
            {
                return NotFound();
            }

            return View(cityMaster);
        }

        // GET: CityMaster/Create
        public IActionResult Create()
        {
            ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName");
            ViewBag.StateId = new List<SelectListItem>();

            return View();
        }

        // POST: CityMaster/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CityMaster cityMaster)
        {
            if (ModelState.IsValid)
            {
                if(cityMaster != null)
                {
                    try
                    {
                        cityMaster.CityName = cityMaster.CityName.Trim();

                        if (!_context.CityMasters.Any(x => (x.CountryId == cityMaster.CountryId || x.StateId == cityMaster.StateId) && x.CityName == cityMaster.CityName && cityMaster.IsDeleted == false))
                        {
                            await _context.AddAsync(cityMaster);
                            await _context.SaveChangesAsync();

                            TempData["Success"] = "City Successfully Created";

                            var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, null, null, "City successfully add in database.");


                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            TempData["show-modal"] = "showModal('Warning!','City with same name already in table!');";
                        }
                    }
                    catch(Exception ex)
                    {
                        string exMessage = CommonFunctions.getExceptionMessage(ex);

                        TempData["Warning"] = "showModal('Warning!','An error has occurred. Please try again. Error:- " + exMessage + " ');";

                        var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, ex, null, exMessage);

                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            if(cityMaster != null)
            {
                if(cityMaster.CountryId > 0)
                {
                    ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName",cityMaster.CountryId);

                    if(cityMaster.StateId > 0)
                    {
                        ViewBag.StateId = new SelectList(_context.StateMasters.Where(x => x.IsDeleted == false), "StateId", "StateName", cityMaster.StateId);
                    }
                    else
                    {
                        ViewBag.StateId = new SelectList(_context.StateMasters.Where(x => x.IsDeleted == false), "StateId", "StateName");
                    }
                }
                else
                {
                    ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName");
                    ViewBag.StateId = new List<SelectListItem>();
                }
            }
            else
            {
                ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName");
                ViewBag.StateId = new List<SelectListItem>();
            }


            return View(cityMaster);
        }

        // GET: CityMaster/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cityMaster = await _context.CityMasters.FindAsync(id);
            if (cityMaster == null)
            {
                return NotFound();
            }
            return View(cityMaster);
        }

        // POST: CityMaster/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CityMaster cityMaster)
        {
            if (id != cityMaster.CityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string jsonInput = JsonConvert.SerializeObject(cityMaster);
                var isEntryInNlog = CommonFunctions.AddEntryOfLog(logger, null, jsonInput, "City edit action input.");

                try
                {
                    cityMaster.CityName = cityMaster.CityName.Trim();

                    if (!_context.CityMasters.Any(x => (x.CountryId == cityMaster.CountryId || x.StateId == cityMaster.StateId) && x.CityName == cityMaster.CityName && cityMaster.IsDeleted == false))
                    {
                        string jsonInputTwo = JsonConvert.SerializeObject(id);

                        var model = await _context.CityMasters.FindAsync(id);

                        if(model != null)
                        {
                            model.CityName = cityMaster.CityName;
                            model.CountryId = cityMaster.CountryId;
                            model.StateId = cityMaster.StateId;

                            model.WhenModified = DateTime.UtcNow;

                            await _context.SaveChangesAsync();

                            TempData["Success"] = "City Successfully Updated.";

                            var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, null, null, "City successfully update in database");

                            return RedirectToAction(nameof(Index));

                        }
                        else
                        {
                            TempData["show-modal"] = "showModal('Record Not Found Warning!',' We could not locate any data for the provided ID.');";

                            var isEntryInNlogTwo = CommonFunctions.AddEntryOfLog(logger, null, jsonInputTwo, "City data not found in given ID");
                        }
                    }
                    else
                    {
                        TempData["show-modal"] = "showModal('Warning!','City with same name already in table!');";
                    }

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!CityMasterExists(cityMaster.CityId))
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
                //return RedirectToAction(nameof(Index));
            }

            if (cityMaster != null)
            {
                if (cityMaster.CountryId > 0)
                {
                    ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName", cityMaster.CountryId);

                    if (cityMaster.StateId > 0)
                    {
                        ViewBag.StateId = new SelectList(_context.StateMasters.Where(x => x.IsDeleted == false), "StateId", "StateName", cityMaster.StateId);
                    }
                    else
                    {
                        ViewBag.StateId = new SelectList(_context.StateMasters.Where(x => x.IsDeleted == false), "StateId", "StateName");
                    }
                }
                else
                {
                    ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName");
                    ViewBag.StateId = new List<SelectListItem>();
                }
            }
            else
            {
                ViewBag.CountryId = new SelectList(_context.CountryMasters.Where(x => x.IsDeleted == false), "CountryId", "CountryName");
                ViewBag.StateId = new List<SelectListItem>();
            }

            return View(cityMaster);
        }

        // GET: CityMaster/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cityMaster = await _context.CityMasters
                .FirstOrDefaultAsync(m => m.CityId == id);
            if (cityMaster == null)
            {
                return NotFound();
            }

            return View(cityMaster);
        }

        // POST: CityMaster/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jsonObject = new { Id = id };
            string jsonInput = JsonConvert.SerializeObject(jsonObject);

            try
            {
                var cityMaster = await _context.CityMasters.FindAsync(id);

                if (cityMaster != null)
                {
                    cityMaster.IsDeleted = true;
                    cityMaster.IsActive = false;

                    await _context.SaveChangesAsync();

                    TempData["Success"] = "City Successfully Deleted.";

                    var isEntryInNlogOne = CommonFunctions.AddEntryOfLog(logger, null, jsonInput, "City successfully deleted.");
                }
                else
                {
                    TempData["show-modal"] = "showModal('Record Not Found!',' We could not locate any data for the provided ID.');";

                    var isEntryInNlog = CommonFunctions.AddEntryOfLog(logger, null, jsonInput, "City data not found in given ID");
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

        private bool CityMasterExists(int id)
        {
            return _context.CityMasters.Any(e => e.CityId == id);
        }
    }
}
