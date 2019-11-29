using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HISInterfaceService.Core.EntityModel;
using HISInterfaceService.Dapper.Repository;

namespace HISInterfaceService.Service.DbService
{
    public class PatientService
    {
        private PatientRepository patientRep = new PatientRepository();
        public Patient Insert(Patient entity)
        {
            return patientRep.Insert(entity);
        }

        public Patient AddOrUpdate(Patient entity)
        {
            Patient patient= patientRep.Query<Patient>("select top 1 * from dbo.[Patient] where GlobalPatientId=@GlobalPatientId",
                new { GlobalPatientId = entity.GlobalPatientId }).FirstOrDefault();
            if (patient == null) return patientRep.Insert(entity);
            else
            {
                entity.Id = patient.Id;
                return patientRep.Update(entity);
            }

        }

        public Patient GetPatientByGlobalPatientId(string globalPatientId)
        {
            return patientRep.Query<Patient>(
                  "select top 1 *  from Patient where GlobalPatientId=@GlobalPatientId ",
                  new { @GlobalPatientId = globalPatientId }).FirstOrDefault();
        }

    }
}
