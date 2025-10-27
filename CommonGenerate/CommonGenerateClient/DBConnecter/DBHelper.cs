using CommonGenerateClient.Resource.Dto.Out;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;

namespace DBConnecter
{
    public class DBHelper
    {
        SqlSugarClient db;
        public DbType DBType { get; set; }
        public DBHelper(DbType dbType, string connectionString)
        {
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connectionString,
                DbType = dbType,
                IsAutoCloseConnection = true
            });
            DBType = dbType;
        }

        public KeyValuePair<string, CSharpDataType> GetDBTypeMapper(DbType dbType, string dbTypeName)
        {
            List<KeyValuePair<string, CSharpDataType>> mapperTypes = null;
            switch (dbType)
            {
                case DbType.MySql:
                    mapperTypes = MySqlDbBind.MappingTypesConst;
                    break;
                case DbType.SqlServer:
                    mapperTypes = SqlServerDbBind.MappingTypesConst;
                    break;
                case DbType.Sqlite:
                    mapperTypes = SqliteDbBind.MappingTypesConst;
                    break;
                case DbType.Oracle:
                    mapperTypes = OracleDbBind.MappingTypesConst;
                    break;
                case DbType.PostgreSQL:
                    mapperTypes = PostgreSQLDbBind.MappingTypesConst;
                    break;
                case DbType.Dm:
                    mapperTypes = DmDbBind.MappingTypesConst;
                    break;
                case DbType.Kdbndp:
                    mapperTypes = KdbndpDbBind.MappingTypesConst;
                    break;
                case DbType.Oscar:
                    mapperTypes = OscarDbBind.MappingTypesConst;
                    break;
                case DbType.MySqlConnector:
                    mapperTypes = MySqlDbBind.MappingTypesConst;
                    break;
                case DbType.QuestDB:
                    mapperTypes = QuestDBDbBind.MappingTypesConst;
                    break;
                case DbType.Custom:
                    throw new System.Exception("自定义数据库类型需要特殊处理");
                default:
                    throw new System.Exception("暂不支持该数据库类型");
            }

            if (mapperTypes == null)
                throw new System.Exception("数据库类型映射未初始化");

            var typeName = mapperTypes.FirstOrDefault(f => f.Key.ToLower() == dbTypeName.ToLower());
            return typeName;
        }

        public IList<UserTabColumnOutDto> GetColumnInfo(string table)
        {
            var fieldList = GetFieldInfoList(table);
            if (fieldList == null || fieldList.Count == 0)
            {
                return null;
            }

            var result = new List<UserTabColumnOutDto>();
            foreach (var field in fieldList)
            {
                //获取实体类型
                var typeName = GetDBTypeMapper(DBType, !string.IsNullOrEmpty(field.OracleDataType) ? field.OracleDataType : field.DataType);

                result.Add(new UserTabColumnOutDto
                {
                    ColumnName = field.DbColumnName,
                    Comment = field.ColumnDescription,
                    DataDefault = field.DefaultValue,
                    //DataType = field.DataType.ToString(),
                    DataType = typeName.Value.ToString(),
                    ISPrimaryKey = field.IsPrimarykey,
                    Nullable = field.IsNullable ? "Y" : "N"
                });
            }

            return result;
        }

        /// <summary>
        /// 根据表名获取表信息
        /// </summary>
        /// <param name="dataName"></param>
        /// <returns></returns>
        public UserTabCommentsOutDto GetTableByName(string dataName)
        {
            var allTableList = GetTableList();
            if (allTableList == null || allTableList.Count == 0)
            {
                return null;
            }

            foreach (var table in allTableList)
            {
                if (table.TableName == dataName)
                {
                    return table;
                }
            }

            return null;
        }

        public List<UserTabCommentsOutDto> GetTableList()
        {
            var tableList = db.DbMaintenance.GetTableInfoList(false);

            if (tableList == null || tableList.Count == 0)
            {
                return null;
            }

            var result = new List<UserTabCommentsOutDto>();
            foreach (var table in tableList)
            {
                result.Add(new UserTabCommentsOutDto
                {
                    Comments = table.Description,
                    TableName = table.Name,
                    TableType = "TABLE"
                });
            }

            var viewList = db.DbMaintenance.GetViewInfoList(false);
            if (viewList == null || viewList.Count == 0)
            {
                foreach (var view in viewList)
                {
                    result.Add(new UserTabCommentsOutDto
                    {
                        Comments = view.Description,
                        TableName = view.Name,
                        TableType = "VIEW"
                    });
                }
            }

            return result.OrderBy(x => x.TableName).ToList();
        }

        /// <summary>
        /// 获取表的字段信息
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<DbColumnInfo> GetFieldInfoList(string tableName)
        {
            List<DbColumnInfo> data = db.DbMaintenance.GetColumnInfosByTableName(tableName, false);
            return data;
        }
    }
}
