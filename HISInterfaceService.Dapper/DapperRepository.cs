using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using HISInterfaceService.Core.Dapper;

namespace HISInterfaceService.Dapper
{
    public class DapperRepository : IDapperRepository
    {
        internal static string connStr = ConfigurationManager.ConnectionStrings["OrderSyncConn"].ConnectionString;// @"Server=192.168.2.27\sql;Database=OrderSync;User Id =sa; Password=password";



        public virtual T ConnectionExcute<T>(Func<IDbConnection, T> func)
        {
            using (IDbConnection conn = new SqlConnection(connStr))
            {    
                return func(conn);
            }
        }


        public virtual T ConnectionExcute<T>(Func<IDbConnection, T> func, string conStr)
        {
            using (IDbConnection conn = new SqlConnection(conStr))
            {
                return func(conn);
            }
        }

        #region  

        public List<IEnumerable<object>> QueryMultiple(string sql, object p, params Type[] type)
        {
            List<IEnumerable<object>> list = new List<IEnumerable<object>>();
            var data = ConnectionExcute(x =>
            {
                var procMultiple = x.QueryMultiple(sql, p);
                list.AddRange(type.Select(node => procMultiple.Read(node)));
                return list;
            });
            return list;
        }

        #endregion

        #region Sync

        public IEnumerable<TType> Query<TType>(string sql, object parament = null)
        {
            return ConnectionExcute(x => x.Query<TType>(sql, parament));
        }

        public int Excute(string sql, object parament = null)
        {
            return ConnectionExcute(x => x.Execute(sql, parament));
        }

        public void ExecProc(string procName, object parament = null, Action func = null)
        {
            ConnectionExcute(x => x.Execute(procName, parament, commandType: CommandType.StoredProcedure));
            func?.Invoke();
        }

        public IEnumerable<TModel> ExecProc<TModel>(string procName, object parament, Action func = null)
        {
            return ConnectionExcute(x => x.Query<TModel>(procName, parament, commandType: CommandType.StoredProcedure));
        }

        public TOutType ExecProc<TModel, TOutType>(string procName, object parament, Func<IEnumerable<TModel>, TOutType> func)
        {

            var data = ConnectionExcute(x => x.Query<TModel>(procName, parament, commandType: CommandType.StoredProcedure));
            return func(data);
        }

        public TOutType ExecProc<TModel, TOutType>(string procName, Func<IEnumerable<TModel>, TOutType> func)
        {
            return ExecProc(procName, null, func);
        }


        #endregion

        #region Async
        public Task<IEnumerable<TType>> QueryAsync<TType>(string sql, object parament = null)
        {
            return ConnectionExcute(x => x.QueryAsync<TType>(sql, parament));
        }

        public Task<int> ExcuteAsync(string sql, object parament = null)
        {
            return ConnectionExcute(x => x.ExecuteAsync(sql, parament));
        }

        public async Task ExecProcAsync(string procName, object parament = null, Action func = null)
        {
            await ConnectionExcute(x => x.ExecuteAsync(procName, parament, commandType: CommandType.StoredProcedure));
            func?.Invoke();
        }

        public Task<IEnumerable<TModel>> ExecProcAsync<TModel>(string procName, object parament, Action func = null)
        {
            return ConnectionExcute(x => x.QueryAsync<TModel>(procName, parament, commandType: CommandType.StoredProcedure));
        }

        public async Task<TOutType> ExecProcAsync<TModel, TOutType>(string procName, object parament, Func<IEnumerable<TModel>, TOutType> func)
        {
            var data = ConnectionExcute(x => x.QueryAsync<TModel>(procName, parament, commandType: CommandType.StoredProcedure));
            return func(await data);
        }

        public Task<TOutType> ExecProcAsync<TModel, TOutType>(string procName, Func<IEnumerable<TModel>, TOutType> func)
        {
            return ExecProcAsync(procName, null, func);
        }

        #endregion

    }
    public class DapperRepository<TEntity, T> : DapperRepository, IDapperRepository<TEntity, T>
        where TEntity : IEntity<T>
    {
        public string TableName { get; set; }

        public DapperRepository()
        {
            TableName = typeof(TEntity).Name;
        }


        public List<TEntity> GetList(string where, object parament = null, string field = "*")
        {
            return ConnectionExcute(x => x.Query<TEntity>($"select {field} from [" + TableName + "] " + where, parament).ToList());
        }

        public Task<List<TEntity>> GetAllListAsync(string @where = null, object parament = null, string field = "*")
        {
            return Task.FromResult(GetList(where, parament, field));
        }

        public TEntity Get(T id)
        {
            return ConnectionExcute(x => x.Query<TEntity>("select top 1 * from [" + TableName + "] where id =@id", new { id = id }).FirstOrDefault());
        }

        public Task<TEntity> GetAsync(T id)
        {
            return Task.Run(() => Get(id));
        }

        public TEntity Insert(TEntity entity)
        {
            List<string> list;
            var parament = GetParament(entity, out list);
            if (entity.Id != null)
            {
                parament.Add("@Id", entity.Id);
                list.Add("Id");
            }
            var sql = $"INSERT INTO [{TableName}] ({string.Join(",", list)}) VALUES (@{string.Join(",@", list)});SELECT @@IDENTITY";
            var id = ConnectionExcute(x => x.ExecuteScalar<T>(sql, parament));
            if (typeof(T) == typeof(long) || typeof(T) == typeof(int))
            {
                if (id != null)
                {
                    entity.Id = id;
                }
            }
            return entity;
        }

        public Task<TEntity> InsertAsync(TEntity entity)
        {
            return Task.FromResult(Insert(entity));
        }


        public TEntity Update(TEntity entity)
        {
            List<string> list;
            var parament = GetParament(entity, out list);
            var agg = list.Aggregate(new StringBuilder(), (x, y) =>
            {
                x.Append(y);
                x.Append("=@");
                x.Append(y);
                x.Append(",");
                return x;
            });
            var sql = $"UPDATE [{TableName}] SET {agg.ToString().TrimEnd(',')} WHERE ID = '{entity.Id}'";
            ConnectionExcute(x => x.Execute(sql, parament));
            return entity;
        }

        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            return Task.FromResult(Update(entity));
        }

