using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.DBHelpers;


namespace Common.Behavior
{
    public class WorkCheckHolder : IDisposable
    {
        private CheckContext _cont { get; set; }

        public WorkCheckHolder(string connectionString)
        {
            _cont = new CheckContext(connectionString);
        }

        public void Dispose()
        {
            _cont.Dispose();
        }

        public async Task<Message> AddWorkLine(string usrmail, string wlname)
        {
            //TODO не окончена   
            return new Message{Code = MessageCode.success, Text = ""};
        }
    }
}
