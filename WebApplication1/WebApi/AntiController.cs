using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApplication1.WebApi
{
    [Route("api/[controller]/")]
    [IgnoreAntiforgeryToken]
    [ApiController]
    public class AntiController : Controller
    {
        IAntiforgery antiforgery;

        public AntiController(IAntiforgery antiForgery)
        {
            this.antiforgery = antiForgery;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetToken")]
        public IActionResult GetToken()
        {
            var tokens = antiforgery.GetTokens(HttpContext);

            if (tokens != null) return Ok(tokens.RequestToken);

            return BadRequest("Token Empty");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("ValidationToken")]
        public IActionResult ValidationToken(string Token)
        {
            HttpContext.Request.Headers.Add("RequestVerificationToken", Token);
            var tokens = antiforgery.ValidateRequestAsync(HttpContext);

            if (tokens != null) return Ok($"{tokens.Status} {tokens.AsyncState} {tokens.CreationOptions} {tokens.IsCompletedSuccessfully}");

            return BadRequest("Token Empty");
        }
    }
}