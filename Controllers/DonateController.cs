using Base.Models;
using BaseApi.Controllers;
using EarthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EarthApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class DonateController : BaseCtrl
    {
        [HttpPost]
        public async Task<ContentResult> GetPage(EasyDtDto dt)
        {
            //var aa = new Test1().MaskPII("LeetCode@LeetCode.com");
            return JsonToCnt(await new DonateR().GetPageA(Ctrl, dt));
        }

        private DonateE EditService()
        {
            return new DonateE(Ctrl);
        }

        public async Task<ContentResult> Detail(string id)
        {
            return JsonToCnt(await EditService().GetViewJsonA(id));
        }

    }//class
}