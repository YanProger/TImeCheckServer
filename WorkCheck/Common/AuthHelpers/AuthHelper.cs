using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.DBHelpers;

namespace Common.AuthHelpers
{
    public class AuthHelper: IDisposable
    {
        private CheckContext _cont { get; set; }
        public AuthHelper(string connectionString)
        {
            _cont = new CheckContext(connectionString);
        }
        
        public async Task<Message> Authorize(string login, string password)
        {
            try
            {
                var user = _cont.Users.FirstOrDefault(x => x.Mail == login);
                if (user == null)
                    user = _cont.Users.FirstOrDefault(x => x.Login == login);

                if (user == null)
                {
                    return new Message { Code = MessageCode.error, Text = "This user is not exist" };
                }

                if (user.Password != password)
                {
                    return new Message { Code = MessageCode.error, Text = "Wrong password" };
                }

                return new Message { Code = MessageCode.success, Text = "You are logged in", Data = user };
            }
            catch(Exception exc)
            {
                return new Message { Code = MessageCode.error, Text = exc.Message, Data = exc.StackTrace };
            }
        }

        public async Task<Message> Register(string mail, string login, string password)
        {
            try
            {
                if (_cont.Users.FirstOrDefault(x => x.Mail == mail) != null)
                {
                    return new Message { Code = MessageCode.error, Text = "This mail is already in use" };
                }

                if (_cont.Users.FirstOrDefault(x => x.Login == login) != null)
                {
                    return new Message { Code = MessageCode.error, Text = "This login is already in use" };
                }

                _cont.Users.Add(new User { Login = login, Mail = mail, Password = password });

                await _cont.SaveChangesAsync();

                return new Message { Code = MessageCode.success, Text = "new user added", Data = _cont.Users.FirstOrDefault(x => x.Mail == mail) };
            }
            catch(Exception exc)
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
