using CommunityToolkit.Maui.Views;
using CS.ERP.PL.HMS.DAT;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.ViewsModel.SSM;
using Syncfusion.Maui.Scheduler;

namespace CS.ERP_MOB.Views.SSM;

public partial class FrmSsmDashboardLst : ContentView
{
    VmlSsmDashboard mVmlSsmDashboard { get; set; }
    public FrmSsmDashboardLst()
	{
        try
        {
            InitializeComponent();
            mVmlSsmDashboard = new VmlSsmDashboard();
            BindingContext = mVmlSsmDashboard;
            ConfigureScheduler();
            _ = mVmlSsmDashboard.InitializeAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"FrmSsmScheduleLst error: {ex}");

            throw;
        }
    }

    private void ConfigureScheduler()
    {
        ScheduleView.View = SchedulerView.Month;
    }
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        System.Diagnostics.Debug.WriteLine(
            $"FrmSsmDashboardLst BindingContext = {BindingContext?.GetType().FullName}");
    }

}