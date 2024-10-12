using Base.Models;
using Base.Services;

namespace EarthApi.Services
{
    public class ActE : BaseEditSvc
    {
        public ActE(string ctrl) : base(ctrl) { }

        override public EditDto GetDto()
        {
            return new EditDto
            {
                ReadSql = $@"
select *
from dbo.Act
where Id=@Id
",
                Childs = new EditDto[]
                {
                    new EditDto
                    {
                        ReadSql = @"
select a.*,
    ItemName=i.Name, i.Unit
from dbo.ActItem a
join dbo.Item i on a.ItemId=i.Id
where a.ActId=@Id
order by i.Id
",
                    },
                    new EditDto
                    {
                        ReadSql = @"
select Id
from dbo.[Output]
where ActId=@Id
order by Id
",
                    },
                },
            };
        }

    } //class
}
