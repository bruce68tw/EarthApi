using Base.Models;
using Base.Services;

namespace EarthApi.Services
{
    public class DonateE : BaseEditSvc
    {
        public DonateE(string ctrl) : base(ctrl) { }

        override public EditDto GetDto()
        {
            return new EditDto
            {
                ReadSql = @"
select a.*, DonorName=u.Name
from dbo.Donate a
join dbo.Donor u on a.DonorId=u.Id
where a.Id=@Id
",
                Childs = new EditDto[]
                {
                    new EditDto
                    {
                        ReadSql = @"
select a.*,
    ItemName=i.Name, i.Unit
from dbo.DonateItem a
join dbo.Item i on a.ItemId=i.Id
where a.DonateId=@Id
order by i.Id
",
                    },
                    new EditDto
                    {
                        ReadSql = @"
select Id
from dbo.[Input]
where DonateId=@Id
order by Id
",
                    },
                },
            };
        }

    } //class
}