        public bool Update<T>(T Id, object props)
        {
            var updateField = new ExpandoObject();
            var propDic = GetObjectValues(props);
            StringBuilder s = new StringBuilder();
            foreach (var keyValuePair in propDic)
            {
                s.Append(keyValuePair.Key);
                s.Append("=@");
                s.Append(keyValuePair.Key);
                s.Append(",");
                (updateField as System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, object>>).Add(keyValuePair);
            }
            (updateField as System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, object>>).Add(new KeyValuePair<string,object>("Id",Id));
            string sql = $"UPDATE [{TableName}] SET {s.ToString().TrimEnd(',')} WHERE  Id=@Id";
            return ConnectionExcute(x => x.Execute(sql, updateField)) > 0;
        }

        public void Delete(TEntity entity)
        {
            Delete(entity.Id);
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            var sql = $"delete from [{ TableName }] where id in @Id";
            var list = entities.Select(x => x.Id);
            ConnectionExcute(x => x.Execute(sql, new { Id = list }));
        }

        public Task DeleteAsync(TEntity entity)
        {
            return Task.Run(() => Delete(entity));
        }

        public void Delete(T id)
        {
            ConnectionExcute(x => x.Execute($"delete from [{TableName}] where id = '{id}'"));
            //根据Id删除

        }

        public Task DeleteAsync(T id)
        {
            return Task.Run(() => Delete(id));
        }

        public void Delete(string where, object parament = null)
        {
            var sql = $"delete from [{ TableName }] {where}";
            ConnectionExcute(x => x.Execute(sql, parament));
            //根据条件删除

        }

        public Task DeleteAsync(string @where, object parament = null)
        {
            return Task.Run(() => Delete(where, parament));
        }

        public int Count()
        {
            return ConnectionExcute(x => x.ExecuteScalar<int>("SELECT COUNT(1) FROM [" + TableName + "]"));
        }

        public Task<int> CountAsync()
        {
            return Task.FromResult(Count());
        }

        public int Count(string where, object parament = null)
        {
            return ConnectionExcute(x => x.ExecuteScalar<int>("SELECT COUNT(1) FROM [" + TableName + "] " + where, parament));
        }

        public Task<int> CountAsync(string @where, object parament = null)
        {
            return Task.FromResult(Count(where, parament));
        }


        public TEntity FirstOrDefault(string where, object parament = null, string field = "*")
        {
            return ConnectionExcute(x => x.Query<TEntity>($" select top 1 {field} from [" + TableName + "] " + where, parament).FirstOrDefault());
        }

        public List<string> GetFieNameArray()
        {
            return (from node in typeof(T).GetProperties() where node.Name.ToLower() != "id" select node.Name).ToList();
        }

        //根据对象获取DynamicParament  
        public DynamicParameters GetParament(TEntity t, out List<string> list)
        {
            list = new List<string>();
            var parament = new DynamicParameters();
            foreach (var node in typeof(TEntity).GetProperties())
            {
                if (node.Name.ToLower() == "id" || node.GetValue(t) == null) continue;
                if (node.Name == "CreateTime")
                    node.SetValue(t, DateTime.Now);
                list.Add(node.Name);
                parament.Add("@" + node.Name, node.GetValue(t));
            }
            return parament;
        }


        /// <summary>
        ///     Use Action Connection
        /// </summary>
        /// <param name="action"></param>
        public static void UseConnectObj(Action<SqlConnection> action)
        {
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                action(conn);
            }
        }

        /// <summary>
        ///     Used for cud
        /// </summary>
        /// <returns>Execute Result</returns>
        /// <param name="act"></param>
        public static bool ExecuteWithTransaction(Action<SqlConnection, IDbTransaction> act)
        {
            bool flag = true;
            try
            {
                UseConnectObj(conn =>
                {
                    IDbTransaction trans = conn.BeginTransaction();
                    act(conn, trans);
                    trans.Commit();
                });
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;

        }


        /// <summary>
        /// 对象生成属性名及属性值的字典返回
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>属性名及属性值的字典</returns>
        private IDictionary<string, object> GetObjectValues(object obj)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            if (obj == null)
            {
                return result;
            }


            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                string name = propertyInfo.Name;
                object value = propertyInfo.GetValue(obj, null);
                result[name] = value;
            }

            return result;
        }




    }
    public class DapperRepository<TEntity> : DapperRepository<TEntity, int> where TEntity : IEntity<int>
    {

    }
}
