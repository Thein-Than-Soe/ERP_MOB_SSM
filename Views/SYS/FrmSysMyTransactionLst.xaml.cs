using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.ECO.DAT;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Views.Frame;
using CS.ERP_MOB.ViewsModel.SYS;
using CS.ERP_MOB.ViewsModel.POS;
using Microsoft.Maui.Controls;
using RGPopup.Maui.Services;
using System.Collections.ObjectModel;

namespace CS.ERP_MOB.Views.SYS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmSysMyTransactionLst : ContentView
    {
        #region "Declaring"
        VmlMyTransaction mVmlMyTransaction { get; set; }
        #endregion
        #region "Constructor"
        public FrmSysMyTransactionLst()
        {
            try
            {
                InitializeComponent();
                BindingContext = mVmlMyTransaction = new VmlMyTransaction();

                mVmlMyTransaction.getMyTransactionHistory();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
        #endregion

        #region "Private Mehtod"
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (mVmlMyTransaction != null)
            {
                int newColumns = width switch
                {
                    < 400 => 2,
                    < 600 => 2,
                    < 800 => 4,
                    < 1000 => 5,
                    < 1200 => 6,
                    < 1400 => 7,
                    < 1600 => 8,
                    < 1800 => 9,
                    _ => 10
                };

                if (collectionView.ItemsLayout is not GridItemsLayout currentLayout ||
                    currentLayout.Span != newColumns)
                {
                    var layout = new GridItemsLayout(newColumns, ItemsLayoutOrientation.Vertical)
                    {
                        VerticalItemSpacing = 10,
                        HorizontalItemSpacing = 10
                    };
                    collectionView.ItemsLayout = layout;
                }
            }
        }
        private void sortSalesInvoiceList(string sortBy)
        {
            if (mVmlMyTransaction.SaleOrderLst == null || !mVmlMyTransaction.SaleOrderLst.Any())
                return;

            IEnumerable<RES_SALE_BROWSE> sorted;

            switch (sortBy)
            {
                case "Date":
                    sorted = mVmlMyTransaction.IsAscending
                        ? mVmlMyTransaction.SaleOrderLst.OrderBy(x => x.Date)
                        : mVmlMyTransaction.SaleOrderLst.OrderByDescending(x => x.Date);
                    break;

                case "Code_0_50":
                    sorted = mVmlMyTransaction.IsAscending
                        ? mVmlMyTransaction.SaleOrderLst.OrderBy(x => x.Code_0_50)
                        : mVmlMyTransaction.SaleOrderLst.OrderByDescending(x => x.Code_0_50);
                    break;

                case "StatusName_0_255":
                    sorted = mVmlMyTransaction.IsAscending
                        ? mVmlMyTransaction.SaleOrderLst.OrderBy(x => x.StatusName_0_255)
                        : mVmlMyTransaction.SaleOrderLst.OrderByDescending(x => x.StatusName_0_255);
                    break;

                case "PaymentTypeName_0_255":
                    sorted = mVmlMyTransaction.IsAscending
                        ? mVmlMyTransaction.SaleOrderLst.OrderBy(x => x.GrandTotal)
                        : mVmlMyTransaction.SaleOrderLst.OrderByDescending(x => x.GrandTotal);
                    break;

                default:
                    return;
            }

            mVmlMyTransaction.SaleOrderLst = new List<RES_SALE_BROWSE>(sorted);
            if (mVmlMyTransaction.IsCardView)
            {
                collectionView.ItemsSource = mVmlMyTransaction.SaleOrderLst;
            }
            else if (mVmlMyTransaction.IsListView)
            {
                lstView.ItemsSource = mVmlMyTransaction.SaleOrderLst;
            }
            else
            {
                MyGrid.ItemsSource = mVmlMyTransaction.SaleOrderLst;
            }
        }
        private void getCheckedData(List<RES_SALE_BROWSE> l_RES_SALE_BROWSE_LST)
        {
            for (int i = 0; i < mVmlMyTransaction.SaleOrderLst.Count; i++)
            {
                //if (mVmlMyTransaction.SaleOrderLst[i].IsChecked == "1")
                //{
                    l_RES_SALE_BROWSE_LST.Add(mVmlMyTransaction.SaleOrderLst[i]);
                //}
            }
        }
        #endregion

        #region "Task"
        private async Task btnNew_onClick(RES_CONTROL argRES_CONTROL)
        {
            //string result = "";
            //Common.mCommon.getConfirmation(argRES_CONTROL);
            //if (Common.mCommon.ConfirmationUserJun.ConfirmationStatus == "0")
            //{
            //    result = "7";
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "1")//1 for Yes/No
            //{
            //    var popup = new PopConfirmYesNo();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "2")//2 for EmailOTP
            //{
            //    var popup = new PopConfirmEmailOTP();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "3")//3 for SMSOTP
            //{
            //    var popup = new PopConfirmSMSOTP();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "4")//4 for Password
            //{
            //    var popup = new PopConfirmPassword();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "5")//5 for Signature
            //{
            //    var popup = new PopConfirmSignature();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //if (result != null && result == "7")
            //{
            //    Common.mCommon.saveNoti(argRES_CONTROL);
            //    await Navigation.PushAsync(new FrmPosSaleInvoiceSet());
            //}
        }
        private async Task btnEdit_onClick(object tappedItem, RES_CONTROL argRES_CONTROL)
        {
        }
        private async Task btnDelete_onClick(object tappedItem, RES_CONTROL argRES_CONTROL)
        {

        }
        private async Task btnPrint_onClick(RES_CONTROL argRES_CONTROL)
        {
            List<RES_SALE_BROWSE> l_RES_SALE_BROWSE_LST = new List<RES_SALE_BROWSE>();
            getCheckedData(l_RES_SALE_BROWSE_LST);
            string messageInfo = Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Print");
            string notiInfo = "";
            foreach (RES_SALE_BROWSE item in l_RES_SALE_BROWSE_LST)
            {
                messageInfo += item.Code_0_50 + ",";
                notiInfo += item.Ask + ",";
            }
            if (messageInfo.Length > 0)
            {
                messageInfo = messageInfo.TrimEnd(',');
                notiInfo = notiInfo.TrimEnd(',');
            }

            string result = "";
            //Common.mCommon.getConfirmation(argRES_CONTROL);
            //if (Common.mCommon.ConfirmationUserJun.ConfirmationStatus == "0")
            //{
            //    result = "7";
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "1")//1 for Yes/No
            //{
            //    var popup = new PopConfirmYesNo();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "2")//2 for EmailOTP
            //{
            //    var popup = new PopConfirmEmailOTP();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "3")//3 for SMSOTP
            //{
            //    var popup = new PopConfirmSMSOTP();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "4")//4 for Password
            //{
            //    var popup = new PopConfirmPassword();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "5")//5 for Signature
            //{
            //    var popup = new PopConfirmSignature();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //if (result != null && result == "7")
            //{
            //    Common.mCommon.saveNoti(argRES_CONTROL, notiInfo);
            //}
        }
        private async Task btnSendMail_onClick(RES_CONTROL argRES_CONTROL)
        {
            List<RES_SALE_BROWSE> l_RES_SALE_BROWSE_LST = new List<RES_SALE_BROWSE>();
            for (int i = 0; i < mVmlMyTransaction.SaleOrderLst.Count; i++)
            {
                if (mVmlMyTransaction.SaleOrderLst[i].IsChecked == "1")
                {
                    l_RES_SALE_BROWSE_LST.Add(mVmlMyTransaction.SaleOrderLst[i]);
                }
            }
            string messageInfo = Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Send");
            string notiInfo = "";
            foreach (RES_SALE_BROWSE item in l_RES_SALE_BROWSE_LST)
            {
                messageInfo += item.Code_0_50 + ",";
                notiInfo += item.Ask + ",";
            }
            if (messageInfo.Length > 0)
            {
                messageInfo = messageInfo.TrimEnd(',');
                notiInfo = notiInfo.TrimEnd(',');
            }

            string result = "";
            //Common.mCommon.getConfirmation(argRES_CONTROL);
            //if (Common.mCommon.ConfirmationUserJun.ConfirmationStatus == "0")
            //{
            //    result = "7";
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "1")//1 for Yes/No
            //{
            //    var popup = new PopConfirmYesNo();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "2")//2 for EmailOTP
            //{
            //    var popup = new PopConfirmEmailOTP();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "3")//3 for SMSOTP
            //{
            //    var popup = new PopConfirmSMSOTP();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "4")//4 for Password
            //{
            //    var popup = new PopConfirmPassword();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "5")//5 for Signature
            //{
            //    var popup = new PopConfirmSignature();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //if (result != null && result == "7")
            //{
            //    Common.mCommon.saveNoti(argRES_CONTROL, notiInfo);
            //}

        }
        private async Task btnExpPDF_onClick(RES_CONTROL argRES_CONTROL)
        {
            //List<RES_SALE_BROWSE> l_RES_SALE_BROWSE_LST = new List<RES_SALE_BROWSE>();
            //for (int i = 0; i < mVmlMyTransaction.SaleOrderLst.Count; i++)
            //{
            //    if (mVmlMyTransaction.SaleOrderLst[i].IsChecked == "1")
            //    {
            //        l_RES_SALE_BROWSE_LST.Add(mVmlMyTransaction.SaleOrderLst[i]);
            //    }
            //}
            //string messageInfo = Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Export");
            //string notiInfo = "";
            //foreach (RES_SALE_BROWSE item in l_RES_SALE_BROWSE_LST)
            //{
            //    messageInfo += item.Code_0_50 + ",";
            //    notiInfo += item.Ask + ",";
            //}
            //if (messageInfo.Length > 0)
            //{
            //    messageInfo = messageInfo.TrimEnd(',');
            //    notiInfo = notiInfo.TrimEnd(',');
            //}

            //string result = "";
            //Common.mCommon.getConfirmation(argRES_CONTROL);
            //if (Common.mCommon.ConfirmationUserJun.ConfirmationStatus == "0")
            //{
            //    result = "7";
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "1")//1 for Yes/No
            //{
            //    var popup = new PopConfirmYesNo();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "2")//2 for EmailOTP
            //{
            //    var popup = new PopConfirmEmailOTP();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "3")//3 for SMSOTP
            //{
            //    var popup = new PopConfirmSMSOTP();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "4")//4 for Password
            //{
            //    var popup = new PopConfirmPassword();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "5")//5 for Signature
            //{
            //    var popup = new PopConfirmSignature();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //if (result != null && result == "7")
            //{
            //    Common.mCommon.saveNoti(argRES_CONTROL, notiInfo);
            //}
        }
        private async Task btnExpExcel_onClick(RES_CONTROL argRES_CONTROL)
        {
            List<RES_SALE_BROWSE> l_RES_SALE_BROWSE_LST = new List<RES_SALE_BROWSE>();
            for (int i = 0; i < mVmlMyTransaction.SaleOrderLst.Count; i++)
            {
                if (mVmlMyTransaction.SaleOrderLst[i].IsChecked == "1")
                {
                    l_RES_SALE_BROWSE_LST.Add(mVmlMyTransaction.SaleOrderLst[i]);
                }
            }
            string messageInfo = Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Export");
            string notiInfo = "";
            foreach (RES_SALE_BROWSE item in l_RES_SALE_BROWSE_LST)
            {
                messageInfo += item.Code_0_50 + ",";
                notiInfo += item.Ask + ",";
            }
            if (messageInfo.Length > 0)
            {
                messageInfo = messageInfo.TrimEnd(',');
                notiInfo = notiInfo.TrimEnd(',');
            }
            string result = "";
            //Common.mCommon.getConfirmation(argRES_CONTROL);
            //if (Common.mCommon.ConfirmationUserJun.ConfirmationStatus == "0")
            //{
            //    result = "7";
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "1")//1 for Yes/No
            //{
            //    var popup = new PopConfirmYesNo();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "2")//2 for EmailOTP
            //{
            //    var popup = new PopConfirmEmailOTP();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "3")//3 for SMSOTP
            //{
            //    var popup = new PopConfirmSMSOTP();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "4")//4 for Password
            //{
            //    var popup = new PopConfirmPassword();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "5")//5 for Signature
            //{
            //    var popup = new PopConfirmSignature();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //if (result != null && result == "7")
            //{
            //    Common.mCommon.saveNoti(argRES_CONTROL, notiInfo);
            //}
        }
        private async Task btnExpCSV_onClick(RES_CONTROL argRES_CONTROL)
        {
            List<RES_SALE_BROWSE> l_RES_SALE_BROWSE_LST = new List<RES_SALE_BROWSE>();
            for (int i = 0; i < mVmlMyTransaction.SaleOrderLst.Count; i++)
            {
                if (mVmlMyTransaction.SaleOrderLst[i].IsChecked == "1")
                {
                    l_RES_SALE_BROWSE_LST.Add(mVmlMyTransaction.SaleOrderLst[i]);
                }
            }
            string messageInfo = Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Export");
            string notiInfo = "";
            foreach (RES_SALE_BROWSE item in l_RES_SALE_BROWSE_LST)
            {
                messageInfo += item.Code_0_50 + ",";
                notiInfo += item.Ask + ",";
            }
            if (messageInfo.Length > 0)
            {
                messageInfo = messageInfo.TrimEnd(',');
                notiInfo = notiInfo.TrimEnd(',');
            }
            string result = "";
            //Common.mCommon.getConfirmation(argRES_CONTROL);
            //if (Common.mCommon.ConfirmationUserJun.ConfirmationStatus == "0")
            //{
            //    result = "7";
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "1")//1 for Yes/No
            //{
            //    var popup = new PopConfirmYesNo();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "2")//2 for EmailOTP
            //{
            //    var popup = new PopConfirmEmailOTP();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "3")//3 for SMSOTP
            //{
            //    var popup = new PopConfirmSMSOTP();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "4")//4 for Password
            //{
            //    var popup = new PopConfirmPassword();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "5")//5 for Signature
            //{
            //    var popup = new PopConfirmSignature();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //if (result != null && result == "7")
            //{
            //    Common.mCommon.saveNoti(argRES_CONTROL, notiInfo);
            //}
        }
        private async Task btnPost_onClick(RES_CONTROL argRES_CONTROL)
        {
            //List<RES_SALE_BROWSE> l_RES_SALE_BROWSE_LST = new List<RES_SALE_BROWSE>();
            //for (int i = 0; i < mVmlMyTransaction.SaleOrderLst.Count; i++)
            //{
            //    if (mVmlMyTransaction.SaleOrderLst[i].IsChecked == "1")
            //    {
            //        l_RES_SALE_BROWSE_LST.Add(mVmlMyTransaction.SaleOrderLst[i]);
            //    }
            //}
            //string messageInfo = Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Post");
            //string notiInfo = "";
            //foreach (RES_SALE_BROWSE item in l_RES_SALE_BROWSE_LST)
            //{
            //    messageInfo += item.Code_0_50 + ",";
            //    notiInfo += item.Ask + ",";
            //}
            //if (messageInfo.Length > 0)
            //{
            //    messageInfo = messageInfo.TrimEnd(',');
            //    notiInfo = notiInfo.TrimEnd(',');
            //}

            //string result = "";
            //Common.mCommon.getConfirmation(argRES_CONTROL);
            //if (Common.mCommon.ConfirmationUserJun.ConfirmationStatus == "0")
            //{
            //    result = "7";
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "1")//1 for Yes/No
            //{
            //    var popup = new PopConfirmYesNo();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "2")//2 for EmailOTP
            //{
            //    var popup = new PopConfirmEmailOTP();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "3")//3 for SMSOTP
            //{
            //    var popup = new PopConfirmSMSOTP();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "4")//4 for Password
            //{
            //    var popup = new PopConfirmPassword();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "5")//5 for Signature
            //{
            //    var popup = new PopConfirmSignature();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //if (result != null && result == "7")
            //{
            //    Common.mCommon.saveNoti(argRES_CONTROL, notiInfo);
            //}
        }
        private async Task btnSummary_onClick(RES_CONTROL argRES_CONTROL)
        {
            List<RES_SALE_BROWSE> l_RES_SALE_BROWSE_LST = new List<RES_SALE_BROWSE>();
            for (int i = 0; i < mVmlMyTransaction.SaleOrderLst.Count; i++)
            {
                if (mVmlMyTransaction.SaleOrderLst[i].IsChecked == "1")
                {
                    l_RES_SALE_BROWSE_LST.Add(mVmlMyTransaction.SaleOrderLst[i]);
                }
            }
            string messageInfo = Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Export");
            string notiInfo = "";
            foreach (RES_SALE_BROWSE item in l_RES_SALE_BROWSE_LST)
            {
                messageInfo += item.Code_0_50 + ",";
                notiInfo += item.Ask + ",";
            }
            if (messageInfo.Length > 0)
            {
                messageInfo = messageInfo.TrimEnd(',');
                notiInfo = notiInfo.TrimEnd(',');
            }
            string result = "";
            //Common.mCommon.getConfirmation(argRES_CONTROL);
            //if (Common.mCommon.ConfirmationUserJun.ConfirmationStatus == "0")
            //{
            //    result = "7";
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "1")//1 for Yes/No
            //{
            //    var popup = new PopConfirmYesNo();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "2")//2 for EmailOTP
            //{
            //    var popup = new PopConfirmEmailOTP();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "3")//3 for SMSOTP
            //{
            //    var popup = new PopConfirmSMSOTP();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "4")//4 for Password
            //{
            //    var popup = new PopConfirmPassword();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //else if (Common.mCommon.ConfirmationUserJun.ConfirmationTypeAsk == "5")//5 for Signature
            //{
            //    var popup = new PopConfirmSignature();
            //    await PopupNavigation.Instance.PushAsync(popup);
            //    result = await popup.ShowAsync();
            //}
            //if (result != null && result == "7")
            //{
            //    Common.mCommon.saveNoti(argRES_CONTROL, notiInfo);
            //}
        }
        #endregion

        #region "Event"

        private void OnEntryCompleted(object sender, EventArgs e)
        {
            try
            {
                string text = ((Entry)sender).Text;
                if (Common.mCommon.UserSetting.TLSearchTypeAsk == "1")//1 for local search
                {

                    if (text != null && text != "")
                    {
                        mVmlMyTransaction.searchData(text);
                    }
                    else
                    {
                        mVmlMyTransaction.searchData("");
                    }
                }
                else
                {
                    mVmlMyTransaction.searchDataApi(text);
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private async void OnEditSwipeInvoked(object sender, EventArgs e)
        {
            //if (sender is SwipeItem swipeItem && swipeItem.BindingContext is RES_SALE_BROWSE selectedItem)
            //{
            //    await Navigation.PushAsync(new FrmPosSaleInvoiceSet(selectedItem));
            //}
        }
        private void OnMenuTapped(object sender, TappedEventArgs e)
        {
            Overlay.IsVisible = true;
            MenuBox.IsVisible = true;
        }
        private void OnOverlayTapped(object sender, EventArgs e)
        {
            MenuBox.IsVisible = false;
            Overlay.IsVisible = false;
        }
        private async void OnItemSingleTapped(object sender, object tappedItem)
        {
            //if (Utility.checkButtonAccess("Edit"))
            //{
            //    await Navigation.PushAsync(new FrmPosSaleInvoiceSet((RES_SALE_BROWSE)tappedItem));
            //}
            //else
            //{
            //    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
            //}
        }
        private async void OnItemDoubleTapped(object sender, object tappedItem)
        {
            var popup = new OptionsPopup(Common.mCommon.SelectedMenu.button);
            popup.OnNewClicked = async (RES_CONTROL argRES_CONTROL) =>
            {
                await btnNew_onClick(argRES_CONTROL);
            };
            popup.OnEditClicked = async (RES_CONTROL argRES_CONTROL) =>
            {
                await btnEdit_onClick(tappedItem, argRES_CONTROL);
            };
            popup.OnDeleteClicked = async (RES_CONTROL argRES_CONTROL) =>
            {
                await btnDelete_onClick(tappedItem, argRES_CONTROL);
            };
            popup.OnPrintClicked = async (RES_CONTROL argRES_CONTROL) =>
            {
                await btnPrint_onClick(argRES_CONTROL);
            };
            popup.OnSendMailClicked = async (RES_CONTROL argRES_CONTROL) =>
            {
                await btnSendMail_onClick(argRES_CONTROL);
            };
            popup.OnExpPDFClicked = async (RES_CONTROL argRES_CONTROL) =>
            {
                await btnExpPDF_onClick(argRES_CONTROL);
            };
            popup.OnExpExcelClicked = async (RES_CONTROL argRES_CONTROL) =>
            {
                await btnExpExcel_onClick(argRES_CONTROL);
            };
            popup.OnExpCSVClicked = async (RES_CONTROL argRES_CONTROL) =>
            {
                await btnExpCSV_onClick(argRES_CONTROL);
            };
            popup.OnPostClicked = async (RES_CONTROL argRES_CONTROL) =>
            {
                await btnPost_onClick(argRES_CONTROL);
            };
            popup.OnSummaryClicked = async (RES_CONTROL argRES_CONTROL) =>
            {
                await btnSummary_onClick(argRES_CONTROL);
            };

            await this.GetParentPage().ShowPopupAsync(popup);

        }
        private void OnCheckAllCheckChanged(object sender, CheckedChangedEventArgs e)
        {
            bool checkAll = chkSelectAll.IsChecked;

            for (int i = 0; i < mVmlMyTransaction.SaleOrderLst.Count; i++)
            {
                var item = mVmlMyTransaction.SaleOrderLst[i];
                item.IsChecked = checkAll ? "1" : "0";

                mVmlMyTransaction.SaleOrderLst.RemoveAt(i);
                mVmlMyTransaction.SaleOrderLst.Insert(i, item);
            }
        }
        private void Sorting_Tapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is SortingItem tappedItem)
            {
                // Hide all icons
                foreach (var item in mVmlMyTransaction.sortingList)
                    item.ShowIcon = false;

                // Show only tapped item’s icon
                tappedItem.ShowIcon = true;
                sortSalesInvoiceList(tappedItem.value);
            }
        }
        private void Ascending_Tapped(object sender, TappedEventArgs e)
        {
            mVmlMyTransaction.IsDescending = false;
            mVmlMyTransaction.IsAscending = true;
        }
        private void Descending_Tapped(object sender, TappedEventArgs e)
        {
            mVmlMyTransaction.IsDescending = true;
            mVmlMyTransaction.IsAscending = false;
        }
        private void chkSelectItem_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is RES_SALE_BROWSE item)
            {
                item.IsChecked = e.Value ? "1" : "0";
            }
        }
        private async void OnListSingleTap(object sender, TappedEventArgs e)
        {
            //if (Utility.checkButtonAccess("Edit"))
            //{
            //    if (e.Parameter is RES_SALE_BROWSE tappedItem)
            //    {
            //        await Navigation.PushAsync(new FrmPosSaleInvoiceSet(tappedItem));
            //    }
            //}
            //else
            //{
            //    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
            //}
        }
        private void OnListNGridLongPress(object sender, EventArgs e)
        {
            if (sender is Grid gridItem && gridItem.BindingContext is RES_SALE_BROWSE selectedItem)
            {
                if (selectedItem == null)
                    return;
                int index = mVmlMyTransaction.SaleOrderLst.IndexOf(selectedItem);
                selectedItem.IsChecked = "1";
                mVmlMyTransaction.SaleOrderLst.RemoveAt(index);
                mVmlMyTransaction.SaleOrderLst.Insert(index, selectedItem);
                OnItemDoubleTapped(sender, selectedItem);
            }
        }
        private void OnListDoubleTap(object sender, TappedEventArgs e)
        {
            var tappedItem = e.Parameter as RES_SALE_BROWSE;

            if (tappedItem == null)
                return;
            tappedItem.IsChecked = "1";
            OnItemDoubleTapped(sender, tappedItem);
        }
        private async void OnGridSingleTap(object sender, TappedEventArgs e)
        {
            //if (!Utility.checkButtonAccess("Edit"))
            //{
            //    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
            //    return;
            //}

            //if (sender is VisualElement ve && ve.BindingContext is RES_SALE_BROWSE tappedItem)
            //{
            //    await Navigation.PushAsync(new FrmPosSaleInvoiceSet(tappedItem));
            //}
        }
        private void OnGridDoubleTap(object sender, TappedEventArgs e)
        {
            if (sender is VisualElement ve && ve.BindingContext is RES_SALE_BROWSE tappedItem)
            {
                tappedItem.IsChecked = "1";
                OnItemDoubleTapped(sender, tappedItem);
            }
        }
        #endregion
    }
}