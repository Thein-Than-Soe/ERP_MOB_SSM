using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.POS.REQ;
using CS.ERP.PL.POS.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.POS;
using CS.ERP_MOB.ViewsModel.Frame;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using static CS.ERP_MOB.General.Utility;

namespace CS.ERP_MOB.ViewsModel.POS
{
    public class VmlSalesQuotation : BaseViewModel
    {
        #region "Declaring"
        public JSN_REQ_SALE_QUOTATION_JUN mJSN_REQ_SALE_QUOTATION_JUN = new JSN_REQ_SALE_QUOTATION_JUN();
        public JSN_SALE_QUOTATION_JUN mJSN_SALE_QUOTATION_JUN = new JSN_SALE_QUOTATION_JUN();
        public JSN_LOAD_SALE_QUOTATION mJSN_LOAD_SALE_QUOTATION = new JSN_LOAD_SALE_QUOTATION();
        string mRequest = "";
        string mResponse = "";
        #endregion

        #region "Contructor"
        public VmlSalesQuotation()
        {
            this.switchDisplayView(DisplayView.Card);
            QuotationLoad = new JSN_LOAD_SALE_QUOTATION();
            QuotationActiveList = new List<RES_SALE_QUOTATION>();
            QuotationInActiveList = new List<RES_SALE_QUOTATION>();
            QuotationList = new List<RES_SALE_QUOTATION>();
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
        #endregion

        #region "Data Tab"
        public JSN_LOAD_SALE_QUOTATION JSN_LOAD_SALE_QUOTATION = new JSN_LOAD_SALE_QUOTATION();
        public JSN_LOAD_SALE_QUOTATION QuotationLoad
        {
            get { return JSN_LOAD_SALE_QUOTATION; }
            set { JSN_LOAD_SALE_QUOTATION = value; NotifyPropertyChanged("QuotationLoad"); }
        }

        public RES_SALE_QUOTATION mRES_SALE_QUOTATION = new RES_SALE_QUOTATION();
        public RES_SALE_QUOTATION_DETAIL mRES_SALE_QUOTATION_DETAIL = new RES_SALE_QUOTATION_DETAIL();
        public RES_SALE_BROWSE mRES_SALE_BROWSE = new RES_SALE_BROWSE();
        public RES_COMPANY rES_COMPANY = new RES_COMPANY();

        public RES_SALE_BROWSE RES_SALE_BROWSE
        {
            get { return mRES_SALE_BROWSE; }
            set { mRES_SALE_BROWSE = value; NotifyPropertyChanged("RES_SALE_BROWSE"); }
        }
        public RES_SALE_QUOTATION_DETAIL RES_SALE_QUOTATION_DETAIL
        {
            get { return mRES_SALE_QUOTATION_DETAIL; }
            set { mRES_SALE_QUOTATION_DETAIL = value; NotifyPropertyChanged("RES_SALE_QUOTATION_DETAIL"); }
        }
      
        public RES_SALE_QUOTATION RES_SALE_QUOTATION
        {
            get { return mRES_SALE_QUOTATION; }
            set { mRES_SALE_QUOTATION = value; NotifyPropertyChanged("RES_SALE_QUOTATION"); }
        }

        public RES_COMPANY RES_COMPANY
        {
            get { return rES_COMPANY; }
            set { rES_COMPANY = value; NotifyPropertyChanged("RES_SALE_QUOTATION"); }
        }


        public List<RES_SALE_QUOTATION> mQuotationList;
        public List<RES_SALE_QUOTATION> QuotationList
        {
            get { return mQuotationList; }
            set { mQuotationList = value; NotifyPropertyChanged("QuotationList"); }
        }

        public List<RES_SALE_QUOTATION> mQuotationActiveList;
        public List<RES_SALE_QUOTATION> QuotationActiveList
        {
            get { return mQuotationActiveList; }
            set { mQuotationActiveList = value; NotifyPropertyChanged("QuotationActiveList"); }
        }

        public List<RES_SALE_QUOTATION> mQuotationInActiveList;
        public List<RES_SALE_QUOTATION> QuotationInActiveList
        {
            get { return mQuotationInActiveList; }
            set { mQuotationInActiveList = value; NotifyPropertyChanged("QuotationInActiveList"); }
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
                    mRefreshCommand = new Command(() => this.getQuotation());
                }
                return mRefreshCommand;
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
        private void bindDataTab(List<RES_SALE_QUOTATION> argRES_SALE_QUOTATION_LST)
        {
            try
            {
                List<RES_SALE_QUOTATION> l_RES_SALE_QUOTATION_ACTIVE = new List<RES_SALE_QUOTATION>();
                List<RES_SALE_QUOTATION> l_RES_SALE_QUOTATION_InACTIVE = new List<RES_SALE_QUOTATION>();
                if (argRES_SALE_QUOTATION_LST != null && argRES_SALE_QUOTATION_LST.Count > 0)
                {

                    foreach (RES_SALE_QUOTATION l_RES_SALE_QUOTATION in argRES_SALE_QUOTATION_LST)
                    {
                        l_RES_SALE_QUOTATION.QuotationDate = Utility.getDateTimeString(l_RES_SALE_QUOTATION.QuotationDate);

                        if (l_RES_SALE_QUOTATION.StatusAsk.Equals("1"))
                        {
                            l_RES_SALE_QUOTATION_ACTIVE.Add(l_RES_SALE_QUOTATION);
                        }
                        else if (l_RES_SALE_QUOTATION.StatusAsk.Equals("8"))
                        {
                            l_RES_SALE_QUOTATION_InACTIVE.Add(l_RES_SALE_QUOTATION);
                        }
                    }

                    RES_SALE_QUOTATION = argRES_SALE_QUOTATION_LST[0];
                    QuotationList = argRES_SALE_QUOTATION_LST;
                    QuotationActiveList = l_RES_SALE_QUOTATION_ACTIVE;
                    QuotationInActiveList = l_RES_SALE_QUOTATION_InACTIVE;
                }
                else
                {
                    QuotationList = new List<RES_SALE_QUOTATION>();
                    QuotationActiveList = new List<RES_SALE_QUOTATION>();
                    QuotationInActiveList = new List<RES_SALE_QUOTATION>();
                }
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
                List<RES_SALE_QUOTATION> l_RES_SALE_QUOTATION_lst = new List<RES_SALE_QUOTATION>();
                if (argKeyword != null && !argKeyword.Equals(""))
                {
                    foreach (RES_SALE_QUOTATION l_RES_SALE_QUOTATION in mJSN_SALE_QUOTATION_JUN.RES_SALE_QUOTATION)
                    {
                        argKeyword = argKeyword.ToLower();
                        if (l_RES_SALE_QUOTATION.QuotationCode.ToLower().Contains(argKeyword)
                            || l_RES_SALE_QUOTATION.QuotationDate.ToLower().Contains(argKeyword)
                            || l_RES_SALE_QUOTATION.CustomerName.ToLower().Contains(argKeyword))
                        {
                            l_RES_SALE_QUOTATION_lst.Add(l_RES_SALE_QUOTATION);
                        }
                    }
                }
                else
                {
                    l_RES_SALE_QUOTATION_lst = mJSN_SALE_QUOTATION_JUN.RES_SALE_QUOTATION;// OriginalQuotationList.GetRange(0, OriginalQuotationList.Count);
                }
                bindDataTab(l_RES_SALE_QUOTATION_lst);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        #endregion

        #region "Web Service Api"
        public async void getQuotation()
        {
            try
            {
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_SALE_QUOTATION_JUN);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsgetSaleQuotation);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_SALE_QUOTATION_JUN = JsonConvert.DeserializeObject<JSN_SALE_QUOTATION_JUN>(mResponse);
                    if (mJSN_SALE_QUOTATION_JUN.Message.Code == "7")
                    {
                        if (this.mJSN_SALE_QUOTATION_JUN.RES_SALE_QUOTATION.Count > 0)
                        {
                            bindDataTab(this.mJSN_SALE_QUOTATION_JUN.RES_SALE_QUOTATION);
                            MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.LoadSuccess);
                        }
                        else
                        {
                            MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.NoData);
                        }
                    }
                    else
                    {
                        MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, this.mJSN_SALE_QUOTATION_JUN.Message.Message);
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
        }

