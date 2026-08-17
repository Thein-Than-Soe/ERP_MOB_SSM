using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.POS.REQ;
using CS.ERP.PL.POS.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.POS;
using CS.ERP_MOB.ViewsModel.Frame;
using Newtonsoft.Json;

using System.Windows.Input;
using Microsoft.Maui.Controls;
using static CS.ERP_MOB.General.Utility;
using CommunityToolkit.Mvvm.Messaging;
using CS.ERP_MOB.Views.POS;
using System.Diagnostics;
using RGPopup.Maui.Services;

namespace CS.ERP_MOB.ViewsModel.POS
{
    public class VmlSalesInvoice : BaseViewModel
    {
        #region "Declaring"
        public JSN_REQ_SALE_INVOICE_JUN mJSN_REQ_SALE_INVOICE_JUN = new JSN_REQ_SALE_INVOICE_JUN();
        public JSN_SALE_INVOICE_JUN mJSN_SALE_INVOICE_JUN = new JSN_SALE_INVOICE_JUN();
        public JSN_LOAD_SALE_INVOICE mJSN_LOAD_SALE_INVOICE = new JSN_LOAD_SALE_INVOICE();
        string mRequest = "";
        string mResponse = "";
        public ICommand LoadMoreCommand { get; }
        private bool isLoadingMore = false;
        public bool IsLoadingMore
        {
            get => isLoadingMore;
            set
            {
                isLoadingMore = value;
                NotifyPropertyChanged(nameof(IsLoadingMore));
            }
        }
        #endregion

        #region "Contructor"
        public VmlSalesInvoice()
        {
            this.switchDisplayView(DisplayView.Card);
            InvoiceLoad = new JSN_LOAD_SALE_INVOICE();
            SalesInvoiceList = new List<RES_SALE_INVOICE>();
            LoadMoreCommand = new Command(async () => await LoadMoreItems());
        }
        #endregion

        #region "Display View"
        private bool mIsCardView;
        public bool IsCardView
        {
            get
            {
                return mIsCardView;
            }
            set
            {
                mIsCardView = value;
                NotifyPropertyChanged("IsCardView");
            }
        }

        private bool mIsListView;
        public bool IsListView
        {
            get
            {
                return mIsListView;
            }
            set
            {
                mIsListView = value;
                NotifyPropertyChanged("IsListView");
            }
        }

        private bool mIsGridView;
        public bool IsGridView
        {
            get
            {
                return mIsGridView;
            }
            set
            {
                mIsGridView = value;
                NotifyPropertyChanged("IsGridView");
            }
        }

        private bool mIsRefreshing;
        public bool IsRefreshing
        {
            get
            {
                return mIsRefreshing;
            }
            set
            {
                mIsRefreshing = value;
                NotifyPropertyChanged("IsRefreshing");
                if (value) // Only when refreshing starts
                {
                    IsRefreshing = false;
                    
                }
            }
        }
        #endregion

        #region "Data Tab"
        public JSN_LOAD_SALE_INVOICE JSN_LOAD_SALE_INVOICE = new JSN_LOAD_SALE_INVOICE();
        public JSN_LOAD_SALE_INVOICE InvoiceLoad
        {
            get { return JSN_LOAD_SALE_INVOICE; }
            set { JSN_LOAD_SALE_INVOICE = value; NotifyPropertyChanged("InvoiceLoad"); }
        }

        public RES_SALE_INVOICE mRES_SALE_INVOICE = new RES_SALE_INVOICE();
        public RES_SALE_INVOICE_DETAIL mRES_SALE_INVOICE_DETAIL = new RES_SALE_INVOICE_DETAIL();
        public RES_COMPANY mRES_COMPANY = new RES_COMPANY();
        public RES_SALE_BROWSE mRES_SALE_BROWSE = new RES_SALE_BROWSE();

        public List<RES_SALE_INVOICE> mRES_SALE_INVOICE_LST = new List<RES_SALE_INVOICE>();

        public RES_SALE_BROWSE RES_SALE_BROWSE
        {
            get { return mRES_SALE_BROWSE; }
            set { mRES_SALE_BROWSE = value; NotifyPropertyChanged("RES_SALE_BROWSE"); }
        }

        public RES_SALE_INVOICE RES_SALE_INVOICE
        {
            get { return mRES_SALE_INVOICE; }
            set { mRES_SALE_INVOICE = value; NotifyPropertyChanged("RES_SALE_INVOICE"); }
        }

        public RES_SALE_INVOICE_DETAIL RES_SALE_INVOICE_DETAIL
        {
            get { return mRES_SALE_INVOICE_DETAIL; }
            set { mRES_SALE_INVOICE_DETAIL = value; NotifyPropertyChanged("RES_SALE_INVOICE_DETAIL"); }
        }

