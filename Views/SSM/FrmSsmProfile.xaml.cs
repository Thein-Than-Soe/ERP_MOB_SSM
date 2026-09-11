using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.DB;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Views.SSM;
using CS.ERP_MOB.ViewsModel.SSM;
using Stripe;
using System.Globalization;
using System.Xml.Xsl;
//using System.Windows.Forms;

namespace CS.ERP_MOB.Views.SSM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmSsmProfile : ContentView
    {
        #region "Declaring"
        
        VmlSsmProfile mVmlSsmProfile;
        // At the top of your class
        private FileResult selectedImageFile;
        string filePath;

        #endregion
        #region "Constructor"
        public FrmSsmProfile()
        {
            try
            {
                InitializeComponent();
                mVmlSsmProfile = new VmlSsmProfile();
                BindingContext = mVmlSsmProfile;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }


        #endregion


        #region "Load data"
        private async void OnSaveBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                await mVmlSsmProfile.saveUser();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
        #endregion



    }
}