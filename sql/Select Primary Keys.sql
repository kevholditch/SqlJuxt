SELECT  OBJECT_SCHEMA_NAME(o.parent_object_id) AS [Schema], 
		OBJECT_NAME(o.parent_object_id) AS [Table], 
		o.name AS PrimaryKey, 
		ic.key_ordinal "KeyOrdinal", 
		c.name AS ColumnName, 
		ic.is_descending_key "IsDescending",
		CASE WHEN i.type_desc = 'CLUSTERED' THEN 1 ELSE 0 END as "IsClustered"
from sys.objects o
inner join sys.indexes i on i.object_id = o.parent_object_id and i.is_primary_key = 1
inner join sys.index_columns ic on ic.object_id = i.object_id and ic.index_id = i.index_id
inner join sys.columns c on c.object_id = o.parent_object_id and c.column_id = ic.column_id
inner join sys.types t ON t.user_type_id = c.user_type_id
where o.type = 'PK'
ORDER BY OBJECT_SCHEMA_NAME(o.parent_object_id), OBJECT_NAME(o.parent_object_id), ic.key_ordinal




