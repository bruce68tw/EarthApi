using BaseApi.Controllers;
using EarthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EarthApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class GoVolController : BaseCtrl
    {
        [HttpPost]
        public async Task<JsonResult> Create(string json)
        {
            return Json(await _Xp.CreateMsgA(Ctrl, false, json));
        }

    }//class
}