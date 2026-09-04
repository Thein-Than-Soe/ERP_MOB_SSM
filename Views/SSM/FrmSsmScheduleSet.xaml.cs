
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.HMS.DAT;
using CS.ERP.PL.HMS.RES;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.POS.RES;
using CS.ERP_MOB.General;
using CS.ERP_MOB.ViewsModel.SSM;
using Microsoft.Maui.Controls;

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

            // Optionally bind to the UI
            BindingContext = frontDesk;

            SetAvailableAction();
        }
        public FrmSsmScheduleSet(DAT_FRONT_DESK selectedFrontDesk)
        {
            InitializeComponent();
            frontDesk = selectedFrontDesk;

            // Optionally bind to the UI
            BindingContext = frontDesk;
            Title = !string.IsNullOrWhiteSpace(
                        selectedFrontDesk?.OrderCode_0_50)
                    ? selectedFrontDesk.OrderCode_0_50
                    : "New Order";
            SetAvailableAction();
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
                        vm.mDAT_FRONT_DESK = frontDesk;
                        vm.mDAT_FRONT_DESK.InOutStatusAsk = "2";
                        vm.mDAT_FRONT_DESK.ReferenceDocument = vm.ReferenceUploadFilePath;
                        vm.mDAT_FRONT_DESK.ServiceDescription_0_500 = Ent_Description.Text;

                        await vm.updateServiceStatus();

                        break;


                    case "Check In":

                        vm.mDAT_FRONT_DESK = frontDesk;
                        vm.mDAT_FRONT_DESK.InOutStatusAsk = "3";
                        vm.mDAT_FRONT_DESK.InOutStatusName_0_255 = "Check In";

                        vm.mDAT_FRONT_DESK.ReferenceDocument = vm.ReferenceUploadFilePath;
                        vm.mDAT_FRONT_DESK.ServiceDescription_0_500 = Ent_Description.Text;
                        vm.mDAT_FRONT_DESK.OrderSD =
                            Utility.getTLFormLoadSD();

                        await vm.GetCurrentLocation();
                        await vm.updateServiceStatus();

                        break;


                    case "WIP":

                        vm.mDAT_FRONT_DESK = frontDesk;
                        vm.mDAT_FRONT_DESK.InOutStatusAsk = "4";

                        vm.mDAT_FRONT_DESK.ReferenceDocument = vm.ReferenceUploadFilePath;
                        vm.mDAT_FRONT_DESK.ServiceDescription_0_500 = Ent_Description.Text;
                        await vm.updateServiceStatus();

                        break;


                    case "Done":

                        vm.mDAT_FRONT_DESK = frontDesk;
                        vm.mDAT_FRONT_DESK.InOutStatusAsk = "5";

                        vm.mDAT_FRONT_DESK.ReferenceDocument = vm.ReferenceUploadFilePath;
                        vm.mDAT_FRONT_DESK.ServiceDescription_0_500 = Ent_Description.Text;
                        await vm.updateServiceStatus();

                        break;


                    case "Check Out":

                        vm.mDAT_FRONT_DESK = frontDesk;
                        vm.mDAT_FRONT_DESK.InOutStatusAsk = "6";

                        vm.mDAT_FRONT_DESK.ReferenceDocument = vm.ReferenceUploadFilePath;
                        vm.mDAT_FRONT_DESK.ServiceDescription_0_500 = Ent_Description.Text;
                        vm.mDAT_FRONT_DESK.ED =
                            Utility.getTLFormLoadED();

                        await vm.updateServiceStatus();

                        break;


                    case "Complete":

                        vm.mDAT_FRONT_DESK = frontDesk;
                        vm.mDAT_FRONT_DESK.InOutStatusAsk = "7";
                        vm.mDAT_FRONT_DESK.ReferenceDocument = vm.ReferenceUploadFilePath;
                        vm.mDAT_FRONT_DESK.ServiceDescription_0_500 = Ent_Description.Text;

                        await vm.updateServiceStatus();

                        break;


                    case "Closed":

                        vm.mDAT_FRONT_DESK = frontDesk;
                        vm.mDAT_FRONT_DESK.InOutStatusAsk = "8";
                        vm.mDAT_FRONT_DESK.ReferenceDocument = vm.ReferenceUploadFilePath;
                        vm.mDAT_FRONT_DESK.ServiceDescription_0_500 = Ent_Description.Text;

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
    }
}
