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
            public string EmailFrequency { get; set; }

            [Display(Name = "Enable email notifications")]
            public bool EnableEmailNotifications { get; set; }

            [Display(Name = "Days between email notifications")]
            public int DaysBetweenEmailNotifications { get; set; }

            [Display(Name = "Send email when no product has been changed")]
            public bool SendEmailWhenNoProductHasBeenChanged { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string[] EmailFrequencies= new[] { "Daily", "Weekly", "Monthly", "Custom" };

        private async Task LoadAsync(IdentityUser user)
        {
            var userProfile = await _userProfileService.GetByUserId(user.Id);
            Input = new InputModel
            {
                EnableEmailNotifications = userProfile.EnableEmailNotifications,
                DaysBetweenEmailNotifications = userProfile.DaysBetweenEmailNotifications,                
                SendEmailWhenNoProductHasBeenChanged = userProfile.SendEmailWhenNoProductHasBeenChanged,
                EmailFrequency = "Custom"
            };

            if (userProfile.DaysBetweenEmailNotifications == 1)
                Input.EmailFrequency = "Daily";
            else if (userProfile.DaysBetweenEmailNotifications == 7)
                Input.EmailFrequency = "Weekly";
            else if (userProfile.DaysBetweenEmailNotifications == 30)
                Input.EmailFrequency = "Monthly";                            
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
                userProfile.EnableEmailNotifications = Input.EnableEmailNotifications;
                switch (Input.EmailFrequency)
                {
                    case "Daily":
                        userProfile.DaysBetweenEmailNotifications = 1;
                        break;
                    case "Weekly":
                        userProfile.DaysBetweenEmailNotifications = 7;
                        break;
                    case "Monthly":
                        userProfile.DaysBetweenEmailNotifications = 30;
                        break;
                    default:
                        userProfile.DaysBetweenEmailNotifications = Input.DaysBetweenEmailNotifications;
                        break;
                }
                userProfile.SendEmailWhenNoProductHasBeenChanged = Input.SendEmailWhenNoProductHasBeenChanged;
                await _userProfileService.UpdateAsync(userProfile);
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}