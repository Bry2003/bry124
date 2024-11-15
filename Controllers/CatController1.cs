using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FoodDelivery4.Models;

namespace FoodDelivery4.Controllers
{
    public class CatController1 : Controller
    {
        HttpClient client = new HttpClient();   
        // GET: CatController1
        public async Task<IActionResult> Index()
        {
            string url = "https://catfact.ninja/fact";
            var response = await client.GetStringAsync(url);
            var catFact = JsonConvert.DeserializeObject<CatFact>(response);

            return View(catFact);   
        }

        // GET: CatController1/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CatController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CatController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CatController1/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CatController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CatController1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CatController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
