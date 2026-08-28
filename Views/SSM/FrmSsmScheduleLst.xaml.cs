using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HMS.DAT;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.NTF;
using CS.ERP_MOB.Views.Frame;
using CS.ERP_MOB.ViewsModel.SSM;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Syncfusion.Maui.Scheduler;
using System.Collections.ObjectModel;

namespace CS.ERP_MOB.Views.SSM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FrmSsmScheduleLst : ContentView
    {
        #region "Declaring"
        VmlSchedule mVmlSchedule { get; set; }
        Ntf_Service_WebSocket ntfSocketService = new Ntf_Service_WebSocket();
        #endregion
        #region "Constructor"
        public FrmSsmScheduleLst()
        {
            try
            {
                InitializeComponent();
                mVmlSchedule = new VmlSchedule();
                BindingContext = mVmlSchedule;
                mVmlSchedule.mJSN_REQ_FRONT_DESK.REQ_AUTHORIZATION = Common.mCommon.REQ_AUTHORIZATION;
                mVmlSchedule.mJSN_REQ_FRONT_DESK.DAT_FRONT_DESK = new DAT_FRONT_DESK();
                mVmlSchedule.mJSN_REQ_FRONT_DESK.DAT_FRONT_DESK =
                            new DAT_FRONT_DESK
                            {
                                CompanyAsk = Common.mCommon.CompanyUserData.CompanyAsk,
                                SD = Utility.getTLFormLoadSD(),
                                ED = Utility.getTLFormLoadED()
                            };
                //mVmlSchedule.mJSN_REQ_FRONT_DESK.DAT_FRONT_DESK_DETAIL.Add(new DAT_FRONT_DESK_DETAIL());
                //mVmlSchedule.mJSN_REQ_FRONT_DESK.RES_SALE_BROWSE.Add(new RES_SALE_BROWSE());
                
                ConfigureScheduler();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"FrmSsmScheduleLst error: {ex}");

                throw;
            }

        }
        private bool _loaded;

        protected override async void OnParentSet()
        {
            base.OnParentSet();

            if (Parent == null || _loaded)
                return;

            _loaded = true;

            try
            {
                await mVmlSchedule.getFrontDeskUser();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                await this.GetParentPage().DisplayAlert(
                    "Schedule error",
                    ex.Message,
                    "OK");
            }
        }

        #endregion

        #region "Private Mehthod"
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (mVmlSchedule != null)
            {
                int newColumns = width switch
                {
                    < 400 => 2,
                    < 600 => 2,
                    < 800 => 4,
                    < 1000 => 5,
                    < 1200 => 6,
                    < 1400 => 7,
                    < 1600 => 8,
                    < 1800 => 9,
                    _ => 10
                };

                if (collectionView.ItemsLayout is not GridItemsLayout currentLayout ||
                    currentLayout.Span != newColumns)
                {
                    var layout = new GridItemsLayout(newColumns, ItemsLayoutOrientation.Vertical)
                    {
                        VerticalItemSpacing = 10,
                        HorizontalItemSpacing = 10
                    };
                    collectionView.ItemsLayout = layout;
                }
            }
        }
        private void sortFrontDeskList(string sortBy)
        {
            if (mVmlSchedule.FrontDeskList == null || !mVmlSchedule.FrontDeskList.Any())
                return;

            IEnumerable<DAT_FRONT_DESK> sorted;

            switch (sortBy)
            {
                case "InOutDate":
                    sorted = mVmlSchedule.IsAscending
                        ? mVmlSchedule.FrontDeskList.OrderBy(x => x.InOutDate)
                        : mVmlSchedule.FrontDeskList.OrderByDescending(x => x.InOutDate);
                    break;

                case "InvoiceCode_0_50":
                    sorted = mVmlSchedule.IsAscending
                        ? mVmlSchedule.FrontDeskList.OrderBy(x => x.InvoiceCode_0_50)
                        : mVmlSchedule.FrontDeskList.OrderByDescending(x => x.InvoiceCode_0_50);
                    break;

                case "CustomerName_0_255":
                    sorted = mVmlSchedule.IsAscending
                        ? mVmlSchedule.FrontDeskList.OrderBy(x => x.CustomerName_0_255)
                        : mVmlSchedule.FrontDeskList.OrderByDescending(x => x.CustomerName_0_255);
                    break;

                case "StatusName_0_255":
                    sorted = mVmlSchedule.IsAscending
                        ? mVmlSchedule.FrontDeskList.OrderBy(x => x.StatusName_0_255)
                        : mVmlSchedule.FrontDeskList.OrderByDescending(x => x.StatusName_0_255);
                    break;

                case "StockName_0_255":
                    sorted = mVmlSchedule.IsAscending
                        ? mVmlSchedule.FrontDeskList.OrderBy(x => x.StockName_0_255)
                        : mVmlSchedule.FrontDeskList.OrderByDescending(x => x.StockName_0_255);
                    break;

                case "UserName_0_255":
                    sorted = mVmlSchedule.IsAscending
                        ? mVmlSchedule.FrontDeskList.OrderBy(x => x.UserName_0_255)
                        : mVmlSchedule.FrontDeskList.OrderByDescending(x => x.UserName_0_255);
                    break;

                default:
                    return;
            }

            mVmlSchedule.FrontDeskList = new ObservableCollection<DAT_FRONT_DESK>(sorted);
            if (mVmlSchedule.IsCardView)
            {
                collectionView.ItemsSource = mVmlSchedule.FrontDeskList;
            }
            else if (mVmlSchedule.IsListView)
            {
                lstView.ItemsSource = mVmlSchedule.FrontDeskList;
            }
            else if (mVmlSchedule.IsScheduleView)
            {
                mVmlSchedule.BuildSchedulerAppointments();
            }
            else
            {
                MyGrid.ItemsSource = mVmlSchedule.FrontDeskList;
            }
        }
        private void getCheckedData(List<DAT_FRONT_DESK> l_DAT_FRONT_DESK_LST)
        {
            for (int i = 0; i < mVmlSchedule.FrontDeskList.Count; i++)
            {
                if (mVmlSchedule.FrontDeskList[i].IsChecked == "1")
                {
                    l_DAT_FRONT_DESK_LST.Add(mVmlSchedule.FrontDeskList[i]);
                }
            }
        }
        
        #endregion

        #region "Task"
        private async Task btnNew_onClick(RES_CONTROL argRES_CONTROL)
        {
            bool answer = await this.GetParentPage().DisplayAlert("Info", Common.mCommon.GetLanguageValueByKey("POS.Common.confirm.AddNew"),
                                Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.Yes"),
                                Common.mCommon.GetLanguageValueByKey("POS.Common.btnName.No"));
            if (answer)
            {
                Common.mCommon.saveNoti(argRES_CONTROL);
                await Navigation.PushAsync(new FrmSsmOrderSet());
            }
        }
        #endregion

        #region "Event"
        private async void TgrNew_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (!Common.bindMenu("ssm-book-now-lst"))
                {
                    Common.mCommon.SelectedMenu = new RES_MENU { ProductAsk = "24", Text = "Book", MenuUrl = "ssm-book-now-lst", logoImg = "" };
                    MessagingCenter.Send<Application, string>(Application.Current, "ToastMessage", ApplicationMessage.Message.MenuAccessRight);
                }
                Common.routeMenu(Common.mCommon.SelectedMenu);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private void OnEntryCompleted(object sender, EventArgs e)
        {
            try
            {
                string text = ((Entry)sender).Text;
                if (Common.mCommon.UserSetting.TLSearchTypeAsk == "1")//1 for local search
                {

                    if (text != null && text != "")
                    {
                        mVmlSchedule.searchData(text);
                    }
                    else
                    {
                        mVmlSchedule.searchData("");
                    }
                }
                else
                {
                    mVmlSchedule.searchDataApi(text);
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        private async void OnEditSwipeInvoked(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem && swipeItem.BindingContext is DAT_FRONT_DESK selectedItem)
            {
                await Navigation.PushAsync(new FrmSsmScheduleSet(selectedItem));
            }
        }
        private void OnMenuTapped(object sender, TappedEventArgs e)
        {
            Overlay.IsVisible = true;
            MenuBox.IsVisible = true;
        }
        private void OnOverlayTapped(object sender, EventArgs e)
        {
            MenuBox.IsVisible = false;
            Overlay.IsVisible = false;
        }
        private async void OnItemSingleTapped(object sender, object tappedItem)
        {
            if (Utility.checkButtonAccess("Edit"))
            {
                await Navigation.PushAsync(new FrmSsmScheduleSet((DAT_FRONT_DESK)tappedItem));
            }
            else
            {
                WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
            }
        }
        private async void OnItemDoubleTapped(object sender, object tappedItem)
        {
            var popup = new OptionsPopup(Common.mCommon.SelectedMenu.button);
            popup.OnNewClicked = async (RES_CONTROL argRES_CONTROL) =>
            {
                await btnNew_onClick(argRES_CONTROL);
            };
           
            await this.GetParentPage().ShowPopupAsync(popup);

        }
        private void OnCheckAllCheckChanged(object sender, CheckedChangedEventArgs e)
        {
            bool checkAll = chkSelectAll.IsChecked;

            for (int i = 0; i < mVmlSchedule.FrontDeskList.Count; i++)
            {
                var item = mVmlSchedule.FrontDeskList[i];
                item.IsChecked = checkAll ? "1" : "0";

                mVmlSchedule.FrontDeskList.RemoveAt(i);
                mVmlSchedule.FrontDeskList.Insert(i, item);
            }
        }
        private void Sorting_Tapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is SortingItem tappedItem)
            {
                // Hide all icons
                foreach (var item in mVmlSchedule.sortingList)
                    item.ShowIcon = false;

                // Show only tapped item’s icon
                tappedItem.ShowIcon = true;
                sortFrontDeskList(tappedItem.value);
            }
        }
        private void Ascending_Tapped(object sender, TappedEventArgs e)
        {
            mVmlSchedule.IsDescending = false;
            mVmlSchedule.IsAscending = true;
        }
        private void Descending_Tapped(object sender, TappedEventArgs e)
        {
            mVmlSchedule.IsDescending = true;
            mVmlSchedule.IsAscending = false;
        }
        private void chkSelectItem_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is DAT_FRONT_DESK item)
            {
                item.IsChecked = e.Value ? "1" : "0";
            }
        }
        private async void OnListSingleTap(object sender, TappedEventArgs e)
        {
            if (Utility.checkButtonAccess("Edit"))
            {
                if (e.Parameter is DAT_FRONT_DESK tappedItem)
                {
                    await Navigation.PushAsync(new FrmSsmScheduleSet(tappedItem));
                }
            }
            else
            {
                WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
            }
        }
        private void OnListDoubleTap(object sender, TappedEventArgs e)
        {
            var tappedItem = e.Parameter as DAT_FRONT_DESK;

            if (tappedItem == null)
                return;
            tappedItem.IsChecked = "1";
            OnItemDoubleTapped(sender, tappedItem);
        }
        private async void OnGridSingleTap(object sender, TappedEventArgs e)
        {
            if (!Utility.checkButtonAccess("Edit"))
            {
                WeakReferenceMessenger.Default.Send(Common.mCommon.GetMessageValueByKey("MsgAccess"));
                return;
            }

            if (sender is VisualElement ve && ve.BindingContext is DAT_FRONT_DESK tappedItem)
            {
                await Navigation.PushAsync(new FrmSsmScheduleSet(tappedItem));
            }
        }
        private void OnGridDoubleTap(object sender, TappedEventArgs e)
        {
            if (sender is VisualElement ve && ve.BindingContext is DAT_FRONT_DESK tappedItem)
            {
                tappedItem.IsChecked = "1";
                OnItemDoubleTapped(sender, tappedItem);
            }
        }
        #endregion


        private void ConfigureScheduler()
        {
            ScheduleView.View = SchedulerView.TimelineDay;
        }

        private async void Scheduler_Tapped(object sender,SchedulerTappedEventArgs e)
        {
            // Nothing selected
            if (e?.Appointments == null ||
                e.Appointments.Count == 0)
                return;

            // Get ONLY the appointment that was tapped
            var appt = e.Appointments[0] as SchedulerAppointment;

            if (appt == null) return;

            // Convert SchedulerAppointment -> your DAT_FRONT_DESK
            var item = mVmlSchedule.GetFrontDeskFromAppointment(appt);
            if (item == null) return;

            // Get actions according to current status
            var actions =
                mVmlSchedule.GetAvailableScheduleActions(item);
            if (actions == null || actions.Count == 0)  return;

            // Show your custom action popup
            // File: ViewsModel/SSM/VmlSchedule.cs
            // Section: Your action popup method

            var popup = new Popup();

            var stack = new VerticalStackLayout
            {
                Padding = 10,
                Spacing = 5
            };

            // Always add Edit
            actions.Insert(0, "Edit");

            foreach (var action in actions)
            {
                var btn = new Button
                {
                    Text = action
                };

                btn.Clicked += async (s, e) =>
                {
                    // Close popup first for navigate
                    popup.Close();
                    await Task.Delay(100);

                    switch (action)
                    {
                        case "Edit":

                            // Route to edit/service page
                            await Navigation.PushAsync(
                                new FrmSsmScheduleSet(item));

                            break;


                        case "Assign":

                            // API call, update UI
                            mVmlSchedule.mDAT_FRONT_DESK = item;
                            mVmlSchedule.mDAT_FRONT_DESK.StatusAsk = "2";

                            await mVmlSchedule.updateServiceStatus();

                            //go to book now (getBookNow) or service set with order no.

                            break;


                        case "Check In":

                            // API call, update UI
                            mVmlSchedule.mDAT_FRONT_DESK = item;
                            mVmlSchedule.mDAT_FRONT_DESK.StatusAsk = "3";
                            mVmlSchedule.mDAT_FRONT_DESK.OrderSD =
                                Utility.getTLFormLoadSD();

                            await mVmlSchedule.GetCurrentLocation();
                            await mVmlSchedule.updateServiceStatus();

                            break;


                        case "WIP":

                            // API call, update UI
                            mVmlSchedule.mDAT_FRONT_DESK = item;
                            mVmlSchedule.mDAT_FRONT_DESK.StatusAsk = "4";

                            await mVmlSchedule.updateServiceStatus();

                            break;


                        case "Done":

                            // API call, update UI
                            mVmlSchedule.mDAT_FRONT_DESK = item;
                            mVmlSchedule.mDAT_FRONT_DESK.StatusAsk = "5";
                            await mVmlSchedule.updateServiceStatus();

                            break;


                        case "Check Out":

                            // API call, update UI
                            mVmlSchedule.mDAT_FRONT_DESK = item;
                            mVmlSchedule.mDAT_FRONT_DESK.StatusAsk = "6";
                            mVmlSchedule.mDAT_FRONT_DESK.ED = Utility.getTLFormLoadED();

                            await mVmlSchedule.updateServiceStatus();

                            break;


                        case "Complete":

                            // API call, update UI
                            mVmlSchedule.mDAT_FRONT_DESK = item;
                            mVmlSchedule.mDAT_FRONT_DESK.StatusAsk = "7";

                            await mVmlSchedule.updateServiceStatus();

                            break;


                        case "Closed":

                            // API call, update UI
                            mVmlSchedule.mDAT_FRONT_DESK = item;
                            mVmlSchedule.mDAT_FRONT_DESK.StatusAsk = "8";
                            await mVmlSchedule.updateServiceStatus();

                            break;
                    }

                };

                stack.Children.Add(btn);
            }

            popup.Content = stack;

            await Application.Current.MainPage.ShowPopupAsync(popup);

        }

        private async void Scheduler_DoubleTapped(object sender,SchedulerDoubleTappedEventArgs e)
        {
            // Nothing selected
            if (e?.Appointments == null ||
                e.Appointments.Count == 0)
                return;

            // Get the appointment that was double-tapped
            var appt = e.Appointments[0] as SchedulerAppointment;

            if (appt == null)
                return;

            // Get your DAT_FRONT_DESK object
            var item = mVmlSchedule.GetFrontDeskFromAppointment(appt);
            string FrontDeskAsk = item.Ask;

            if (item == null)
                return;

            // Open your book now with data
            if (!Common.bindMenu("ssm-book-now-lst"))
            {
                Common.mCommon.SelectedMenu = new RES_MENU { ProductAsk = "24", Text = "Book", MenuUrl = "ssm-book-now-lst", logoImg = "" };
                MessagingCenter.Send<Application, string>(Application.Current, "ToastMessage", ApplicationMessage.Message.MenuAccessRight);
            }
            Common.routeMenu(Common.mCommon.SelectedMenu, FrontDeskAsk);
        }


    }

}