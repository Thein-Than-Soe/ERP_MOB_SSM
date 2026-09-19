using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.AMS.RES;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.POS.REQ;
using CS.ERP.PL.POS.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.POS;
using CS.ERP_MOB.Views.POS;
using CS.ERP_MOB.ViewsModel.Frame;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using RGPopup.Maui.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using static CS.ERP_MOB.General.Utility;

namespace CS.ERP_MOB.ViewsModel.POS
{
    public class VmlSalesEnquiry : BaseViewModel
    {
        #region "Declaring"
        string mRequest = "";
        string mResponse = "";

        public JSN_REQ_SALE_ENQUIRY mJSN_REQ_SALE_ENQUIRY = new JSN_REQ_SALE_ENQUIRY();
        public JSN_SALE_ENQUIRY mJSN_SALE_ENQUIRY = new JSN_SALE_ENQUIRY();
        public JSN_LOAD_SALE_ENQUIRY mJSN_LOAD_SALE_ENQUIRY = new JSN_LOAD_SALE_ENQUIRY();
        public List<RES_SALE_ENQUIRY> mRES_SALE_ENQUIRY_LST = new List<RES_SALE_ENQUIRY>();
        public ObservableCollection<RES_SALE_ENQUIRY> SalesEnquiryList { get; set; }
        public ObservableCollection<SortingItem> sortingList { get; set; }
        SortingItem[] labelTexts = [
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.SalesEnquiryJunOva.lbl.EnquiryDate"), value = "EnquiryDate", ShowIcon = true },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.SalesEnquiryJunOva.lbl.EnquiryNo"), value = "EnquiryCode_0_50", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.SalesEnquiryJunOva.lbl.Customer"), value = "CustomerName_0_255", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.SalesEnquiryJunOva.lbl.Status"), value = "StatusName_0_255", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("POS.SalesEnquiryJunOva.lbl.Price"), value = "GrandTotal", ShowIcon = false}
            ];

        #endregion

        #region "Contructor"
        public VmlSalesEnquiry()
        {
            this.switchDisplayView(DisplayView.Card);
            SalesEnquiryLoad = new JSN_LOAD_SALE_ENQUIRY();
            SalesEnquiryList = new ObservableCollection<RES_SALE_ENQUIRY>();
            LoadMoreCommand = new Command(async () => await LoadMoreItems());
            sortingList = new ObservableCollection<SortingItem>(labelTexts);
            IsAscending = true;
            IsDescending = false;
        }
        #endregion

        #region "Boolean Declaring"
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

        #region "Get Set"
        public JSN_LOAD_SALE_ENQUIRY JSN_LOAD_SALE_ENQUIRY = new JSN_LOAD_SALE_ENQUIRY();
        public JSN_LOAD_SALE_ENQUIRY SalesEnquiryLoad
        {
            get { return JSN_LOAD_SALE_ENQUIRY; }
            set { JSN_LOAD_SALE_ENQUIRY = value; NotifyPropertyChanged("SalesEnquiryLoad"); }
        }


        //public RES_SALE_BROWSE mRES_SALE_BROWSE = new RES_SALE_BROWSE();
        //public RES_SALE_BROWSE RES_SALE_BROWSE
        //{
        //    get { return mRES_SALE_BROWSE; }
        //    set { mRES_SALE_BROWSE = value; NotifyPropertyChanged("RES_SALE_BROWSE"); }
        //}

        //public RES_SALE_ENQUIRY mRES_SALE_ENQUIRY = new RES_SALE_ENQUIRY();
        //public RES_SALE_ENQUIRY RES_SALE_ENQUIRY
        //{
        //    get { return mRES_SALE_ENQUIRY; }
        //    set { mRES_SALE_ENQUIRY = value; NotifyPropertyChanged("RES_SALE_ENQUIRY"); }
        //}

        //public RES_SALE_ENQUIRY_DETAIL mRES_SALE_ENQUIRY_DETAIL = new RES_SALE_ENQUIRY_DETAIL();
        //public RES_SALE_ENQUIRY_DETAIL RES_SALE_ENQUIRY_DETAIL
        //{
        //    get { return mRES_SALE_ENQUIRY_DETAIL; }
        //    set { mRES_SALE_ENQUIRY_DETAIL = value; NotifyPropertyChanged("RES_SALE_ENQUIRY_DETAIL"); }
        //}

        //public RES_COMPANY mRES_COMPANY = new RES_COMPANY();
        //public RES_COMPANY RES_COMPANY
        //{
        //    get { return mRES_COMPANY; }
        //    set { mRES_COMPANY = value; NotifyPropertyChanged("RES_COMPANY"); }
        //}

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
                        //    this.getEnquiry();
                        //}
                        mJSN_REQ_SALE_ENQUIRY.RES_SALE_ENQUIRY = new RES_SALE_ENQUIRY();
                        mJSN_REQ_SALE_ENQUIRY.RES_SALE_ENQUIRY.Sequence = "0";
                        this.getEnquiry();
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
                    mEditItemCommand = new Command<RES_SALE_ENQUIRY>(async (item) =>
                    {
                        if (Utility.checkButtonAccess("Edit"))
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                               $"{item.EnquiryCode_0_50}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Send")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                            if (answer)
                            {
                            }
                        }
                    });
                    //mEditItemCommand = new Command(() => this.switchDisplayView(DisplayView.Grid));
                    //mRefreshCommand = new Command(() => this.getEnquiry());
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
                    mDeleteItemCommand = new Command<RES_SALE_ENQUIRY>(async (item) =>
                    {
                        if (Utility.checkButtonAccess("Delete") && item.PostingStatusAsk != "1" && item.StatusAsk != "9")
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                               $"{item.EnquiryCode_0_50}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Delete")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                            if (answer)
                            {
                                mJSN_REQ_SALE_ENQUIRY.RES_SALE_ENQUIRY = item;
                                mJSN_REQ_SALE_ENQUIRY.RES_SALE_ENQUIRY.StatusAsk = "6";
                                saveEnquiry();
                            }
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgDelete"));
                        }
                    });
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
                    //mRefreshCommand = new Command(() => this.getEnquiry());
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
                    mSendItemCommand = new Command<RES_SALE_ENQUIRY>(async (item) =>
                    {
                        if (Utility.checkButtonAccess("Send"))
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                                $"{item.EnquiryCode_0_50}",
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
                    mActiveItemCommand = new Command<RES_SALE_ENQUIRY>(async (item) =>
                    {
                        if (item.StatusAsk == "8" && Utility.checkButtonAccess("Active"))
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                                $"{item.EnquiryCode_0_50}?",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Active")}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                            if (answer)
                            {
                                item.StatusAsk = "1";//1 for active
                                mJSN_REQ_SALE_ENQUIRY.RES_SALE_ENQUIRY = item;
                                await ExecuteActiveItem();
                            }
                        }
                        else if (item.StatusAsk != "8" && Utility.checkButtonAccess("Inactive"))
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                                $"{item.EnquiryCode_0_50}?",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Inactive")}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                                $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                            if (answer)
                            {
                                item.StatusAsk = "8";//8 for inactive
                                mJSN_REQ_SALE_ENQUIRY.RES_SALE_ENQUIRY = item;
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
                    mCardItemTappedCommand = new Command<RES_SALE_ENQUIRY>(async (item) =>
                    {
                        bool answer = await Application.Current.MainPage.DisplayAlert(
                               $"{item.EnquiryCode_0_50}?",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Active")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                        if (answer)
                        {
                            //await Navigation.PushAsync(new FrmPosSaleEnquirySet(item));

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

        #region "Task"
        private async Task LoadMoreItems()
        {
            if (IsLoadingMore) return;
            IsLoadingMore = true;
            getEnquiry();
            IsLoadingMore = false;
        }
        private Task ExecuteActiveItem()
        {
            saveEnquiry();
            return Task.CompletedTask;
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

                var tmp = SalesEnquiryList;
                SalesEnquiryList = null;
                NotifyPropertyChanged(nameof(SalesEnquiryList));

                SalesEnquiryList = tmp;
                NotifyPropertyChanged(nameof(SalesEnquiryList));
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void bindDataTab(List<RES_SALE_ENQUIRY> argRES_SALE_ENQUIRY_LST)
        {
            try
            {
                if (argRES_SALE_ENQUIRY_LST != null && argRES_SALE_ENQUIRY_LST.Count > 0)
                {
                    foreach (RES_SALE_ENQUIRY l_RES_SALE_ENQUIRY in argRES_SALE_ENQUIRY_LST)
                    {
                        SalesEnquiryList.Add(l_RES_SALE_ENQUIRY);
                    }
                }
                else
                {
                    SalesEnquiryList = new ObservableCollection<RES_SALE_ENQUIRY>();
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
                mJSN_REQ_SALE_ENQUIRY.RES_SALE_ENQUIRY = new RES_SALE_ENQUIRY();
                mJSN_REQ_SALE_ENQUIRY.RES_SALE_ENQUIRY.Remark = argKeyword;
                getEnquiry();
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
                List<RES_SALE_ENQUIRY> l_RES_SALE_ENQUIRY_Lst = new List<RES_SALE_ENQUIRY>();
                if (argKeyword != null && !argKeyword.Equals(""))
                {
                    foreach (RES_SALE_ENQUIRY l_RES_SALE_ENQUIRY in mJSN_SALE_ENQUIRY.RES_SALE_ENQUIRY)
                    {
                        argKeyword = argKeyword.ToLower();
                        if (l_RES_SALE_ENQUIRY.EnquiryCode_0_50.ToLower().Contains(argKeyword)
                            || l_RES_SALE_ENQUIRY.EnquiryDate.ToLower().Contains(argKeyword)
                            || l_RES_SALE_ENQUIRY.OutstandingAmount.ToLower().Contains(argKeyword)
                            || l_RES_SALE_ENQUIRY.GrandTotal.ToLower().Contains(argKeyword))
                        {
                            l_RES_SALE_ENQUIRY_Lst.Add(l_RES_SALE_ENQUIRY);
                        }
                    }
                }
                else
                {
                    l_RES_SALE_ENQUIRY_Lst = new List<RES_SALE_ENQUIRY>(mJSN_SALE_ENQUIRY.RES_SALE_ENQUIRY);// OriginalEnquiryClosedList.GetRange(0, OriginalEnquiryClosedList.Count);
                }
                bindDataTab(l_RES_SALE_ENQUIRY_Lst);
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
                loadEnquiry();
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
                //var popup = new FrmPosSaleEnquiryPop(this.SalesEnquiryLoad);
                //await PopupNavigation.Instance.PushAsync(popup);

                //var result = await popup.PopupClosedTask;
                //if (result is RES_SALE_ENQUIRY selectedData)
                //{
                //    mJSN_REQ_SALE_ENQUIRY.RES_SALE_ENQUIRY = selectedData;
                //    if (Common.mCommon.UserSetting.TLSearchTypeAsk == "1")//1 for local search
                //    {
                //        SalesEnquiryList = new ObservableCollection<RES_SALE_ENQUIRY>(mRES_SALE_ENQUIRY_LST.Where(data => (data.CustomerAsk == selectedData.CustomerAsk)
                //                                                               || (data.EnquiryCode_0_50 == selectedData.EnquiryCode_0_50)).ToList());
                //    }
                //    else
                //    {
                //        getEnquiry();
                //    }
                //}
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
        public async void getEnquiry()
        {
            try
            {
                Utility.openLoader();
                mJSN_REQ_SALE_ENQUIRY.RES_SALE_ENQUIRY.CompanyAsk = Common.mCommon.CompanyUserData.CompanyAsk;

                mRequest = JsonConvert.SerializeObject(mJSN_REQ_SALE_ENQUIRY);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsgetSaleEnquiry);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_SALE_ENQUIRY = JsonConvert.DeserializeObject<JSN_SALE_ENQUIRY>(mResponse);
                    if (mJSN_SALE_ENQUIRY.Message.Code == "7")
                    {
                        if (this.mJSN_SALE_ENQUIRY.RES_SALE_ENQUIRY.Count > 0)
                        {
                            mRES_SALE_ENQUIRY_LST = this.mJSN_SALE_ENQUIRY.RES_SALE_ENQUIRY;
                            bindDataTab(this.mJSN_SALE_ENQUIRY.RES_SALE_ENQUIRY);
                            WeakReferenceMessenger.Default.Send(this.mJSN_SALE_ENQUIRY.Message.Message);
                        }
                        else
                        {
                             WeakReferenceMessenger.Default.Send(this.mJSN_SALE_ENQUIRY.Message.Message);
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_SALE_ENQUIRY.Message.Message);
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
            finally
            {
                Utility.closeLoader();
            }
        }

        public async void saveEnquiry()
        {
            try
            {
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_SALE_ENQUIRY);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsgetSaleEnquiry);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_SALE_ENQUIRY = JsonConvert.DeserializeObject<JSN_SALE_ENQUIRY>(mResponse);
                    if (mJSN_SALE_ENQUIRY.Message.Code == "7")
                    {
                        if (this.mJSN_SALE_ENQUIRY.RES_SALE_ENQUIRY.Count > 0)
                        {
                            mJSN_REQ_SALE_ENQUIRY.RES_SALE_ENQUIRY = this.mJSN_SALE_ENQUIRY.RES_SALE_ENQUIRY[0];
                            getEnquiry();

                            MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.SaveSuccess);
                            //route parent list form after save
                            Common.routeMenu(Common.mCommon.SelectedMenu);
                        }
                        else
                        {
                             WeakReferenceMessenger.Default.Send(this.mJSN_SALE_ENQUIRY.Message.Message);
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_SALE_ENQUIRY.Message.Message);
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

        public async void loadEnquiry()
        {
            try
            {
                Utility.openLoader();
                mRequest = JsonConvert.SerializeObject(Common.mCommon.REQ_AUTHORIZATION);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsLoadSaleEnquiry);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_LOAD_SALE_ENQUIRY = JsonConvert.DeserializeObject<JSN_LOAD_SALE_ENQUIRY>(mResponse);
                    if (mJSN_LOAD_SALE_ENQUIRY.Message.Code == "7")
                    {
                        this.SalesEnquiryLoad = mJSN_LOAD_SALE_ENQUIRY;
                        callSearchMorePopup();
                    }
                    else
                    {
                        MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, this.mJSN_LOAD_SALE_ENQUIRY.Message.Message);
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
            finally
            {
                Utility.closeLoader();
            }
        }


        #endregion
    }
}

