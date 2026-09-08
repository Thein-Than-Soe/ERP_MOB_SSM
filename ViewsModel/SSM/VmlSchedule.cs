using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HMS.DAT;
using CS.ERP.PL.HMS.REQ;
using CS.ERP.PL.HMS.RES;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.POS.REQ;
using CS.ERP.PL.POS.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.HMS;
using CS.ERP_MOB.Services.POS;
using CS.ERP_MOB.Services.SSM;
using CS.ERP_MOB.Views.SSM;
using CS.ERP_MOB.ViewsModel.Frame;
using Microsoft.Maui.Devices.Sensors;
using Newtonsoft.Json;
using RGPopup.Maui.Services;
using Syncfusion.Maui.Scheduler;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using static CS.ERP_MOB.General.Utility;

namespace CS.ERP_MOB.ViewsModel.SSM
{
    public class VmlSchedule : BaseViewModel
    {
        #region "Declaring"
        string mRequest = "";
        string mResponse = "";

        //getFrontDeskUser
        public JSN_REQ_FRONT_DESK mJSN_REQ_FRONT_DESK = new JSN_REQ_FRONT_DESK();
        public JSN_RES_FRONT_DESK_USER mJSN_RES_FRONT_DESK_USER = new JSN_RES_FRONT_DESK_USER();

        //updateServiceStatus
        public JSN_REQ_UPDATE_SERVICE_STATUS mJSN_REQ_UPDATE_SERVICE_STATUS = new JSN_REQ_UPDATE_SERVICE_STATUS();
        public JSN_RES_UPDATE_SERVICE_STATUS mJSN_RES_UPDATE_SERVICE_STATUS = new JSN_RES_UPDATE_SERVICE_STATUS();

