using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.DBHelpers;
using Microsoft.EntityFrameworkCore;

namespace Common.Behavior
{
    public class WorkCheckHolder : IDisposable
    {
        private CheckContext _cont { get; set; }

        public WorkCheckHolder(string connectionString)
        {
            _cont = new CheckContext(connectionString);
        }

        public WorkCheckHolder(CheckContext cont)
        {
            _cont = cont;
        }

        private DateTime UnixTimeToDateTime(int unix)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unix).ToLocalTime();
            return dtDateTime;
        }

        public async Task<Message> AddWorkLine(string usrlogin, string wlname)
        {
            try {
                var user = _cont.Users.Include(u => u.WorkLines).FirstOrDefault(x => x.Login == usrlogin);
                if (user == null)
                    return new Message { Code = MessageCode.error, Text = "This user is not exist" };

                if (string.IsNullOrWhiteSpace(wlname))
                    return new Message { Code = MessageCode.error, Text = "Work line's name cannot be empty" };

                var thisWorkLine = _cont.WorkLines.FirstOrDefault(x => x.UserID == user.UserID && x.Name == wlname);

                if (thisWorkLine != null)
                    return new Message { Code = MessageCode.error, Text = "This work line already exist" };

                _cont.WorkLines.Add(new WorkLine { Name = wlname, User = user });
                await _cont.SaveChangesAsync();

                thisWorkLine = _cont.WorkLines.FirstOrDefault(x => x.UserID == user.UserID && x.Name == wlname);

                return new Message { Code = MessageCode.success, Text = $"Workline {wlname} added successfully", Data=user.WorkLines };
            }
            catch (Exception exc)
            {
                return new Message { Code = MessageCode.error, Text = exc.Message, Data = exc.StackTrace};
            }
        }
        public async Task<Message> AddPeriod(string usrlogin, string wlname, int tstart, int tfinish)
        {
            try
            {
                var timeStart = UnixTimeToDateTime(tstart);
                var timeFinish = UnixTimeToDateTime(tfinish);

                if (timeStart == null && timeFinish == null)
                    return new Message { Code = MessageCode.error, Text = "invalid parameters :("};


                var user = _cont.Users.FirstOrDefault(x => x.Login == usrlogin);

                if (user == null)
                    return new Message { Code = MessageCode.error, Text = "this user is not exist" };

                var workLine = _cont.WorkLines.FirstOrDefault(x => x.UserID == user.UserID && x.Name == wlname);

                if (workLine == null)
                    return new Message { Code = MessageCode.error, Text = $"this workline is not exist for {user.Login}" };
                //TODO разделить периоды, если они затрагивают 2 дня
                _cont.Periods.Add(new Period { Start = timeStart, Finish = timeFinish, WorkLine = workLine});

                await _cont.SaveChangesAsync();

                return new Message { Code = MessageCode.success, Text = $"Period added successfully"};
            }
            catch (Exception exc)
            {
                return new Message { Code = MessageCode.error, Text = exc.Message, Data = exc.StackTrace };
            }
        }

        public async Task<Message> GetDayPeriods(string usrlogin, string wlname)
        {
            try
            {
                var user = _cont.Users.FirstOrDefault(x => x.Login == usrlogin);

                if (user == null)
                    return new Message { Code = MessageCode.error, Text = "this user is not exist" };

                var thisWorkLine = _cont.WorkLines.Include(wl => wl.Periods).FirstOrDefault(x => x.UserID == user.UserID && x.Name == wlname);

                if (thisWorkLine == null)
                    return new Message { Code = MessageCode.error, Text = $"this workline is not exist for {user.Login}" };
                
                var todayperiods = thisWorkLine.Periods.Where(x => x.Finish.Date == DateTime.Now.Date).OrderBy(x => x.Start).ToList();
                var periods = todayperiods.Select(x => new {
                    tstart = $"{x.Start.Hour}:{x.Start.Minute}",
                    tfinish = $"{x.Finish.Hour}:{x.Finish.Minute}",
                    duration = $"{x.Finish.Subtract(x.Start).Hours}:{x.Finish.Subtract(x.Start).Minutes}",
                }).ToArray();

                TimeSpan sum, lazy;
                for (int i = 0; i < todayperiods.Count; i++)
                {
                    if (i != 0)
                        lazy += todayperiods[i].Start.Subtract(todayperiods[i-1].Finish);
                    sum += todayperiods[i].Finish.Subtract(todayperiods[i].Start);
                }
                return new Message { Code = MessageCode.success, Text = "Here u r",
                    Data = new {
                        row = periods,
                        res = new {
                            sum = $"{sum.Hours}:{sum.Minutes}",
                            lazy = $"{lazy.Hours}:{lazy.Minutes}"
                        }
                    }
                };
            }
            catch (Exception exc)
            {
                return new Message { Code = MessageCode.error, Text = exc.Message, Data = exc.StackTrace };
            }
        }

        public async Task<Message> GetWeekPeriods(string usrlogin, string wlname)
        {
            try
            {
                var user = _cont.Users.FirstOrDefault(x => x.Login == usrlogin);

                if (user == null)
                    return new Message { Code = MessageCode.error, Text = "this user is not exist" };

                var thisWorkLine = _cont.WorkLines.Include(wl => wl.Periods).FirstOrDefault(x => x.UserID == user.UserID && x.Name == wlname);

                if (thisWorkLine == null)
                    return new Message { Code = MessageCode.error, Text = $"this workline is not exist for {user.Login}" };

                var lev = DateTime.Now.AddDays(-7);

                var weekperiods = thisWorkLine.Periods.Where(x => x.Finish.Date > lev).OrderBy(x => x.Start).ToList();
                var periods = weekperiods.Select(x => new {
                    tstart = $"{x.Start.Hour}:{x.Start.Minute}",
                    tfinish = $"{x.Finish.Hour}:{x.Finish.Minute}",
                    duration = $"{x.Finish.Subtract(x.Start).Hours}:{x.Finish.Subtract(x.Start).Minutes}",
                }).ToArray();

                TimeSpan sum, lazy;
                for (int i = 0; i < weekperiods.Count; i++)
                {
                    if (i != 0)
                        lazy += weekperiods[i].Start.Subtract(weekperiods[i - 1].Finish);
                    sum += weekperiods[i].Finish.Subtract(weekperiods[i].Start);
                }
                return new Message
                {
                    Code = MessageCode.success,
                    Text = "Here u r",
                    Data = new
                    {
                        row = periods,
                        res = new
                        {
                            sum = $"{sum.Hours}:{sum.Minutes}",
                            lazy = $"{lazy.Hours}:{lazy.Minutes}"
                        }
                    }
                };
            }
            catch (Exception exc)
            {
                return new Message { Code = MessageCode.error, Text = exc.Message, Data = exc.StackTrace };
            }
        }

        public async Task<Message> GetMonthPeriods(string usrlogin, string wlname)
        {
            try
            {
                var user = _cont.Users.FirstOrDefault(x => x.Login == usrlogin);

                if (user == null)
                    return new Message { Code = MessageCode.error, Text = "this user is not exist" };

                var thisWorkLine = _cont.WorkLines.Include(wl => wl.Periods).FirstOrDefault(x => x.UserID == user.UserID && x.Name == wlname);

                if (thisWorkLine == null)
                    return new Message { Code = MessageCode.error, Text = $"this workline is not exist for {user.Login}" };

                var lev = DateTime.Now.AddDays(-30);

                var weekperiods = thisWorkLine.Periods.Where(x => x.Finish.Date > lev).OrderBy(x => x.Start).ToList();
                var periods = weekperiods.Select(x => new {
                    tstart = $"{x.Start.Hour}:{x.Start.Minute}",
                    tfinish = $"{x.Finish.Hour}:{x.Finish.Minute}",
                    duration = $"{x.Finish.Subtract(x.Start).Hours}:{x.Finish.Subtract(x.Start).Minutes}",
                }).ToArray();

                TimeSpan sum, lazy;
                for (int i = 0; i < weekperiods.Count; i++)
                {
                    if (i != 0)
                        lazy += weekperiods[i].Start.Subtract(weekperiods[i - 1].Finish);
                    sum += weekperiods[i].Finish.Subtract(weekperiods[i].Start);
                }
                return new Message
                {
                    Code = MessageCode.success,
                    Text = "Here u r",
                    Data = new
                    {
                        row = periods,
                        res = new
                        {
                            sum = $"{sum.Hours}:{sum.Minutes}",
                            lazy = $"{lazy.Hours}:{lazy.Minutes}"
                        }
                    }
                };
            }
            catch (Exception exc)
            {
                return new Message { Code = MessageCode.error, Text = exc.Message, Data = exc.StackTrace };
            }
        }

        public void Dispose()
        {
            _cont.Dispose();
        }
    }
}
