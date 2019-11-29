using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HISInterfaceService.Core.EntityModel;
using HISInterfaceService.Core.HisRequestModel;

namespace HISInterfaceService.Core.DataMapper
{
    public class MapperConfig : Profile
    {
        private readonly string timePattern = "yyyy-MM-dd HH:mm";

        public MapperConfig()
        {
            #region common config for automapper

            base.CreateMap<string, Guid>().ConstructUsing((opt, dest) =>
            {
                Guid refs = Guid.Empty;
                if (!Guid.TryParse(opt, out refs))
                {
                    refs = Guid.NewGuid();
                }
                return refs;
            });
            base.CreateMap<Guid, string>().ConstructUsing((opt, dest) => opt.ToString());
            base.CreateMap<DateTime, string>().ConstructUsing((opt, dest) => opt.ToString(timePattern));
            base.CreateMap<string, DateTime>().ConstructUsing((opt, dest) => Convert.ToDateTime(opt));

            #endregion

            #region 病人信息
            base.CreateMap<EntityModel.Patient, PatientForUploadDTOForApi>().ForMember(dest => dest.PidType, opt => opt.Ignore())
                .ForMember(dest => dest.PidSourceCode, opt => opt.Ignore())//
                .ForMember(dest => dest.PatientUpdVersion, opt => opt.Ignore())
                .AfterMap((opt, dest) => { dest.PidType = 2;});//
            #endregion

            #region 申请单信息
            base.CreateMap<Order, OrderForUploadDTOForApi>().ForMember(dest => dest.AccNumType, opt => opt.Ignore())
                .ForMember(dest => dest.ApplyDepartmentCode, opt => opt.MapFrom(m => m.OrderDepartmentCode))
                .ForMember(dest => dest.ApplyDepartmentName, opt => opt.MapFrom(m => m.OrderDepartmentName))
                .ForMember(dest => dest.ArriveTime, opt => opt.Ignore()) //
                .ForMember(dest => dest.DeviceAETitle, opt => opt.Ignore()) //
                .ForMember(dest => dest.DeviceCode, opt => opt.Ignore()) //
                .ForMember(dest => dest.ExamRoom, opt => opt.Ignore()) //
                .ForMember(dest => dest.ExecDepartmentCode, opt => opt.MapFrom(m => m.ExamDepartmentCode))
                .ForMember(dest => dest.ExecDepartmentName, opt => opt.MapFrom(m => m.ExamDepartmentName))
                .ForMember(dest => dest.ExecDoctorCode, opt => opt.MapFrom(m => m.ExamDoctorCode))
                .ForMember(dest => dest.ExecDoctorName, opt => opt.MapFrom(m => m.ExamDoctorName))
                .ForMember(dest => dest.IsFromHIS, opt => opt.Ignore()) //
                .ForMember(dest => dest.ModalityName, opt => opt.Ignore()) //
                .ForMember(dest => dest.CheckItems, opt => opt.MapFrom(m=>m.OrderExamCode)) //
                .ForMember(dest => dest.Status, opt => opt.MapFrom(m => m.OrderStatus))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(m => m.Notes))
                .ForMember(dest => dest.RegisterTime, opt => opt.MapFrom(m => m.CheckInDateTime))
                .ForMember(dest => dest.ReportDoctorCode, opt => opt.Ignore()) //
                .ForMember(dest => dest.ReportDoctorName, opt => opt.Ignore()) //
                .ForMember(dest => dest.StudyDate, opt => opt.MapFrom(m => m.StudyDateTime))
                .ForMember(dest => dest.StudyId, opt => opt.Ignore()) //
                .ForMember(dest => dest.Suggestion, opt => opt.Ignore()) //
                .ForMember(dest => dest.Age, opt => opt.Ignore()) //
                .ForMember(dest => dest.AgeUnit, opt => opt.Ignore()) //
                .ForMember(dest => dest.TotalFee, opt => opt.MapFrom(m => m.TotalCosts))
                .ForMember(dest => dest.FilmingRank, opt => opt.Ignore()) //
                .ForMember(dest => dest.RegionAssist, opt => opt.Ignore()) //
                .ForMember(dest => dest.RegionAssistType, opt => opt.Ignore()) //
                .ForMember(dest => dest.RegionAssistComments, opt => opt.Ignore());
            #endregion

            #region 报告信息
            base.CreateMap<Report, ReportForUploadDTOForApi>()
                .ForMember(dest => dest.SnapshotFilePath, opt => opt.MapFrom(m=>m.ReportUrl))
                .ForMember(dest => dest.Accord, opt => opt.Ignore());//
            #endregion

        }
    }
}
