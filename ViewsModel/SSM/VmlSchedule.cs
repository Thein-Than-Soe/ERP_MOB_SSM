using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.Messaging;
using CS.ERP.PL.HCM.DAT;
using CS.ERP.PL.HCM.REQ;
using CS.ERP.PL.HCM.RES;
using CS.ERP.PL.JOB.REQ;
using CS.ERP.PL.JOB.RES;
using CS.ERP.PL.PMA_API.DAT;
using CS.ERP.PL.POS.DAT;
using CS.ERP.PL.POS.REQ;
using CS.ERP.PL.POS.RES;
using CS.ERP.PL.SYS.DAT;
using CS.ERP.PL.SYS.REQ;
using CS.ERP_MOB.Extensions;
using CS.ERP_MOB.General;
using CS.ERP_MOB.Services.SSM;
using CS.ERP_MOB.Services.SYS;
using CS.ERP_MOB.ViewsModel.Frame;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using static CS.ERP_MOB.General.Utility;

namespace CS.ERP_MOB.ViewsModel.SSM
{
    public class VmlSchedule : BaseViewModel
    {

    }
    public class CalendarMonth
    {
        public DateTime Month { get; set; }

        public string MonthText { get; set; }

        public int WeekCount { get; set; }

        public double CalendarHeight =>
            WeekCount * 50;

        public List<CalendarDay> Days { get; set; }
            = new();
    }


    public class CalendarDay
    {
        public DateTime? Date { get; set; }

        public bool IsCurrentMonth { get; set; }

        public bool IsToday { get; set; }

        public bool IsSelected { get; set; }

        public bool HasEvent { get; set; }

        public string DayNumber =>
            Date.HasValue
                ? Date.Value.Day.ToString()
                : string.Empty;
    }


    public class CalendarItem
    {
        public DateTime Date { get; set; }

        public string Time { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }
    }
}
