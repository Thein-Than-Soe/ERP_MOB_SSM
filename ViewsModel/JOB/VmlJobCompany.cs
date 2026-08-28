using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.HCM.REQ;
using CS.ERP.PL.HCM.RES;
using CS.ERP.PL.JOB.DAT;
using CS.ERP.PL.JOB.REQ;
using CS.ERP.PL.JOB.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP.PL.SYS.REQ;
using CS.ERP.PL.SYS.RES;
using CS.ERP_MOB.Extensions;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.JOB;
using CS.ERP_MOB.Services.SYS;
using CS.ERP_MOB.Views.Frame;
using CS.ERP_MOB.Views.JOB;
using CS.ERP_MOB.ViewsModel.Frame;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using RGPopup.Maui.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using static CS.ERP_MOB.General.Utility;

namespace CS.ERP_MOB.ViewsModel.JOB
{
    public class VmlJobCompany : BaseViewModel
    {
        #region "Declaring"
        public JSN_REQ_COMPANY mJSN_REQ_COMPANY = new JSN_REQ_COMPANY();
        public JSN_REQ_APPLICANT_COMPANY_JUN mJSN_REQ_APPLICANT_COMPANY_JUN = new JSN_REQ_APPLICANT_COMPANY_JUN();
        public JSN_RES_APPLICANT_COMPANY_JUN mJSN_RES_APPLICANT_COMPANY_JUN = new JSN_RES_APPLICANT_COMPANY_JUN();
        public JSN_RES_COMPANY mJSN_RES_COMPANY = new JSN_RES_COMPANY();
        public JSN_RES_COMPANY_DETAIL mJSN_RES_COMPANY_DETAIL = new JSN_RES_COMPANY_DETAIL();
        public JSN_RES_LOAD_COMPANY mJSN_RES_LOAD_COMPANY = new JSN_RES_LOAD_COMPANY();
        public DAT_APPLICANT mDAT_APPLICANT = new DAT_APPLICANT();
        string mRequest = "";
        string mResponse = "";
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
        public ObservableCollection<SortingItem> sortingList { get; set; }
        SortingItem[] labelTexts = [
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobCompany.lbl.CompanyName"), value = "CompanyName_0_255", ShowIcon = true },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobCompany.lbl.Date"), value = "CompanyRegSDate", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobCompany.lbl.CompanyType"), value = "CompanyTypeName_0_255", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobCompany.lbl.City"), value = "CityName_0_255", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobCompany.lbl.Country"), value = "CountryName_0_255", ShowIcon = false }
            ];

        #endregion

        #region "Contructor"
        public VmlJobCompany()
        {
            this.switchDisplayView(DisplayView.Card);
            CompanyLoad = new JSN_RES_LOAD_COMPANY();
            //JobCompanyList = new List<RES_COMPANY>();
            JobCompanyList = new ObservableCollection<RES_COMPANY>();
            LoadMoreCommand = new Command(async () => await LoadMoreItems());
            sortingList = new ObservableCollection<SortingItem>(labelTexts);
            IsAscending = true;
            IsDescending = false;
        }
        #endregion

        #region "Display View"

        // ✅ Bind this to Button Text
        public string SaveButtonText =>
                      (SelectedCompany?.IsSaved == "1" || SelectedCompany?.IsSaved == "Gold")
                      ? "Unsave"
                      : "Save";

        public Color SaveButtonColor =>
                     (SelectedCompany?.IsSaved == "1" || SelectedCompany?.IsSaved == "Gold")
                     ? Colors.OrangeRed
                     : Colors.Green;

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
        private Color _isSaved = Colors.Gray;
        public Color IsSaved
        {
            get => _isSaved;
            set
            {
                _isSaved = value;
                NotifyPropertyChanged("IsSaved"); // this triggers UI update
            }
        }

        #endregion

        #region "Data Tab"
        public JSN_RES_LOAD_COMPANY JSN_RES_LOAD_COMPANY = new JSN_RES_LOAD_COMPANY();
        public JSN_RES_LOAD_COMPANY CompanyLoad
        {
            get { return JSN_RES_LOAD_COMPANY; }
            set { JSN_RES_LOAD_COMPANY = value; NotifyPropertyChanged("CompanyLoad"); }
        }

        public RES_COMPANY mRES_COMPANY = new RES_COMPANY();
        public List<RES_COMPANY> mRES_COMPANY_LST = new List<RES_COMPANY>();

        public RES_COMPANY RES_COMPANY
        {
            get { return mRES_COMPANY; }
            set { mRES_COMPANY = value; NotifyPropertyChanged("RES_COMPANY"); }
        }
        //public List<RES_COMPANY> mJobCompanyList;
        //public List<RES_COMPANY> JobCompanyList
        //{
        //    get { return mJobCompanyList; }
        //    set { mJobCompanyList = value; NotifyPropertyChanged("JobCompanyList"); }
        //}
        public ObservableCollection<RES_COMPANY> JobCompanyList { get; set; }




        // For vacancy list in company detail
        public DAT_JOB_SEARCH mDAT_JOB_SEARCH = new DAT_JOB_SEARCH();
        public List<DAT_JOB_SEARCH> mDAT_JOB_SEARCH_LST = new List<DAT_JOB_SEARCH>();

        public DAT_JOB_SEARCH DAT_JOB_SEARCH
        {
            get { return mDAT_JOB_SEARCH; }
            set { mDAT_JOB_SEARCH = value; NotifyPropertyChanged("DAT_JOB_SEARCH"); }
        }

        public List<DAT_JOB_SEARCH> mJobVacancyList;
        public List<DAT_JOB_SEARCH> JobVacancyList
        {
            get { return mJobVacancyList; }
            set { mJobVacancyList = value; NotifyPropertyChanged("JobVacancyList"); }
        }

        
        public RES_COMPANY mSelectedCompany;
        public RES_COMPANY SelectedCompany
        {
            get { return mSelectedCompany; }
            set { mSelectedCompany = value; NotifyPropertyChanged("SelectedCompany"); }
        }


        // Pickers in Searchmore Popup
        public List<RES_COMPANY_TYPE> mJobCompanyTypeList;
        public List<RES_COMPANY_TYPE> JobCompanyTypeList
        {
            get { return mJobCompanyTypeList; }
            set { mJobCompanyTypeList = value; NotifyPropertyChanged("JobCompanyTypeList"); }
        }
        public List<RES_CITY> mJobCompanyLocationCity;
        public List<RES_CITY> JobCompanyLocationCity
        {
            get { return mJobCompanyLocationCity; }
            set { mJobCompanyLocationCity = value; NotifyPropertyChanged("JobCompanyLocationCity"); }
        }
        public List<RES_COUNTRY> mJobCompanyLocationCountry;
        public List<RES_COUNTRY> JobCompanyLocationCountry
        {
            get { return mJobCompanyLocationCountry; }
            set { mJobCompanyLocationCountry = value; NotifyPropertyChanged("JobCompanyLocationCountry"); }
        }

        // End of Pickers in Searchmore Popup

        
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
                    mRefreshCommand = new Command(() =>
                    {
                        // Correcting the assignment to match the expected type  
                        mJSN_REQ_COMPANY.RES_COMPANY = new List<RES_COMPANY> { new RES_COMPANY() };
                        this.getJobCompany();
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
                    //mRefreshCommand = new Command(() => this.getJobCompany());
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
                    //mRefreshCommand = new Command(() => this.getJobCompany());
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
                    //mRefreshCommand = new Command(() => this.getJobCompany());
                }
                return mSelectItemCommand;
            }
        }

        private ICommand mSaveItemCommand;

        public ICommand SaveItemCommand
        {
            get
            {
                if (mSaveItemCommand == null)
                {
                    mSaveItemCommand = new Command<RES_COMPANY>(async (item) =>
                    {
                        DAT_APPLICANT_COMPANY_JUN l_item = new DAT_APPLICANT_COMPANY_JUN();
                        //if (item.StatusAsk == "8" && Utility.checkButtonAccess("Active"))
                        if (item.IsSaved == "Gold" || item.IsSaved == "1")
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                                $"{item.CompanyName_0_255}?",
                                $"{Common.mCommon.GetLanguageValueByKey("JOB.Common.confirm.SaveCancel")}",
                                $"{Common.mCommon.GetLanguageValueByKey("JOB.Common.btnName.Yes")}",
                                $"{Common.mCommon.GetLanguageValueByKey("JOB.Common.btnName.No")}");

                            if (answer)
                            {
                                l_item.Ask = "0";
                                l_item.CompanyAsk = item.Ask;
                                l_item.ApplicantAsk = mDAT_APPLICANT.Ask;
                                l_item.StatusAsk = "6"; // 6 for unsave and delete record 
                                this.saveJobCompany(l_item);
                            }
                        }
                        //else if (item.StatusAsk != "8" && Utility.checkButtonAccess("Inactive"))
                        else if (item.IsSaved == "Gray" || item.IsSaved == "0")
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                                $"{item.CompanyName_0_255}?",
                                $"{Common.mCommon.GetLanguageValueByKey("JOB.Common.confirm.Save")}",
                                $"{Common.mCommon.GetLanguageValueByKey("JOB.Common.btnName.Yes")}",
                                $"{Common.mCommon.GetLanguageValueByKey("JOB.Common.btnName.No")}");

                            if (answer)
                            {
                                l_item.Ask = "0"; // 8 for inactive  
                                l_item.CompanyAsk = item.Ask;
                                l_item.ApplicantAsk = mDAT_APPLICANT.Ask;
                                l_item.StatusAsk = "1"; // 1 for save record
                                this.saveJobCompany(l_item);
                            }
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
                        }
                    });
                }
                return mSaveItemCommand;
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
                    mCardItemTappedCommand = new Command<RES_COMPANY>(async (companyDtl) =>
                    {
                        //await Shell.Current.GoToAsync(nameof(FrmJobCompanyDtl), true, new Dictionary<string, object>
                        //{
                        //    ["companyDtl"] = companyDtl
                        //});
                        await Application.Current.MainPage.Navigation.PushAsync(new FrmJobCompanyDtl(companyDtl));
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


        //Command for url
        private ICommand mOpenWebsiteCommand;
        public ICommand OpenWebsiteCommand
                => new Command<string>(async (url) =>
                   {
                       if (!string.IsNullOrWhiteSpace(url))
                       {
                           try
                           {
                               await Launcher.Default.OpenAsync(new Uri(url));
                           }
                           catch (Exception ex)
                           {
                               // Optional: handle or log invalid URL exceptions
                           }
                       }
                   });
       
        #endregion

        #region "Method"

        private void switchDisplayView(DisplayView argDisplayView)
        {
            try
            {
                IsCardView = argDisplayView == DisplayView.Card;
                IsListView = argDisplayView == DisplayView.List;
                IsGridView = argDisplayView == DisplayView.Grid;

                var tmp = JobCompanyList;
                JobCompanyList = null;
                NotifyPropertyChanged(nameof(JobCompanyList));

                JobCompanyList = tmp;
                NotifyPropertyChanged(nameof(JobCompanyList));
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        //Data bind for company detail
        private void bindVacancy(List<DAT_JOB_SEARCH> argDAT_JOB_SEARCH)
        {
            try
            {
                if (argDAT_JOB_SEARCH != null && argDAT_JOB_SEARCH.Count > 0)
                {
                    //DAT_JOB_SEARCH = argDAT_JOB_SEARCH[0];
                    JobVacancyList = argDAT_JOB_SEARCH;
                }
                else
                {
                    JobVacancyList = new List<DAT_JOB_SEARCH>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void bindSelectedCompany(List<RES_COMPANY> argSelected_RES_COMPANY)
        {
            try
            {
                if (argSelected_RES_COMPANY != null && argSelected_RES_COMPANY.Count > 0)
                {
                    SelectedCompany = argSelected_RES_COMPANY[0]; // Use the first item in the list
                }
                else
                {
                    SelectedCompany = new RES_COMPANY();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        //End of Data bind for company detail
        private void bindDataTab(List<RES_COMPANY> argRES_COMPANY_LST)
        {
            try
            {
                if (argRES_COMPANY_LST != null && argRES_COMPANY_LST.Count > 0)
                {
                    RES_COMPANY = argRES_COMPANY_LST[0];
                    JobCompanyList.Clear();
                    foreach (RES_COMPANY l_RES_COMPANY in argRES_COMPANY_LST)
                    {
                        JobCompanyList.Add(l_RES_COMPANY);
                    }
                }
                else
                {
                    //JobCompanyList = new List<RES_COMPANY>();
                    JobCompanyList = new ObservableCollection<RES_COMPANY>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void combineImageUrl(List<RES_COMPANY> argRES_COMPANY_LST)
        {
            try
            {
                if (argRES_COMPANY_LST != null && argRES_COMPANY_LST.Count > 0)
                {
                    foreach (RES_COMPANY l_RES_COMPANY in argRES_COMPANY_LST)
                    {
                        l_RES_COMPANY.CompanyLogo = Sys_Service.getUploadURL() + l_RES_COMPANY.CompanyLogo;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void assignIsSavedBtn(List<RES_COMPANY> argRES_COMPANY_LST)
        {
            try
            {
                if (argRES_COMPANY_LST != null && argRES_COMPANY_LST.Count > 0)
                {
                    foreach (RES_COMPANY l_RES_COMPANY in argRES_COMPANY_LST)
                    {
                        l_RES_COMPANY.IsSaved = l_RES_COMPANY.IsSaved == "1" ? "Gold" : "Gray";
                    }
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
                mJSN_REQ_COMPANY.RES_COMPANY = new List<RES_COMPANY> // Fix for CS0029: Initialize as a List<RES_COMPANY>
                {
                    new RES_COMPANY
                    {
                        Remark = argKeyword // Fix for IDE0017: Simplify object initialization
                    }
                };
                getJobCompany();
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
                List<RES_COMPANY> l_RES_COMPANY_Lst = new List<RES_COMPANY>();
                if (argKeyword != null && !argKeyword.Equals(""))
                {
                    foreach (RES_COMPANY l_RES_COMPANY in mJSN_RES_COMPANY.RES_COMPANY)
                    {
                        argKeyword = argKeyword.ToLower();
                        if (l_RES_COMPANY.CompanyName_0_255.ToLower().Contains(argKeyword)
                            || l_RES_COMPANY.CompanyTypeName_0_255.ToLower().Contains(argKeyword)
                            || l_RES_COMPANY.CompanyGroupName_0_255.ToLower().Contains(argKeyword))
                        {
                            l_RES_COMPANY_Lst.Add(l_RES_COMPANY);
                        }
                    }
                }
                else
                {
                    l_RES_COMPANY_Lst = mJSN_RES_COMPANY.RES_COMPANY;// OriginalInvoiceClosedList.GetRange(0, OriginalInvoiceClosedList.Count);
                }
                bindDataTab(l_RES_COMPANY_Lst);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void formatUserSettingData(List<RES_COMPANY> argRES_COMPANY_LST)
        {
            try
            {
                if (argRES_COMPANY_LST != null && argRES_COMPANY_LST.Count > 0)
                {
                    foreach (RES_COMPANY l_RES_COMPANY in argRES_COMPANY_LST)
                    {
                        l_RES_COMPANY.CompanyRegSDate = Utility.getDateTimeString(l_RES_COMPANY.CompanyRegSDate).ToString();

                        l_RES_COMPANY.TotalRate = Utility
                            .getGrandTotalDecimal(l_RES_COMPANY.TotalRate, "1", "0")
                            .ToString();
                        l_RES_COMPANY.ReviewCount = Utility
                            .getGrandTotalDecimal(l_RES_COMPANY.ReviewCount, "0", "0")
                            .ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void formatUserSettingData_Vacancy(List<DAT_JOB_SEARCH> argDAT_JOB_SEARCH_LST)
        {
            try
            {
                if (argDAT_JOB_SEARCH_LST != null && argDAT_JOB_SEARCH_LST.Count > 0)
                {
                    foreach (DAT_JOB_SEARCH l_DAT_JOB_SEARCH in argDAT_JOB_SEARCH_LST)
                    {
                        l_DAT_JOB_SEARCH.SD = Utility.getDateTimeString(l_DAT_JOB_SEARCH.SD).ToString();
                        l_DAT_JOB_SEARCH.ED = Utility.getDateTimeString(l_DAT_JOB_SEARCH.ED).ToString();
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
            //saveJobCompany();
            //getJobCompany();
            return Task.CompletedTask;
        }
        private void selectMoreSearch()
        {
            try
            {
                loadJobCompany();
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
                var popup = new FrmJobCompanyPop(this.CompanyLoad);
                await PopupNavigation.Instance.PushAsync(popup);

                var result = await popup.PopupClosedTask;
                if (result is RES_COMPANY selectedData)
                {
                    mJSN_REQ_COMPANY.RES_COMPANY = new List<RES_COMPANY> { selectedData };
                    if (Common.mCommon.UserSetting.TLSearchTypeAsk == "1")//1 for local search
                    {
                        List<RES_COMPANY> l_RES_COMPANY_Lst = new List<RES_COMPANY>();
                        l_RES_COMPANY_Lst = mRES_COMPANY_LST.Where(data =>
                                                                            (selectedData.CompanyTypeAsk == "0" || data.CompanyTypeAsk == selectedData.CompanyTypeAsk) &&
                                                                            (selectedData.CityAsk == "0" || data.CityAsk == selectedData.CityAsk) &&
                                                                            (selectedData.CountryAsk == "0" || data.CountryAsk == selectedData.CountryAsk) &&
                                                                            (string.IsNullOrEmpty(selectedData.CompanyName_0_255) ||
                                                                            data.CompanyName_0_255?.ToLower().Contains(selectedData.CompanyName_0_255.ToLower()) == true)).ToList();
                        
                        bindDataTab(l_RES_COMPANY_Lst);

                    }
                    else
                    {
                        getJobCompany();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        // Binding data for pickers in Searchmore Popup
        public void bindCompanyType(List<RES_COMPANY_TYPE> argRES_COMPANY_TYPE_LST)
        {
            try
            {
                if (argRES_COMPANY_TYPE_LST != null && argRES_COMPANY_TYPE_LST.Count > 0)
                {
                    JobCompanyTypeList = argRES_COMPANY_TYPE_LST;
                }
                else
                {
                    JobCompanyTypeList = new List<RES_COMPANY_TYPE>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public void bindCompanyLocationCity(List<RES_CITY> argRES_CITY_LST)
        {
            try
            {
                if (argRES_CITY_LST != null && argRES_CITY_LST.Count > 0)
                {
                    JobCompanyLocationCity = argRES_CITY_LST;
                }
                else
                {
                    JobCompanyLocationCity = new List<RES_CITY>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public void bindCompanyLocationCountry(List<RES_COUNTRY> argRES_COUNTRY)
        {
            try
            {
                if (argRES_COUNTRY != null && argRES_COUNTRY.Count > 0)
                {
                    JobCompanyLocationCountry = argRES_COUNTRY;
                }
                else
                {
                    JobCompanyLocationCountry = new List<RES_COUNTRY>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        // End of Binding data for pickers in Searchmore Popup

        #endregion

        #region "Web Service Api"
        public async Task getJobCompanyDtl(RES_COMPANY argRES_COMPANY)

        {
            try
            {
                Utility.openLoader();
                //SelectedCompany.Add(argRES_COMPANY);
                mJSN_REQ_COMPANY.RES_COMPANY.Clear();
                mJSN_REQ_COMPANY.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_COMPANY.RES_COMPANY.Add(argRES_COMPANY);
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_COMPANY);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wsgetCompanyDetail);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_COMPANY_DETAIL = JsonConvert.DeserializeObject<JSN_RES_COMPANY_DETAIL>(mResponse);
                    if (this.mJSN_RES_COMPANY_DETAIL.Message.Code == "7")
                    {
                        if (this.mJSN_RES_COMPANY_DETAIL.RES_COMPANY.Count > 0)
                        {
                            formatUserSettingData(this.mJSN_RES_COMPANY_DETAIL.RES_COMPANY);
                            formatUserSettingData_Vacancy(this.mJSN_RES_COMPANY_DETAIL.DAT_JOB_SEARCH);
                            //combineImageUrl(this.mJSN_RES_COMPANY_DETAIL.RES_COMPANY);
                            assignIsSavedBtn(this.mJSN_RES_COMPANY_DETAIL.RES_COMPANY);
                            bindVacancy(this.mJSN_RES_COMPANY_DETAIL.DAT_JOB_SEARCH);
                            bindSelectedCompany(this.mJSN_RES_COMPANY_DETAIL.RES_COMPANY);

                            //For save button
                            NotifyPropertyChanged(nameof(SelectedCompany));
                            NotifyPropertyChanged(nameof(SaveButtonText));
                            NotifyPropertyChanged(nameof(SaveButtonColor));

                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_COMPANY.Message.Message);
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_COMPANY.Message.Message);
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_COMPANY.Message.Message);
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

        public async Task getJobCompany()
        {
            try
            {
                Utility.openLoader();
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_COMPANY);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wsgetCompany);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_COMPANY = JsonConvert.DeserializeObject<JSN_RES_COMPANY>(mResponse);
                    if (this.mJSN_RES_COMPANY.Message.Code == "7")
                    {
                        mDAT_APPLICANT.Ask = mJSN_RES_COMPANY.DAT_APPLICANT.Ask ;
                        if (this.mJSN_RES_COMPANY.RES_COMPANY.Count > 0)
                        {
                            formatUserSettingData(this.mJSN_RES_COMPANY.RES_COMPANY);
                            mRES_COMPANY_LST = this.mJSN_RES_COMPANY.RES_COMPANY;
                            //combineImageUrl(this.mJSN_RES_COMPANY.RES_COMPANY);
                            assignIsSavedBtn(this.mJSN_RES_COMPANY.RES_COMPANY);
                            bindDataTab(this.mJSN_RES_COMPANY.RES_COMPANY);
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_COMPANY.Message.Message);
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_COMPANY.Message.Message);
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_COMPANY.Message.Message);
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

        public async void saveJobCompany(DAT_APPLICANT_COMPANY_JUN arg_DAT_APPLICANT_COMPANY_JUN)
        {
            try
            {
                mJSN_REQ_APPLICANT_COMPANY_JUN.DAT_APPLICANT_COMPANY_JUN.Clear();
                mJSN_REQ_APPLICANT_COMPANY_JUN.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_APPLICANT_COMPANY_JUN.DAT_APPLICANT_COMPANY_JUN.Add(arg_DAT_APPLICANT_COMPANY_JUN);
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_APPLICANT_COMPANY_JUN);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wssaveApplicantCompany);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_APPLICANT_COMPANY_JUN = JsonConvert.DeserializeObject<JSN_RES_APPLICANT_COMPANY_JUN>(mResponse);
                    if (mJSN_RES_APPLICANT_COMPANY_JUN.Message.Code == "7")
                    {
                        
                        //For list page save btn update
                        for (int i = 0; i < JobCompanyList.Count; i++)
                        {
                            RES_COMPANY l_RES_COMPANY = JobCompanyList[i];

                            if (l_RES_COMPANY.Ask == mJSN_RES_APPLICANT_COMPANY_JUN.DAT_APPLICANT_COMPANY_JUN[0].CompanyAsk)
                            {
                                l_RES_COMPANY.IsSaved = l_RES_COMPANY.IsSaved ==  "1" ? "Gold" : "Gray";

                                JobCompanyList.RemoveAt(i);       // Remove original item at index i
                                JobCompanyList.Insert(i, l_RES_COMPANY);   // Insert updated item at same index
                                //WeakReferenceMessenger.Default.Send(this.mJSN_RES_COMPANY.Message.Message);
                                break; // Quit loop after successful match
                            }
                        }

                        //For Detail page save button update
                        if (SelectedCompany != null)
                        {
                            if (arg_DAT_APPLICANT_COMPANY_JUN.StatusAsk == "1")
                                SelectedCompany.IsSaved = "1";
                            else
                                SelectedCompany.IsSaved = "0";

                            // Notify UI of dependent changes
                            NotifyPropertyChanged(nameof(SelectedCompany));
                            NotifyPropertyChanged(nameof(SaveButtonText));
                            NotifyPropertyChanged(nameof(SaveButtonColor));
                        }

                            

                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("ErrWebService"));
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

        public async void loadJobCompany()
        {
            try
            {
                Utility.openLoader();
                mRequest = JsonConvert.SerializeObject(Common.mCommon.REQ_AUTHORIZATION);
                mResponse = await Job_Service.ApiCall(mRequest, Job_Name.wsLoadCompany);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_LOAD_COMPANY = JsonConvert.DeserializeObject<JSN_RES_LOAD_COMPANY>(mResponse);
                    if (mJSN_RES_LOAD_COMPANY.Message.Code == "7")
                    {
                        Utility.closeLoader();
                        this.CompanyLoad = mJSN_RES_LOAD_COMPANY;
                        callSearchMorePopup();
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_LOAD_COMPANY.Message.Message);
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

            getJobCompany(); // Your data fetch


            IsLoadingMore = false;
        }

        //private async Task ToggleFavorite()
        //{
        //    string l_Value = IsFavorite == "1" ? "0" : "1";

        //    var request = new RES_COMPANY
        //    {
        //        Ask = "1", // or whatever is update
                
        //        IsSaved = l_Value,
        //        // include other required fields
        //    };

        //    var result = Common.mCommon.SaveCompany(request); // your save API call

        //    if (result.Message == "7") // success
        //    {
        //        IsFavorite = l_Value;
        //    }
        //    else
        //    {
        //        await Application.Current.MainPage.DisplayAlert("Error", "Failed to save", "OK");
        //    }
        //}
        #endregion
    }
}
