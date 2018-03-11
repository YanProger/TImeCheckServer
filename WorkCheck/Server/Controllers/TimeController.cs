using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.AuthHelpers;
using Common.DBHelpers;
using Common.Behavior;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Common;

namespace Server.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TimeController : Controller
    {
        public static IConfiguration Configuration { get; set; }
        public TimeController()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        [HttpGet]
        public async Task<IActionResult> CreateDB(string test)
        {
            try
            {
                using (var cont = new CheckContext(Configuration["ConnectionStrings:database"]))
                {
                    cont.Database.EnsureCreated();
                    cont.Users.Add(new User { Login = "Temp", Mail = "temp@temp.com", Password = "temp" });
                    await cont.SaveChangesAsync();
                    cont.Users.Remove(cont.Users.FirstOrDefault(x => x.Login == "Temp" && x.Mail == "temp@temp.com" && x.Password == "temp" ));
                    await cont.SaveChangesAsync();
                }
                return Ok(new Message {Code = MessageCode.error, Text=$"Database has been successfully created"});
            } catch(Exception exc)
            {
                return Ok(new Message {Code = MessageCode.error, Text=$"There was some troubles while creating a DataBase. Error:{exc.Message}", Data=exc.StackTrace});
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> SetPeriod(string usrlogin, string wlname, int tstart, int tfinish)
        {
            using (var holder = new WorkCheckHolder(Configuration["ConnectionStrings:database"]))
            {
                return Ok(await holder.AddPeriod(usrlogin, wlname, tstart, tfinish));
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddWorkLine(string usrlogin, string wlname)
        {
            using(var holder = new WorkCheckHolder(Configuration["ConnectionStrings:database"]))
            {
                return Ok(await holder.AddWorkLine(usrlogin, wlname));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Registration(string mail, string login, string password)
        {
            using (var auth = new AuthHelper(Configuration["ConnectionStrings:database"]))
            {
                return Ok(await auth.Register(mail, login, password));
            }
        }
            
        [HttpGet]
        public async Task<IActionResult> Authorization(string login, string password)
        {
            using (var auth = new AuthHelper(Configuration["ConnectionStrings:database"]))
            {
                return Ok(await auth.Authorize(login, password));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetReport(string usrlogin, string wlname, int period)
        {
            using (var holder = new WorkCheckHolder(Configuration["ConnectionStrings:database"]))
            {
                switch(period)
                {
                    case (int)PeriodTypes.Day:
                        return Ok(await holder.GetDayPeriods(usrlogin, wlname));
                    case (int)PeriodTypes.Week:
                        return Ok(await holder.GetWeekPeriods(usrlogin, wlname));
                    case (int)PeriodTypes.Month:
                        return Ok(await holder.GetMonthPeriods(usrlogin, wlname));
                    default:
                        return Ok(new Message {  Code = MessageCode.error, Text = "Wrong period"});
                }
            }
        }
    }
}