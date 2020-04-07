using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductScraper.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductScraper.Areas.Identity.Pages.Account.Manage
{
    public class EmailNotificationsSettings : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<EmailNotificationsSettings> _logger;

        public EmailNotificationsSettings(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            ILogger<EmailNotificationsSettings> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
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
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(t => t.UserId == user.Id);


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

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var userProfile = _context.UserProfiles.FirstOrDefault(t => t.UserId == user.Id);
            if (userProfile != null)
            {
                userProfile.DaysBetweenEmailNotifications = Input.DaysBetweenEmailNotifications;
                userProfile.SendEmailWhenNoProductHasBeenChanged = Input.SendEmailWhenNoProductHasBeenChanged;
                _context.UserProfiles.Update(userProfile);
                await _context.SaveChangesAsync();
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}