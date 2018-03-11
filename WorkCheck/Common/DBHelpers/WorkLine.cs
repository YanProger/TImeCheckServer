using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.DBHelpers
{
    public class WorkLine
    {
        [Key]
        [JsonIgnore]
        public int WorkLineID { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public int UserID { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual ICollection<Period> Periods { get; set; }
    }
}
