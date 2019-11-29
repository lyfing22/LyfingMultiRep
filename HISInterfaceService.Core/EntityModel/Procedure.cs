using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HISInterfaceService.Core.Dapper;

namespace HISInterfaceService.Core.EntityModel
{
        public  class Procedure:IEntity<Guid>
        {
            public Procedure()
            { }

            public Procedure(Guid Id,string hisOrderCode,int syncStatus,bool isSynced,bool isActive,bool isDelete,Guid order_Id)
            {
                _id = Id;
                _hisordercode = hisOrderCode;
                _syncstatus = syncStatus;
                _issynced = isSynced;
                _isactive = isActive;
                _isdelete = isDelete;
                _order_id = order_Id;
            }
        #region Model
            private Guid _id;
            private string _hisordercode;
            private int _syncstatus;
            private bool _issynced;
            private bool _isactive;
            private bool _isdelete;
            private string _checkitemcode;
            private string _checkitemname;
            private double? _fee;
            private bool _ismatch;
            private DateTime? _performdatetime;
            private DateTime? _lastupdatedatetime;
            private DateTime? _scheduledstarttime;
            private DateTime? _scheduledendtime;
            private string _studyinstanceuid;
            private string _devicename;
            private string _performstaffname;
            private DateTime? _studydatetime;
            private string _lastmodifiername;
            private Guid _order_id;
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
            public bool IsDelete
            {
                set { _isdelete = value; }
                get { return _isdelete; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string CheckItemCode
            {
                set { _checkitemcode = value; }
                get { return _checkitemcode; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string CheckItemName
            {
                set { _checkitemname = value; }
                get { return _checkitemname; }
            }
            /// <summary>
            /// 
            /// </summary>
            public double? Fee
            {
                set { _fee = value; }
                get { return _fee; }
            }
            /// <summary>
            /// 
            /// </summary>
            public bool IsMatch
            {
                set { _ismatch = value; }
                get { return _ismatch; }
            }
            /// <summary>
            /// 
            /// </summary>
            public DateTime? PerformDateTime
            {
                set { _performdatetime = value; }
                get { return _performdatetime; }
            }
            /// <summary>
            /// 
            /// </summary>
            public DateTime? LastUpdateDateTime
            {
                set { _lastupdatedatetime = value; }
                get { return _lastupdatedatetime; }
            }
            /// <summary>
            /// 
            /// </summary>
            public DateTime? ScheduledStartTime
            {
                set { _scheduledstarttime = value; }
                get { return _scheduledstarttime; }
            }
            /// <summary>
            /// 
            /// </summary>
            public DateTime? ScheduledEndTime
            {
                set { _scheduledendtime = value; }
                get { return _scheduledendtime; }
            }
            /// <summary>
            /// 
            /// </summary>
            public string StudyInstanceUId
            {
                set { _studyinstanceuid = value; }
                get { return _studyinstanceuid; }
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
            public string PerformStaffName
            {
                set { _performstaffname = value; }
                get { return _performstaffname; }
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
            public string LastModifierName
            {
                set { _lastmodifiername = value; }
                get { return _lastmodifiername; }
            }
            /// <summary>
            /// 
            /// </summary>
            public Guid Order_Id
            {
                set { _order_id = value; }
                get { return _order_id; }
            }
            #endregion Model

        }


}
