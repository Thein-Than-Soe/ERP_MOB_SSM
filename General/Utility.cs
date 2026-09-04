using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.SYS.DAT;
using CS.ERP_MOB.Views.Frame;
//using Plugin.Connectivity;
//using Rg.Plugins.Popup.Services;
using Microsoft.Maui.Networking;
using Newtonsoft.Json;
using RGPopup.Maui;
using RGPopup.Maui.Services;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
namespace CS.ERP_MOB.General
{
    public class Utility
    {
        #region "Enum"
        public enum SignUpState
        {
            SignUp,
            EmailActivate,
            PhoneActivate,
            Subscribe,
            SubscriptionType,
            SubscriptionPlan
        }
        public enum DisplayView
        {
            Card,
            List,
            Grid,
            Schedule
        }
        public enum SignInState
        {
            SignIn,
            VerifyEmail,
            VerifyPhone,
        }



        #endregion





        public Utility()
        {
        }
        public static async void openLoader()
        {
            try
            {
                var popup = new FrmLoader();  // Using the PopupPage that wraps FrmLoader
                await PopupNavigation.Instance.PushAsync(popup);  // Show the popup
            }
            catch (Exception ex)
            {
                //throw ex.InnerException;
            }

        }
        public static async void closeLoader()
        {
            try
            {
                await PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("loader Already closed");
            }
        }
        public static void checkInternetCon()
        {
            try
            {
                if (!(Connectivity.NetworkAccess == NetworkAccess.Internet))
                { Common.mCommon.ApplicationAlert = true; }
                else { Common.mCommon.ApplicationAlert = false; }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        #region "Date time"
        public static string getFormLoadSD()
        {
            try
            {
                //1 for top of the rows
                //2 for botton of the rows
                if (Common.mCommon.UserSetting.LFPeriodTypeAsk == "1" || Common.mCommon.UserSetting.LFPeriodTypeAsk == "2")
                {
                    return Common.mCommon.UserSetting.LFPeriodTypeName_0_255;
                }
                else
                {
                    return getStartPeriod(Common.mCommon.UserSetting.LFPeriodTypeAsk, int.Parse(Common.mCommon.UserSetting.LFDiplayPeriod));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
        public static string getFormLoadED()
        {
            try
            {
                //1 for top of the rows
                //2 for botton of the rows
                if (Common.mCommon.UserSetting.LFPeriodTypeAsk == "1" || Common.mCommon.UserSetting.LFPeriodTypeAsk == "2")
                {
                    return Common.mCommon.UserSetting.LFDiplayPeriod;
                }
                else
                {
                    return DateTime.UtcNow.ToString("o");
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }

        public static string getTLFormLoadSD()
        {
            try
            {
                //1 for top of the rows
                //2 for botton of the rows
                if (Common.mCommon.UserSetting.TLPeriodTypeAsk == "1" || Common.mCommon.UserSetting.TLPeriodTypeAsk == "2")
                {
                    return Common.mCommon.UserSetting.TLPeriodTypeName_0_255;
                }
                else
                {
                    //int displayPeriod = (int)double.Parse(Common.mCommon.UserSetting.TLDiplayPeriod, CultureInfo.InvariantCulture);
                    //return getStartPeriod(Common.mCommon.UserSetting.TLPeriodTypeAsk, displayPeriod);
                    return getStartPeriod(Common.mCommon.UserSetting.TLPeriodTypeAsk, (int)double.Parse(Common.mCommon.UserSetting.TLDiplayPeriod));
                    //return getStartPeriod("4", 6);
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
        public static string getTLFormLoadED()
        {
            try
            {
                //1 for top of the rows
                //2 for botton of the rows
                if (Common.mCommon.UserSetting.TLPeriodTypeAsk == "1" || Common.mCommon.UserSetting.TLPeriodTypeAsk == "2")
                {
                    return Common.mCommon.UserSetting.TLDiplayPeriod;
                }
                else
                {
                    //return DateTime.UtcNow.ToString("o");
                    return DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }

        }
        public static string getStartPeriod(string periodType, int displayPeriod)
        {
            DateTime date = DateTime.UtcNow; // Use UTC like in JS

            switch (periodType)
            {
                case "3": // Days
                    date = date.AddDays(-displayPeriod);
                    break;

                case "4": // Months
                    date = date.AddMonths(-displayPeriod);
                    break;

                case "5": // Years
                    date = date.AddYears(-displayPeriod);
                    break;

                case "6": // Hours
                    date = date.AddHours(-displayPeriod);
                    break;
            }

            //return date.ToString("o"); // ISO 8601 format (e.g., 2025-07-10T11:35:00.0000000Z)
            return date.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
        }

        public static string getDateTimeString(string argUTCDate)
        {
            try
            {
                if (argUTCDate != null && argUTCDate != "")
                {
                    return DateTime.Parse(argUTCDate).ToLocalTime().ToString(Common.mCommon.UserSetting.DateTimeFormatName_0_255);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        //public static DateTime getDateTime(string argUTCDate)
        //{
        //    try
        //    {
        //        if (argUTCDate != null && argUTCDate != "")
        //        {
        //            return DateTime.ParseExact(DateTime.Parse(argUTCDate).ToLocalTime().ToString(), Common.mCommon.UserSetting.DateTimeFormatName_0_255, CultureInfo.InvariantCulture);
        //        }
        //        else
        //        {
        //            return DateTime.ParseExact((DateTime.Now).ToString(), Common.mCommon.UserSetting.DateTimeFormatName_0_255, CultureInfo.InvariantCulture);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex.InnerException;
        //    }
        //}
        public static DateTime getDateTime(string argUTCDate)
        {
            if (string.IsNullOrWhiteSpace(argUTCDate))
                return DateTime.Now;

            if (DateTime.TryParse(
                argUTCDate,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal,
                out DateTime utcDateTime))
            {
                return utcDateTime.ToLocalTime();
            }

            return DateTime.Now;
        }

        //Filter range

        public class DateRange
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public static DateRange CalendarFormat(DAT_FILTER_RANGE filterRangeData)
        {
            DateTime today = DateTime.Today;
            double count = double.Parse(filterRangeData.Count);

            DateTime startDate;
            DateTime endDate;

            switch (filterRangeData.PeriodTypeAsk)
            {
                // ===== Day =====
                case "3":
                    startDate = today.AddDays(-(count - 1)).Date;
                    endDate = today.Date.AddDays(1).AddTicks(-1);
                    break;

                // ===== Week (Mon–Sun) =====
                case "8":
                    int dayOfWeek = (int)today.DayOfWeek;

                    // Sunday = 0, Monday = 1
                    int diffToMonday = dayOfWeek == 0
                        ? -6
                        : 1 - dayOfWeek;

                    DateTime currentWeekStart = today.AddDays(diffToMonday);

                    startDate = currentWeekStart.Date;

                    endDate = currentWeekStart
                        .AddDays(6)
                        .Date
                        .AddDays(1)
                        .AddTicks(-1);

                    break;

                // ===== Month =====
                case "4":
                    startDate = new DateTime(
                        today.Year,
                        today.Month,
                        1
                    );

                    endDate = new DateTime(
                        today.Year,
                        today.Month,
                        DateTime.DaysInMonth(today.Year, today.Month)
                    )
                    .Date
                    .AddDays(1)
                    .AddTicks(-1);

                    break;

                // ===== Quarter =====
                case "7":
                    int currentQuarter = (today.Month - 1) / 3;
                    int quarterStartMonth = currentQuarter * 3 + 1;

                    startDate = new DateTime(
                        today.Year,
                        quarterStartMonth,
                        1
                    ).AddMonths(-(int)((count - 1) * 3));

                    endDate = new DateTime(
                        today.Year,
                        quarterStartMonth,
                        1
                    )
                    .AddMonths(3)
                    .AddTicks(-1);

                    break;

                // ===== Year =====
                case "5":
                    startDate = new DateTime(
                        today.Year - (int)(count - 1),
                        1,
                        1
                    );

                    endDate = new DateTime(
                        today.Year,
                        12,
                        31
                    )
                    .Date
                    .AddDays(1)
                    .AddTicks(-1);

                    break;

                default:
                    return null;
            }

            return new DateRange
            {
                StartDate = startDate,
                EndDate = endDate
            };
        }

        public static DateRange CurrentFormat(DAT_FILTER_RANGE filterRangeData)
        {
            DateTime today = DateTime.Now;
            double count = double.Parse(filterRangeData.Count);

            DateTime startDate;
            DateTime endDate;

            switch (filterRangeData.PeriodTypeAsk)
            {
                // ===== Day (rolling) =====
                case "3":
                    startDate = today.Date.AddDays(-count);
                    endDate = today;
                    break;

                // ===== Week (rolling) =====
                case "8":
                    startDate = today.AddDays(-count * 7);
                    endDate = today;
                    break;

                // ===== Month (rolling) =====
                case "4":
                    startDate = today.AddMonths(-(int)count);
                    endDate = today;
                    break;

                // ===== Quarter (rolling) =====
                case "7":
                    startDate = today.AddMonths(-(int)(count * 3));
                    endDate = today;
                    break;

                // ===== Year (rolling) =====
                case "5":
                    startDate = today.AddYears(-(int)count);
                    endDate = today;
                    break;

                default:
                    return null;
            }

            return new DateRange
            {
                StartDate = startDate,
                EndDate = endDate
            };
        }

        public static DateRange OnFilterRangeChanged(DAT_FILTER_RANGE filterRangeData)
        {
            if (filterRangeData == null)
                return null;

            DateRange range = null;

            // 1 = Calendar Format
            if (filterRangeData.FilterRangeTypeAsk == "1")
            {
                range = CalendarFormat(filterRangeData);
            }
            // 2 = Current Format
            else if (filterRangeData.FilterRangeTypeAsk == "2")
            {
                range = CurrentFormat(filterRangeData);
            }

            return range;
        }
        #endregion
        public static Boolean checkButtonAccess(string menuName)
        {
            try
            {
                if (menuName != "")
                {
                    foreach (var l_Button in Common.mCommon.SelectedMenu.button)
                    {
                        menuName = menuName.ToLower();
                        if (l_Button.text.ToLower().Contains(menuName))
                        {
                            return true;
                        }
                    }
                    return false;

                }
                else { return false; }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        #region "DecimalFormat"
        //roundoffamount, usersetting decimal place, amount after dis
        public static string getDecimalFormatString(string argDecimal, string argDecimalPlace, string argDecimaRounding)
        {
            try
            {
                if (argDecimal != null && argDecimal != "")
                {

                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argDecimal), Convert.ToInt32(Convert.ToDecimal(argDecimaRounding))));
                }
                else
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(argDecimaRounding))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static string getDecimalFormatString(string argDecimal)
        {
            try
            {
                if (argDecimal != null && argDecimal != "")
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.GrandTotalDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argDecimal), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.GrandTotalRoundAsk))));
                }
                else
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.GrandTotalDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.GrandTotalRoundAsk))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static decimal getDecimalFormatDecimal(string argDecimal, string argDecimalPlace, string argRounding)
        {
            try
            {
                if (argDecimal != null && argDecimal != "")
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argDecimal), Convert.ToInt32(Convert.ToDecimal(argRounding)))));
                }
                else
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(argRounding)))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static decimal getDecimalFormatDecimal(string argDecimal)
        {
            try
            {
                if (argDecimal != null && argDecimal != "")
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.GrandTotalDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argDecimal), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.GrandTotalRoundAsk)))));
                }
                else
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.GrandTotalDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.GrandTotalRoundAsk)))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        #endregion

        #region "Price"
        public static string getPriceString(string argPrice, string argPriceDecimalPlace, string argPriceRoundAsking)
        {
            try
            {
                if (argPrice != null && argPrice != "")
                {
                    //Decimal.Round(Convert.ToDecimal(argPrice), Convert.ToInt32(argRounding));
                    //string.Format($"{{0:F{argDecimalPlace}}}", argPrice);
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argPriceDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(Convert.ToDecimal(argPrice)), Convert.ToInt32(Convert.ToDecimal(argPriceRoundAsking))));
                }
                else
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argPriceDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(argPriceRoundAsking))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static string getPriceString(string argPrice)
        {
            try
            {
                if (argPrice != null && argPrice != "")
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.PriceDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argPrice), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.PriceRoundAsk))));
                }
                else
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.PriceDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.PriceRoundAsk))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static decimal getPriceDecimal(string argPrice, string argPriceDecimalPlace, string argPriceRoundAsking)
        {
            try
            {
                if (argPrice != null && argPrice != "")
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argPriceDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argPrice), Convert.ToInt32(Convert.ToDecimal(argPriceRoundAsking)))));
                }
                else
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToDecimal(Convert.ToInt32(argPriceDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(argPriceRoundAsking)))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static decimal getPriceDecimal(string argPrice)
        {
            try
            {
                if (argPrice != null && argPrice != "")
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.PriceDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argPrice), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.PriceRoundAsk)))));
                }
                else
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.PriceDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.PriceRoundAsk)))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        #endregion

        #region "QTY"
        public static string getQTYString(string argQTY, string argQTYDecimalPlace, string argQTYRoundAsking)
        {
            try
            {
                if (argQTY != null && argQTY != "")
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argQTYDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argQTY), Convert.ToInt32(Convert.ToDecimal(argQTYRoundAsking))));
                }
                else
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argQTYDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(argQTYRoundAsking))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static string getQTYString(string argQTY)
        {
            try
            {
                if (argQTY != null && argQTY != "")
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.QTYDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argQTY), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.QTYRoundAsk))));
                }
                else
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.QTYDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.QTYRoundAsk))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static decimal getQTYDecimal(string argQTY, string argQTYDecimalPlace, string argQTYRoundAsking)
        {
            try
            {
                if (argQTY != null && argQTY != "")
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argQTYDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argQTY), Convert.ToInt32(Convert.ToDecimal(argQTYRoundAsking)))));
                }
                else
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argQTYDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(argQTYRoundAsking)))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static decimal getQTYDecimal(string argQTY)
        {
            try
            {
                if (argQTY != null && argQTY != "")
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.QTYDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argQTY), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.QTYRoundAsk)))));
                }
                else
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.QTYDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.QTYRoundAsk)))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        #endregion

        #region "Amount"
        public static string getAmountString(string argAmount, string argAmountDecimalPlace, string argAmountRoundAsking)
        {
            try
            {
                if (argAmount != null && argAmount != "")
                {
                    //Decimal.Round(Convert.ToDecimal(argPrice), Convert.ToInt32(argRounding));
                    //string.Format($"{{0:F{argDecimalPlace}}}", argPrice);
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argAmountDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argAmount), Convert.ToInt32(Convert.ToDecimal(argAmountRoundAsking))));
                }
                else
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argAmountDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(argAmountRoundAsking))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static string getAmountString(string argAmount)
        {
            try
            {
                if (argAmount != null && argAmount != "")
                {
                    //int ad = Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.AmountDecimalPlace));
                    ////decimal aef = new decimal()
                    //decimal aef = new decimal(51.2345679807);
                    //decimal ae = Decimal.Round(new decimal(51.2345679807), 1);
                    ////decimal a = Decimal.Round(Convert.ToDecimal(argAmount), Convert.ToInt32(Common.mCommon.UserSetting.AmountRoundAsk));
                    /////string ss = string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.AmountDecimalPlace))}}}", aef);

                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.AmountDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argAmount), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.AmountRoundAsk))));
                }
                else
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.AmountDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.AmountRoundAsk))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static decimal getAmountDecimal(string argAmount, string argAmountDecimalPlace, string argAmountRoundAsking)
        {
            try
            {
                if (argAmount != null && argAmount != "")
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argAmountDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argAmount), Convert.ToInt32(Convert.ToDecimal(argAmountRoundAsking)))));
                }
                else
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argAmountDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(argAmountRoundAsking)))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static decimal getAmountDecimal(string argAmount)
        {
            try
            {
                if (argAmount != null && argAmount != "")
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.AmountDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argAmount), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.AmountRoundAsk)))));
                }
                else
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.AmountDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.AmountRoundAsk)))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        #endregion

        #region "Percentage"
        public static string getPercentageString(string argPercentage, string argPercentageDecimalPlace, string argPercentageRoundAsking)
        {
            try
            {
                if (argPercentage != null && argPercentage != "")
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argPercentageDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argPercentage), Convert.ToInt32(Convert.ToDecimal(argPercentageRoundAsking))));
                }
                else
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argPercentageDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(argPercentageRoundAsking))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static string getPercentageString(string argPercentage)
        {
            try
            {
                if (argPercentage != null && argPercentage != "")
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.PercentageDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argPercentage), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.PercentageRoundAsk))));
                }
                else
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.PercentageDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.PercentageRoundAsk))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static decimal getPercentageDecimal(string argPercentage, string argPercentageDecimalPlace, string argPercentageRoundAsking)
        {
            try
            {
                if (argPercentage != null && argPercentage != "")
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argPercentageDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argPercentage), Convert.ToInt32(Convert.ToDecimal(argPercentageRoundAsking)))));
                }
                else
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argPercentageDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(argPercentageRoundAsking)))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static decimal getPercentageDecimal(string argPercentage)
        {
            try
            {
                if (argPercentage != null && argPercentage != "")
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.PercentageDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argPercentage), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.PercentageRoundAsk)))));
                }
                else
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.PercentageDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.PercentageRoundAsk)))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        #endregion

        #region "Tax"
        public static string getTaxString(string argTax, string argTaxDecimalPlace, string argTaxRoundAsking)
        {
            try
            {
                if (argTax != null && argTax != "")
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argTaxDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argTax), Convert.ToInt32(Convert.ToDecimal(argTaxRoundAsking))));
                }
                else
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argTaxDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(argTaxRoundAsking))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static string getTaxString(string argTax)
        {
            try
            {
                if (argTax != null && argTax != "")
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.TaxDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argTax), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.TaxRoundAsk))));
                }
                else
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.TaxDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.TaxRoundAsk))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static decimal getTaxDecimal(string argTax, string argTaxDecimalPlace, string argTaxRoundAsking)
        {
            try
            {
                if (argTax != null && argTax != "")
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argTaxDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argTax), Convert.ToInt32(Convert.ToDecimal(argTaxRoundAsking)))));
                }
                else
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argTaxDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(argTaxRoundAsking)))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static decimal getTaxDecimal(string argTax)
        {
            try
            {
                if (argTax != null && argTax != "")
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.TaxDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argTax), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.TaxRoundAsk)))));
                }
                else
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.TaxDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.TaxRoundAsk)))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        #endregion


        #region "GrandTotal"
        public static string getGrandTotalString(string argGrandTotal, string argGrandTotalDecimalPlace, string argGrandTotalRoundAsking)
        {
            try
            {
                if (argGrandTotal != null && argGrandTotal != "")
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argGrandTotalDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argGrandTotal), Convert.ToInt32(Convert.ToDecimal(argGrandTotalRoundAsking))));
                }
                else
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argGrandTotalDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(argGrandTotalRoundAsking))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static string getGrandTotalString(string argGrandTotal)
        {
            try
            {
                if (argGrandTotal != null && argGrandTotal != "")
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.GrandTotalDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argGrandTotal), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.GrandTotalRoundAsk))));
                }
                else
                {
                    return string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.GrandTotalDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.GrandTotalRoundAsk))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static decimal getGrandTotalDecimal(string argGrandTotal, string argGrandTotalDecimalPlace, string argGrandTotalRoundAsking)
        {
            try
            {
                if (argGrandTotal != null && argGrandTotal != "")
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argGrandTotalDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argGrandTotal), Convert.ToInt32(Convert.ToDecimal(argGrandTotalRoundAsking)))));
                }
                else
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(argGrandTotalDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(argGrandTotalRoundAsking)))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public static decimal getGrandTotalDecimal(string argGrandTotal)
        {
            try
            {
                if (argGrandTotal != null && argGrandTotal != "")
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.GrandTotalDecimalPlace))}}}", Decimal.Round(Convert.ToDecimal(argGrandTotal), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.GrandTotalRoundAsk)))));
                }
                else
                {
                    return Convert.ToDecimal(string.Format($"{{0:F{Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.GrandTotalDecimalPlace))}}}", Decimal.Round(new decimal(0), Convert.ToInt32(Convert.ToDecimal(Common.mCommon.UserSetting.GrandTotalRoundAsk)))));
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        #endregion


        #region 
        public static byte[] GetImageBytes(Stream stream)
        {
            byte[] ImageBytes;
            using (var memoryStream = new System.IO.MemoryStream())
            {
                stream.CopyTo(memoryStream);
                ImageBytes = memoryStream.ToArray();
            }
            return ImageBytes;
        }

        #endregion




        public static T Clone<T>(T source)
        {
            try
            {
                var serialized = JsonConvert.SerializeObject(source);
                return JsonConvert.DeserializeObject<T>(serialized);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

    }
}
