using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HISInterfaceService.Core.Dapper;

namespace HISInterfaceService.Core.EntityModel
{
    /// <summary>
    /// Order:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    public class Order : IEntity<Guid>
    {
        public Order()
        { }

        public Order(Guid id, String hisOrderCode, Int32 syncStatus, Boolean isSynced, Boolean isActive, Boolean isDeleted)
        {
            _id = id;
            _hisordercode = hisOrderCode;
            _syncstatus = syncStatus;
            _issynced = isSynced;
            _isactive = isActive;
            _isdeleted = _isactive;
        }

        #region Model
        private Guid _id;
        private string _hisordercode;
        private string _hispatientid;
        private string _confirmtohispatientid;
        private string _accessionnumber;
        private int _syncstatus;
        private bool _issynced;
        private bool _isactive;
        private bool _isdeleted;
        private string _gloablepatientid;
        private string _patientname;
        private string _nameinpy;
        private string _birthday;
        private string _gender;
        private string _address;
        private string _telephone;
        private string _idcard;
        private string _visitnumber;
        private string _inhospitalnumber;
        private string _clinicnumber;
        private string _patienttype;
        private string _roomno;
        private string _bedno;
        private string _orderdepartmentcode;
        private string _orderdepartmentname;
        private string _applydoctorcode;
        private string _applydoctorname;
        private string _examdepartmentcode;
        private string _examdepartmentname;
        private string _examdoctorcode;
        private string _examdoctorname;
        private string _orderexamcode;
        private string _orderexamname;
        private string _checkindoctorname;
        private string _orderdoctorname;
        private double? _examcosts;
        private string _clinicaldiagnosis;
        private double? _totalcosts;
        private DateTime? _orderdatetime;
        private DateTime? _studydatetime;
        private string _diseasehistory;
        private string _testresults;
        private string _predicate;
        private string _orderstatus;
        private string _ismatch;
        private string _notes;
        private string _societynumber;
        private string _modalitycode;
        private string _devicename;
        private string _familyname;
        private string _givenname;
        private string _middlename;
        private DateTime? _lastupdatetime;
        private string _orgcode;
        private string _symptom;
        private DateTime? _scheduleddatetime;
        private string _clinic;
        private string _updateusercode;
        private DateTime? _updatedate;
        private TimeSpan? _updatetime;
        private string _bodypartnames;
        private string _bodypartcodes;
        private string _orderexamcosts;
        private string _orderdoctorcode;
        private string _canceldoctorname;
        private string _canceldoctorcode;
        private string _checkindoctorcode;
        private DateTime? _canceldatetime;
        private DateTime? _scheduledatetime;
        private DateTime? _checkindatetime;
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
        public string HisOrderCode
        {
            set { _hisordercode = value; }
            get { return _hisordercode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string HisPatientId
        {
            set { _hispatientid = value; }
            get { return _hispatientid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ConfirmToHisPatientId
        {
            set { _confirmtohispatientid = value; }
            get { return _confirmtohispatientid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string AccessionNumber
        {
            set { _accessionnumber = value; }
            get { return _accessionnumber; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int SyncStatus
        {
            set { _syncstatus = value; }
            get { return _syncstatus; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsSynced
        {
            set { _issynced = value; }
            get { return _issynced; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsActive
        {
            set { _isactive = value; }
            get { return _isactive; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDeleted
        {
            set { _isdeleted = value; }
            get { return _isdeleted; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string GloablePatientId
        {
            set { _gloablepatientid = value; }
            get { return _gloablepatientid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PatientName
        {
            set { _patientname = value; }
            get { return _patientname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string NameInPy
        {
            set { _nameinpy = value; }
            get { return _nameinpy; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Birthday
        {
            set { _birthday = value; }
            get { return _birthday; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Gender
        {
            set { _gender = value; }
            get { return _gender; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Address
        {
            set { _address = value; }
            get { return _address; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Telephone
        {
            set { _telephone = value; }
            get { return _telephone; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string IdCard
        {
            set { _idcard = value; }
            get { return _idcard; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string VisitNumber
        {
            set { _visitnumber = value; }
            get { return _visitnumber; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string InhospitalNumber
        {
            set { _inhospitalnumber = value; }
            get { return _inhospitalnumber; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ClinicNumber
        {
            set { _clinicnumber = value; }
            get { return _clinicnumber; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PatientType
        {
            set { _patienttype = value; }
            get { return _patienttype; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string RoomNo
        {
            set { _roomno = value; }
            get { return _roomno; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string BedNo
        {
            set { _bedno = value; }
            get { return _bedno; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OrderDepartmentCode
        {
            set { _orderdepartmentcode = value; }
            get { return _orderdepartmentcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OrderDepartmentName
        {
            set { _orderdepartmentname = value; }
            get { return _orderdepartmentname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ApplyDoctorCode
        {
            set { _applydoctorcode = value; }
            get { return _applydoctorcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ApplyDoctorName
        {
            set { _applydoctorname = value; }
            get { return _applydoctorname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ExamDepartmentCode
        {
            set { _examdepartmentcode = value; }
            get { return _examdepartmentcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ExamDepartmentName
        {
            set { _examdepartmentname = value; }
            get { return _examdepartmentname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ExamDoctorCode
        {
            set { _examdoctorcode = value; }
            get { return _examdoctorcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ExamDoctorName
        {
            set { _examdoctorname = value; }
            get { return _examdoctorname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OrderExamCode
        {
            set { _orderexamcode = value; }
            get { return _orderexamcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OrderExamName
        {
            set { _orderexamname = value; }
            get { return _orderexamname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CheckInDoctorName
        {
            set { _checkindoctorname = value; }
            get { return _checkindoctorname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OrderDoctorName
        {
            set { _orderdoctorname = value; }
            get { return _orderdoctorname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public double? ExamCosts
        {
            set { _examcosts = value; }
            get { return _examcosts; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ClinicalDiagnosis
        {
            set { _clinicaldiagnosis = value; }
            get { return _clinicaldiagnosis; }
        }
        /// <summary>
        /// 
        /// </summary>
        public double? TotalCosts
        {
            set { _totalcosts = value; }
            get { return _totalcosts; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? OrderDateTime
        {
            set { _orderdatetime = value; }
            get { return _orderdatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? StudyDateTime
        {
            set { _studydatetime = value; }
            get { return _studydatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DiseaseHistory
        {
            set { _diseasehistory = value; }
            get { return _diseasehistory; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TestResults
        {
            set { _testresults = value; }
            get { return _testresults; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Predicate
        {
            set { _predicate = value; }
            get { return _predicate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OrderStatus
        {
            set { _orderstatus = value; }
            get { return _orderstatus; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string IsMatch
        {
            set { _ismatch = value; }
            get { return _ismatch; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Notes
        {
            set { _notes = value; }
            get { return _notes; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SocietyNumber
        {
            set { _societynumber = value; }
            get { return _societynumber; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ModalityCode
        {
            set { _modalitycode = value; }
            get { return _modalitycode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DeviceName
        {
            set { _devicename = value; }
            get { return _devicename; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FamilyName
        {
            set { _familyname = value; }
            get { return _familyname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string GivenName
        {
            set { _givenname = value; }
            get { return _givenname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string MiddleName
        {
            set { _middlename = value; }
            get { return _middlename; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastUpdateTime
        {
            set { _lastupdatetime = value; }
            get { return _lastupdatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OrgCode
        {
            set { _orgcode = value; }
            get { return _orgcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Symptom
        {
            set { _symptom = value; }
            get { return _symptom; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ScheduledDateTime
        {
            set { _scheduleddatetime = value; }
            get { return _scheduleddatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string clinic
        {
            set { _clinic = value; }
            get { return _clinic; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string UpdateUserCode
        {
            set { _updateusercode = value; }
            get { return _updateusercode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpdateDate
        {
            set { _updatedate = value; }
            get { return _updatedate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan? UpdateTime
        {
            set { _updatetime = value; }
            get { return _updatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string BodyPartNames
        {
            set { _bodypartnames = value; }
            get { return _bodypartnames; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string BodyPartCodes
        {
            set { _bodypartcodes = value; }
            get { return _bodypartcodes; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OrderExamCosts
        {
            set { _orderexamcosts = value; }
            get { return _orderexamcosts; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OrderDoctorCode
        {
            set { _orderdoctorcode = value; }
            get { return _orderdoctorcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CancelDoctorName
        {
            set { _canceldoctorname = value; }
            get { return _canceldoctorname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CancelDoctorCode
        {
            set { _canceldoctorcode = value; }
            get { return _canceldoctorcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CheckInDoctorCode
        {
            set { _checkindoctorcode = value; }
            get { return _checkindoctorcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CancelDateTime
        {
            set { _canceldatetime = value; }
            get { return _canceldatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ScheduleDateTime
        {
            set { _scheduledatetime = value; }
            get { return _scheduledatetime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CheckInDateTime
        {
            set { _checkindatetime = value; }
            get { return _checkindatetime; }
        }
        #endregion Model


    }
}
