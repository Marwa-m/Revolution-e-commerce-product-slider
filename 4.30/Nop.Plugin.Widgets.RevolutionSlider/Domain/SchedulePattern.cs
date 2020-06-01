using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Widgets.RevolutionSlider.Domain
{
    public enum SchedulePattern
    {
        EveryDay = 0,
        EveryMonth = 5,
        OnOddDay = 10,
        OnEvenDay = 15,
        OnMonday = 20,
        OnTuesday = 25,
        OnWednesday = 30,
        OnThursday = 35,
        OnFriday = 40,
        OnSaturday = 45,
        OnSunday = 50,
        FromSundayToThursday = 55,
        FromMondayToFriday = 60,
        OnThursdayAndFriday = 65,
        OnFridayAndSaturday = 70,
        OnSaturdayAndSunday = 75,
    }

}
