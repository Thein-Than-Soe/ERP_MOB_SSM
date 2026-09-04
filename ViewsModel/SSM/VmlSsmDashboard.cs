using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HMS.DAT;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.POS.REQ;
using CS.ERP.PL.POS.RES;
using CS.ERP.PL.SSM.DAT;
using CS.ERP.PL.SSM.REQ;
using CS.ERP.PL.SSM.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.POS;
using CS.ERP_MOB.ViewsModel.Frame;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Newtonsoft.Json;
using RGPopup.Maui.Services;
using Syncfusion.Maui.Scheduler;
using Syncfusion.Maui.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Input;
using static CS.ERP_MOB.General.Utility;

namespace CS.ERP_MOB.ViewsModel.SSM
{
    public class VmlSsmDashboard : BaseViewModel
    {
        #region "Declaring"
        public JSN_REQ_SSM_DASHBOARD mJSN_REQ_SSM_DASHBOARD = new JSN_REQ_SSM_DASHBOARD();
        public JSN_RES_SSM_DASHBOARD mJSN_RES_SSM_DASHBOARD = new JSN_RES_SSM_DASHBOARD();

        public ObservableCollection<SchedulerAppointment> SchedulerAppointments
        {
            get;
            set;
        } = new ObservableCollection<SchedulerAppointment>();

        public ObservableCollection<DonutChartData> DoughnutChartData
        {
            get;
            set;
        } = new ObservableCollection<DonutChartData>();

        string mRequest = "";
        string mResponse = "";
        #endregion

        #region "Contructor"
        public VmlSsmDashboard()
        {
            mJSN_REQ_SSM_DASHBOARD.DAT_SSM_DASHBOARD.SD = Utility.getTLFormLoadSD();
            mJSN_REQ_SSM_DASHBOARD.DAT_SSM_DASHBOARD.ED = Utility.getTLFormLoadED();
            getSSMDashboard();

            CustomBrushes = new ObservableCollection<Brush>()
            {
                // Completed Jobs
                new SolidColorBrush(Color.FromArgb("#22C55E")),

                // Unassigned Jobs
                new SolidColorBrush(Color.FromArgb("#F59E0B")),

                // Late Check-ins
                new SolidColorBrush(Color.FromArgb("#EF4444"))
            };
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


        #endregion

        #region "Data Tab"
        private bool mIsLoadingDashboard;
        private ObservableCollection<Brush> mCustomBrushes =new ObservableCollection<Brush>();

        public ObservableCollection<Brush> CustomBrushes
        {
            get => mCustomBrushes;
            set
            {
                mCustomBrushes = value;
                NotifyPropertyChanged(nameof(CustomBrushes));
            }
        }

        private List<DAT_DONUT_CHART> mDonutChartList = new List<DAT_DONUT_CHART>();
        public List<DAT_DONUT_CHART> DonutChartList
        {
            get { return mDonutChartList; }
            set { mDonutChartList = value; NotifyPropertyChanged("DonutChartList"); }
        }
        private string mTotalTaskName;

        public string TotalTaskName
        {
            get => mTotalTaskName;
            set
            {
                mTotalTaskName = value;
                NotifyPropertyChanged(nameof(TotalTaskName));
            }
        }
        private string mTotalTask;

        public string TotalTask
        {
            get => mTotalTask;
            set
            {
                mTotalTask = value;
                NotifyPropertyChanged(nameof(TotalTask));
            }
        }

        private List<DAT_FILTER_RANGE> mFilterRangeList = new List<DAT_FILTER_RANGE>();
        public List<DAT_FILTER_RANGE> FilterRangeList
        {
            get { return mFilterRangeList; }
            set { mFilterRangeList = value; NotifyPropertyChanged("FilterRangeList"); }
        }
        private DAT_FILTER_RANGE mSelectedFilterRange;

        public DAT_FILTER_RANGE SelectedFilterRange
        {
            get => mSelectedFilterRange;
            set
            {
                if (mSelectedFilterRange == value)
                    return;

                mSelectedFilterRange = value;
                NotifyPropertyChanged(nameof(SelectedFilterRange));

                if (value == null || mIsLoadingDashboard)
                    return;

                DateRange range = Utility.OnFilterRangeChanged(value);

                if (range == null)
                    return;

                string startDate = range.StartDate
                    .ToUniversalTime()
                    .ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                string endDate = range.EndDate
                    .ToUniversalTime()
                    .ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                mJSN_REQ_SSM_DASHBOARD.DAT_SSM_DASHBOARD.SD = startDate;
                mJSN_REQ_SSM_DASHBOARD.DAT_SSM_DASHBOARD.ED = endDate;

                _ = getSSMDashboard();
            }
        }

        private List<DAT_FRONT_DESK> mFrontDeskList = new List<DAT_FRONT_DESK>();
        public List<DAT_FRONT_DESK> FrontDeskList
        {
            get { return mFrontDeskList; }
            set { mFrontDeskList = value; NotifyPropertyChanged("FrontDeskList"); }
        }

        private List<DAT_REVENUE> mRevenueList = new List<DAT_REVENUE>();
        public List<DAT_REVENUE> RevenueList
        {
            get { return mRevenueList; }
            set { mRevenueList = value; NotifyPropertyChanged("RevenueList"); }
        }
        private DAT_REVENUE mTodayRevenue = new DAT_REVENUE();
        public DAT_REVENUE TodayRevenue
        {
            get { return mTodayRevenue; }
            set { mTodayRevenue = value; NotifyPropertyChanged("TodayRevenue"); }
        }
        private List<DAT_STAFF_OVERVIEW> mStaffOverviewList = new List<DAT_STAFF_OVERVIEW>();
        public List<DAT_STAFF_OVERVIEW> StaffOverviewList
        {
            get { return mStaffOverviewList; }
            set { mStaffOverviewList = value; NotifyPropertyChanged("StaffOverviewList"); }
        }
        private List<DAT_TASK_COMPARISON> mTaskComparisonList = new List<DAT_TASK_COMPARISON>();
        public List<DAT_TASK_COMPARISON> TaskComparisonList
        {
            get { return mTaskComparisonList; }
            set { mTaskComparisonList = value; NotifyPropertyChanged("TaskComparisonList"); }
        }

        private List<TaskComparisonDisplayModel> mTaskComparisonDisplayList = new List<TaskComparisonDisplayModel>();
        public List<TaskComparisonDisplayModel> TaskComparisonDisplayList
        {
            get => mTaskComparisonDisplayList;
            set
            {
                mTaskComparisonDisplayList = value;
                NotifyPropertyChanged(nameof(TaskComparisonDisplayList));
            }
        }



        #endregion

        #region "Schedule method"
        private void BuildSchedulerAppointments()
        {
            try
            {
                SchedulerAppointments.Clear();

                if (FrontDeskList == null || FrontDeskList.Count == 0)
                    return;

                foreach (var frontDesk in FrontDeskList)
                {
                    if (frontDesk == null)
                        continue;

                    // DATES
                    DateTime startTime = Utility.getDateTime(frontDesk.OrderSD);
                    DateTime endTime = Utility.getDateTime(frontDesk.OrderED);

                    // Prevent invalid duration
                    if (endTime <= startTime)
                    {
                        endTime = startTime.AddMinutes(30);
                    }

                    // APPOINTMENT
                    var appointment = new SchedulerAppointment
                    {
                        // Store Front Desk ID
                        Id = frontDesk.Ask,

                        // Appointment title
                        Subject =
                            $"({frontDesk.OrderCode_0_50}) " +
                            $"{frontDesk.CustomerName_0_255}",

                        // Appointment time
                        StartTime = startTime,
                        EndTime = endTime,

                        // Optional
                        Notes = frontDesk.InOutStatusName_0_255,

                        // Status color
                        Background = GetScheduleStatusColor(
                            frontDesk.InOutStatusAsk)
                    };

                    SchedulerAppointments.Add(appointment);
                }

                NotifyPropertyChanged(
                    nameof(SchedulerAppointments));
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"BuildSchedulerAppointments Error: {ex}");
            }
        }
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
        #endregion

