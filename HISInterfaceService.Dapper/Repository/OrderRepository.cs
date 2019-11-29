using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperDal;
using HISInterfaceService.Core.EntityModel;
using HISInterfaceService.Core.Enum;

namespace HISInterfaceService.Dapper.Repository
{
  public   class OrderRepository: DapperRepository<Order, Guid>
    {
        public bool DeleteOrder(Order order)
        {
            return ExecuteWithTransaction((conn, trans) =>
            {
           conn.Execute(@"update dbo.[Order] set IsDeleted=@IsDeleted where HisOrderCode=@HisOrderCode and SyncStatus=@SyncStatus;
                                                       update dbo.[Procedure] set IsDeleted=@IsDeleted where SyncStatus=@SyncStatus and  HisOrderCode in 
                                                        (select distinct HisOrderCode from dbo.[Order] where HisOrderCode=@HisOrderCode and SyncStatus=@SyncStatus;) ", new { IsDeleted = 1, SyncStatus = (int)OrderSyncStatus.OrderSynced, HisOrderCode = order.HisOrderCode }, trans,
                        null, null);
                    conn.Execute("delete from Person where id=@id", new { id = 1010 }, trans, null, null);
            });
        }
    }
}
