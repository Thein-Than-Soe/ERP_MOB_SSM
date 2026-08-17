using CS.ERP_MOB.Views.SSM;
using CS.ERP_MOB.Views.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.ERP_MOB.Route
{
    class Ssm_Route
    {
        public static Dictionary<string, Type> DicRouteList { get; private set; }
        static Ssm_Route()
        {
            DicRouteList = new Dictionary<string, Type>();
            DicRouteList.Add("home", typeof(HomePage));
            DicRouteList.Add("login", typeof(FrmSignIn));
            DicRouteList.Add("ovareg", typeof(FrmSignUp));
            DicRouteList.Add("change-password", typeof(ChangePasswordPage));

            DicRouteList.Add("csm-profile", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-dashboard", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-front-desk", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-enquiry-lst", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-enquiry-set", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-enquiry-dtl", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-enquiry-import", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-quotation-lst", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-quotation-set", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-quotation-dtl", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-quotation-import", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-book-lst", typeof(FrmSsmBookLst));
            DicRouteList.Add("csm-book-set", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-book-dtl", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-book-import", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-service-lst", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-service-set", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-service-dtl", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-service-import", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-book-now-lst", typeof(FrmSsmBookLst));
            DicRouteList.Add("csm-book-now-set", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-book-now-dtl", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-book-now-import", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-check-in-out-lst", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-check-in-out-set", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-check-in-out-dtl", typeof(FrmSsmProfile));
            DicRouteList.Add("csm-check-in-out-import", typeof(FrmSsmProfile));
            DicRouteList.Add("privacy", typeof(FrmSsmProfile));

        }
    }
}
