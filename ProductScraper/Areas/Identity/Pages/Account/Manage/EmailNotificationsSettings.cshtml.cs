using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ProductScraper.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentityUser = ElCamino.AspNetCore.Identity.AzureTable.Model.IdentityUser;

namespace ProductScraper.Areas.Identity.Pages.Account.Manage
{
    public class EmailNotificationsSettings : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<EmailNotificationsSettings> _logger;
        private readonly IUserProfileService _userProfileService;

        public EmailNotificationsSettings(
            IUserProfileService userProfileService,
            UserManager<IdentityUser> userManager,
            ILogger<EmailNotificationsSettings> logger)
        {
            _userManager = userManager;
            _logger = logger;
            _userProfileService = userProfileService;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            await LoadAsync(user);
            return Page();
        }

        public class InputModel
        {
            public bool EveryDay { get; set; }
            public bool EveryMonday { get; set; }
            public bool EveryMonth { get; set; }

            [Display(Name = "Days between email notifications")]
            public int DaysBetweenEmailNotifications { get; set; }

            [Display(Name = "Send email when no product has been changed")]
            public bool SendEmailWhenNoProductHasBeenChanged { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        private async Task LoadAsync(IdentityUser user)
        {
            var userProfile = await _userProfileService.GetByUserId(user.Id);
            Input = new InputModel
            {
                DaysBetweenEmailNotifications = userProfile.DaysBetweenEmailNotifications,
                SendEmailWhenNoProductHasBeenChanged = userProfile.SendEmailWhenNoProductHasBeenChanged,
                EveryDay = userProfile.DaysBetweenEmailNotifications == 1,
                EveryMonday = userProfile.DaysBetweenEmailNotifications == 7,
                EveryMonth = userProfile.DaysBetweenEmailNotifications == 30,
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userProfile = await _userProfileService.GetByUserId(user.Id);
            if (userProfile != null)
            {
                userProfile.DaysBetweenEmailNotifications = Input.DaysBetweenEmailNotifications;
                userProfile.SendEmailWhenNoProductHasBeenChanged = Input.SendEmailWhenNoProductHasBeenChanged;
                await _userProfileService.UpdateAsync(userProfile);
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}