        public RES_COMPANY RES_COMPANY
        {
            get { return mRES_COMPANY; }
            set { mRES_COMPANY = value; NotifyPropertyChanged("RES_SALE_INVOICE"); }
        }


        public List<RES_SALE_INVOICE> mSalesInvoiceList;
        public List<RES_SALE_INVOICE> SalesInvoiceList
        {
            get { return mSalesInvoiceList; }
            set { mSalesInvoiceList = value; NotifyPropertyChanged("SalesInvoiceList"); }
        }

        public List<RES_CUSTOMER_DTL> mCustomerDtlList;
        public List<RES_CUSTOMER_DTL> CustomerDtlList
        {
            get { return mCustomerDtlList; }
            set { mCustomerDtlList = value; NotifyPropertyChanged("CustomerDtlList"); }
        }

        #endregion

        #region "Commands"
        private ICommand mCardViewCommand;
        public ICommand CardViewCommand
        {
            get
            {
                if (mCardViewCommand == null)
                {
                    mCardViewCommand = new Command(() => this.switchDisplayView(DisplayView.Card));
                }
                return mCardViewCommand;
            }
        }

        private ICommand mListViewCommand;
        public ICommand ListViewCommand
        {
            get
            {
                if (mListViewCommand == null)
                {
                    mListViewCommand = new Command(() => this.switchDisplayView(DisplayView.List));
                }
                return mListViewCommand;
            }
        }

        private ICommand mGridViewCommand;
        public ICommand GridViewCommand
        {
            get
            {
                if (mGridViewCommand == null)
                {
                    mGridViewCommand = new Command(() => this.switchDisplayView(DisplayView.Grid));
                }
                return mGridViewCommand;
            }
        }

