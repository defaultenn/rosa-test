using Microsoft.AspNetCore.Mvc;
using RosATest.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RosATest.Controllers
{
    public class InquiryRequestsController : Controller
    {
        private readonly IInquiryRequestService _inquiryRequestSerivce;
        private readonly ILogger<InquiryRequestsController> _logger;

        public InquiryRequestsController(IInquiryRequestService service, ILogger<InquiryRequestsController> logger)
        {
            _inquiryRequestSerivce = service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> List(ListInquiryRequestsViewModel model)
        {
            string? userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _inquiryRequestSerivce.GetUser(userID);
            
            var statuses = await _inquiryRequestSerivce.GetStatusesAsync();

            ViewBag.Statuses = statuses.Select(s => new SelectListItem
            {
                Value = s.ID.ToString(),
                Text = s.Name,
            }).ToList();
            
            ViewBag.Requests = await _inquiryRequestSerivce.GetInquiryRequests(user, model);
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var types = await _inquiryRequestSerivce.GetInquiryTypesAsync();
            ViewBag.InquiryTypes = new SelectList(types, "ID", "Name");
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateInquiryRequestViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return await Create();
            }

            string? userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _inquiryRequestSerivce.GetUser(userID);
            try {
                await _inquiryRequestSerivce.CreateInquiryRequestAsync(user, model);
            } catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return await Create();
            }
            return RedirectToAction("List");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Update(int id)
        {
            string? userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _inquiryRequestSerivce.GetUser(userID);

            var statuses = await _inquiryRequestSerivce.GetStatusesAsync();
            ViewBag.Statuses = statuses.Select(s => new SelectListItem
            {
                Value = s.ID.ToString(),
                Text = s.Name,
                Selected = false
            }).ToList();

            try
            {
                var request = await _inquiryRequestSerivce.GetByIDAsync(user, id);
                ViewBag.Request = request;
            } catch (Exception)
            {
                // TODO: оставить сообщение
                return RedirectToAction("List");
            }
            
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateInquiryRequestViewModel model, int id)
        {
            if(!ModelState.IsValid)
            {
                return await Update(id);
            }

            string? userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _inquiryRequestSerivce.GetUser(userID);

            if(user.Group.Codename == Model.Entity.Group.GroupCodename.Accountant)
            {
                await _inquiryRequestSerivce.UpdateInquiryRequestAsync(user, id, model);
            }

            return RedirectToAction("List");
        }
    }
}