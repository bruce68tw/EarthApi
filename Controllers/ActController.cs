using Base.Models;
using BaseApi.Controllers;
using EarthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EarthApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ActController : BaseCtrl
    {
        [HttpPost]
        public async Task<ContentResult> GetPage(EasyDtDto dt)
        {
            return JsonToCnt(await new ActR().GetPageA(Ctrl, dt));
        }

        private ActE EditService()
        {
            return new ActE(Ctrl);
        }

        public async Task<FileResult?> Image(string key, string ext)
        {
            return await _Xp.ViewActImageA(key, ext);
        }

        //get item
        [HttpPost]
        public async Task<ContentResult> Detail(string id)
        {
            return JsonToCnt(await EditService().GetViewJsonA(id));
        }

    }//class
}