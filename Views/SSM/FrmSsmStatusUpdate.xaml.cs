
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.HMS.DAT;
using CS.ERP.PL.HMS.RES;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.POS.RES;
using CS.ERP_MOB.General;
using CS.ERP_MOB.ViewsModel.SSM;
using Microsoft.Maui.Controls;
using Syncfusion.Maui.Core.Carousel;

namespace CS.ERP_MOB.Views.SSM
{
    public partial class FrmSsmStatusUpdate : ContentPage
    {
        VmlSchedule vm = new VmlSchedule();
        private DAT_FRONT_DESK frontDesk;

        private string mCurrentAction;

        #region "Constructor"
        public FrmSsmStatusUpdate()
        {
            InitializeComponent();
            frontDesk = new DAT_FRONT_DESK();

            vm.SelectedFrontDesk = frontDesk;

            vm.LoadOrderReference(frontDesk);

            BindingContext = vm;

            DateTime now = DateTime.Now;

            // Start
            pkr_SD.Date = now.Date;
            pkr_ST.Time = now.TimeOfDay;

            // End
            pkr_ED.Date = now.Date;
            pkr_ET.Time = now.TimeOfDay;

            SetAvailableAction();
            UpdateStatusDisplay();
        }
        public FrmSsmStatusUpdate(DAT_FRONT_DESK selectedFrontDesk)
        {
            InitializeComponent();
            frontDesk = selectedFrontDesk;
            vm.SelectedFrontDesk = frontDesk;

            DateTime now = DateTime.Now;

            // Start
            pkr_SD.Date = now.Date;
            pkr_ST.Time = now.TimeOfDay;

            // End
            pkr_ED.Date = now.Date;
            pkr_ET.Time = now.TimeOfDay;

            // Optionally bind to the UI
            vm.LoadOrderReference(frontDesk);
            BindingContext = vm;
            Title = !string.IsNullOrWhiteSpace(
                        selectedFrontDesk?.OrderCode_0_50)
                    ? selectedFrontDesk.OrderCode_0_50
                    : "New Order";
            SetAvailableAction();
            UpdateStatusDisplay();
        }

        #endregion

        #region "Method"

        private async void OnSaveBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(mCurrentAction))
                    return;

                btn_save.IsEnabled = false;
                bool success = false;

                switch (mCurrentAction)
                {
                    case "Assign":

                        // go to book now
                        vm.SelectedFrontDesk.InOutStatusAsk = "2";
                        vm.SelectedFrontDesk.InOutStatusName_0_255 = "Assign";
                        break;

                    case "Travelling":

                        // go to book now
                        vm.SelectedFrontDesk.InOutStatusAsk = "3";
                        vm.SelectedFrontDesk.InOutStatusName_0_255 = "Travelling";
                        break;


                    case "Check In":

                        
                        vm.SelectedFrontDesk.InOutStatusAsk = "4";
                        vm.SelectedFrontDesk.InOutStatusName_0_255 = "Check In";
                        break;


                    case "WIP":

                        vm.SelectedFrontDesk.InOutStatusAsk = "5";
                        vm.SelectedFrontDesk.InOutStatusName_0_255 = "WIP";
                        break;


                    case "Done":

                        vm.SelectedFrontDesk.InOutStatusAsk = "6";
                        vm.SelectedFrontDesk.InOutStatusName_0_255 = "Done";
                        break;


                    case "Check Out":

                        vm.SelectedFrontDesk.InOutStatusAsk = "7";
                        vm.SelectedFrontDesk.InOutStatusName_0_255 = "Check Out";
                        break;


                    case "Complete":

                        vm.SelectedFrontDesk.InOutStatusAsk = "8";
                        vm.SelectedFrontDesk.InOutStatusName_0_255 = "Complete";
                        break;


                    case "Closed":

                        vm.SelectedFrontDesk.InOutStatusAsk = "9";
                        vm.SelectedFrontDesk.InOutStatusName_0_255 = "Closed";
                        break;

                }

                vm.SelectedFrontDesk.SD = GetUtcDateTimeString(pkr_SD.Date, pkr_ST.Time);
                vm.SelectedFrontDesk.ED = GetUtcDateTimeString(pkr_ED.Date, pkr_ET.Time);
                vm.SelectedFrontDesk.ReferenceDocument = vm.ReferenceUploadFilePath;
                vm.SelectedFrontDesk.ServiceDescription_0_500 = Ent_Description.Text;

                success = await vm.updateServiceStatus();

                if (success)
                {
                    frontDesk.InOutStatusAsk = vm.SelectedFrontDesk.InOutStatusAsk;
                    SetAvailableAction();
                    UpdateStatusDisplay();
                }
                else
                {
                    await DisplayAlert(
                        "Error",
                        "Failed to update status.",
                        "OK");
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert(
                    "Error",
                    ex.Message,
                    "OK");
            }
            finally
            {
                btn_save.IsEnabled = true;
            }
        }

        #endregion

        #region "Helper"

        private void NextImage_Clicked(object sender, EventArgs e)
        {
            int total = ReferenceCarousel.ItemsSource.Cast<object>().Count();

            if (ReferenceCarousel.Position < total - 1)
                ReferenceCarousel.Position++;
        }

        private void PreviousImage_Clicked(object sender, EventArgs e)
        {
            if (ReferenceCarousel.Position > 0)
                ReferenceCarousel.Position--;
        }

        private void SetAvailableAction()
        {
            var actions = vm.GetAvailableScheduleActions(frontDesk);

            if (actions != null && actions.Count > 0)
            {
                mCurrentAction = actions[0];

                btn_save.Text = mCurrentAction;
            }
        }

        private void UpdateStatusDisplay()
        {
            // Default all statuses
            Label[] statusLabels =
            {
                lblOpen,
                lblAssign,
                lblTravelling,
                lblCheckIn,
                lblWip,
                lblDone,
                lblCheckOut,
                lblComplete,
                lblClosed
            };

            // Reset all labels first
            foreach (var label in statusLabels)
            {
                label.TextColor = Color.FromArgb("#9CA3AF"); // Gray
                label.FontSize = 13; // Default size
            }

            if (frontDesk == null)
                return;

            if (!int.TryParse(frontDesk.InOutStatusAsk, out int currentStatus))
                return;

            for (int i = 0; i < statusLabels.Length; i++)
            {
                int status = i + 1;

                if (status < currentStatus)
                {
                    // Previous/completed status
                    statusLabels[i].TextColor = Color.FromArgb("#22C55E"); // Green
                    statusLabels[i].FontSize = 13;
                }
                else if (status == currentStatus)
                {
                    // Current status
                    statusLabels[i].TextColor = Colors.Black;
                    statusLabels[i].FontSize = 15;
                }
                else
                {
                    // Future status
                    statusLabels[i].TextColor = Color.FromArgb("#9CA3AF"); // Gray
                    statusLabels[i].FontSize = 13;
                }
            }
        }

        private string GetUtcDateTimeString(DateTime date, TimeSpan time)
        {
            DateTime localDateTime = date.Date + time;

            return localDateTime
                .ToUniversalTime()
                .ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
    
        #endregion

    }
}
