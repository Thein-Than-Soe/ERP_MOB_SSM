
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
    public partial class FrmSsmScheduleSet : ContentPage
    {
        VmlSchedule vm = new VmlSchedule();
        private DAT_FRONT_DESK frontDesk;

        private string mCurrentAction;
        public FrmSsmScheduleSet()
        {
            InitializeComponent();
            frontDesk = new DAT_FRONT_DESK();

            vm.SelectedFrontDesk = frontDesk;

            vm.LoadReferenceDocuments(frontDesk);

            BindingContext = vm;

            SetAvailableAction();
            UpdateStatusDisplay();
        }
        public FrmSsmScheduleSet(DAT_FRONT_DESK selectedFrontDesk)
        {
            InitializeComponent();
            frontDesk = selectedFrontDesk;
            vm.SelectedFrontDesk = frontDesk;

            // Optionally bind to the UI
            vm.LoadReferenceDocuments(frontDesk);
            BindingContext = vm;
            Title = !string.IsNullOrWhiteSpace(
                        selectedFrontDesk?.OrderCode_0_50)
                    ? selectedFrontDesk.OrderCode_0_50
                    : "New Order";
            SetAvailableAction();
            UpdateStatusDisplay();
        }

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
        private async void OnSaveBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(mCurrentAction))
                    return;

                btn_save.IsEnabled = false;

                switch (mCurrentAction)
                {
                    case "Assign":

                        // go to book now
                        vm.SelectedFrontDesk.InOutStatusAsk = "2";


                        vm.SelectedFrontDesk.ReferenceDocument = vm.ReferenceUploadFilePath;
                        vm.SelectedFrontDesk.ServiceDescription_0_500 = Ent_Description.Text;

                        await vm.updateServiceStatus();

                        break;

                    case "Travelling":

                        // go to book now
                        vm.SelectedFrontDesk.InOutStatusAsk = "3";
                        vm.SelectedFrontDesk.ReferenceDocument = vm.ReferenceUploadFilePath;
                        vm.SelectedFrontDesk.ServiceDescription_0_500 = Ent_Description.Text;

                        await vm.updateServiceStatus();

                        break;


                    case "Check In":

                        
                        vm.SelectedFrontDesk.InOutStatusAsk = "4";
                        vm.SelectedFrontDesk.InOutStatusName_0_255 = "Check In";

                        vm.SelectedFrontDesk.ReferenceDocument = vm.ReferenceUploadFilePath;
                        vm.SelectedFrontDesk.ServiceDescription_0_500 = Ent_Description.Text;
                        vm.SelectedFrontDesk.OrderSD = Utility.getTLFormLoadSD();

                        await vm.GetCurrentLocation();
                        await vm.updateServiceStatus();

                        break;


                    case "WIP":

                        vm.SelectedFrontDesk.InOutStatusAsk = "5";

                        vm.SelectedFrontDesk.ReferenceDocument = vm.ReferenceUploadFilePath;
                        vm.SelectedFrontDesk.ServiceDescription_0_500 = Ent_Description.Text;
                        await vm.updateServiceStatus();

                        break;


                    case "Done":

                        vm.SelectedFrontDesk.InOutStatusAsk = "6";

                        vm.SelectedFrontDesk.ReferenceDocument = vm.ReferenceUploadFilePath;
                        vm.SelectedFrontDesk.ServiceDescription_0_500 = Ent_Description.Text;
                        await vm.updateServiceStatus();

                        break;


                    case "Check Out":

                        vm.SelectedFrontDesk.InOutStatusAsk = "7";

                        vm.SelectedFrontDesk.ReferenceDocument = vm.ReferenceUploadFilePath;
                        vm.SelectedFrontDesk.ServiceDescription_0_500 = Ent_Description.Text;
                        vm.SelectedFrontDesk.ED =
                            Utility.getTLFormLoadED();

                        await vm.updateServiceStatus();

                        break;


                    case "Complete":

                        vm.SelectedFrontDesk.InOutStatusAsk = "8";
                        vm.SelectedFrontDesk.ReferenceDocument = vm.ReferenceUploadFilePath;
                        vm.SelectedFrontDesk.ServiceDescription_0_500 = Ent_Description.Text;

                        await vm.updateServiceStatus();

                        break;


                    case "Closed":

                        vm.SelectedFrontDesk.InOutStatusAsk = "9";
                        vm.SelectedFrontDesk.ReferenceDocument = vm.ReferenceUploadFilePath;
                        vm.SelectedFrontDesk.ServiceDescription_0_500 = Ent_Description.Text;

                        await vm.updateServiceStatus();

                        break;

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
    

        // File: Views/SSM/FrmSsmServiceSet.xaml.cs

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
            // Default all statuses to gray
            lblOpen.TextColor = Color.FromArgb("#9CA3AF");
            lblAssign.TextColor = Color.FromArgb("#9CA3AF");
            lblTravelling.TextColor = Color.FromArgb("#3B82F6");
            lblCheckIn.TextColor = Color.FromArgb("#9CA3AF");
            lblWip.TextColor = Color.FromArgb("#9CA3AF");
            lblDone.TextColor = Color.FromArgb("#9CA3AF");
            lblCheckOut.TextColor = Color.FromArgb("#9CA3AF");
            lblComplete.TextColor = Color.FromArgb("#9CA3AF");
            lblClosed.TextColor = Color.FromArgb("#9CA3AF");


            if (frontDesk == null)
                return;

            if (!int.TryParse(frontDesk.InOutStatusAsk, out int currentStatus))
                return;

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

            for (int i = 0; i < statusLabels.Length; i++)
            {
                int status = i + 1;

                if (status < currentStatus)
                {
                    // Already completed
                    statusLabels[i].TextColor = Color.FromArgb("#22C55E");
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
                    statusLabels[i].TextColor = Color.FromArgb("#9CA3AF");
                }
            }
        }


    }
}
