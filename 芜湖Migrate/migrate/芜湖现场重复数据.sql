SELECT
  s.STUDYID                     		AS AccessionNumber,
  s.CHECKSERIALNUM                          AS GlobalPatientId0,
  p.ENETPATIENTID					AS GlobalPatientId,
  p.PATIENTNAME                       AS Name,
  p.PatientSpellName                  AS SpellName,
  CASE WHEN p.SEX='男' THEN 'M'      -- M=男/F=女/U=未知
       WHEN p.SEX='女' THEN 'F'
       ELSE 'U' END                   AS Gender,
  p.BIRTHDAY                           AS DateOfBirth,
  p.Address,
  p.Phonenumber                       AS Telephone,
  P.IDNUMBER 						  AS IDCard,
  p.SOCIETYID 							AS SocietyNumber,
  s.STUDYID                     		AS AccessionNumber,
  s.SICKROOM                          AS BedNumber,
  CASE WHEN p.hispatienttype='1' THEN 'OP'      
       WHEN p.hispatienttype='2' THEN 'IH'
       WHEN p.hispatienttype='3' THEN 'PE'
       WHEN p.hispatienttype='6' THEN 'PE' --旧住院
       ELSE 'OP' END                   AS PatientType,
  DECODE(p.hispatienttype,'1','OP','2','IH','OP') AS PatientType0,  
  CASE WHEN s.IFEMERGENCY=1 THEN '1' ELSE '3' END AS EmergencyDegree,  -- 1=急诊/3=普通
  p.clinicpatientid                   AS ClinicalNumber,
  p.infeepatientid                    AS InpatientNumber,
  s.DIAGID                            AS HisOrderCode,
  s.PREDIAGNOSE                       AS ClinicalDiagnosis,
  s.DOCTORCODE 						  AS  ApplyDoctorName,
  dpt.DEPARTMENTCODE				  AS ApplyDepartmentCode,
  dpt.DEPARTMENTNAME				  AS ApplyDepartmentName,
  s.ABSTRACTHISTORY					  AS Sign,
  s.FEETOTAL                          AS TotalFee,
  s.AGE,
  s.AGEUNIT,
  s.DSTUDYUID                         AS StudyInstanceUID,
  s.STUDYTIME                          AS StudyDate,
  --s.OPERATETIME                         AS StudyDate,
  s.CHKDEPTID                         AS ExecDepartmentCode,
  dt.Modality                         AS ModalityCode,
  s.DEVICEID                          AS DeviceCode,
  s.DEPARTMENTID                      AS ApplyDepartmentCode,
  s.SEPERATETIME                       AS ArriveTime,
  s.SEPERATETIME                       AS RegisterTime,
  s.OPERATORID                        AS CheckInDoctorCode,
  (SELECT USERNAME FROM PACS31.PACSUSER WHERE USERID=s.OPERATORID) AS CheckInDoctorName,  
  CASE WHEN s.IFMASCULINE='1' THEN '0'      -- 是否阳性，0：未确定，1：阴性，2：阳性 ----->/// -1:未知0:阴性 1:阳性 
       WHEN s.IFMASCULINE='2' THEN '1'
       ELSE '-1' END                   AS PositiveStatus,
  r.REPORTDESCRIBE                    AS Findings,
  r.REPORTDIAGNOSE                    AS Impression,
  r.OPERATETIME                        AS SubmitDateTime,
  r.DOCID1                            AS SubmitDoctorCode,
  (SELECT USERNAME FROM PACS31.PACSUSER WHERE USERID=r.DOCID1) AS SubmitDoctorName,
  r.OPERATORID                        AS ApproveDoctorCode,
  (SELECT USERNAME FROM PACS31.PACSUSER WHERE USERID=r.OPERATORID) AS ApproveDoctorName,
  psi.IMAGECOUNT                      AS ImageCount,
  s.STUDYSCRIPTION                     AS StudyScription,
  s.BESPEAKTIME                        AS ScheduledDate,
  s.QUEUENO                            AS QueueNo
FROM PACS31.STUDYINFO s
LEFT JOIN PACS31.PATIENTINFO p           ON s.CHECKSERIALNUM = p.CHECKSERIALNUM
LEFT JOIN PACS31.PATIENTDIAGRPTINFO r    ON s.DIAGRPTID = r.DIAGRPTID
LEFT JOIN PACS31.DEVICETYPEINFO dt       ON s.DEVICETYPEID = dt.DevicetypeId
LEFT JOIN PACS31.PACS_STUDYINFO psi      ON s.CHECKSERIALNUM = psi.ACCESSIONNUMBER
LEFT JOIN PACS31.PACSDEPARTMENT dpt      ON s.DEPARTMENTID = dpt.DEPARTMENTID
WHERE 
   s.ISAVAILABLE = 1  AND  p.hispatienttype='6'-- s.STUDYID  ='6504'
ORDER BY s.SEPERATETIME desc
FETCH  first 100 ROWS only