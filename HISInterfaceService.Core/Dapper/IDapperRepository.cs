using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISInterfaceService.Core.Dapper
{
    public  interface IDapperRepository
    {
        IEnumerable<TType> Query<TType>(string sql, object parament = null);

        int Excute(string sql, object parament = null);

        #region Proc

        void ExecProc(string procName, object parament = null, Action func = null);

        IEnumerable<TModel> ExecProc<TModel>(string procName, object parament = null, Action func = null);

        TOutType ExecProc<TModel, TOutType>(string procName, object parament, Func<IEnumerable<TModel>, TOutType> func);

        TOutType ExecProc<TModel, TOutType>(string procName, Func<IEnumerable<TModel>, TOutType> func);


        List<IEnumerable<object>> QueryMultiple(string sql, object p, params Type[] type);

        #endregion
    }

    /// <summary>
    /// 对象仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="T"></typeparam>
    public interface IDapperRepository<TEntity, T> : IDapperRepository where TEntity : IEntity<T>
    {

        #region Select/Get/Query  

        List<TEntity> GetList(string where = null, object parament = null, string field = "*");

        Task<List<TEntity>> GetAllListAsync(string where = null, object parament = null, string field = "*");

        TEntity Get(T id);

        Task<TEntity> GetAsync(T id);

        #endregion

        #region Insert

        TEntity Insert(TEntity entity);

        Task<TEntity> InsertAsync(TEntity entity);

        #endregion

        #region Update

        TEntity Update(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        #endregion

        #region Delete

        void Delete(TEntity entity);

        void Delete(IEnumerable<TEntity> entities);

        Task DeleteAsync(TEntity entity);

        void Delete(T id);

        Task DeleteAsync(T id);

        void Delete(string where, object parament = null);

        Task DeleteAsync(string where, object parament = null);

        #endregion

        #region Aggregates

        int Count();

        Task<int> CountAsync();

        int Count(string where, object parament = null);

        Task<int> CountAsync(string where, object parament = null);

        #endregion 

    }

    public interface IDapperRepository<TEntity> : IDapperRepository<TEntity, int> where TEntity : IEntity<int>
    {

    }

}
