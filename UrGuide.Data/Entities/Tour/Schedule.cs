using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace UrGuide.Data.Entities.Tour
{
    [Owned]
    public class Schedule
    {
        public RecurringType Type { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveUntil { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public DateTime? NextRun { get; set; }

        public bool Active => DateTime.UtcNow.Date >= ActiveUntil.Date;

        public void Reevaluate()
        {
            if (!NextRun.HasValue)
            {
                NextRun = ActiveFrom.Date;
                return;
            }

            static DateTime GetNextMonday(DateTime day)
            {
                return day.DayOfWeek == DayOfWeek.Monday ? day.AddDays(7) : day.AddDays(Math.Abs((7 - (int)day.DayOfWeek + (int)DayOfWeek.Monday) % 7));
            }

            if (Active)
            {
                var weekEnds = new[] { DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };
                NextRun = Type switch
                {
                    RecurringType.Daily => NextRun.Value.Add(TimeSpan.FromDays(1)),
                    RecurringType.WorkingDays => weekEnds.Contains(NextRun.Value.DayOfWeek)
                    ? GetNextMonday(NextRun.Value) :
                    NextRun.Value.AddDays(1),
                    RecurringType.Weekly => NextRun.Value.AddDays(7),
                    RecurringType.WeekEnds => NextRun.Value.DayOfWeek == DayOfWeek.Sunday ?
                        NextRun.Value.AddDays(6) : NextRun.Value.DayOfWeek != DayOfWeek.Saturday ?
                        NextRun.Value.AddDays(6 - (int)NextRun.Value.DayOfWeek) :
                        NextRun.Value.AddDays(1),
                    _ => NextRun.Value
                };
            }
        }
    }

}
