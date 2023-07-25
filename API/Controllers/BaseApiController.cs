using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (result == null) return NotFound();
            if (result.IsSucces && result.Value != null) 
                return Ok(result.Value); //成功有資料

            if (result.IsSucces && result.Value == null) 
                return NotFound();  //沒有資料

            return BadRequest(result.Error);    //邏輯問題
        }
    }
}