using Base.Services;
using BaseApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace EarthApi.Controllers
{
	[ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : BaseCtrl
    {
        [HttpPost]
        public async Task<ContentResult> GetData()
        {
            //rpt1: 物品大類募集與使用情形
            var db = new Db();
            var typeUse = await db.GetRowsA(@"
select Name=t.Name+' ('+t.CountName+')', 
	StockWayAmount=Sum(a.StockWayAmount),
	OutputAmount=Sum(a.OutputAmount),
	StockAmount=Sum(a.StockAmount)
from (
	--在途庫存
	select a.ItemId, StockWayAmount=Sum(case when a.Amount > a.InputAmount then a.Amount - a.InputAmount else 0 end),
		OutputAmount=0, StockAmount=0
	from dbo.DonateItem a
	join dbo.Donate d on a.DonateId=d.Id
	where d.AuditStatus='Y'
	and d.InputStatus != 'Y'
	group by a.ItemId

	--已出貨
	union select a.ItemId, StockWayAmount=0, OutputAmount=Sum(a.Amount), StockAmount=0
	from dbo.OutputItem a
	join dbo.Output o on a.OutputId=o.Id
	where o.OutputStatus=1
	group by a.ItemId

	--實際庫存
	union select ItemId=a.Id, StockWayAmount=0, OutputAmount=0, StockAmount=Sum(a.StockAmount)
	from dbo.Item a
	group by a.Id
) a
join dbo.Item i on a.ItemId=i.Id
join dbo.ItemType t on i.TypeId=t.Id
where a.StockWayAmount + a.OutputAmount + a.StockAmount > 0
group by t.Name, t.CountName, t.Sort
order by t.Sort
");

			//rpt2: 物品短缺前10項統計
			var short10 = await db.GetRowsA(@"
select top 10 a.Name, a.Amount
from (
	--短缺數量 = 活動數量 - 庫存 - 在途庫存
	select i.Name, Amount=(a.ActAmount - i.StockAmount - i.StockWayAmount) * i.CountAmount
	from (
		--活動單未轉出貨
		select ai.ItemId, ActAmount=Sum(ai.Amount)
		from dbo.ActItem ai
		join dbo.Act a on ai.ActId=a.Id
		where a.AuditStatus='Y'
		and a.TranStatus=0
		group by ai.ItemId
	) a 
	join dbo.Item i on a.ItemId=i.Id
) a 
where a.Amount > 0
order by a.Amount desc
");

            //rpt3: 物品大類募集比例
            var donatePct = await db.GetRowsA(@"

select Name=it.Name+' ('+it.CountName+')', a.Amount
from (
	select i.TypeId, Amount=Sum(di.Amount * i.CountAmount)
	from dbo.DonateItem di
	join dbo.Donate d on di.DonateId=d.Id
	join dbo.Item i on di.ItemId=i.Id
	where d.AuditStatus='Y'
	group by i.TypeId
) a
join dbo.ItemType it on a.TypeId=it.Id
order by a.Amount desc
");

            //rpt4: 物品過去7天使用前5項
			//var beforeDays = 
            var top5Item = await db.GetRowsA(@"
--建立變數table
declare @table table (
    ItemId varchar(10),
	OutputDate date,
    Amount decimal
);

--寫入@table
insert into @table(ItemId, OutputDate, Amount)
select a.ItemId, OutputDate=cast(o.OutputTime as date), Amount=Sum(a.Amount * i.CountAmount)
from dbo.OutputItem a
join dbo.Output o on a.OutputId=o.Id
join dbo.Item i on a.ItemId=i.Id
where o.OutputStatus=1
--dateAdd的參數為datetime, 不是date
and o.OutputTime between cast(dateAdd(day, -7, getDate()) as date) and dateAdd(second, -1, cast(cast(getdate() as date) as datetime))
group by a.ItemId, cast(o.OutputTime as date)

--傳回
select i.Name, a.OutputDate, Amount=a.Amount * i.CountAmount
from @table a
join dbo.Item i on a.ItemId=i.Id
where a.ItemId in (
	--取合計數量最大者前5筆
	select top 5 ItemId
	from (
		select ItemId, Amount=Sum(Amount)
		from @table
		group by ItemId
	) a 
	order by Amount desc
)
");

			//close db
            await db.DisposeAsync();

            return JsonToCnt(new JObject
			{
				{ "typeUse", typeUse },
                { "short10", short10 },
                { "donatePct", donatePct },
                { "top5Item", top5Item },
            });

        }

    }//class
}