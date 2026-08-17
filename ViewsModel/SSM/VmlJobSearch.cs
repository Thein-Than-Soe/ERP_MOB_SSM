using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.HCM.REQ;
using CS.ERP.PL.HCM.RES;
using CS.ERP.PL.JOB.DAT;
using CS.ERP.PL.JOB.REQ;
using CS.ERP.PL.JOB.RES;
using CS.ERP.PL.PMA_API.DAT;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.SYS.DAT;
using CS.ERP.PL.SYS.REQ;
using CS.ERP.PL.SYS.RES;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.SSM;
using CS.ERP_MOB.Views.SSM;
using CS.ERP_MOB.ViewsModel.Frame;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using RGPopup.Maui.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using static CS.ERP_MOB.General.Utility;

namespace CS.ERP_MOB.ViewsModel.SSM
{
    public class VmlJobSearch : BaseViewModel
    {
        #region "Declaring"
        
        public JSN_REQ_JOB_SEARCH mJSN_REQ_JOB_SEARCH = new JSN_REQ_JOB_SEARCH();
        public JSN_REQ_APPLICANT_VACANCY_JUN mJSN_REQ_APPLICANT_VACANCY_JUN = new JSN_REQ_APPLICANT_VACANCY_JUN();
        public JSN_RES_APPLICANT_VACANCY_JUN mJSN_RES_APPLICANT_VACANCY_JUN = new JSN_RES_APPLICANT_VACANCY_JUN();
        public JSN_RES_JOB_SEARCH mJSN_RES_JOB_SEARCH = new JSN_RES_JOB_SEARCH();
        public JSN_RES_JOB_SEARCH mJSN_RES_JOB_SEARCH_DETAIL = new JSN_RES_JOB_SEARCH();
        public JSN_LOAD_JOB_VACANCY mJSN_LOAD_JOB_VACANCY = new JSN_LOAD_JOB_VACANCY();
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
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobSearch.lbl.Designation"), value = "DesignationName_0_255", ShowIcon = true },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobSearch.lbl.CompanyName"), value = "CompanyName_0_255", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobSearch.lbl.Availability"), value = "VacancyAvailabilityName_0_255", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobSearch.lbl.City"), value = "CityName_0_255", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobSearch.lbl.StartDate"), value = "SD", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("JOB.JobSearch.lbl.EndDate"), value = "ED", ShowIcon = false },
            ];

        #endregion

        #region "Contructor"
        public VmlJobSearch()
        {
            this.switchDisplayView(DisplayView.Card);
            JobVacancyLoad = new JSN_LOAD_JOB_VACANCY();
            JobVacancyList = new ObservableCollection<DAT_JOB_SEARCH>();
            LoadMoreCommand = new Command(async () => await LoadMoreItems());
            sortingList = new ObservableCollection<SortingItem>(labelTexts);
            IsAscending = true;
            IsDescending = false;
        }
        #endregion

        #region "Display View"
        // ✅ Bind this to Button Text
        public string SaveButtonText =>
                      (SelectedVacancy?.IsSaved == "1" || SelectedVacancy?.IsSaved == "Gold")
                      ? "Unsave"
                      : "Save";

        public Color SaveButtonColor =>
                     (SelectedVacancy?.IsSaved == "1" || SelectedVacancy?.IsSaved == "Gold")
                     ? Colors.OrangeRed
                     : Colors.Green;

        public string ApplyButtonText =>
                      (SelectedVacancy?.VacancyStatusAsk == "1")
                      ? "Apply"
                      : "Applied";

        public Color ApplyButtonColor =>
                     (SelectedVacancy?.VacancyStatusAsk == "1")
                     ? Colors.Green
                     : Colors.Gray;

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
        public JSN_LOAD_JOB_VACANCY JSN_LOAD_JOB_VACANCY = new JSN_LOAD_JOB_VACANCY();
        public JSN_LOAD_JOB_VACANCY JobVacancyLoad
        {
            get { return JSN_LOAD_JOB_VACANCY; }
            set { JSN_LOAD_JOB_VACANCY = value; NotifyPropertyChanged("JobVacancyLoad"); }
        }

        public DAT_JOB_SEARCH mDAT_JOB_SEARCH = new DAT_JOB_SEARCH();
        public List<DAT_JOB_SEARCH> mDAT_JOB_SEARCH_LST = new List<DAT_JOB_SEARCH>();

        public DAT_JOB_SEARCH DAT_JOB_SEARCH
        {
            get { return mDAT_JOB_SEARCH; }
            set { mDAT_JOB_SEARCH = value; NotifyPropertyChanged("DAT_JOB_SEARCH"); }
        }

        public List<DAT_JOB_SEARCH> mJobVacancyList;
        public ObservableCollection<DAT_JOB_SEARCH> JobVacancyList { get; set; }

        // For job search detail
        public DAT_JOB_SEARCH mSelectedVacancy;
        public DAT_JOB_SEARCH SelectedVacancy
        {
            get { return mSelectedVacancy; }
            set { mSelectedVacancy = value; NotifyPropertyChanged("SelectedVacancy"); }
        }


        // Pickers in Searchmore Popup
        public List<DAT_JOB_CLASSIFICATION> mJobClassificationList;
        public List<DAT_JOB_CLASSIFICATION> JobClassificationList
        {
            get { return mJobClassificationList; }
            set { mJobClassificationList = value; NotifyPropertyChanged("JobClassificationList"); }
        }

        public List<DAT_VACANCY_AVAILABILITY> mJobVacancyAvailabilityList;
        public List<DAT_VACANCY_AVAILABILITY> JobVacancyAvailabilityList
        {
            get { return mJobVacancyAvailabilityList; }
            set { mJobVacancyAvailabilityList = value; NotifyPropertyChanged("JobVacancyAvailabilityList"); }
        }

        public List<DAT_DESIGNATION> mJobDesignationList;
        public List<DAT_DESIGNATION> JobDesignationList
        {
            get { return mJobDesignationList; }
            set { mJobDesignationList = value; NotifyPropertyChanged("JobDesignationList"); }
        }

        public List<RES_COUNTRY_DTL> mJobLocationCountryList;
        public List<RES_COUNTRY_DTL> JobLocationCountryList
        {
            get { return mJobLocationCountryList; }
            set { mJobLocationCountryList = value; NotifyPropertyChanged("JobLocationCountryList"); }
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
                    mRefreshCommand = new Command(() => {
                        //if (Common.mCommon.UserSetting.TLSearchTypeAsk == "1")//1 for local search
                        //{

                        //}
                        //else
                        //{
                        //    this.getJobVacancy();
                        //}
                        mJSN_REQ_JOB_SEARCH.DAT_JOB_SEARCH = new List<DAT_JOB_SEARCH> { new DAT_JOB_SEARCH() };
                        mJSN_REQ_JOB_SEARCH.DAT_JOB_SEARCH[0].Sequence = "0";
                        this.getJobVacancy();
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
                    //mRefreshCommand = new Command(() => this.getJobVacancy());
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
                    //mRefreshCommand = new Command(() => this.getJobVacancy());
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
                    //mRefreshCommand = new Command(() => this.getJobVacancy());
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
                    mSaveItemCommand = new Command<DAT_JOB_SEARCH>(async (item) =>
                    {
                        DAT_APPLICANT_VACANCY_JUN l_item = new DAT_APPLICANT_VACANCY_JUN();
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
                                l_item.IsSaved = "0";
                                l_item.ApplicantAsk = mDAT_APPLICANT.Ask;
                                l_item.JobVacancyAsk = item.Ask;
                                l_item.VacancyStatusAsk = item.VacancyStatusAsk;

                                this.saveJobVacancy(l_item);
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
                                l_item.JobVacancyAsk = item.Ask;
                                l_item.VacancyStatusAsk = item.VacancyStatusAsk;
                                l_item.IsSaved = "1";
                                l_item.ApplicantAsk = mDAT_APPLICANT.Ask;
                                this.saveJobVacancy(l_item);
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
        private ICommand mApplyItemCommand;

        public ICommand ApplyItemCommand
        {
            get
            {
                if (mApplyItemCommand == null)
                {
                    mApplyItemCommand = new Command<DAT_JOB_SEARCH>(async (item) =>
                    {
                        DAT_APPLICANT_VACANCY_JUN l_item = new DAT_APPLICANT_VACANCY_JUN();
                        //else if (item.StatusAsk != "8" && Utility.checkButtonAccess("Inactive"))
                        if (item.VacancyStatusAsk == "1")
                        { 
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                                $"{item.CompanyName_0_255}",
                                $"{Common.mCommon.GetLanguageValueByKey("JOB.Common.confirm.Apply")}",
                                $"{Common.mCommon.GetLanguageValueByKey("JOB.Common.btnName.Yes")}",
                                $"{Common.mCommon.GetLanguageValueByKey("JOB.Common.btnName.No")}");

                            if (answer)
                            {
                                l_item.Ask = "0"; // 8 for inactive  
                                l_item.JobVacancyAsk = item.Ask;
                                l_item.IsSaved = item.IsSaved;
                                l_item.VacancyStatusAsk = "7";
                                l_item.ApplicantAsk = mDAT_APPLICANT.Ask;
                                this.applyJobVacancy(l_item);
                            }
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send("Already applied this job");
                            await Application.Current.MainPage.DisplayAlert(
                                $"{item.CompanyName_0_255}?",
                                $"{Common.mCommon.GetLanguageValueByKey("Already applied this job")}",
                                $"{Common.mCommon.GetLanguageValueByKey("JOB.Common.btnName.Ok")}");
                        }
                    });
                }
                return mApplyItemCommand;
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

                var tmp = JobVacancyList;
                JobVacancyList = null;
                NotifyPropertyChanged(nameof(JobVacancyList));

                JobVacancyList = tmp;
                NotifyPropertyChanged(nameof(JobVacancyList));
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        //Data bind for job search detail
        private void bindSelectedVacancy(List<DAT_JOB_SEARCH> argSelected_DAT_JOB_SEARCH)
        {
            try
            {
                if (argSelected_DAT_JOB_SEARCH != null)
                {
                    SelectedVacancy = argSelected_DAT_JOB_SEARCH[0];
                }
                else
                {
                    SelectedVacancy = new DAT_JOB_SEARCH();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        //End of Data bind for job search detail
        private void bindDataTab(List<DAT_JOB_SEARCH> argDAT_JOB_SEARCH_LST)
        {
            try
            {
                if (argDAT_JOB_SEARCH_LST != null && argDAT_JOB_SEARCH_LST.Count > 0)
                {
                    DAT_JOB_SEARCH = argDAT_JOB_SEARCH_LST[0];
                    JobVacancyList.Clear();
                    foreach (DAT_JOB_SEARCH l_DAT_JOB_SEARCH in argDAT_JOB_SEARCH_LST)
                    {
                        JobVacancyList.Add(l_DAT_JOB_SEARCH);

                    }
                }
                else
                {
                    JobVacancyList = new ObservableCollection<DAT_JOB_SEARCH>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void assignIsSavedBtn(List<DAT_JOB_SEARCH> argDAT_JOB_SEARCH_LST)
        {
            try
            {
                if (argDAT_JOB_SEARCH_LST != null && argDAT_JOB_SEARCH_LST.Count > 0)
                {
                    foreach (DAT_JOB_SEARCH l_DAT_JOB_SEARCH in argDAT_JOB_SEARCH_LST)
                    {
                        l_DAT_JOB_SEARCH.IsSaved = l_DAT_JOB_SEARCH.IsSaved == "1" ? "Gold" : "Gray";
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
                mJSN_REQ_JOB_SEARCH.DAT_JOB_SEARCH = new List<DAT_JOB_SEARCH>
                {
                    new DAT_JOB_SEARCH
                    {
                        Remark = argKeyword 
                    }
                };
                getJobVacancy();
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
                List<DAT_JOB_SEARCH> l_DAT_JOB_SEARCH_Lst = new List<DAT_JOB_SEARCH>();
                if (argKeyword != null && !argKeyword.Equals(""))
                {
                    foreach (DAT_JOB_SEARCH l_DAT_JOB_SEARCH in mJSN_RES_JOB_SEARCH.DAT_JOB_SEARCH)
                    {
                        argKeyword = argKeyword.ToLower();
                        if (l_DAT_JOB_SEARCH.DesignationName_0_255.ToLower().Contains(argKeyword)
                            || l_DAT_JOB_SEARCH.EmploymentTypeName_0_255.ToLower().Contains(argKeyword))
                        {
                            l_DAT_JOB_SEARCH_Lst.Add(l_DAT_JOB_SEARCH);
                        }
                    }
                }
                else
                {
                    l_DAT_JOB_SEARCH_Lst = mJSN_RES_JOB_SEARCH.DAT_JOB_SEARCH;// OriginalInvoiceClosedList.GetRange(0, OriginalInvoiceClosedList.Count);
                }
                bindDataTab(l_DAT_JOB_SEARCH_Lst);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void formatUserSettingData(List<DAT_JOB_SEARCH> argDAT_JOB_SEARCH_LST)
        {
            try
            {
                if (argDAT_JOB_SEARCH_LST != null && argDAT_JOB_SEARCH_LST.Count > 0)
                {
                    foreach (DAT_JOB_SEARCH l_DAT_JOB_SEARCH in argDAT_JOB_SEARCH_LST)
                    {
                        l_DAT_JOB_SEARCH.SD = Utility.getDateTimeString(l_DAT_JOB_SEARCH.SD).ToString();
                        l_DAT_JOB_SEARCH.ED = Utility.getDateTimeString(l_DAT_JOB_SEARCH.ED).ToString();

                        l_DAT_JOB_SEARCH.MinSalary = Utility
                            .getGrandTotalDecimal(l_DAT_JOB_SEARCH.MinSalary, "1", "0")
                            .ToString();
                        l_DAT_JOB_SEARCH.MaxSalary = Utility
                            .getGrandTotalDecimal(l_DAT_JOB_SEARCH.MaxSalary, "1", "0")
                            .ToString();
                        if (l_DAT_JOB_SEARCH.StatusAsk == "1")
                        {
                            l_DAT_JOB_SEARCH.StatusName_0_255 = "Inactive";
                        }
                        else
                        {
                            l_DAT_JOB_SEARCH.StatusName_0_255 = "Active";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        //private Task ExecuteActiveItem()
        //{
        //    saveInvoice();
        //    return Task.CompletedTask;
        //}
        private void selectMoreSearch()
        {
            try
            {
                loadJobVacancy();
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
        //        var popup = new FrmJobSearchPop(this.JobVacancyLoad);
        //        await PopupNavigation.Instance.PushAsync(popup);

        //        var result = await popup.PopupClosedTask;
        //        if (result is DAT_JOB_SEARCH selectedData)
        //        {
        //            mJSN_REQ_JOB_SEARCH.DAT_JOB_SEARCH = new List<DAT_JOB_SEARCH> { selectedData };
        //            if (Common.mCommon.UserSetting.TLSearchTypeAsk == "1")//1 for local search
        //            {
        //                List<DAT_JOB_SEARCH> l_DAT_JOB_SEARCH_Lst = new List<DAT_JOB_SEARCH>();
        //                l_DAT_JOB_SEARCH_Lst = mDAT_JOB_SEARCH_LST.Where(data =>
        //                                                                    (selectedData.JobClassificationAsk == "0" || data.JobClassificationAsk == selectedData.JobClassificationAsk) &&
        //                                                                    (selectedData.DesignationAsk == "0" || data.DesignationAsk == selectedData.DesignationAsk) &&
        //                                                                    (selectedData.CountryAsk == "0" || data.CountryAsk == selectedData.CountryAsk) &&
        //                                                                    (string.IsNullOrEmpty(selectedData.DesignationAsk) ||
        //                                                                    data.DesignationAsk?.ToLower().Contains(selectedData.DesignationAsk.ToLower()) == true)).ToList();
        //                bindDataTab(l_DAT_JOB_SEARCH_Lst);
        //            }
        //            else
        //            {
        //                getJobVacancy();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex.InnerException;
        //    }
        //}


        // Binding data for pickers in Searchmore Popup
        public void bindJobClassification(List<DAT_JOB_CLASSIFICATION> argDAT_JOB_CLASSIFICATION)
        {
            try
            {
                if (argDAT_JOB_CLASSIFICATION != null && argDAT_JOB_CLASSIFICATION.Count > 0)
                {
                    JobClassificationList = argDAT_JOB_CLASSIFICATION;
                }
                else
                {
                    JobClassificationList = new List<DAT_JOB_CLASSIFICATION>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public void bindJobVacancyAvailability(List<DAT_VACANCY_AVAILABILITY> argDAT_VACANCY_AVAILABILITY)
        {
            try
            {
                if (argDAT_VACANCY_AVAILABILITY != null && argDAT_VACANCY_AVAILABILITY.Count > 0)
                {
                    JobVacancyAvailabilityList = argDAT_VACANCY_AVAILABILITY;
                }
                else
                {
                    JobVacancyAvailabilityList = new List<DAT_VACANCY_AVAILABILITY>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public void bindJobDesignation(List<DAT_DESIGNATION> argDAT_DESIGNATION)
        {
            try
            {
                if (argDAT_DESIGNATION != null && argDAT_DESIGNATION.Count > 0)
                {
                    JobDesignationList = argDAT_DESIGNATION;
                }
                else
                {
                    JobDesignationList = new List<DAT_DESIGNATION>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public void bindJobLocationCountry(List<RES_COUNTRY_DTL> argRES_COUNTRY_DTL)
        {
            try
            {
                if (argRES_COUNTRY_DTL != null && argRES_COUNTRY_DTL.Count > 0)
                {
                    JobLocationCountryList = argRES_COUNTRY_DTL;
                }
                else
                {
                    JobLocationCountryList = new List<RES_COUNTRY_DTL>();
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
        public async Task getJobVacancyDtl(DAT_JOB_SEARCH argDAT_JOB_SEARCH)

        {
            try
            {
                Utility.openLoader();
                mJSN_REQ_JOB_SEARCH.DAT_JOB_SEARCH.Clear();
                mJSN_REQ_JOB_SEARCH.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_JOB_SEARCH.DAT_JOB_SEARCH.Add(argDAT_JOB_SEARCH);
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_JOB_SEARCH);
                mResponse = await Ssm_Service.ApiCall(mRequest, Ssm_Name.wsgetJobSearch);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_JOB_SEARCH_DETAIL = JsonConvert.DeserializeObject<JSN_RES_JOB_SEARCH>(mResponse);
                    if (this.mJSN_RES_JOB_SEARCH_DETAIL.Message.Code == "7")
                    {
                        mDAT_APPLICANT.Ask = mJSN_RES_JOB_SEARCH_DETAIL.DAT_APPLICANT.Ask;
                        if (this.mJSN_RES_JOB_SEARCH_DETAIL.DAT_JOB_SEARCH.Count > 0)
                        {
                            formatUserSettingData(this.mJSN_RES_JOB_SEARCH_DETAIL.DAT_JOB_SEARCH);
                            bindSelectedVacancy(this.mJSN_RES_JOB_SEARCH_DETAIL.DAT_JOB_SEARCH);
                            //For save button
                            NotifyPropertyChanged(nameof(SelectedVacancy));
                            NotifyPropertyChanged(nameof(SaveButtonText));
                            NotifyPropertyChanged(nameof(SaveButtonColor));
                            NotifyPropertyChanged(nameof(ApplyButtonText));
                            NotifyPropertyChanged(nameof(ApplyButtonColor));
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_JOB_SEARCH_DETAIL.Message.Message);
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_JOB_SEARCH_DETAIL.Message.Message);
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_JOB_SEARCH_DETAIL.Message.Message);
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

        public async void getJobVacancy()
        {
            try
            {
                Utility.openLoader();
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_JOB_SEARCH);
                mResponse = await Ssm_Service.ApiCall(mRequest, Ssm_Name.wsgetJobSearch);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_JOB_SEARCH = JsonConvert.DeserializeObject<JSN_RES_JOB_SEARCH>(mResponse);
                    if (this.mJSN_RES_JOB_SEARCH.Message.Code == "7")
                    {
                        mDAT_APPLICANT.Ask = mJSN_RES_JOB_SEARCH.DAT_APPLICANT.Ask;
                        if (this.mJSN_RES_JOB_SEARCH.DAT_JOB_SEARCH.Count > 0)
                        {
                            formatUserSettingData(this.mJSN_RES_JOB_SEARCH.DAT_JOB_SEARCH);
                            mDAT_JOB_SEARCH_LST = this.mJSN_RES_JOB_SEARCH.DAT_JOB_SEARCH;
                            assignIsSavedBtn(this.mJSN_RES_JOB_SEARCH.DAT_JOB_SEARCH);
                            bindDataTab(this.mJSN_RES_JOB_SEARCH.DAT_JOB_SEARCH);
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_JOB_SEARCH.Message.Message);
                        }
                        else
                        {
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_JOB_SEARCH.Message.Message);
                        }
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_JOB_SEARCH.Message.Message);
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

        public async void saveJobVacancy(DAT_APPLICANT_VACANCY_JUN arg_DAT_APPLICANT_VACANCY_JUN)
        {
            try
            {
                mJSN_REQ_APPLICANT_VACANCY_JUN.DAT_APPLICANT_VACANCY_JUN.Clear();
                mJSN_REQ_APPLICANT_VACANCY_JUN.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_APPLICANT_VACANCY_JUN.DAT_APPLICANT_VACANCY_JUN.Add(arg_DAT_APPLICANT_VACANCY_JUN);
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_APPLICANT_VACANCY_JUN);
                mResponse = await Ssm_Service.ApiCall(mRequest, Ssm_Name.wssaveApplicantVacancy);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_APPLICANT_VACANCY_JUN = JsonConvert.DeserializeObject<JSN_RES_APPLICANT_VACANCY_JUN>(mResponse);
                    if (mJSN_RES_APPLICANT_VACANCY_JUN.Message.Code == "7")
                    {
                        for (int i = 0; i < JobVacancyList.Count; i++)
                        {
                            DAT_JOB_SEARCH l_DAT_JOB_SEARCH = JobVacancyList[i];

                            if (l_DAT_JOB_SEARCH.Ask == mJSN_RES_APPLICANT_VACANCY_JUN.DAT_APPLICANT_VACANCY_JUN[0].JobVacancyAsk)
                            {
                                l_DAT_JOB_SEARCH.IsSaved = mJSN_RES_APPLICANT_VACANCY_JUN.DAT_APPLICANT_VACANCY_JUN[0].IsSaved;
                                l_DAT_JOB_SEARCH.IsSaved = l_DAT_JOB_SEARCH.IsSaved == "1" ? "Gold" : "Gray";
                                JobVacancyList.RemoveAt(i);       // Remove original item at index i
                                JobVacancyList.Insert(i, l_DAT_JOB_SEARCH);   // Insert updated item at same index
                                //SelectedVacancy.IsSaved = mJSN_RES_APPLICANT_VACANCY_JUN.DAT_APPLICANT_VACANCY_JUN[0].IsSaved;
                                break; // Quit loop after successful match
                            }
                        }

                        //For Detail page save button update
                        if (SelectedVacancy != null)
                        {
                            // Notify UI of dependent changes
                            NotifyPropertyChanged(nameof(SelectedVacancy));
                            NotifyPropertyChanged(nameof(SaveButtonText));
                            NotifyPropertyChanged(nameof(SaveButtonColor));
                        }


                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_JOB_SEARCH.Message.Message);
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_JOB_SEARCH.Message.Message);
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
        public async void applyJobVacancy(DAT_APPLICANT_VACANCY_JUN arg_DAT_APPLICANT_VACANCY_JUN)
        {
            try
            {
                mJSN_REQ_APPLICANT_VACANCY_JUN.DAT_APPLICANT_VACANCY_JUN.Clear();
                mJSN_REQ_APPLICANT_VACANCY_JUN.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_APPLICANT_VACANCY_JUN.DAT_APPLICANT_VACANCY_JUN.Add(arg_DAT_APPLICANT_VACANCY_JUN);
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_APPLICANT_VACANCY_JUN);
                mResponse = await Ssm_Service.ApiCall(mRequest, Ssm_Name.wssaveApplicantVacancy);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_APPLICANT_VACANCY_JUN = JsonConvert.DeserializeObject<JSN_RES_APPLICANT_VACANCY_JUN>(mResponse);
                    if (mJSN_RES_APPLICANT_VACANCY_JUN.Message.Code == "7")
                    {
                        for (int i = 0; i < JobVacancyList.Count; i++)
                        {
                            DAT_JOB_SEARCH l_DAT_JOB_SEARCH = JobVacancyList[i];

                            if (l_DAT_JOB_SEARCH.Ask == mJSN_RES_APPLICANT_VACANCY_JUN.DAT_APPLICANT_VACANCY_JUN[0].JobVacancyAsk)
                            {
                                l_DAT_JOB_SEARCH.VacancyStatusAsk = mJSN_RES_APPLICANT_VACANCY_JUN.DAT_APPLICANT_VACANCY_JUN[0].VacancyStatusAsk;
                                JobVacancyList.RemoveAt(i);       // Remove original item at index i
                                JobVacancyList.Insert(i, l_DAT_JOB_SEARCH);   // Insert updated item at same index

                                break; // Quit loop after successful match
                            }
                        }
                        //For Detail page apply button update
                        if (SelectedVacancy != null)
                        {
                            if (arg_DAT_APPLICANT_VACANCY_JUN.VacancyStatusAsk == "7")
                            {
                                SelectedVacancy.VacancyStatusAsk = "7";
                                SelectedVacancy.VacancyStatusName_0_255 = "Applied";
                            }
                            // Notify UI of dependent changes (Option for btn)
                            NotifyPropertyChanged(nameof(SelectedVacancy));
                            NotifyPropertyChanged(nameof(ApplyButtonText));
                            NotifyPropertyChanged(nameof(ApplyButtonColor));
                        }


                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_JOB_SEARCH.Message.Message);
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_JOB_SEARCH.Message.Message);
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

        public async void loadJobVacancy()
        {
            try
            {
                Utility.openLoader();
                mRequest = JsonConvert.SerializeObject(Common.mCommon.REQ_AUTHORIZATION);
                mResponse = await Ssm_Service.ApiCall(mRequest, Ssm_Name.wsLoadJobVacancy);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_LOAD_JOB_VACANCY = JsonConvert.DeserializeObject<JSN_LOAD_JOB_VACANCY>(mResponse);
                    if (mJSN_LOAD_JOB_VACANCY.Message.Code == "7")
                    {
                        Utility.closeLoader();
                        this.JobVacancyLoad = mJSN_LOAD_JOB_VACANCY;
                        //callSearchMorePopup();
                    }
                    else
                    {
                        WeakReferenceMessenger.Default.Send(this.mJSN_LOAD_JOB_VACANCY.Message.Message);
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

            getJobVacancy(); // Your data fetch


            IsLoadingMore = false;
        }
        #endregion
    }
}
