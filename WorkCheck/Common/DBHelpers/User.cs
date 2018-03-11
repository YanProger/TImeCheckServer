using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.DBHelpers
{
    public class User
    {
        [JsonIgnore]
        [Key]
        public int UserID { get; set; }
        public string Mail { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public virtual ICollection<WorkLine> WorkLines { get; set; }
    }
}
