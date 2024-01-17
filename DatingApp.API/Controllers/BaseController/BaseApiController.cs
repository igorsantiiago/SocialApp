using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/v1/[controller]")]
public class BaseApiController : ControllerBase
{

}