        public List<DAT_FRONT_DESK> mDAT_FRONT_DESK_LST = new List<DAT_FRONT_DESK>();
        public DAT_FRONT_DESK mDAT_FRONT_DESK = new DAT_FRONT_DESK();

       
        public ObservableCollection<SortingItem> sortingList { get; set; }
        SortingItem[] labelTexts = [
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("SSM.FrontDesk.lbl.OrderDate"), value = "OrderDate", ShowIcon = true },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("SSM.FrontDesk.lbl.OrderCode"), value = "OrderCode_0_50", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("SSM.FrontDesk.lbl.CustomerName"), value = "CustomerName_0_255", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("SSM.FrontDesk.lbl.InOutStatusName"), value = "InOutStatusName_0_255", ShowIcon = false },
            new SortingItem{ label = Common.mCommon.GetLanguageValueByKey("SSM.FrontDesk.lbl.StockName"), value = "StockName_0_255", ShowIcon = false}
            ];
        public ObservableCollection<DAT_FRONT_DESK> FrontDeskList { get; set; }
        public ObservableCollection<RES_USER_LST> UserList { get; set; }

        public ObservableCollection<string> ProductPhotos { get; set; }= new ObservableCollection<string>();
        public bool HasPhotos => ProductPhotos.Count > 0;
        public ObservableCollection<SchedulerAppointment> SchedulerAppointments
{
            get;
            set;
        } = new ObservableCollection<SchedulerAppointment>();

        public ObservableCollection<SchedulerResource> ScheduleUsers
        {
            get;
            set;
        } = new ObservableCollection<SchedulerResource>();

        public Dictionary<SchedulerAppointment, DAT_FRONT_DESK> SchedulerAppointmentMap
        {
            get;
            set;
        } = new Dictionary<SchedulerAppointment, DAT_FRONT_DESK>();


        #endregion

        #region "Contructor"
        public VmlSchedule()
        {
            SalesInvoiceLoad = new JSN_RES_CHECK_IN_OUT();
            FrontDeskList = new ObservableCollection<DAT_FRONT_DESK>();
            UserList = new ObservableCollection<RES_USER_LST>();

            SchedulerAppointments = new ObservableCollection<SchedulerAppointment>();
            ScheduleUsers =  new ObservableCollection<SchedulerResource>();

            LoadMoreCommand = new Command(async () => await LoadMoreItems());
            sortingList = new ObservableCollection<SortingItem>(labelTexts);
            IsAscending = true;
            IsDescending = false;

            this.switchDisplayView(DisplayView.Schedule);
        }
        #endregion

        #region "Boolean Declaring"
        private bool mIsScheduleView;
        public bool IsScheduleView
        {
            get
            {
                return mIsScheduleView;
            }
            set
            {
                mIsScheduleView = value;
                NotifyPropertyChanged("IsScheduleView");
            }
        }

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
        public JSN_RES_CHECK_IN_OUT JSN_RES_CHECK_IN_OUT = new JSN_RES_CHECK_IN_OUT();
        public JSN_RES_CHECK_IN_OUT SalesInvoiceLoad
        {
            get { return JSN_RES_CHECK_IN_OUT; }
            set { JSN_RES_CHECK_IN_OUT = value; NotifyPropertyChanged("SalesInvoiceLoad"); }
        }

        private DAT_FRONT_DESK mSelectedFrontDesk;
        public DAT_FRONT_DESK SelectedFrontDesk
        {
            get => mSelectedFrontDesk;
            set
            {
                if (mSelectedFrontDesk == value)
                    return;

                mSelectedFrontDesk = value;
                NotifyPropertyChanged(nameof(SelectedFrontDesk));
            }
        }

        public List<RES_USER_LST> mCustomerDtlList;
        public List<RES_USER_LST> CustomerDtlList
        {
            get { return mCustomerDtlList; }
            set { mCustomerDtlList = value; NotifyPropertyChanged("CustomerDtlList"); }
        }

        // File: ViewsModel/Job/VmlJobProfile.cs

        private string mReferenceFileName;

        public string ReferenceFileName
        {
            get => mReferenceFileName;
            set
            {
                if (mReferenceFileName == value)
                    return;

                mReferenceFileName = value;
                NotifyPropertyChanged(nameof(ReferenceFileName));
            }
        }

        private string mReferenceUploadFilePath;

        public string ReferenceUploadFilePath
        {
            get => mReferenceUploadFilePath;
            set
            {
                if (mReferenceUploadFilePath == value)
                    return;

                mReferenceUploadFilePath = value;
                NotifyPropertyChanged(nameof(ReferenceUploadFilePath));
            }
        }
        private DateTime mStartDate = Utility.getDateTime( Utility.getTLFormLoadSD() ).Date;

        public DateTime StartDate
        {
            get => mStartDate;
            set
            {
                if (mStartDate == value)
                    return;

                mStartDate = value;

                NotifyPropertyChanged(nameof(StartDate));

            }
        }
        private TimeSpan mStartTime = Utility.getDateTime(Utility.getTLFormLoadSD()).TimeOfDay;

        public TimeSpan StartTime
        {
            get => mStartTime;
            set
            {
                if (mStartTime == value)
                    return;

                mStartTime = value;

                NotifyPropertyChanged(nameof(StartTime));

            }
        }
        private DateTime mEndDate = Utility.getDateTime(Utility.getTLFormLoadED() ).Date;

        public DateTime EndDate
        {
            get => mEndDate;
            set
            {
                if (mEndDate == value)
                    return;

                mEndDate = value;
                NotifyPropertyChanged(nameof(EndDate));
            }
        }
        private TimeSpan mEndTime = Utility.getDateTime(Utility.getTLFormLoadED() ).TimeOfDay;

        public TimeSpan EndTime
        {
            get => mEndTime;
            set
            {
                if (mEndTime == value)
                    return;

                mEndTime = value;
                NotifyPropertyChanged(nameof(EndTime));
            }
        }
        #endregion

        #region "Commands"

        // File: ViewsModel/Job/VmlJobProfile.cs

        public ICommand PickReferenceFileCommand => new Command(async () =>
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(
                    new PickOptions
                    {
                        PickerTitle = "Select Reference No. File",
                        FileTypes = FilePickerFileType.Images
                    });

                if (result == null)
                    return;

                // Selected file name
                ReferenceFileName = result.FileName;

                using var stream = await result.OpenReadAsync();
                using var memoryStream = new MemoryStream();

                await stream.CopyToAsync(memoryStream);

                byte[] fileBytes = memoryStream.ToArray();

                var uploadFolderName = Ssm_UploadFolder.ssm_service;

                string response = await Ssm_Service.UploadImageToServer(
                    uploadFolderName,
                    "reference",
                    result.FileName,
                    fileBytes);

                if (!string.IsNullOrWhiteSpace(response))
                {
                    // Final path of the newly uploaded file
                    ReferenceUploadFilePath =
                        "/uploads" + uploadFolderName + "/" + response;

                    // Add the newly uploaded file to the selected front desk
                    if (string.IsNullOrWhiteSpace(mDAT_FRONT_DESK.ReferenceDocument))
                    {
                        mDAT_FRONT_DESK.ReferenceDocument =
                            ReferenceUploadFilePath;
                    }
                    else
                    {
                        mDAT_FRONT_DESK.ReferenceDocument += ";" +
                                                             ReferenceUploadFilePath;
                    }

                    // Add the new file to the selected front desk's UI list
                    ProductPhotos.Add(ReferenceUploadFilePath);

                    NotifyPropertyChanged(nameof(HasPhotos));
                }
                else
                {
                    // Upload failed.
                    // Keep the existing selected front desk documents unchanged.

                    ReferenceUploadFilePath = null;

                    if (!string.IsNullOrWhiteSpace(mDAT_FRONT_DESK.ReferenceDocument))
                    {
                        ReferenceFileName =
                            Path.GetFileName(
                                mDAT_FRONT_DESK.ReferenceDocument
                                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                                    .Last());
                    }
                    else
                    {
                        ReferenceFileName = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        });

        private ICommand mScheduleViewCommand;
        public ICommand ScheduleViewCommand
        {
            get
            {
                if (mScheduleViewCommand == null)
                {
                    mScheduleViewCommand = new Command(() => this.switchDisplayView(DisplayView.Schedule));
                }
                return mScheduleViewCommand;
            }
        }

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
                        //    this.getFrontDeskUser();
                        //}
                        mJSN_REQ_FRONT_DESK.DAT_FRONT_DESK.Sequence = "0";
                        this.getFrontDeskUser();
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
                    mEditItemCommand = new Command<DAT_FRONT_DESK>(async (item) =>
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
                            }
                        }
                    });
                    //mEditItemCommand = new Command(() => this.switchDisplayView(DisplayView.Grid));
                    //mRefreshCommand = new Command(() => this.getFrontDeskUser());
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
                    mDeleteItemCommand = new Command<DAT_FRONT_DESK>(async (item) =>
                    {
                        //if (Utility.checkButtonAccess("Delete") && item.PostingStatusAsk != "1" && item.StatusAsk != "9")
                        if (Utility.checkButtonAccess("Delete") && item.StatusAsk != "9")
                        {
                            bool answer = await Application.Current.MainPage.DisplayAlert(
                               $"{item.OrderCode_0_50}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.Delete")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes")}",
                               $"{Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No")}");

                            if (answer)
                            {
                                VmlSsmBookNow vm = new VmlSsmBookNow();
                                vm.mDAT_BOOK_NOW_HEADER.Ask = item.Ask;
                                vm.mDAT_BOOK_NOW_HEADER.StatusAsk = "6";
                                await vm.saveBookNow();
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
                    //mRefreshCommand = new Command(() => this.getFrontDeskUser());
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
                    mSendItemCommand = new Command<DAT_FRONT_DESK>(async (item) =>
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
                    mActiveItemCommand = new Command<DAT_FRONT_DESK>(async (item) =>
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
                                mJSN_REQ_FRONT_DESK.DAT_FRONT_DESK = item;
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
                                mJSN_REQ_FRONT_DESK.DAT_FRONT_DESK = item;
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
                    mCardItemTappedCommand = new Command<DAT_FRONT_DESK>(async (item) =>
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

        #region "Task"
        private async Task LoadMoreItems()
        {
            if (IsLoadingMore) return;
            IsLoadingMore = true;
            await getFrontDeskUser();
            IsLoadingMore = false;
        }
        private Task ExecuteActiveItem()
        {
            updateServiceStatus();
            return Task.CompletedTask;
        }
        #endregion

        #region "Schedule"
        //public ICommand AppointmentTappedCommand => new Command<SchedulerAppointment>(OnAppointmentTapped);


        public class ScheduleUser
        {
            public string UserAsk { get; set; }
            public string UserID { get; set; }
            public string UserName { get; set; }
            public Color UserColor { get; set; }
        }
        private readonly string[] ScheduleUserColors =
        {
            "#1976D2", // Blue
            "#42A5F5", // Light Blue
            "#00897B", // Teal
            "#43A047", // Green
            "#7CB342", // Light Green
            "#F9A825", // Yellow
            "#FB8C00", // Orange
            "#D81B60", // Pink
            "#8E24AA", // Purple
            "#5E35B1"  // Indigo
        };

        private Dictionary<string, Color> _userColors = new();

        private Color GetUserColor(string userAsk)
        {
            if (string.IsNullOrWhiteSpace(userAsk))
                return Colors.Gray;

            if (_userColors.TryGetValue(userAsk, out Color existingColor))
                return existingColor;

            int index = _userColors.Count % ScheduleUserColors.Length;

            Color color = Color.FromArgb(ScheduleUserColors[index]);

            _userColors[userAsk] = color;

            return color;
        }

        // File: ViewsModel/SSM/VmlSchedule.cs

        private Color GetScheduleStatusColor(string statusAsk)
        {
            return statusAsk switch
            {
                "1" => Color.FromArgb("#28A745"), // Open      - Green
                "2" => Color.FromArgb("#EA9300"), // Assign    - Orange
                "3" => Color.FromArgb("#0000FF"), // Check In  - Blue
                "4" => Color.FromArgb("#DBF019"), // WIP       - Yellow
                "5" => Color.FromArgb("#FF0000"), // Done      - Red
                "6" => Color.FromArgb("#3A0707"), // Check Out - Dark Red
                "7" => Color.FromArgb("#14B9E7"), // Complete  - Cyan
                "8" => Color.FromArgb("#F10EF1"), // Closed    - Magenta

                _ => Color.FromArgb("#9E9E9E")
            };
        }

        public void BuildSchedulerAppointments()
        {
            try
            {
                SchedulerAppointments.Clear();
                SchedulerAppointmentMap.Clear();
                ScheduleUsers.Clear();

                if (FrontDeskList == null || UserList == null)
                    return;

                BuildScheduleUsers();

                foreach (DAT_FRONT_DESK item in FrontDeskList)
                {
                    if (item == null)
                        continue;

                    if (string.IsNullOrWhiteSpace(item.UserAsk))
                        continue;

                    SchedulerResource resource =
                        ScheduleUsers.FirstOrDefault(
                            x => x.Id?.ToString() == item.UserAsk);

                    if (resource == null)
                        continue;

                    // DATES
                    DateTime startTime = Utility.getDateTime(item.OrderSD);
                    DateTime endTime = Utility.getDateTime(item.OrderED);


                    // Prevent invalid duration

                    if (endTime <= startTime)
                    {
                        endTime = startTime.AddMinutes(30);
                    }


                    // APPOINTMENT

                    var appointment = new SchedulerAppointment
                    {
                        Id = item.Ask,

                        Subject =
                                $"({item.InOutStatusName_0_255}) " +
                                $"{item.StockName_0_255} - " +
                                $"{item.CustomerName_0_255}",

                        StartTime = startTime,
                        EndTime = endTime,

                        Notes = item.InOutStatusName_0_255,

                        Background = GetScheduleStatusColor(
                                item.InOutStatusAsk),

                        ResourceIds = new ObservableCollection<object>
                                            {
                                                resource.Id
                                            }
                    };


                    SchedulerAppointments.Add(appointment);

                    // KEEP ORIGINAL DAT_FRONT_DESK
                    SchedulerAppointmentMap[appointment] =
                        item;
                }


                NotifyPropertyChanged(
                    nameof(SchedulerAppointments));

                NotifyPropertyChanged(
                    nameof(ScheduleUsers));
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        private void BuildScheduleUsers()
        {
            ScheduleUsers.Clear();

            if (UserList == null)
                return;

            foreach (RES_USER_LST user in UserList)
            {
                if (user == null ||
                    string.IsNullOrWhiteSpace(user.Ask))
                    continue;

                string userName =
                    !string.IsNullOrWhiteSpace(user.UserName_0_255)
                        ? user.UserName_0_255
                        : user.UserID;

                var resource = new SchedulerResource
                {
                    Id = user.Ask,
                    Name = userName,
                    Foreground = Colors.White,
                    Background = GetUserColor(user.Ask)
                };

                ScheduleUsers.Add(resource);
            }

            NotifyPropertyChanged(nameof(ScheduleUsers));
        }
        #endregion


        #region Get Original Front Desk

        public DAT_FRONT_DESK GetFrontDeskFromAppointment(
            SchedulerAppointment appointment)
        {
            if (appointment == null)
                return null;


            if (SchedulerAppointmentMap.TryGetValue(
                    appointment,
                    out DAT_FRONT_DESK frontDesk))
            {
                return frontDesk;
            }


            return null;
        }

        #endregion


        #region "Status Update Action"
        public List<string> GetAvailableScheduleActions( DAT_FRONT_DESK item)
        {
            var actions = new List<string>();

            if (item == null)
                return actions;

            switch (item.InOutStatusAsk)
            {
                case "1":
                    // Open
                    actions.Add("Assign");
                    break;

                case "2":
                    // Assign
                    actions.Add("Travelling");
                    break;

                case "3":
                    // Travelling
                    actions.Add("Check In");
                    break;

                case "4":
                    // Check In
                    actions.Add("WIP");
                    break;

                case "5":
                    // WIP
                    actions.Add("Done");
                    break;

                case "6":
                    // Done
                    actions.Add("Check Out");
                    break;

                case "7":
                    // Check Out
                    actions.Add("Complete");
                    break;

                case "8":
                    // Complete
                    actions.Add("Closed");
                    break;

                case "9":
                    // Closed
                    break;
            }

            return actions;
        }

        public async Task GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(
                    GeolocationAccuracy.Medium,
                    TimeSpan.FromSeconds(10));

                Location location = await Geolocation.Default.GetLocationAsync(request);

                if (location != null)
                {
                    mDAT_FRONT_DESK.UserGPSLatitude = location.Latitude.ToString("0.########");
                    mDAT_FRONT_DESK.UserGPSLongitude = location.Longitude.ToString("0.########");
                }
            }
            catch (FeatureNotEnabledException)
            {
                // Location service is disabled on the device
                mDAT_FRONT_DESK.UserGPSLatitude = "";
                mDAT_FRONT_DESK.UserGPSLongitude = "";
            }
            catch (PermissionException)
            {
                // User denied location permission
                mDAT_FRONT_DESK.UserGPSLatitude = "";
                mDAT_FRONT_DESK.UserGPSLongitude = "";
            }
            catch (Exception)
            {
                mDAT_FRONT_DESK.UserGPSLatitude = "";
                mDAT_FRONT_DESK.UserGPSLongitude = "";
            }
        }
        #endregion


        #region "Method"
        private void switchDisplayView(DisplayView argDisplayView)
        {
            try
            {
                IsScheduleView = argDisplayView == DisplayView.Schedule;
                IsCardView = argDisplayView == DisplayView.Card;
                IsListView = argDisplayView == DisplayView.List;
                IsGridView = argDisplayView == DisplayView.Grid;

                if (IsScheduleView)
                    BuildSchedulerAppointments();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void bindDataTabUser(List<RES_USER_LST> argRES_USER_LST)
        {
            UserList ??= new ObservableCollection<RES_USER_LST>();
            UserList.Clear();

            if (argRES_USER_LST == null)
                return;
            foreach (RES_USER_LST l_RES_USER_LST in argRES_USER_LST)
            {
                UserList.Add(l_RES_USER_LST);
            }
        }
        
        private void bindDataTab(List<DAT_FRONT_DESK> argDAT_FRONT_DESK_LST)
        {
            FrontDeskList ??= new ObservableCollection<DAT_FRONT_DESK>();
            FrontDeskList.Clear();
            if (argDAT_FRONT_DESK_LST == null)
                return;
            foreach (DAT_FRONT_DESK l_DAT_FRONT_DESK in argDAT_FRONT_DESK_LST)
            {
                FrontDeskList.Add(l_DAT_FRONT_DESK);
            }
        }
        
        public async void searchDataApi(string argKeyword)
        {
            try
            {
                mJSN_REQ_FRONT_DESK.DAT_FRONT_DESK = new DAT_FRONT_DESK();
                mJSN_REQ_FRONT_DESK.DAT_FRONT_DESK.Remark = argKeyword;
                await getFrontDeskUser();
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
                List<DAT_FRONT_DESK> l_DAT_FRONT_DESK_Lst = new List<DAT_FRONT_DESK>();
                if (argKeyword != null && !argKeyword.Equals(""))
                {
                    foreach (DAT_FRONT_DESK l_DAT_FRONT_DESK in mJSN_RES_FRONT_DESK_USER.DAT_FRONT_DESK)
                    {
                        argKeyword = argKeyword.ToLower();
                        if (l_DAT_FRONT_DESK.InvoiceCode_0_50.ToLower().Contains(argKeyword)
                            || l_DAT_FRONT_DESK.OrderCode_0_50.ToLower().Contains(argKeyword)
                            || l_DAT_FRONT_DESK.OrderDate.ToLower().Contains(argKeyword)
                            || l_DAT_FRONT_DESK.StockName_0_255.ToLower().Contains(argKeyword)
                            || l_DAT_FRONT_DESK.StatusName_0_255.ToLower().Contains(argKeyword))
                        {
                            l_DAT_FRONT_DESK_Lst.Add(l_DAT_FRONT_DESK);
                        }
                    }
                }
                else
                {
                    l_DAT_FRONT_DESK_Lst = new List<DAT_FRONT_DESK>(mJSN_RES_FRONT_DESK_USER.DAT_FRONT_DESK);// OriginalInvoiceClosedList.GetRange(0, OriginalInvoiceClosedList.Count);
                }
                bindDataTab(l_DAT_FRONT_DESK_Lst);
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
                //loadInvoice(); // if needed call load api for pickers
                callSearchMorePopup();
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
                var popup = new FrmSsmSchedulePop(this.mJSN_RES_FRONT_DESK_USER);
                await PopupNavigation.Instance.PushAsync(popup);

                var result = await popup.PopupClosedTask;
                if (result is DAT_FRONT_DESK selectedData)
                {
                    mJSN_REQ_FRONT_DESK.DAT_FRONT_DESK = selectedData;
                    if (Common.mCommon.UserSetting.TLSearchTypeAsk == "1")//1 for local search
                    {
                        FrontDeskList = new ObservableCollection<DAT_FRONT_DESK>(mDAT_FRONT_DESK_LST.Where(data => (data.CustomerAsk == selectedData.CustomerAsk)).ToList());
                    }
                    else
                    {

                        await getFrontDeskUser();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
        public void bindCustomer(List<RES_USER_LST> argRES_USER_LST_LST)
        {
            try
            {
                if (argRES_USER_LST_LST != null && argRES_USER_LST_LST.Count > 0)
                {
                    CustomerDtlList = argRES_USER_LST_LST;
                }
                else
                {
                    CustomerDtlList = new List<RES_USER_LST>();
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        //mmn
        public void LoadOrderReference(DAT_FRONT_DESK selectedFrontDesk)
        {
            ProductPhotos.Clear();

            if (selectedFrontDesk == null ||
                string.IsNullOrWhiteSpace(selectedFrontDesk.ReferenceDocument))
            {
                NotifyPropertyChanged(nameof(HasPhotos));
                return;
            }

            var paths = selectedFrontDesk.ReferenceDocument
                .Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var path in paths)
            {
                var trimmedPath = path.Trim();

                if (!string.IsNullOrWhiteSpace(trimmedPath))
                {
                    ProductPhotos.Add(trimmedPath);
                }
            }

            NotifyPropertyChanged(nameof(HasPhotos));
        }
        public void LoadReferenceDocuments(DAT_FRONT_DESK selectedFrontDesk)
        {
            ProductPhotos.Clear();

            if (selectedFrontDesk == null ||
                string.IsNullOrWhiteSpace(selectedFrontDesk.ReferenceDocument))
            {
                NotifyPropertyChanged(nameof(HasPhotos));
                return;
            }

            var paths = selectedFrontDesk.ReferenceDocument
                .Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var path in paths)
            {
                var trimmedPath = path.Trim();

                if (!string.IsNullOrWhiteSpace(trimmedPath))
                {
                    ProductPhotos.Add(trimmedPath);
                }
            }

            NotifyPropertyChanged(nameof(HasPhotos));
        }


        #endregion

        #region "Web Service Api"
        public async Task getFrontDeskUser()
        {
            try
            {
                Utility.openLoader();
                mJSN_REQ_FRONT_DESK.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_FRONT_DESK.DAT_FRONT_DESK = new DAT_FRONT_DESK();
                mJSN_REQ_FRONT_DESK.DAT_FRONT_DESK.CompanyAsk = Common.mCommon.CompanyUserData.CompanyAsk;
                DateTime SD = StartDate.Date + StartTime;
                mJSN_REQ_FRONT_DESK.DAT_FRONT_DESK.SD = SD.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                DateTime ED = EndDate.Date + EndTime;
                mJSN_REQ_FRONT_DESK.DAT_FRONT_DESK.ED = ED.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                mRequest = JsonConvert.SerializeObject(mJSN_REQ_FRONT_DESK);
                mResponse = await Hms_Service.ApiCall(mRequest, Hms_Name.wsgetFrontDeskUser);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_FRONT_DESK_USER = JsonConvert.DeserializeObject<JSN_RES_FRONT_DESK_USER>(mResponse);
                    if (this.mJSN_RES_FRONT_DESK_USER.Message.Code == "7")
                    {
                        if (this.mJSN_RES_FRONT_DESK_USER.DAT_FRONT_DESK.Count > 0)
                        {
                            mDAT_FRONT_DESK_LST = this.mJSN_RES_FRONT_DESK_USER.DAT_FRONT_DESK;
                            bindDataTab(this.mJSN_RES_FRONT_DESK_USER.DAT_FRONT_DESK);
                            bindDataTabUser(this.mJSN_RES_FRONT_DESK_USER.RES_USER_LST);
                            BuildSchedulerAppointments();
                            
                            Utility.closeLoader();
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_FRONT_DESK_USER.Message.Message);
                        }
                        else
                        {
                            Utility.closeLoader();
                            WeakReferenceMessenger.Default.Send(this.mJSN_RES_FRONT_DESK_USER.Message.Message);
                        }
                    }
                    else
                    {
                        Utility.closeLoader();
                        WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("DAT.ErrWebService"));
                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw;
            }
            finally
            {
                Utility.closeLoader();
            }
        }

        public async Task updateServiceStatus()
        {
            try
            {
                Utility.openLoader();
                mJSN_REQ_UPDATE_SERVICE_STATUS.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mJSN_REQ_UPDATE_SERVICE_STATUS.DAT_FRONT_DESK = mDAT_FRONT_DESK;
                mRequest = JsonConvert.SerializeObject(mJSN_REQ_UPDATE_SERVICE_STATUS);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsupdateServiceStatus);
                if (mResponse != null && mResponse != "")
                {
                    this.mJSN_RES_UPDATE_SERVICE_STATUS = JsonConvert.DeserializeObject<JSN_RES_UPDATE_SERVICE_STATUS>(mResponse);
                    if (mJSN_RES_UPDATE_SERVICE_STATUS.Message.Code == "7")
                    {
                        //method for update service status of the card
                        await getFrontDeskUser();

                        Utility.closeLoader();
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_UPDATE_SERVICE_STATUS.Message.Message);
                    }
                    else
                    {
                        Utility.closeLoader();
                        WeakReferenceMessenger.Default.Send(this.mJSN_RES_UPDATE_SERVICE_STATUS.Message.Message);
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
                System.Diagnostics.Debug.WriteLine(ex);
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
