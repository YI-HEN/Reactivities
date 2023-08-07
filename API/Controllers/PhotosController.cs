using Application.Photos;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PhotosController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> Add ([FromForm] Add.Command command)
        //public async Task<IActionResult> Add ([FromForm] IFormFile command) 標註與未標註擇一使用
        {
            return HandleResult(await Mediator.Send(command));
            //return HandleResult(await Mediator.Send(new Add.Command{File = command})); 標註與未標註擇一使用
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete (string id)
        // public async Task<IActionResult> Delete (Delete.Command id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command{Id = id}));
            // return HandleResult(await Mediator.Send(id));
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMain (string id)
        {
            return HandleResult(await Mediator.Send(new SetMain.Command{Id = id}));
        }
    }
}