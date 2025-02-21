
using AdminDesk.Entities;
using AdminDesk.Models.Report;
using AdminDesk.Models.ServiceOrder;
using AdminDesk.Repositories;
using Google.Protobuf;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using static AdminDesk.Entities.Report;

namespace AdminDesk.Controllers
{
    public class ReportController : Controller

    {

        private readonly IReportRepository _reportRepository;
        private readonly UserManager<IdentityUser> _userManager;


        public ReportController(IReportRepository reportRepository, UserManager<IdentityUser> userManager)
        {
            _reportRepository = reportRepository;
            _userManager = userManager;
        }


        [HttpGet]
        public IActionResult Index(int id)
        {
            var model = new ReportFullViewModel
            {
                UpsertModel = new ReportViewModel
                {
                    ServiceOrderId = id
                }
            };

            return View(model);
        }


        [HttpGet]
        public IActionResult CheckList(int id)
        {
            var model = new ReportFullViewModel
            {
                UpsertModel = new ReportViewModel
                {
                    ServiceOrderId = id
                }
            };

            return View("CheckList2", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostReport(ReportFullViewModel report)
        {

            var currentUser = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                if (currentUser != null)
                {
                    var reportEntity = new Report
                    {
                        // Map properties from the ViewModel
                        ServiceOrderId = report.UpsertModel.ServiceOrderId,
                        Mechanic = report.UpsertModel.Mechanic,
                        ServiceType = report.UpsertModel.ServiceType,
                        MechanicComment = report.UpsertModel.MechanicComment,
                        ServiceDescription = report.UpsertModel.ServiceDescription,
                        ReportWriteDate = report.UpsertModel.ReportWriteDate,

                        // Set UserSign to the username of the current logged-in user
                        UserSign = currentUser.Id
                    };

                    _reportRepository.Upsert(reportEntity);

                    // Redirect to the details page or any other page as needed
                    return RedirectToAction("Spesific", "ServiceOrder", new { id = report.UpsertModel.ServiceOrderId });
                }
                else
                {
                    // Handle the case where the current user is not available
                    return BadRequest("Unable to determine the current user.");
                }

            }
            else
            {
                return View("Index", report);
            }
        }


    }
}
