using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.ERP_MOB.Services.SSM
{
    public class Ssm_UploadFolder
    {
        public Ssm_UploadFolder()
        {
        }
        #region "Upload"
        public static string job_User = "/job/user";
        public static string job_Applicant = "/job/applicant";
        public static string job_Product = "/job/product";
        public static string job_Ad = "/job/ad";
        public static string job_Promotion = "/job/promotion";
        public static string job_Brief = "/job/brief";
        public static string job_Company = "/job/company";
        public static string job_ImgUploadServiceName = "/api/uploadImage";
        #endregion
    }
}
