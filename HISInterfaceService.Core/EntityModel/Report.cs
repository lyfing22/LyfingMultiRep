using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HISInterfaceService.Core.Dapper;

namespace HISInterfaceService.Core.EntityModel
{
        /// <summary>
        /// Report:实体类(属性说明自动提取数据库字段的描述信息)
        /// </summary>
        public  class Report:IEntity<Guid>
        {
            public Report()
            { }
            #region Model
            private Guid _id;
            private string _ispositive;
            private string _reportcreatercode;
            private string _reportcreatername;
            private string _keywords;
            private string _reporturl;
            private string _reportstatus;
            private string _approvedoctorcode;
            private string _approvedoctorname;
            private DateTime? _approvedatetime;
            private string _submitdoctorcode;
            private string _submitdoctorname;
            private DateTime? _submitdatetime;
            private string _publishdoctorname;
            private string _publishdoctorcode;
            private DateTime? _publishdatetime;
            private string _printdoctorname;
            private string _printdoctoridcode;
            private DateTime? _printdatetime;
            private string _filmingrank;
            private string _track;
            private string _trackercode;
            private string _trackername;
            private string _forum;
            private string _isprintfilm;
            private string _findings;
            private string _impression;
            private Guid _order_id;
            private bool _iscritical;
            /// <summary>
            /// 
            /// </summary>
            public  Guid Id
            {
                set { _id = value; }
                get { return _id; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string IsPositive
            {
                set { _ispositive = value; }
                get { return _ispositive; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string ReportCreaterCode
            {
                set { _reportcreatercode = value; }
                get { return _reportcreatercode; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string ReportCreaterName
            {
                set { _reportcreatername = value; }
                get { return _reportcreatername; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string Keywords
            {
                set { _keywords = value; }
                get { return _keywords; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string ReportUrl
            {
                set { _reporturl = value; }
                get { return _reporturl; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string ReportStatus
            {
                set { _reportstatus = value; }
                get { return _reportstatus; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string ApproveDoctorCode
            {
                set { _approvedoctorcode = value; }
                get { return _approvedoctorcode; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string ApproveDoctorName
            {
                set { _approvedoctorname = value; }
                get { return _approvedoctorname; }
            }
            /// <summary>
            /// 
            /// </summary>
            public DateTime? ApproveDatetime
            {
                set { _approvedatetime = value; }
                get { return _approvedatetime; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string SubmitDoctorCode
            {
                set { _submitdoctorcode = value; }
                get { return _submitdoctorcode; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string SubmitDoctorName
            {
                set { _submitdoctorname = value; }
                get { return _submitdoctorname; }
            }
            /// <summary>
            /// 
            /// </summary>
            public DateTime? SubmitDateTime
            {
                set { _submitdatetime = value; }
                get { return _submitdatetime; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string PublishDoctorName
            {
                set { _publishdoctorname = value; }
                get { return _publishdoctorname; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string PublishDoctorCode
            {
                set { _publishdoctorcode = value; }
                get { return _publishdoctorcode; }
            }
            /// <summary>
            /// 
            /// </summary>
            public DateTime? PublishDateTime
            {
                set { _publishdatetime = value; }
                get { return _publishdatetime; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string PrintDoctorName
            {
                set { _printdoctorname = value; }
                get { return _printdoctorname; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string PrintDoctorIdCode
            {
                set { _printdoctoridcode = value; }
                get { return _printdoctoridcode; }
            }
            /// <summary>
            /// 
            /// </summary>
            public DateTime? PrintDateTime
            {
                set { _printdatetime = value; }
                get { return _printdatetime; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string FilmingRank
            {
                set { _filmingrank = value; }
                get { return _filmingrank; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string Track
            {
                set { _track = value; }
                get { return _track; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string TrackerCode
            {
                set { _trackercode = value; }
                get { return _trackercode; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string TrackerName
            {
                set { _trackername = value; }
                get { return _trackername; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string Forum
            {
                set { _forum = value; }
                get { return _forum; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string IsPrintFilm
            {
                set { _isprintfilm = value; }
                get { return _isprintfilm; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string Findings
            {
                set { _findings = value; }
                get { return _findings; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string Impression
            {
                set { _impression = value; }
                get { return _impression; }
            }
            /// <summary>
            /// 
            /// </summary>
            public Guid Order_Id
            {
                set { _order_id = value; }
                get { return _order_id; }
            }
            /// <summary>
            /// 
            /// </summary>
            public bool IsCritical
            {
                set { _iscritical = value; }
                get { return _iscritical; }
            }
            #endregion Model

        }

}
