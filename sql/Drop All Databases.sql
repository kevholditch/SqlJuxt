
create table #temptable(rowNumber int, name nvarchar(max))

insert into #temptable(rowNumber, name)
select ROW_NUMBER() OVER(ORDER BY name), 'ALTER DATABASE [' + name + '] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [' + name +'];'
from sys.databases 
where name like '%Sql%'

declare @rows int

declare @i int
declare @sql nvarchar(max)
set @i = 1

select @rows = count(*)
from #temptable

while @i < @rows
begin

	select @sql = name
	from #temptable
	where rowNumber = @i

	exec sp_executesql @statement = @sql

	set @i = @i +1
end

drop table #temptable