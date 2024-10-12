using Base.Models;
using Base.Services;
using Newtonsoft.Json.Linq;

namespace EarthApi.Services
{
    public class DonateR
    {
        private ReadDto GetDto()
        {
            return new()
            {
                ReadSql = $@"
select a.*, DonorName=d.Name
from dbo.Donate a
join dbo.Donor d on a.DonorId=d.Id
order by a.Id
",
            };
        }

        public async Task<JObject?> GetPageA(string ctrl, EasyDtDto dt)
        {
            return await new CrudReadSvc().GetPageA(GetDto(), dt, ctrl);
        }

    } //class
}