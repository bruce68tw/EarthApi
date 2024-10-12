using Base.Enums;
using Base.Models;
using Base.Services;
using BaseApi.Controllers;
using BaseApi.Services;
using EarthApi.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace EarthApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class XpController : BaseCtrl
    {
        [HttpPost]
        public async Task<ContentResult> GetItem(string id)
        {
            return JsonToCnt(await _Db.GetRowA("select * from dbo.Item where Id=@Id", new() { "Id", id }));
        }

        //get donor list for modal
        [HttpPost]
        public async Task<ContentResult> GetDonors(string? name, string? phone)
        {
            //sql 考慮可能使用Dept table, 所以加上 table alias
            var sql = @"
select Id, Name, Phone
from dbo.Donor
where (@Name='' or Name like @Name)
and (@Phone='' or Phone like @Phone)
order by Name
";
            var rows = await _Db.GetRowsA(sql, new List<object>()
                { "Name", _Str.AddLike(name), "Phone", _Str.AddLike(phone) });
            return JsonsToCnt(rows);
        }

        //get act list for modal
        [HttpPost]
        public async Task<ContentResult> GetActs(string? Id, string? Name)
        {
            var sql = @"
select Id, Name
from dbo.Act
where (@Id='' or Id like @Id)
and (@Name='' or Name like @Name)
order by Id
";
            var rows = await _Db.GetRowsA(sql, new List<object>()
                { "Id", _Str.AddLike(Id), "Name", _Str.AddLike(Name) });
            return JsonsToCnt(rows);
        }

        //get donate list for modal, auditStatus='Y' only
        [HttpPost]
        public async Task<ContentResult> GetDonates(string? PlanDate, string? PlanDate2, int? MyData, int? InputStatus)
        {
            //sql 考慮可能使用Dept table, 所以加上 table alias
            var findJson = new JObject()
            {
                ["PlanDate"] = PlanDate,
                ["PlanDate2"] = PlanDate2,
            };
            if (MyData == 1) findJson["Creator"] = _Fun.UserId();
            if (InputStatus == 1) findJson["InputStatus"] = "N,P";  //未入庫or部分入庫

            ReadDto dto = new()
            {
                ReadSql = @"
select a.Id, a.PlanDate, a.InputStatus, a.Title,
    u.Name
from dbo.Donate a
join dbo.[User] u on a.Creator=u.Id
where a.AuditStatus='Y'
order by a.Id
",
                TableAs = "a",
                Items = new QitemDto[] {
                    new() { Fid = "PlanDate", Type = QitemTypeEnum.Date },
                    new() { Fid = "InputStatus", Op = ItemOpEstr.In },
                    new() { Fid = "Creator" },
                },
            };
            return JsonsToCnt(await new CrudReadSvc().GetRowsA(dto, findJson, 300));
        }

        /// <summary>
        /// 傳回 Captcha 圖檔
        /// </summary>
        /// <returns></returns>
        public FileResult Captcha()
        {
            //session write code
            var userId = _Http.GetCookie("UserId");
            var code = _Str.RandomStr(5, 1);
            _Cache.SetStr(userId, _Xp.CaptchaCacheFid, code);

            //response Captcha image
            return File(_Http.OutputStrImage(code, 70).ToArray(), "image/png");
        }
    }//class
}