        public async void saveQuotation()
        {
            try
            {
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_SALE_QUOTATION_JUN);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsgetSaleQuotation);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_SALE_QUOTATION_JUN = JsonConvert.DeserializeObject<JSN_SALE_QUOTATION_JUN>(mResponse);
                    if (mJSN_SALE_QUOTATION_JUN.Message.Code == "7")
                    {
                        if (this.mJSN_SALE_QUOTATION_JUN.RES_SALE_QUOTATION.Count > 0)
                        {
                            RES_SALE_QUOTATION = this.mJSN_SALE_QUOTATION_JUN.RES_SALE_QUOTATION[0];
                            MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.SaveSuccess);
                            //route parent list form after save
                            Common.routeMenu(Common.mCommon.SelectedMenu);
                        }
                        else
                        {
                            MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.NoData);
                        }
                    }
                    else
                    {
                        MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, this.mJSN_SALE_QUOTATION_JUN.Message.Message);
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
        }

        public async void loadQuotation()
        {
            try
            {
                mRequest = JsonConvert.SerializeObject(Common.mCommon.REQ_AUTHORIZATION);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsLoadSaleQuotation);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_LOAD_SALE_QUOTATION = JsonConvert.DeserializeObject<JSN_LOAD_SALE_QUOTATION>(mResponse);
                    if (mJSN_LOAD_SALE_QUOTATION.Message.Code == "7")
                    {
                        this.QuotationLoad = mJSN_LOAD_SALE_QUOTATION;

                        //if (this.mJSN_LOAD_APPLICANT.RES_SUPPLIER.Count > 0)
                        //{
                        //   this.JSN_LOAD_SUPPLIER= bindDataTab(this.mJSN_SUPPLIERNCONTACT.RES_SUPPLIER);
                        //    MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.LoadSuccess);
                        //}
                        //else
                        //{
                        //    MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.NoData);
                        //}
                    }
                    else
                    {
                        MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, this.mJSN_LOAD_SALE_QUOTATION.Message.Message);
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
        }

        #endregion

    }
}
