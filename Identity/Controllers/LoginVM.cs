using Microsoft.AspNetCore.Identity;

namespace Identity.Controllers
{
    public class LoginVM
    {
        public string Password { get; internal set; }
        public IdentityUser UserName { get; internal set; }
    }
}