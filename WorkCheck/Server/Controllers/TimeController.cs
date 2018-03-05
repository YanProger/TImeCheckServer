using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.AuthHelpers;
using Common.DBHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
        public async Task<IActionResult> Get(string test)
        {
            try
            {
                using (var cont = new CheckContext(Configuration["ConnectionStrings:database"]))
                {
                    cont.Database.EnsureCreated();
                    cont.Users.Add(new User { Login = "Yan", Mail = "losevyan@gmail.com", Password = "123qwe" });
                    cont.SaveChanges();

                }
            } catch(Exception exc)
            {

            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> SetStartTime(int tstart)
        {
            //do work
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> SetFinishTime(int tfinish)
        {
            //do work
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> AddWorkLine(string usrmail, string wlname)
        {
            //do work
            return Ok();
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
        public async Task<IActionResult> GetReport(string usrmail, int period)
        {
            //do work
            return Ok();
        }
    }
}