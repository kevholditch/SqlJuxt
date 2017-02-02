select OBJECT_SCHEMA_NAME(o.object_id) AS [Schema], 
		OBJECT_NAME(o.object_id) AS TableName,
		i.name AS IndexName,
		ic.key_ordinal "KeyOrdinal", 
		c.name AS ColumnName, 
		ic.is_descending_key "IsDescending",
		CASE WHEN i.type_desc = 'CLUSTERED' THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS "IsClustered",
		i.is_primary_key AS "IsPrimaryKey",
		i.is_unique AS "IsUnique"
from sys.objects o
join sys.indexes i on i.object_id = o.object_id
join sys.index_columns ic on ic.object_id = i.object_id and ic.index_id = i.index_id
join sys.columns c on c.object_id = o.object_id and c.column_id = ic.column_id
where OBJECT_SCHEMA_NAME(o.object_id) <> 'sys'
ORDER BY OBJECT_SCHEMA_NAME(o.object_id), OBJECT_NAME(o.object_id), i.name, ic.key_ordinal
where i.object_id = 245575913 and OBJECT_SCHEMA_NAME(o.object_id) <> 'dbo'

select *
from sys.indexes


select *
from sys.objects o
join sys.indexes i on i.object_id = o.object_id
join sys.index_columns ic on ic.object_id = i.object_id and ic.index_id = i.index_id
join sys.columns c on c.object_id = o.object_id and c.column_id = ic.column_id
where i.object_id = 245575913
and i.is_primary_key = 0
and i.type = 1