        #region "Donut method"
        public class DonutChartData
        {
            public string TypeName { get; set; }

            public double Volume { get; set; }

            public double Percentage { get; set; }

            public Color Color { get; set; }
        }
        private Color GetDonutColor(string typeAsk)
        {
            switch (typeAsk)
            {
                case "1":
                    return Color.FromArgb("#808080");

                case "2":
                    return Color.FromArgb("#22C55E");

                case "3":
                    return Color.FromArgb("#F59E0B");

                case "4":
                    return Color.FromArgb("#EF4444");

                default:
                    return Color.FromArgb("#64748B");
            }
        }
        private void PrepareDonutChart()
        {
            DoughnutChartData.Clear();

            if (DonutChartList == null || DonutChartList.Count == 0)
                return;

            // First item = overall total
            var totalItem = DonutChartList[0];

            TotalTaskName = totalItem.TypeName;

            if (double.TryParse(
                totalItem.Volume,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out double total))
            {
                TotalTask = total.ToString("0");
            }
            else
            {
                TotalTask = "0";
            }

            // Remaining items = doughnut slices
            for (int i = 1; i < DonutChartList.Count; i++)
            {
                var item = DonutChartList[i];

                double.TryParse(
                    item.Volume,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double volume);

                double.TryParse(
                    item.Percentage,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double percentage);

                if (volume <= 0)
                    continue;

                DoughnutChartData.Add(new DonutChartData
                {
                    TypeName = item.TypeName,
                    Volume = volume,
                    Percentage = percentage,
                    Color = GetDonutColor(item.TypeAsk)
                });
            }

            NotifyPropertyChanged(nameof(TotalTaskName));
            NotifyPropertyChanged(nameof(TotalTask));
            NotifyPropertyChanged(nameof(DoughnutChartData));
        }
        #endregion

