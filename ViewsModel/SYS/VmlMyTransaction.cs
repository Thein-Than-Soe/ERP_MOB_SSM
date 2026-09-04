using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HMS.DAT;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.POS.REQ;
using CS.ERP.PL.POS.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.POS;
using CS.ERP_MOB.ViewsModel.Frame;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using RGPopup.Maui.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using static CS.ERP_MOB.General.Utility;

namespace CS.ERP_MOB.ViewsModel.SYS
{
    public class VmlMyTransaction : BaseViewModel
    {
        #region "Declaring"
        public JSN_REQ_SALE_LOAD mJSN_REQ_SALE_LOAD = new JSN_REQ_SALE_LOAD();
        public JSN_RES_MY_TRANSACTION mJSN_RES_MY_TRANSACTION = new JSN_RES_MY_TRANSACTION();

        public ObservableCollection<SortingItem> sortingList { get; set; }
        SortingItem[] labelTexts = [
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.Setup.lbl.Code"), value = "Code_0_50", ShowIcon = true },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.Setup.lbl.Date"), value = "Date", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.CompanyPaymentType.lbl.PaymentType"), value = "PaymentTypeName_0_255", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.Setup.lbl.Status"), value = "StatusName_0_255", ShowIcon = false }
            ];
        string mRequest = "";
        string mResponse = "";
        #endregion

        #region "Contructor"
        public VmlMyTransaction()
        {
            this.switchDisplayView(DisplayView.Card);
            OrderLoad = new JSN_LOAD_SALE_ORDER();
            SaleOrderLst = new List<RES_SALE_BROWSE>();

            LoadMoreCommand = new Command(async () => await LoadMoreItems());
            sortingList = new ObservableCollection<SortingItem>(labelTexts);
            IsAscending = true;
            IsDescending = false;
        }
        #endregion

        #region "Display View"
        private bool mIsLoadingTransactionHistory;
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
            }
        }
        private bool mIsAscending;
        public bool IsAscending
        {
            get
            {
                return mIsAscending;
            }
            set
            {
                mIsAscending = value;
                NotifyPropertyChanged("IsAscending");
            }
        }
        private bool mIsDescending;
        public bool IsDescending
        {
            get
            {
                return mIsDescending;
            }
            set
            {
                mIsDescending = value;
                NotifyPropertyChanged("IsDescending");
            }
        }

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

        #region "Data Tab"
        public JSN_LOAD_SALE_ORDER JSN_LOAD_SALE_ORDER = new JSN_LOAD_SALE_ORDER();
        public JSN_LOAD_SALE_ORDER OrderLoad
        {
            get { return JSN_LOAD_SALE_ORDER; }
            set { JSN_LOAD_SALE_ORDER = value; NotifyPropertyChanged("OrderLoad"); }
        }

        public RES_PARENT_TYPE mRES_PARENT_TYPE = new RES_PARENT_TYPE();
        public RES_SALE_BROWSE rES_SALE_BROWSE = new RES_SALE_BROWSE();
        public RES_STATUS mRES_STATUS = new RES_STATUS();

        public RES_PARENT_TYPE RES_PARENT_TYPE
        {
            get { return mRES_PARENT_TYPE; }
            set { mRES_PARENT_TYPE = value; NotifyPropertyChanged("RES_PARENT_TYPE"); }
        }

        public RES_SALE_BROWSE RES_SALE_BROWSE
        {
            get { return rES_SALE_BROWSE; }
            set { rES_SALE_BROWSE = value; NotifyPropertyChanged("RES_SALE_BROWSE"); }
        }

        public List<RES_SALE_BROWSE> mSaleOrderLst;
        public List<RES_SALE_BROWSE> SaleOrderLst
        {
            get { return mSaleOrderLst; }
            set { mSaleOrderLst = value; NotifyPropertyChanged("SaleOrderLst"); }
        }

        public List<RES_SALE_BROWSE_DETAIL> mSaleOrderDetailLst;
        public List<RES_SALE_BROWSE_DETAIL> SaleOrderDetailLst
        {
            get { return mSaleOrderDetailLst; }
            set { mSaleOrderDetailLst = value; NotifyPropertyChanged("SaleOrderDetailLst"); }
        }


        #endregion

        #region "Task"
        private async Task LoadMoreItems()
        {
            if (IsLoadingMore)
                return;

            try
            {
                IsLoadingMore = true;

                await getMyTransactionHistory();
            }
            finally
            {
                IsLoadingMore = false;
            }
        }
        //private Task ExecuteActiveItem()
        //{
        //    saveMyOrder();
        //    return Task.CompletedTask;
        //}

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
                    mRefreshCommand = new Command(async () =>
                    {
                        try
                        {
                            IsRefreshing = true;

                            await getMyTransactionHistory();
                        }
                        finally
                        {
                            IsRefreshing = false;
                        }
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
                //if (mEditItemCommand == null)
                //{
                //    mEditItemCommand = new Command<RES_SALE_INVOICE>(async (item) =>
                //    {
                //        if (Utility.checkButtonAccess("Edit"))
                //        {
                //            bool answer = await Application.Current.MainPage.DisplayAlert(
                //               $"{item.InvoiceTransactionCode_0_50}",
                //               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Send")}",
                //               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                //               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                //            if (answer)
                //            {
                //                //route to detail page
                //            }
                //        }
                //    });
                //    //mEditItemCommand = new Command(() => this.switchDisplayView(DisplayView.Grid));
                //    //mRefreshCommand = new Command(() => this.getInvoice());
                //}
                return mEditItemCommand;
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
        public ICommand LongPressItemCommand { get; }

        private ICommand mCardItemTappedCommand;
        public ICommand CardItemTappedCommand
        {
            get
            {
                //if (mCardItemTappedCommand == null)
                //{
                //    mCardItemTappedCommand = new Command<RES_SALE_INVOICE>(async (item) =>
                //    {
                //        bool answer = await Application.Current.MainPage.DisplayAlert(
                //               $"{item.InvoiceTransactionCode_0_50}?",
                //               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Active")}",
                //               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                //               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                //        if (answer)
                //        {
                //            //await Navigation.PushAsync(new FrmPosSaleInvoiceSet(item));

                //        }
                //    });
                //}
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
        public ICommand LoadMoreCommand { get; }
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
        private void bindDataTab(List<RES_SALE_BROWSE> argRES_SALE_BROWSE_LST)
        {
            try
            {
                if (argRES_SALE_BROWSE_LST != null && argRES_SALE_BROWSE_LST.Count > 0)
                {
                    SaleOrderLst = argRES_SALE_BROWSE_LST;
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
                //mJSN_REQ_SALE_LOAD.RES = new RES_SALE_ORDER();
                //mJSN_REQ_SALE_LOAD.RES_SALE_ORDER.Remark = argKeyword;
                getMyTransactionHistory();
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
                List<RES_SALE_BROWSE> l_RES_SALE_BROWSE_lst = new List<RES_SALE_BROWSE>();
                if (argKeyword != null && !argKeyword.Equals(""))
                {
                    foreach (RES_SALE_BROWSE l_RES_SALE_BROWSE in mJSN_RES_MY_TRANSACTION.RES_SALE_BROWSE)
                    {
                        argKeyword = argKeyword.ToLower();
                        if (l_RES_SALE_BROWSE.Code_0_50.ToLower().Contains(argKeyword)
                            || l_RES_SALE_BROWSE.SD.ToLower().Contains(argKeyword)
                            || l_RES_SALE_BROWSE.ED.ToLower().Contains(argKeyword)
                            )
                        {
                            l_RES_SALE_BROWSE_lst.Add(l_RES_SALE_BROWSE);
                        }
                    }
                }
                else
                {
                    l_RES_SALE_BROWSE_lst = mJSN_RES_MY_TRANSACTION.RES_SALE_BROWSE;// OriginalMyOrderClosedList.GetRange(0, OriginalMyOrderClosedList.Count);
                }
                bindDataTab(l_RES_SALE_BROWSE_lst);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void selectMoreSearch()
        {
            try
            {
                getMyTransactionHistory();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        //private async void callSearchMorePopup()
        //{
        //    try
        //    {
        //        var popup = new FrmSsm(this.mJSN_RES_FRONT_DESK_USER);
        //        await PopupNavigation.Instance.PushAsync(popup);

        //        var result = await popup.PopupClosedTask;
        //        if (result is DAT_FRONT_DESK selectedData)
        //        {
        //            mJSN_REQ_FRONT_DESK.DAT_FRONT_DESK = selectedData;
        //            if (Common.mCommon.UserSetting.TLSearchTypeAsk == "1")//1 for local search
        //            {
        //                FrontDeskList = new ObservableCollection<DAT_FRONT_DESK>(mDAT_FRONT_DESK_LST.Where(data => (data.CustomerAsk == selectedData.CustomerAsk)
        //                                                                      || (data.InvoiceTransactionCode_0_50 == selectedData.InvoiceTransactionCode_0_50)).ToList());
        //            }
        //            else
        //            {
        //                await getFrontDeskUser();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex.InnerException;
        //    }

        //}
        #endregion

        #region "Web Service Api"
        public async Task getMyTransactionHistory()
        {
            if (mIsLoadingTransactionHistory)
                return;
            try
            {
                Utility.openLoader();
                mJSN_REQ_SALE_LOAD.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_SALE_LOAD.RES_SALE_BROWSE.TranTypeAsk = "3";
                mJSN_REQ_SALE_LOAD.RES_SALE_BROWSE.CompanyAsk = Common.mCommon.CompanyUserData.CompanyAsk;
                mJSN_REQ_SALE_LOAD.RES_SALE_BROWSE.SD = Utility.getTLFormLoadSD();
                mJSN_REQ_SALE_LOAD.RES_SALE_BROWSE.ED = Utility.getTLFormLoadED();

                mJSN_REQ_SALE_LOAD.RES_SALE_BROWSE_DETAIL = new List<RES_SALE_BROWSE_DETAIL> { new RES_SALE_BROWSE_DETAIL() };

                mRequest = JsonConvert.SerializeObject(mJSN_REQ_SALE_LOAD);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsmyTransaction);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_RES_MY_TRANSACTION = JsonConvert.DeserializeObject<JSN_RES_MY_TRANSACTION>(mResponse);
                    if (mJSN_RES_MY_TRANSACTION.Message.Code == "7")
                    {
                        if (this.mJSN_RES_MY_TRANSACTION.RES_SALE_BROWSE.Count > 0)
                        {
                            SaleOrderLst = this.mJSN_RES_MY_TRANSACTION.RES_SALE_BROWSE;
                            SaleOrderDetailLst = this.mJSN_RES_MY_TRANSACTION.RES_SALE_BROWSE_DETAIL;
                            MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.LoadSuccess);
                        }
                        else
                        {
                            MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.NoData);
                        }
                    }
                    else
                    {
                        MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, this.mJSN_RES_MY_TRANSACTION.Message.Message);
                    }
                }
                else
                {
                    MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.WebServiceErr);
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            finally
            {
                Utility.closeLoader();
                mIsLoadingTransactionHistory = false;
            }
        }

        #endregion



    }
}