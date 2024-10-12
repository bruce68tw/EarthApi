using Base.Models;
using Base.Services;

namespace EarthApi.Services
{
    public class MsgE : BaseEditSvc
    {
        public MsgE(string ctrl) : base(ctrl) { }

        override public EditDto GetDto()
        {
            return new EditDto
            {
                AutoIdLen = _Fun.AutoIdLong,
                Table = "dbo.Msg",
                PkeyFid = "Id",
                Col4 = new[] { null, "Created" },
                Items = new EitemDto[]
                {
                    new() { Fid = "Id" },
                    new() { Fid = "IsDonate" },
                    new() { Fid = "UserName" },
                    new() { Fid = "Phone" },
                    new() { Fid = "Note" },
                },
            };
        }

    } //class
}
