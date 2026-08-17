using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.ViewsModel.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;


namespace CS.ERP_MOB.Views.POS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmPosSaleEnquiryLst : ContentView
    {

        #region "Declaring"
        VmlSalesEnquiry mVmlSalesEnquiry;
        #endregion
        #region "Constructor"
        public FrmPosSaleEnquiryLst()
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlSalesEnquiry = new VmlSalesEnquiry();
                mVmlSalesEnquiry.mJSN_REQ_SALE_ENQUIRY.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mVmlSalesEnquiry.mJSN_REQ_SALE_ENQUIRY.RES_SALE_ENQUIRY = new RES_SALE_ENQUIRY();
                mVmlSalesEnquiry.getEnquiry();
                switchLanguage();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
        #endregion
        #region "Private Mehtod"
        public void switchLanguage()
        {
            try
            {
                switch (Common.mCommon.UserSetting.LanguageAsk)
                {
                    case "1"://English
                        {
                            entSearch.Placeholder = "Search";
                            TabActive.Text = "Open (" + mVmlSalesEnquiry.EnquiryActiveList.Count + ")";
                            TabPartial.Text = "Partial (" + mVmlSalesEnquiry.EnquiryPartialList.Count + ")";
                            TabClosed.Text = "Closed (" + mVmlSalesEnquiry.EnquiryClosedList.Count + ")";
                        }
                        break;
                    case "2"://Myanmar
                        {                            
                            entSearch.Placeholder = "ရှာဖွေရန်";
                            TabActive.Text = "စာရင်းအဖွင့် (" + mVmlSalesEnquiry.EnquiryActiveList.Count + ")";
                            TabPartial.Text = "လုပ်ဆောင်ဆဲ (" + mVmlSalesEnquiry.EnquiryPartialList.Count + ")";
                            TabClosed.Text = "စာရင်းအပိတ် (" + mVmlSalesEnquiry.EnquiryClosedList.Count + ")";

                            TabCode.Title = "စုံစမ်း အမှတ်စဉ်";
                            TabDate.Title = "စုံစမ်း နေ့ရက်";
                            TabCustomer.Title = "၀ယ်ယူသူ";

                            ACode.Title = "စုံစမ်း အမှတ်စဉ်";
                            ADate.Title = "စုံစမ်း နေ့ရက်";
                            ACustomer.Title = "၀ယ်ယူသူ";

                            PCode.Title = "စုံစမ်း အမှတ်စဉ်";
                            PDate.Title = "စုံစမ်း နေ့ရက်";
                            PCustomer.Title = "၀ယ်ယူသူ";
                            
                        }
                        break;
                    default:
                        {
                            entSearch.Placeholder = "Search";
                            TabActive.Text = "Open (" + mVmlSalesEnquiry.EnquiryActiveList.Count + ")";
                            TabPartial.Text = "Partial (" + mVmlSalesEnquiry.EnquiryPartialList.Count + ")";
                            TabClosed.Text = "Closed (" + mVmlSalesEnquiry.EnquiryClosedList.Count + ")";
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        #endregion
        #region "Event"
        private void TgrNew_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (!Common.bindMenu("access-set"))
                {
                    Common.mCommon.SelectedMenu = new RES_MENU { ProductAsk = "1", Text = "FrmAdmin", MenuUrl = "FrmAdmin", logoImg = "" };
                    MessagingCenter.Send<Application, string>(Application.Current, "ToastMessage", ApplicationMessage.Message.MenuAccessRight);
                }
                Common.routeMenu(Common.mCommon.SelectedMenu);


                //if (Common.bindMenu("access-set"))
                //{
                //    Common.routeMenu("access-set", "Access Entry");
                //}
                //else
                //{
                //    MessagingCenter.Send<Application, string>(Application.Current, "ToastMessage", "no access right");
                //}
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void entSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (entSearch.Text != null && entSearch.Text != "")
                {
                    mVmlSalesEnquiry.searchData(e.NewTextValue);
                }
                else
                {
                    mVmlSalesEnquiry.searchData("");
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
        private void lstView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                var l_RES_PURCHASE_RETURN = (RES_PURCHASE_RETURN)e.SelectedItem;
                if (l_RES_PURCHASE_RETURN != null)
                {
                    if (!Common.bindMenu("access-set"))
                    {
                        Common.mCommon.SelectedMenu = new RES_MENU { ProductAsk = "1", Text = "FrmAdmin", MenuUrl = "FrmAdmin", logoImg = "" };
                        MessagingCenter.Send<Application, string>(Application.Current, "ToastMessage", ApplicationMessage.Message.MenuAccessRight);
                    }
                    Common.routeMenu(Common.mCommon.SelectedMenu);

                    //if (Common.bindMenu("access-set"))
                    //{
                    //    Common.routeMenuStr("access-set", "Access Entry", l_RES_PURCHASE_RETURN.Ask) ;
                    //}
                    //else
                    //{
                    //    MessagingCenter.Send<Application, string>(Application.Current, "ToastMessage", "no access right");
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
        private void grdView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                var l_RES_PURCHASE_RETURN = (RES_PURCHASE_RETURN)e.SelectedItem;
                if (l_RES_PURCHASE_RETURN != null)
                {
                    if (!Common.bindMenu("access-set"))
                    {
                        Common.mCommon.SelectedMenu = new RES_MENU { ProductAsk = "1", Text = "FrmAdmin", MenuUrl = "FrmAdmin", logoImg = "" };
                        MessagingCenter.Send<Application, string>(Application.Current, "ToastMessage", ApplicationMessage.Message.MenuAccessRight);
                    }
                    Common.routeMenu(Common.mCommon.SelectedMenu);


                    //if (Common.bindMenu("access-set"))
                    //{
                    //    Common.routeMenuStr("access-set", "Access Entry", l_RES_PURCHASE_RETURN.Ask);
                    //}
                    //else
                    //{
                    //    MessagingCenter.Send<Application, string>(Application.Current, "ToastMessage", "no access right");
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
        private void TgrCardView_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (!Common.bindMenu("access-set"))
                {
                    Common.mCommon.SelectedMenu = new RES_MENU { ProductAsk = "1", Text = "FrmAdmin", MenuUrl = "FrmAdmin", logoImg = "" };
                    MessagingCenter.Send<Application, string>(Application.Current, "ToastMessage", ApplicationMessage.Message.MenuAccessRight);
                }
                Common.routeMenu(Common.mCommon.SelectedMenu);

                //if (Common.bindMenu("access-set"))
                //{
                //    Common.routeMenu("access-set", "Access Entry");
                //}
                //else
                //{
                //    MessagingCenter.Send<Application, string>(Application.Current, "ToastMessage", "no access right");
                //}
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void TgrRefresh_Tapped(object sender, EventArgs e)
        {
            try
            {
                entSearch.Text = "";
                mVmlSalesEnquiry.getEnquiry();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
        private void TgrDisplayColumn_Tapped(object sender, EventArgs e)
        {
            try
            {
                //mVmlSalesEnquiry.GetAccessData();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }

        #endregion
    }
}