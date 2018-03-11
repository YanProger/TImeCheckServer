using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.DBHelpers
{
    public enum PeriodTypes
    {
        Day = 1,
        Week = 2,
        Month = 3
    }

    public class Period
    {
        [Key]
        public int PeriodID { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }

        public int WorkLineID { get; set; }
        public virtual WorkLine WorkLine { get; set; }
    }
}