        #region "Task Compare Method"
        public class TaskComparisonDisplayModel
        {
            public string TypeAsk { get; set; } = "";
            public string TypeName { get; set; } = "";

            public string LastData { get; set; } = "";
            public string QTY { get; set; } = "";

            public Color UpDownColor { get; set; } = Colors.Orange;
            public Color TypeColor { get; set; } = Colors.Orange;
            public string Icon { get; set; } = "";
        }

        private TaskComparisonDisplayModel CreateTaskComparisonDisplay( DAT_TASK_COMPARISON item)
        {
            Color updownColor;
            Color typeColor;
            string icon;

            switch (item.UpDownStatus)
            {
                case "0":
                    updownColor = Color.FromArgb("#F59E0B"); // Orange
                    icon = "\uf068"; // minus / no change
                    break;

                case "1":
                    updownColor = Color.FromArgb("#22C55E"); // Green
                    icon = "\uf062"; // upward arrow
                    break;

                case "2":
                    updownColor = Color.FromArgb("#EF4444"); // Red
                    icon = "\uf063"; // downward arrow
                    break;

                default:
                    updownColor = Color.FromArgb("#64748B");
                    icon = "\uf068";
                    break;
            }
            switch (item.TypeAsk)
            {
                case "1":
                    typeColor = Color.FromArgb("#808080"); // Gray Total
                    break;

                case "2":
                    typeColor = Color.FromArgb("#22C55E"); // Green Completed
                    
                    break;

                case "3":
                    typeColor = Color.FromArgb("#EF4444"); // Red Unassigned
                    
                    break;

                case "4":
                    typeColor = Color.FromArgb("#FFA500"); // Orange Late Check-ins
                    
                    break;

                default:
                    typeColor = Color.FromArgb("#64748B");
                    
                    break;
            }

            return new TaskComparisonDisplayModel
            {
                TypeAsk = item.TypeAsk ?? "",
                TypeName = item.TypeName ?? "",

                LastData = decimal.TryParse(item.LastData, out decimal lastData)
                            ? lastData.ToString("0")
                            : "0",

                QTY = decimal.TryParse(item.QTY, out decimal qty)
                            ? qty.ToString("0")
                            : "0",

                UpDownColor = updownColor,
                TypeColor = typeColor,
                Icon = icon
            };
        }

        #endregion

        #region "Method"
        private void SetTodayRevenue()
        {
            TodayRevenue = RevenueList.FirstOrDefault(x => Utility.getDateTime(x.Date).Date == DateTime.Today);
        }



        #endregion

        #region "Web Service Api"
        public async Task getSSMDashboard()
        {
            if (mIsLoadingDashboard)
                return;
            try
            {
                mIsLoadingDashboard = true;
                Utility.openLoader();
                mJSN_REQ_SSM_DASHBOARD.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;

                mRequest = JsonConvert.SerializeObject(mJSN_REQ_SSM_DASHBOARD);
                mResponse = await Pos_Service.ApiCall(mRequest, Pos_Name.wsgetSSMDashboard);
                if (mResponse != null || mResponse != "")
                {
                    this.mJSN_RES_SSM_DASHBOARD = JsonConvert.DeserializeObject<JSN_RES_SSM_DASHBOARD>(mResponse);
                    if (mJSN_RES_SSM_DASHBOARD.Message.Code == "7")
                    {
                        if (this.mJSN_RES_SSM_DASHBOARD != null)
                        {
                            FilterRangeList = this.mJSN_RES_SSM_DASHBOARD.DAT_FILTER_RANGE;

                            FrontDeskList = this.mJSN_RES_SSM_DASHBOARD.DAT_FRONT_DESK;
                            BuildSchedulerAppointments();

                            DonutChartList = this.mJSN_RES_SSM_DASHBOARD.DAT_DONUT_CHART;
                            PrepareDonutChart();
                            TaskComparisonList = this.mJSN_RES_SSM_DASHBOARD.DAT_TASK_COMPARISON;
                            TaskComparisonDisplayList = TaskComparisonList?
                                                        .Select(CreateTaskComparisonDisplay)
                                                        .ToList()
                                                        ?? new List<TaskComparisonDisplayModel>();

                            StaffOverviewList = this.mJSN_RES_SSM_DASHBOARD.DAT_STAFF_OVERVIEW;
                            RevenueList = this.mJSN_RES_SSM_DASHBOARD.DAT_REVENUE;
                            SetTodayRevenue();
                            

                            MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.LoadSuccess);
                        }
                        else
                        {
                            MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, ApplicationMessage.Message.NoData);
                        }
                    }
                    else
                    {
                        MessagingCenter.Send<Application, string>(Application.Current, ApplicationMessage.Message.Alert, this.mJSN_RES_SSM_DASHBOARD.Message.Message);
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
                mIsLoadingDashboard = false;
                Utility.closeLoader();
            }
        }

        #endregion



    }
}