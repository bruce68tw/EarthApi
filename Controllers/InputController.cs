using BaseApi.Controllers;
using EarthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EarthApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class InputController : BaseCtrl
    {
        private InputE EditService()
        {
            return new InputE(Ctrl);
        }

        /// <summary>
        /// 傳回明細頁資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ContentResult> Detail(string id)
        {
            return JsonToCnt(await EditService().GetViewJsonA(id));
        }

        /// <summary>
        /// 傳回區塊鏈資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ContentResult> GetTx(string id)
        {
            return JsonToCnt(await _Xp.GetTxSourceAndRow(true, id));
        }

    }//class
}