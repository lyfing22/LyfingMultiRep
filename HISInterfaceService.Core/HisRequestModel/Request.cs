using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HISInterfaceService.Core.HisRequestModel
{
    public class Request
    {
        /// <summary>
        /// 
        /// </summary>
        public Header Header { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Body Body { get; set; }
    }
    public class PATAddress
    {
        /// <summary>
        /// 
        /// </summary>
        public string PATAddressType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATAddressDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATHouseNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATVillage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATCountryside { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATCountyCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATCountyDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATCityCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATCityDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATProvinceCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATProvinceDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATPostalCode { get; set; }
    }

    public class PATAddresses
    {
        /// <summary>
        /// 
        /// </summary>
        public PATAddress PATAddress { get; set; }
    }

    public class PATIdentity
    {
        /// <summary>
        /// 
        /// </summary>
        public string PATIdentityNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATIdTypeCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATIdTypeDesc { get; set; }
    }

    public class PATIdentityList
    {
        /// <summary>
        /// 
        /// </summary>
        public PATIdentity PATIdentity { get; set; }
    }

    public class PATRelationAddress
    {
        /// <summary>
        /// 
        /// </summary>
        public string PATRelationAddressDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATRelationHouseNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATRelationVillage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATRelationCountryside { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATRelationCountyCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATRelationCountyDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATRelationCityCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATRelationCityDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATRelationProvinceCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATRelationProvinceDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATRelationPostalCode { get; set; }
    }

    public class PATRelation
    {
        /// <summary>
        /// 
        /// </summary>
        public string PATRelationCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATRelationDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATRelationName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATRelationPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PATRelationAddress PATRelationAddress { get; set; }
    }

    public class PatientRegistryRt
    {
        /// <summary>
        /// 
        /// </summary>
        public string PATPatientID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATDob { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATSexCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATSexDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATMaritalStatusCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATMaritalStatusDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATNationCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATNationDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATCountryCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATCountryDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATDeceasedDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATDeceasedTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATHealthCardID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATMotherID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATOccupationCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATOccupationDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATWorkPlaceName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATWorkPlaceTelNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PATAddresses PATAddressList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public PATIdentityList PATIdentityList { get; set; }
        public PATIdentityList PATIdentityList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PATRelation PATRelationAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATTelephone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATRemarks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UpdateUserCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UpdateUserDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UpdateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UpdateTime { get; set; }
    }

}