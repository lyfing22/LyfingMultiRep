using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HISInterfaceService.Core.EntityModel;
using HISInterfaceService.Dapper.Repository;

namespace HISInterfaceService.Service.DbService
{
    public class ReportService
    {
        private ReportRepository reportRep = new ReportRepository();
        public Report Insert(Report entity)
        {
            return reportRep.Insert(entity);
        }


        public Report GetReportByOrderId(Guid orderId)
        {
            return reportRep.Query<Report>("select top 1 * from dbo.[Report] where Order_Id=@Order_Id",
                new { Order_Id = orderId }).FirstOrDefault();
        }
    }
}
