using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NueroDrive.Data;
using NueroDrive.Models;
using NueroDrive.Models.ViewModels;

namespace NueroDrive.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ApplicationDbContext context, ILogger<PaymentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Instead of using database fields, create a mock view model
            var viewModel = new SubscriptionViewModel
            {
                HasActiveSubscription = false, // Default to no subscription
                SubscriptionEndDate = null,
                CurrentSubscriptionType = null,
                RecentPayments = new List<Payment>() // Empty payment history
            };

            // For demonstration purposes only
            if (TempData["ShowDemoSubscription"] != null)
            {
                viewModel.HasActiveSubscription = true;
                viewModel.CurrentSubscriptionType = SubscriptionType.Annual;
                viewModel.SubscriptionEndDate = DateTime.Now.AddYears(1);
                
                viewModel.RecentPayments = new List<Payment>
                {
                    new Payment
                    {
                        Amount = 20.00m,
                        PaymentDate = DateTime.Now,
                        SubscriptionType = SubscriptionType.Annual
                    }
                };
            }

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Subscribe()
        {
            return View(new PaymentViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Instead of saving to database, just use TempData to simulate subscription
                    TempData["ShowDemoSubscription"] = true;
                    TempData["SuccessMessage"] = $"Your {model.SubscriptionType.ToString().ToLower()} subscription was successful! (Demo Only)";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in demo subscription");
                    ModelState.AddModelError(string.Empty, "An error occurred. Please try again.");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel()
        {
            // Just remove the demo flag
            TempData.Remove("ShowDemoSubscription");
            TempData["SuccessMessage"] = "Your subscription has been cancelled. (Demo Only)";
            return RedirectToAction(nameof(Index));
        }
    }
} 