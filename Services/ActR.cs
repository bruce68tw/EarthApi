using Base.Models;
using Base.Services;
using Newtonsoft.Json.Linq;

namespace EarthApi.Services
{
    public class ActR
    {
        private int _filter = 0;
        private ReadDto GetDto()
        {
            return new()
            {
                ReadSql = $@"
select *
from dbo.Act
where ({_filter}=0 or ({_filter}=1 and EndDate >= cast(getdate() as date)))
order by Id
",
            };
        }

        public async Task<JObject?> GetPageA(string ctrl, EasyDtDto dt)
        {
            var json = _Str.ToJson(dt.findJson);
            _filter = Convert.ToInt32(json!["_filter"]);
            return await new CrudReadSvc().GetPageA(GetDto(), dt, ctrl);
        }

    } //class
}