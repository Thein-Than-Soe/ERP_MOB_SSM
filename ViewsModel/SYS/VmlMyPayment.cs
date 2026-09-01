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
    public class VmlMyPayment : BaseViewModel
    {
        #region "Declaring"
        public JSN_REQ_SALE_PAYMENT_LST mJSN_REQ_SALE_PAYMENT_LST = new JSN_REQ_SALE_PAYMENT_LST();
        public JSN_SALE_PAYMENT_LST mJSN_SALE_PAYMENT_LST = new JSN_SALE_PAYMENT_LST();

        public ObservableCollection<SortingItem> sortingList { get; set; }
        SortingItem[] labelTexts = [
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.Setup.lbl.Code"), value = "PaymentCode_0_50", ShowIcon = true },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.Setup.lbl.Date"), value = "PaymentDate", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.CompanyPaymentType.lbl.PaymentType"), value = "PaymentTypeName_0_255", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.Setup.lbl.Status"), value = "StatusName_0_255", ShowIcon = false }
            ];
        string mRequest = "";
        string mResponse = "";
        #endregion

        #region "Contructor"
        public VmlMyPayment()
        {
            this.switchDisplayView(DisplayView.Card);
            OrderLoad = new JSN_LOAD_SALE_ORDER();
            SaleOrderLst = new List<RES_SALE_PAYMENT>();

            LoadMoreCommand = new Command(async () => await LoadMoreItems());
            sortingList = new ObservableCollection<SortingItem>(labelTexts);
            IsAscending = true;
            IsDescending = false;
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
        public RES_SALE_PAYMENT rES_SALE_BROWSE = new RES_SALE_PAYMENT();
        public RES_STATUS mRES_STATUS = new RES_STATUS();

        public RES_PARENT_TYPE RES_PARENT_TYPE
        {
            get { return mRES_PARENT_TYPE; }
            set { mRES_PARENT_TYPE = value; NotifyPropertyChanged("RES_PARENT_TYPE"); }
        }

        public RES_SALE_PAYMENT RES_SALE_PAYMENT
        {
            get { return rES_SALE_BROWSE; }
            set { rES_SALE_BROWSE = value; NotifyPropertyChanged("RES_SALE_PAYMENT"); }
        }

        public List<RES_SALE_PAYMENT> mSaleOrderLst;
        public List<RES_SALE_PAYMENT> SaleOrderLst
        {
            get { return mSaleOrderLst; }
            set { mSaleOrderLst = value; NotifyPropertyChanged("SaleOrderLst"); }
        }

        public List<RES_SALE_PAYMENT_DETAIL> mSaleOrderDetailLst;
        public List<RES_SALE_PAYMENT_DETAIL> SaleOrderDetailLst
        {
            get { return mSaleOrderDetailLst; }
            set { mSaleOrderDetailLst = value; NotifyPropertyChanged("SaleOrderDetailLst"); }
        }


        #endregion

        #region "Task"
        private async Task LoadMoreItems()
        {
            if (IsLoadingMore) return;
            IsLoadingMore = true;
            loadSalePayHis();
            IsLoadingMore = false;
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
                    mRefreshCommand = new Command(() => this.loadSalePayHis());
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
                    mEditItemCommand = new Command<RES_SALE_INVOICE>(async (item) =>
                    {
                        if (Utility.checkButtonAccess("Edit"))
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                               $"{item.InvoiceCode_0_50}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Send")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                            if (answer)
                            {
                                //route to detail page
                            }
                        }
                    });
                    //mEditItemCommand = new Command(() => this.switchDisplayView(DisplayView.Grid));
                    //mRefreshCommand = new Command(() => this.getInvoice());
                }
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

        public ICommand LongPressItemCommand { get; }

        private ICommand mCardItemTappedCommand;
        public ICommand CardItemTappedCommand
        {
            get
            {
                if (mCardItemTappedCommand == null)
                {
                    mCardItemTappedCommand = new Command<RES_SALE_INVOICE>(async (item) =>
                    {
                        bool answer = await Application.Current.MainPage.DisplayAlert(
                               $"{item.InvoiceCode_0_50}?",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Active")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                        if (answer)
                        {
                            //await Navigation.PushAsync(new FrmPosSaleInvoiceSet(item));

                        }
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
        private void bindDataTab(List<RES_SALE_PAYMENT> argRES_SALE_PAYMENT_LST)
        {
            try
            {
                if (argRES_SALE_PAYMENT_LST != null && argRES_SALE_PAYMENT_LST.Count > 0)
                {
                    SaleOrderLst = argRES_SALE_PAYMENT_LST;
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
                //mJSN_REQ_SALE_PAYMENT_LST.RES = new RES_SALE_ORDER();
                //mJSN_REQ_SALE_PAYMENT_LST.RES_SALE_ORDER.Remark = argKeyword;
                loadSalePayHis();
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
                //var popup = new FrmWishlistPop(this.SalesInvoiceLoad);
                //await PopupNavigation.Instance.PushAsync(popup);

                //var result = await popup.PopupClosedTask;
                //if (result is RES_SALE_ORDER selectedData)
                //{
                //    mJSN_REQ_SALE_PAYMENT_LST.RES_SALE_ORDER = selectedData;
                //    if (Common.mCommon.UserSetting.TLSearchTypeAsk == "1")//1 for local search
                //    {
                //        SaleOrderLst = new ObservableCollection<RES_SALE_ORDER>(mRES_SALE_ORDER_LST.Where(data => (data.CustomerAsk == selectedData.CustomerAsk)
                //                                                               || (data.OrderCode_0_50 == selectedData.OrderCode_0_50)).ToList());
                //    }
                //    else
                //    {
                //        getMyOrder();
                //    }
                //}
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
                List<RES_SALE_PAYMENT> l_RES_SALE_PAYMENT_lst = new List<RES_SALE_PAYMENT>();
                if (argKeyword != null && !argKeyword.Equals(""))
                {
                    foreach (RES_SALE_PAYMENT l_RES_SALE_PAYMENT in mJSN_SALE_PAYMENT_LST.RES_SALE_PAYMENT)
                    {
                        argKeyword = argKeyword.ToLower();
                        if (l_RES_SALE_PAYMENT.PaymentCode_0_50.ToLower().Contains(argKeyword)
                            || l_RES_SALE_PAYMENT.SD.ToLower().Contains(argKeyword)
                            || l_RES_SALE_PAYMENT.ED.ToLower().Contains(argKeyword)
                            )
                        {
                            l_RES_SALE_PAYMENT_lst.Add(l_RES_SALE_PAYMENT);
                        }
                    }
                }
                else
                {
                    l_RES_SALE_PAYMENT_lst = mJSN_SALE_PAYMENT_LST.RES_SALE_PAYMENT;// OriginalMyOrderClosedList.GetRange(0, OriginalMyOrderClosedList.Count);
                }
                bindDataTab(l_RES_SALE_PAYMENT_lst);
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
                loadSalePayHis();
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
        //                                                                      || (data.InvoiceCode_0_50 == selectedData.InvoiceCode_0_50)).ToList());
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
        public async void loadSalePayHis()
        {
            try
            {
                Utility.openLoader();
                mJSN_REQ_SALE_PAYMENT_LST.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_SALE_PAYMENT_LST.RES_SALE_PAYMENT.CompanyAsk = Common.mCommon.CompanyUserData.CompanyAsk;
                mJSN_REQ_SALE_PAYMENT_LST.RES_SALE_PAYMENT.SD = Utility.getTLFormLoadSD();
                mJSN_REQ_SALE_PAYMENT_LST.RES_SALE_PAYMENT.ED = Utility.getTLFormLoadED();
                
                mJSN_REQ_SALE_PAYMENT_LST.RES_SALE_PAYMENT_HEADER = new List<RES_SALE_PAYMENT_HEADER> { new RES_SALE_PAYMENT_HEADER() };

                mRequest = JsonConvert.SerializeObject(mJSN_REQ_SALE_PAYMENT_LST);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsloadSalePayHis);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_SALE_PAYMENT_LST = JsonConvert.DeserializeObject<JSN_SALE_PAYMENT_LST>(mResponse);
                    if (mJSN_SALE_PAYMENT_LST.Message.Code == "7")
                    {
                        if (this.mJSN_SALE_PAYMENT_LST.RES_SALE_PAYMENT.Count > 0)
                        {
                            SaleOrderLst = this.mJSN_SALE_PAYMENT_LST.RES_SALE_PAYMENT;
                            
                            //SaleOrderDetailLst = this.mJSN_SALE_PAYMENT_LST.RES_SALE_PAYMENT_DETAIL;
                            MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.LoadSuccess);
                        }
                        else
                        {
                            MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.NoData);
                        }
                    }
                    else
                    {
                        MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, this.mJSN_SALE_PAYMENT_LST.Message.Message);
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
            }
        }

        #endregion



    }
}
