using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.Data;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.SYS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Controls;


namespace CS.ERP_MOB.Views.SYS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OvaListFormCartView : ContentView
    {
        public OvaListFormCartView()
        {
            InitializeComponent();
        }

        public OvaListFormCartView(
            string itemAsk,
            string codeLabel,
            string nameLabel,
            string descriptionLabel,
            string desLabel,
            string descLabel,
            string remarkLabel)
        {
            InitializeComponent();
            ItemAsk = itemAsk;
            Code.Text =codeLabel;
            Name.Text =nameLabel;
            Description.Text =descriptionLabel;
            Remark.Text =remarkLabel;
            Des.Text =desLabel;
            Desc.Text =descLabel;
        }

        #region ItemAsk
        public static readonly BindableProperty ItemAskProperty =
            BindableProperty.Create(propertyName: nameof(ItemAsk),
                                    returnType: typeof(string),
                                    declaringType: typeof(OvaListFormCartView),
                                    defaultValue: "0",
                                    defaultBindingMode: BindingMode.OneWay,
                                    propertyChanged: ItemAskChanged);

        public string ItemAsk
        {
            get; set;
        }

        private static void ItemAskChanged(BindableObject bindable, object oldValue, object newValue)
        {
            OvaListFormCartView ThisControl = (OvaListFormCartView)bindable;
            string NewValue = (string)newValue;
            ThisControl.ItemAsk = NewValue;
            //ThisControl.Code.Text =NewValue;
        }

        #endregion ItemAsk

        #region CodeLabel
        public static readonly BindableProperty CodeLabelProperty =
            BindableProperty.Create(propertyName: nameof(CodeLabel),
                                    returnType: typeof(string),
                                    declaringType: typeof(OvaListFormCartView),
                                    defaultValue: "",
                                    defaultBindingMode: BindingMode.OneWay,
                                    propertyChanged: CodeLabelChanged);

        public string CodeLabel
        {
            get; set;
        }

        private static void CodeLabelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            OvaListFormCartView ThisControl = (OvaListFormCartView)bindable;
            string NewValue = (string)newValue;
            ThisControl.Code.Text =NewValue;
        }

        #endregion CodeLabel

        #region NameLabel
        public static readonly BindableProperty NameLabelProperty =
            BindableProperty.Create(propertyName: nameof(NameLabel),
                                    returnType: typeof(string),
                                    declaringType: typeof(OvaListFormCartView),
                                    defaultValue: "",
                                    defaultBindingMode: BindingMode.OneWay,
                                    propertyChanged: NameLabelChanged);

        public string NameLabel
        {
            get; set;
        }

        private static void NameLabelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            OvaListFormCartView ThisControl = (OvaListFormCartView)bindable;
            string NewValue = (string)newValue;
            ThisControl.Name.Text =NewValue;
        }

        #endregion NameLabel

        #region DescriptionLabel
        public static readonly BindableProperty DescriptionLabelProperty =
            BindableProperty.Create(propertyName: nameof(DescriptionLabel),
                                    returnType: typeof(string),
                                    declaringType: typeof(OvaListFormCartView),
                                    defaultValue: "",
                                    defaultBindingMode: BindingMode.OneWay,
                                    propertyChanged: DescriptionLabelChanged);

        public string DescriptionLabel
        {
            get; set;
        }

        private static void DescriptionLabelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            OvaListFormCartView ThisControl = (OvaListFormCartView)bindable;
            string NewValue = (string)newValue;
            ThisControl.Description.Text =NewValue;
        }

        #endregion DescriptionLabel


        #region RemarkLabel
        public static readonly BindableProperty RemarkLabelProperty =
            BindableProperty.Create(propertyName: nameof(RemarkLabel),
                                    returnType: typeof(string),
                                    declaringType: typeof(OvaListFormCartView),
                                    defaultValue: "",
                                    defaultBindingMode: BindingMode.OneWay,
                                    propertyChanged: RemarkLabelChanged);

        public string RemarkLabel
        {
            get; set;
        }

        private static void RemarkLabelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            OvaListFormCartView ThisControl = (OvaListFormCartView)bindable;
            string NewValue = (string)newValue;
            ThisControl.Remark.Text =NewValue;
        }

        #endregion RemarkLabel


        #region DesLabel
        public static readonly BindableProperty DesLabelProperty =
            BindableProperty.Create(propertyName: nameof(DesLabel),
                                    returnType: typeof(string),
                                    declaringType: typeof(OvaListFormCartView),
                                    defaultValue: "",
                                    defaultBindingMode: BindingMode.OneWay,
                                    propertyChanged: DesLabelChanged);

        public string DesLabel
        {
            get; set;
        }

        private static void DesLabelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            OvaListFormCartView ThisControl = (OvaListFormCartView)bindable;
            string NewValue = (string)newValue;
            ThisControl.Des.Text =NewValue;
        }

        #endregion DesLabel


        #region DescLabel
        public static readonly BindableProperty DescLabelProperty =
            BindableProperty.Create(propertyName: nameof(DescLabel),
                                    returnType: typeof(string),
                                    declaringType: typeof(OvaListFormCartView),
                                    defaultValue: "",
                                    defaultBindingMode: BindingMode.OneWay,
                                    propertyChanged: DescLabelChanged);

        public string DescLabel
        {
            get; set;
        }

        private static void DescLabelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            OvaListFormCartView ThisControl = (OvaListFormCartView)bindable;
            string NewValue = (string)newValue;
            ThisControl.Desc.Text =NewValue;
        }

        #endregion DescLabel


        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (!Common.bindMenu("signin"))
            {
                Common.mCommon.SelectedMenu = new RES_MENU { ProductAsk = "1", Text = "FrmAdmin", MenuUrl = "FrmAdmin", logoImg = "" };
                MessagingCenter.Send<Application, string>(Application.Current, "ToastMessage", ApplicationMessage.Message.MenuAccessRight);
            }
            Common.routeMenu(Common.mCommon.SelectedMenu);


            //if (Common.bindMenu("signin"))
            //{
            //    Common.routeMenu(Route.Sys_Route.DicRouteList, Common.mCommon.SelectedMenu);
            //}
            //else
            //{
            //    //remove it after add in menu access for sign in and sign up
            //    Common.mCommon.SelectedMenu = new RES_MENU();
            //    Common.mCommon.SelectedMenu.MenuUrl = "signin";
            //    Common.mCommon.SelectedMenu.Text ="Sign In";
            //    Common.mCommon.SelectedMenu.logoImg = "";
            //    //Common.routeMenu("signin", "Sign In");
            //    Common.routeMenu(Route.Sys_Route.DicRouteList, Common.mCommon.SelectedMenu);
            //}

            //bool isMenuExit = IntercomService.BindMenuUrl("profile");
            //if (isMenuExit)
            //{
            //    IntercomService.RouteMenu("profile", "Access Entry", ItemAsk);
            //}
        }
    }
}