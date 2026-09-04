using CS.ERP_MOB.Views.SSM;
using CS.ERP_MOB.Views.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS.ERP_MOB.Views.SYS;
using CS.ERP_MOB.Views.POS;
using CS.ERP.PL.HMS.DAT;

namespace CS.ERP_MOB.Route
{
    class Ssm_Route
    {
        public static Dictionary<string, Type> DicRouteList { get; private set; }
        static Ssm_Route()
        {
            DicRouteList = new Dictionary<string, Type>();

            DicRouteList.Add("home", typeof(HomePage));
            DicRouteList.Add("signin", typeof(FrmSignIn));
            DicRouteList.Add("signup", typeof(FrmSignUp));

            DicRouteList.Add("ssm-profile", typeof(FrmSsmProfile));
            DicRouteList.Add("ssm-dashboard", typeof(FrmSsmDashboardLst));
            DicRouteList.Add("ssm-front-desk", typeof(FrmSsmScheduleLst));
            DicRouteList.Add("ssm-book-lst", typeof(FrmSsmOrderSet));
            //DicRouteList.Add("ssm-book-set", typeof(FrmSsmProfile));
            //DicRouteList.Add("ssm-book-dtl", typeof(FrmSsmProfile));
            //DicRouteList.Add("ssm-book-import", typeof(FrmSsmProfile));
            DicRouteList.Add("ssm-service-lst", typeof(FrmSsmProfile));
            //DicRouteList.Add("ssm-service-set", typeof(FrmSsmProfile));
            //DicRouteList.Add("ssm-service-dtl", typeof(FrmSsmProfile));
            //DicRouteList.Add("ssm-service-import", typeof(FrmSsmProfile));
            DicRouteList.Add("ssm-book-now-lst", typeof(FrmSsmBookNowLst));
            //DicRouteList.Add("ssm-book-now-set", typeof(FrmSsmBookNowLst(DAT_FRONT_DESK)));
            //DicRouteList.Add("ssm-book-now-dtl", typeof(FrmSsmProfile));
            //DicRouteList.Add("ssm-book-now-import", typeof(FrmSsmProfile));
            //DicRouteList.Add("ssm-check-in-out-lst", typeof(FrmSsmProfile));
            //DicRouteList.Add("ssm-check-in-out-set", typeof(FrmSsmProfile));
            //DicRouteList.Add("ssm-check-in-out-dtl", typeof(FrmSsmProfile));
            //DicRouteList.Add("ssm-check-in-out-import", typeof(FrmSsmProfile));
            DicRouteList.Add("privacy", typeof(FrmSsmProfile));
            DicRouteList.Add("ssm-transaction-lst", typeof(FrmSysMyTransactionLst));
            DicRouteList.Add("ssm-my-order-lst", typeof(FrmSysMyOrderLst));
            DicRouteList.Add("ssm-payment-lst", typeof(FrmSysMyPaymentLst));

        }
    }
}
