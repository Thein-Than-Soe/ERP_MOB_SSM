using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.ERP_MOB.Services.SSM
{
    class Ssm_Name
    {
        public Ssm_Name()
        {
        }
        public static string wssaveShopping = "saveShopping";

        #region "List"
        public static string wsgetAdviceType = "getAdviceType";
        public static string wsgetCommunityType = "getCommunityType";
        public static string wsgetRate = "getRate";

        public static string wsgetJobClassification = "getJobClassification";
        public static string wsgetJobClassificationJun = "getJobClassificationJun";
        public static string wsgetCompanyRating = "getCompanyRating";
        public static string wsgetAdvice = "getAdvice";
        public static string wsgetCommunity = "getCommunity";
        public static string wsgetApplicant = "getApplicant";
        public static string wsgetApplicantContact = "getApplicantContact";
        public static string wsgetApplicantEducation = "getApplicantEducation";
        public static string wsgetApplicantCertificate = "getApplicantCertificate";
        public static string wsgetApplicantWorkingExperience = "getApplicantWorkingExperience";
        public static string wsgetApplicantSocialAffairs = "getApplicantSocialAffairs";
        public static string wsgetApplicantSkill = "getApplicantSkill";
        public static string wsgetApplicantLanguageSkill = "getApplicantLanguageSkill";
        public static string wsgetApplicantTraining = "getApplicantTraining";
        public static string wsgetJobVacancy = "getJobVacancy";
        public static string wsgetCompany = "getCompany";
        public static string wsgetCompanyDetail = "getCompanyDetail";
        public static string wsgetApplicantVacancy = "getApplicantVacancy";
        public static string wsgetApplicantCompany = "getApplicantCompany";
        public static string wsgetJobSearch = "getJobSearch";

        public static string wsgetApplicantDtl = "getApplicantDtl";
        #endregion
        #region "Save"
        public static string wssaveApplicantCompany = "saveApplicantCompany";
        public static string wssaveApplicantVacancy = "saveApplicantVacancy";
        public static string wssaveApplicant = "saveApplicant";
        public static string wssaveApplicantContact = "saveApplicantContact";
        public static string wssaveApplicantEducation = "saveApplicantEducation";
        public static string wssaveApplicantWorkingExperience = "saveApplicantWorkingExperience";
        public static string wssaveApplicantCertificate = "saveApplicantCertificate";
        public static string wssaveApplicantSocialAffairs = "saveApplicantSocialAffairs";
        public static string wssaveApplicantSkill = "saveApplicantSkill";
        public static string wssaveApplicantLanguageSkill = "saveApplicantLanguageSkill";
        public static string wssaveApplicantTraining = "saveApplicantTraining";


        #endregion
        #region "Load"
        public static string wsLoadAdvice = "loadAdvice";
        public static string wsLoadCommunity = "loadCommunity";
        public static string wsLoadCompanyRating = "loadCompanyRating";
        public static string wsLoadCompany = "loadCompany";
        public static string wsloadJobClassification = "loadJobClassification";
        public static string wsLoadJobVacancy = "loadJobVacancy";
        public static string wsLoadApplicant = "loadApplicant";
        #endregion

    }
}
