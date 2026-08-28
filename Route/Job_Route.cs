using CS.ERP_MOB.Views.JOB;
using CS.ERP_MOB.Views.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.ERP_MOB.Route
{
    class Job_Route
    {
        public static Dictionary<string, Type> DicRouteList { get; private set; }
        static Job_Route()
        {
            DicRouteList = new Dictionary<string, Type>();
            DicRouteList.Add("home", typeof(HomePage));
            DicRouteList.Add("signin", typeof(FrmSignIn));
            DicRouteList.Add("signup", typeof(FrmSignUp));
            DicRouteList.Add("change-password", typeof(ChangePasswordPage));

            DicRouteList.Add("job-profile", typeof(FrmJobProfile));
            //DicRouteList.Add("job-application-lst", typeof(FrmJobApplicationSet));
            DicRouteList.Add("job-advice-lst", typeof(FrmJobAdviceLst));
            DicRouteList.Add("job-company-lst", typeof(FrmJobCompanyLst));
            //DicRouteList.Add("job-review-set", typeof(HomePage));
            //DicRouteList.Add("job-review-lst", typeof(HomePage));
            DicRouteList.Add("job-community-lst", typeof(FrmJobCommunityLst));
            DicRouteList.Add("job-job-search-lst", typeof(FrmJobSearchLst));
            //DicRouteList.Add("job-job-search-set", typeof(HomePage));
            //DicRouteList.Add("job-job-vacancy-set", typeof(HomePage));
            //DicRouteList.Add("job-job-vacancy-lst", typeof(HomePage));
        }
    }
}
