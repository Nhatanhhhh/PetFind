using Microsoft.AspNetCore.Mvc;
using PetStore.DAO.Interfaces;
using PetStore.Models.DTOs;

namespace PetStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPetReportDAO _petReportDAO;

        public HomeController(IPetReportDAO petReportDAO)
        {
            _petReportDAO = petReportDAO;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var recentReports = await _petReportDAO.GetAllReportsAsync();
                if (recentReports == null || !recentReports.Any())
                    Console.WriteLine("No pet reports found.");
                return View(recentReports);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching reports: {ex.Message}");
                return View(new List<PetReportResponseDTO>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
