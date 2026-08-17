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
    public partial class FrmPosSaleQuotationLst : ContentView
    {

        #region "Declaring"
        VmlSalesQuotation mVmlSalesQuotation;
        #endregion
        #region "Constructor"
        public FrmPosSaleQuotationLst()
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlSalesQuotation = new VmlSalesQuotation();
                mVmlSalesQuotation.mJSN_REQ_SALE_QUOTATION_JUN.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mVmlSalesQuotation.mJSN_REQ_SALE_QUOTATION_JUN.RES_SALE_QUOTATION = new RES_SALE_QUOTATION();
                mVmlSalesQuotation.mJSN_REQ_SALE_QUOTATION_JUN.RES_SALE_BROWSE.Add(new RES_SALE_BROWSE());
                mVmlSalesQuotation.getQuotation();
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
                            TabActive.Text = "Active (" + mVmlSalesQuotation.QuotationActiveList.Count + ")";
                            TabInActive.Text = "Partial (" + mVmlSalesQuotation.QuotationInActiveList.Count + ")";
                            TabAll.Text = "Closed (" + mVmlSalesQuotation.QuotationList.Count + ")";
                        }
                        break;
                    case "2"://Myanmar
                        {
                            entSearch.Placeholder = "ရှာဖွေရန်";
                            TabActive.Text = "စာရင်းအဖွင့် (" + mVmlSalesQuotation.QuotationActiveList.Count + ")";
                            TabInActive.Text = "လုပ်ဆောင်ဆဲ (" + mVmlSalesQuotation.QuotationInActiveList.Count + ")";
                            TabAll.Text = "စာရင်းအပိတ် (" + mVmlSalesQuotation.QuotationList.Count + ")";

                            ICode.Title = "ပေါက်စျေးမေးမှု နံပါတ်";
                            IDate.Title = "ပေါက်စျေးမေးမှု နေ့စွဲ";
                            ICustomer.Title = "၀ယ်ယူသူ";                           

                            AcCode.Title = "ပေါက်စျေးမေးမှု နံပါတ်";
                            AcDate.Title = "ပေါက်စျေးမေးမှု နေ့စွဲ";
                            AcCustomer.Title = "၀ယ်ယူသူ";

                            ACode.Title = "ပေါက်စျေးမေးမှု နံပါတ်";
                            ADate.Title = "ပေါက်စျေးမေးမှု နေ့စွဲ";
                            ACustomer.Title = "၀ယ်ယူသူ";
                        }
                        break;
                    default:
                        {
                            entSearch.Placeholder = "Search";
                            TabActive.Text = "Active (" + mVmlSalesQuotation.QuotationActiveList.Count + ")";
                            TabInActive.Text = "Partial (" + mVmlSalesQuotation.QuotationInActiveList.Count + ")";
                            TabAll.Text = "Closed (" + mVmlSalesQuotation.QuotationList.Count + ")";
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
                    mVmlSalesQuotation.searchData(e.NewTextValue);
                }
                else
                {
                    mVmlSalesQuotation.searchData("");
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
                var l_RES_SALE_QUOTATION = (RES_SALE_QUOTATION)e.SelectedItem;
                if (l_RES_SALE_QUOTATION != null)
                {
                    if (!Common.bindMenu("access-set"))
                    {
                        Common.mCommon.SelectedMenu = new RES_MENU { ProductAsk = "1", Text = "FrmAdmin", MenuUrl = "FrmAdmin", logoImg = "" };
                        MessagingCenter.Send<Application, string>(Application.Current, "ToastMessage", ApplicationMessage.Message.MenuAccessRight);
                    }
                    Common.routeMenu(Common.mCommon.SelectedMenu);

                    //if (Common.bindMenu("access-set"))
                    //{
                    //    Common.routeMenuStr("access-set", "Access Entry", l_RES_SALE_QUOTATION.Ask) ;
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
                var l_RES_SALE_QUOTATION = (RES_SALE_QUOTATION)e.SelectedItem;
                if (l_RES_SALE_QUOTATION != null)
                {
                    if (!Common.bindMenu("access-set"))
                    {
                        Common.mCommon.SelectedMenu = new RES_MENU { ProductAsk = "1", Text = "FrmAdmin", MenuUrl = "FrmAdmin", logoImg = "" };
                        MessagingCenter.Send<Application, string>(Application.Current, "ToastMessage", ApplicationMessage.Message.MenuAccessRight);
                    }
                    Common.routeMenu(Common.mCommon.SelectedMenu);


                    //if (Common.bindMenu("access-set"))
                    //{
                    //    Common.routeMenuStr("access-set", "Access Entry", l_RES_SALE_QUOTATION.Ask);
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
                mVmlSalesQuotation.getQuotation();
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
                //mVmlSalesQuotation.GetAccessData();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }

        #endregion
    }
}