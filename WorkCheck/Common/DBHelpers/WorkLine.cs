using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.DBHelpers
{
    public class WorkLine
    {
        [Key]
        public int WorkLineID { get; set; }
        public string Name { get; set; }

        public int UserID { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<Period> Periods { get; set; }
    }
}
