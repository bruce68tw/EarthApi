using Base.Models;
using Base.Services;

namespace EarthApi.Services
{
    public class InputE : BaseEditSvc
    {
        public InputE(string ctrl) : base(ctrl) { }

        override public EditDto GetDto()
        {
            return new EditDto
            {
                ReadSql = @"
select *
from dbo.Input
where Id=@Id
",
                Childs = new EditDto[]
                {
                    new EditDto
                    {
                        ReadSql = @"
select a.*,
    ItemName=i.Name, i.Unit
from dbo.InputItem a
join dbo.Item i on a.ItemId=i.Id
where a.InputId=@Id
order by i.Id
",
                    },
                },
            };
        }

    } //class
}
