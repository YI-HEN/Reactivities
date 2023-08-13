using API.Extensions;
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
            if (result.IsSuccess && result.Value != null) 
                return Ok(result.Value); //成功有資料

            if (result.IsSuccess && result.Value == null) 
                return NotFound();  //沒有資料

            return BadRequest(result.Error);    //邏輯問題
        }

        protected ActionResult HandlePageResult<T>(Result<PagedList<T>> result)
        {
            if (result == null) return NotFound();
            if (result.IsSuccess && result.Value != null) 
            {
                Response.AddPaginationHeader(result.Value.CurrentPage, result.Value.PageSize, 
                    result.Value.TotalCount, result.Value.TotalPages);
                return Ok(result.Value); //成功有資料
            }
            if (result.IsSuccess && result.Value == null) 
                return NotFound();  //沒有資料

            return BadRequest(result.Error);    //邏輯問題
        }
    }
}