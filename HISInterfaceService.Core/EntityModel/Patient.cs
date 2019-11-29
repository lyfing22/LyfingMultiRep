using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HISInterfaceService.Core.Dapper;

namespace HISInterfaceService.Core.EntityModel
{
    public class Patient:IEntity<Guid>
    {

        public  Guid Id { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Email { get; set; }
        public string Gender  { get; set; }
        public string Name    { get; set; }
        public string Note    { get; set; }
        public string GlobalPatientId    { get; set; }
        public string  PatientMasterIndex    { get; set; }
        public string SpellName    { get; set; }
        public string SocietyNumber    { get; set; }
        public string  ClinicalNumber    { get; set; }
        public string  InpatientNumber    { get; set; }
        public string   NationName    { get; set; }
        public string  Telephone    { get; set; }
        public Guid MergeId    { get; set; }
        public Guid HospitalId    { get; set; }
        public DateTime LastUpdateDateTime    { get; set; }
        public DateTime? CreateDateTime    { get; set; }
        public string  IDCard    { get; set; }
        public string Address    { get; set; }
        public string  Marriage    { get; set; }
        public string FamilyName    { get; set; }
        public string GivenName    { get; set; }
        public string MiddleName    { get; set; }
        public string HISPatientId    { get; set; }
        public string  ServerNode    { get; set; }
        public bool IsDelete    { get; set; }
        public string UpdateUserCode    { get; set; }
        public string UpdateUserDesc    { get; set; }
        public  DateTime ?  UpdateDate    { get; set; }
        public TimeSpan? UpdateTime    { get; set; }
    }

 
}
