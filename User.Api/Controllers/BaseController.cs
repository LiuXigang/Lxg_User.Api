using Microsoft.AspNetCore.Mvc;
using User.Api.Dtos;

namespace User.Api.Controllers
{
    public class BaseController : Controller//ControllerBase
    {
        protected UserIdentity UserIdentity => new UserIdentity() { UserId = 1, Name = "admin" };
    }
}
