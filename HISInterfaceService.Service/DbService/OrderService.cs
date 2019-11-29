using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HISInterfaceService.Core.EntityModel;
using HISInterfaceService.Core.Enum;
using HISInterfaceService.Dapper.Repository;

namespace HISInterfaceService.Service.DbService
{
    public class OrderService
    {
        private OrderRepository orderRep = new OrderRepository();

        public bool Update(Guid id, object props)
        {
            return orderRep.Update(id, props);
        }
        public Order GetOrderByAccessionNum(string accessionNum)
        {
            return orderRep.Query<Order>("select top 1 * from dbo.[Order] where AccessionNumber=@AccessionNumber",
                new { AccessionNumber = accessionNum }).FirstOrDefault();
        }

        public Order GetOrderById(Guid id)
        {
            return orderRep.Get(id);
        }

        public void DeleteOrder(Order order, bool isThrow = true)
        {
            orderRep.DeleteOrder(order);
        }

        public void CreateOder(Order order)
        {
            if (orderRep
                .Query<int>(
                    "select top 1 Id from dbo.[Order] where HisOrderCode=@HisOrderCode and  IsDeleted=@IsDeleted",
                    new { HisOrderCode = order.HisOrderCode, IsDeleted = 0 }).Any())
            {
                throw new Exception(string.Format("Duplicate HisOrderCode found: {0}", order.HisOrderCode));
            }
            order.SyncStatus = (int)OrderSyncStatus.OrderSynced;
            order.IsSynced = true;
            order.IsActive = true;
            order.IsDeleted = false;
            order.LastUpdateTime = DateTime.Now;
            orderRep.Insert(order);
        }

    }
}