        private ICommand mRefreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (mRefreshCommand == null)
                {
                    mRefreshCommand = new Command(() => {
                        //if (Common.mCommon.UserSetting.TLSearchTypeAsk == "1")//1 for local search
                        //{

                        //}
                        //else
                        //{
                        //    this.getInvoice();
                        //}
                        mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE = new RES_SALE_INVOICE();
                        mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE.Sequence = "0";
                        this.getInvoice();
                    });
                }
                return mRefreshCommand;
            }
        }
        private ICommand mEditItemCommand;
        public ICommand EditItemCommand
        {
            get
            {
                if (mEditItemCommand == null)
                {
                    mEditItemCommand = new Command(() => this.switchDisplayView(DisplayView.Grid));
                    //mRefreshCommand = new Command(() => this.getInvoice());
                }
                return mEditItemCommand;
            }
        }
        private ICommand mDeleteItemCommand;
        public ICommand DeleteItemCommand
        {
            get
            {
                if (mDeleteItemCommand == null)
                {
                    //mRefreshCommand = new Command(() => this.getInvoice());
                }
                return mDeleteItemCommand;
            }
        }
        private ICommand mSelectItemCommand;
        public ICommand SelectItemCommand
        {
            get
            {
                if (mSelectItemCommand == null)
                {
                    //mRefreshCommand = new Command(() => this.getInvoice());
                }
                return mSelectItemCommand;
            }
        }
        private ICommand mSendItemCommand;
        public ICommand SendItemCommand
        {
            get
            {
                if (mSendItemCommand == null)
                {
                    mSendItemCommand = new Command<RES_SALE_INVOICE>(async (item) =>
                    {
                        if (Utility.checkButtonAccess("Send"))
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                                $"{item.InvoiceCode_0_50}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Send")}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                            if (answer)
                            {
                            }
                        }
                    });
                }
                return mSendItemCommand;
            }
        }

        private ICommand mActiveItemCommand;

        public ICommand ActiveItemCommand
        {
            get
            {               
                if (mActiveItemCommand == null)
                {
                    mActiveItemCommand = new Command<RES_SALE_INVOICE>(async (item) =>
                    {
                        if (item.StatusAsk == "8" && Utility.checkButtonAccess("Active"))
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                                $"{item.InvoiceCode_0_50}?",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Active")}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                            if (answer)
                            {
                                item.StatusAsk = "1";//1 for active
                                mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE = item;
                                await ExecuteActiveItem();
                            }
                        }
                        else if (item.StatusAsk != "8" && Utility.checkButtonAccess("Inactive"))
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                                $"{item.InvoiceCode_0_50}?",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Inactive")}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                            if (answer)
                            {
                               item.StatusAsk = "8";//8 for inactive
                               mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE = item;
                               await ExecuteActiveItem();
                            }
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
                        }
                    });
                }
                return mActiveItemCommand;
            }
        }
        public ICommand LongPressItemCommand { get; }

        private ICommand mCardItemTappedCommand;

        public ICommand CardItemTappedCommand
        {
            get
            {
                if (mCardItemTappedCommand == null)
                {
                    mCardItemTappedCommand = new Command(() =>
                    {
                       
                    });
                }
                return mCardItemTappedCommand;
            }
        }
        private ICommand mMoreSearchCommand;
        public ICommand MoreSearchCommand
        {
            get
            {
                if (mMoreSearchCommand == null)
                {
                    mMoreSearchCommand = new Command(() => this.selectMoreSearch());
                }
                return mMoreSearchCommand;
            }
        }
        #endregion

        #region "Method"
       
        private void switchDisplayView(DisplayView argDisplayView)
        {
            try
            {
                IsCardView = argDisplayView == DisplayView.Card;
                IsListView = argDisplayView == DisplayView.List;
                IsGridView = argDisplayView == DisplayView.Grid;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void bindDataTab(List<RES_SALE_INVOICE> argRES_SALE_INVOICE_LST)
        {
            try
            {
                if (argRES_SALE_INVOICE_LST != null && argRES_SALE_INVOICE_LST.Count > 0)
                {
                    RES_SALE_INVOICE = argRES_SALE_INVOICE_LST[0];
                    SalesInvoiceList = argRES_SALE_INVOICE_LST;
                }
                else
                {
                    SalesInvoiceList = new List<RES_SALE_INVOICE>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public void searchDataApi(string argKeyword)
        {
            try
            {
                mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE = new RES_SALE_INVOICE();
                mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE.Remark = argKeyword;
                getInvoice();
            }
            catch (Exception ex)
            {

                throw ex.InnerException;
            }
        }
        public void searchData(string argKeyword)
        {
            try
            {
                List<RES_SALE_INVOICE> l_RES_SALE_INVOICE_Lst = new List<RES_SALE_INVOICE>();
                if (argKeyword != null && !argKeyword.Equals(""))
                {
                    foreach (RES_SALE_INVOICE l_RES_SALE_INVOICE in mJSN_SALE_INVOICE_JUN.RES_SALE_INVOICE)
                    {
                        argKeyword = argKeyword.ToLower();
                        if (l_RES_SALE_INVOICE.InvoiceCode_0_50.ToLower().Contains(argKeyword)
                            || l_RES_SALE_INVOICE.InvoiceDate.ToLower().Contains(argKeyword)
                            || l_RES_SALE_INVOICE.OutstandingAmount.ToLower().Contains(argKeyword)
                            || l_RES_SALE_INVOICE.GrandTotal.ToLower().Contains(argKeyword))
                        {
                            l_RES_SALE_INVOICE_Lst.Add(l_RES_SALE_INVOICE);
                        }
                    }
                }
                else
                {
                    l_RES_SALE_INVOICE_Lst = mJSN_SALE_INVOICE_JUN.RES_SALE_INVOICE;// OriginalInvoiceClosedList.GetRange(0, OriginalInvoiceClosedList.Count);
                }
                bindDataTab(l_RES_SALE_INVOICE_Lst);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void formatUserSettingData(List<RES_SALE_INVOICE> argRES_SALE_INVOICE_LST)
        {
            try
            {
                if (argRES_SALE_INVOICE_LST != null && argRES_SALE_INVOICE_LST.Count > 0)
                {
                    foreach (RES_SALE_INVOICE l_RES_SALE_INVOICE in argRES_SALE_INVOICE_LST)
                    {
                        l_RES_SALE_INVOICE.InvoiceDate = Utility.getDateTimeString(l_RES_SALE_INVOICE.InvoiceDate).ToString();
                        l_RES_SALE_INVOICE.GrandTotal = Utility.getGrandTotalDecimal(l_RES_SALE_INVOICE.GrandTotal).ToString();
                        if(l_RES_SALE_INVOICE.StatusAsk == "1")
                        {
                            l_RES_SALE_INVOICE.StatusName_0_255 = "Inactive";
                        }
                        else
                        {
                            l_RES_SALE_INVOICE.StatusName_0_255 = "Active";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        private Task ExecuteActiveItem()
        {
            saveInvoice();
            return Task.CompletedTask;
        }
        private void selectMoreSearch()
        {
            try
            {
                loadInvoice();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        private async void callSearchMorePopup()
        {
            try
            {
                var popup = new FrmPosSaleInvoicePop(this.InvoiceLoad);
                await PopupNavigation.Instance.PushAsync(popup);

                var result = await popup.PopupClosedTask;
                if (result is RES_SALE_INVOICE selectedData)
                {
                    mJSN_REQ_SALE_INVOICE_JUN.RES_SALE_INVOICE = selectedData;
                    if(Common.mCommon.UserSetting.TLSearchTypeAsk == "1")//1 for local search
                    {
                        SalesInvoiceList = mRES_SALE_INVOICE_LST.Where(data =>(data.CustomerAsk == selectedData.CustomerAsk)
                                                                               || (data.InvoiceCode_0_50 == selectedData.InvoiceCode_0_50)).ToList();
                    }
                    else
                    {
                        getInvoice();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public void bindCustomer(List<RES_CUSTOMER_DTL> argRES_CUSTOMER_DTL_LST)
        {
            try
            {
                if (argRES_CUSTOMER_DTL_LST != null && argRES_CUSTOMER_DTL_LST.Count > 0)
                {
                    CustomerDtlList = argRES_CUSTOMER_DTL_LST;
                }
                else
                {
                    CustomerDtlList = new List<RES_CUSTOMER_DTL>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        #endregion

        #region "Web Service Api"
        public async void getInvoice()
        {
            try
            {
                Utility.openLoader();
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_SALE_INVOICE_JUN);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsgetSaleInvoiceJun);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_SALE_INVOICE_JUN = JsonConvert.DeserializeObject<JSN_SALE_INVOICE_JUN>(mResponse);
                    if (this.mJSN_SALE_INVOICE_JUN.Message.Code == "7")
                    {
                        if (this.mJSN_SALE_INVOICE_JUN.RES_SALE_INVOICE.Count > 0)
                        {
                            formatUserSettingData(this.mJSN_SALE_INVOICE_JUN.RES_SALE_INVOICE);
                            mRES_SALE_INVOICE_LST = this.mJSN_SALE_INVOICE_JUN.RES_SALE_INVOICE;
                            bindDataTab(this.mJSN_SALE_INVOICE_JUN.RES_SALE_INVOICE);
                            WeakReferenceMessenger.Default.Send(this.mJSN_SALE_INVOICE_JUN.Message.Message);
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(this.mJSN_SALE_INVOICE_JUN.Message.Message);
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_SALE_INVOICE_JUN.Message.Message);
                    }

                    Utility.closeLoader();
                }
                else
                {
                    Utility.closeLoader();
                    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("ErrWebService"));
                }
            }
            catch (Exception ex)
            {
                Utility.closeLoader();
                throw ex.InnerException;
            }
        }

        public async void saveInvoice()
        {
            try
            {
                mRequest = JsonConvert.SerializeObject(mRES_SALE_INVOICE);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsgetSaleInvoice);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_SALE_INVOICE_JUN = JsonConvert.DeserializeObject<JSN_SALE_INVOICE_JUN>(mResponse);
                    if (mJSN_SALE_INVOICE_JUN.Message.Code == "7")
                    {
                        if (this.mJSN_SALE_INVOICE_JUN.RES_SALE_INVOICE.Count > 0)
                        {
                            RES_SALE_INVOICE = this.mJSN_SALE_INVOICE_JUN.RES_SALE_INVOICE[0];
                            WeakReferenceMessenger.Default.Send(this.mJSN_SALE_INVOICE_JUN.Message.Message);
                            
                            //route parent list form after save
                            Common.routeMenu(Common.mCommon.SelectedMenu);
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(this.mJSN_SALE_INVOICE_JUN.Message.Message);
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_SALE_INVOICE_JUN.Message.Message);
                    }
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("ErrWebService"));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public async void loadInvoice()
        {
            try
            {
                Utility.openLoader();
                mRequest = JsonConvert.SerializeObject(Common.mCommon.REQ_AUTHORIZATION);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsLoadSaleInvoice);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_LOAD_SALE_INVOICE = JsonConvert.DeserializeObject<JSN_LOAD_SALE_INVOICE>(mResponse);
                    if (mJSN_LOAD_SALE_INVOICE.Message.Code == "7")
                    {
                        Utility.closeLoader();
                        this.InvoiceLoad = mJSN_LOAD_SALE_INVOICE;
                        callSearchMorePopup();
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_LOAD_SALE_INVOICE.Message.Message);
                    }
                }
                else
                {
                    Utility.closeLoader();
                    WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("ErrWebService"));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        private async Task LoadMoreItems()
        {
            if (IsLoadingMore) return;
            IsLoadingMore = true;

            getInvoice(); // Your data fetch


            IsLoadingMore = false;
        }
        #endregion
    }
}
