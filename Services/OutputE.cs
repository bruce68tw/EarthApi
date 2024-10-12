using Base.Models;
using Base.Services;

namespace EarthApi.Services
{
    public class OutputE : BaseEditSvc
    {
        public OutputE(string ctrl) : base(ctrl) { }

        override public EditDto GetDto()
        {
            return new EditDto
            {
                ReadSql = @"
select *
from dbo.Output
where Id=@Id
",
                Childs = new EditDto[]
                {
                    new EditDto
                    {
                        ReadSql = @"
select a.*,
    ItemName=i.Name, i.Unit
from dbo.OutputItem a
join dbo.Item i on a.ItemId=i.Id
where a.OutputId=@Id
order by i.Id
",
                    },
                },
            };
        }

    } //class
}
