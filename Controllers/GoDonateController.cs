using BaseApi.Controllers;
using EarthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EarthApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class GoDonateController : BaseCtrl
    {
        [HttpPost]
        public async Task<JsonResult> Create(string json)
        {
            return Json(await _Xp.CreateMsgA(Ctrl, true, json));
        }

    }//class
}