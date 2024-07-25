using EmployeMasterCrud.Common;
using EmployeMasterCrud.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;

namespace EmployeMasterCrud.Controllers
{
    public class AjaxController : Controller
    {
        private readonly EmployeeDemoDbContext _context;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public AjaxController(EmployeeDemoDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public string GetStates(int countryID)
        {
            string json = string.Empty;

            if(countryID > 0)
            {
                var model = from l in _context.StateMasters where l.IsDeleted == false && l.IsActive == true && l.CountryId == countryID select new {l.StateName,l.StateId};

                if(model != null)
                {
                    if (model.Any())
                    {
                        List<stateValueModel> list = new();

                        foreach(var item in model)
                        {
                            if(item != null)
                            {
                                list.Add(new stateValueModel
                                {
                                    StateId = item.StateId,
                                    StateName = item.StateName,
                                });
                            }
                        }

                        if(list != null)
                        {
                            if (list.Any())
                            {
                                json = JsonConvert.SerializeObject(list);
                            }
                        }
                    }
                }
            }

            return json;
        }

        public string GetCities(int StateID)
        {
            string json = string.Empty;

            if (StateID > 0)
            {
                var model = from l in _context.CityMasters where l.IsDeleted == false && l.IsActive == true && l.StateId == StateID select new { l.CityName, l.CityId };

                if (model != null)
                {
                    if (model.Any())
                    {
                        List<cityValueModel> list = new();

                        foreach (var item in model)
                        {
                            if (item != null)
                            {
                                list.Add(new cityValueModel
                                {
                                    CityId = item.CityId,
                                    CityName = item.CityName,
                                });
                            }
                        }

                        if (list != null)
                        {
                            if (list.Any())
                            {
                                json = JsonConvert.SerializeObject(list);
                            }
                        }
                    }
                }
            }

            return json;
        }
    }
}
