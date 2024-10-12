using BaseApi.Controllers;
using EarthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EarthApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class OutputController : BaseCtrl
    {
        private OutputE EditService()
        {
            return new OutputE(Ctrl);
        }

        public async Task<ContentResult> Detail(string id)
        {
            return JsonToCnt(await EditService().GetViewJsonA(id));
        }

        [HttpPost]
        public async Task<ContentResult> GetTx(string id)
        {
            return JsonToCnt(await _Xp.GetTxSourceAndRow(false, id));
        }

    }//class
}