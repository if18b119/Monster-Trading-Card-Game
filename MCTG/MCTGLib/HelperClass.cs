using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTGclass
{
    public static class HelperClass
    {
        public static long GetTimeStamp(DateTime value)
        {
            return Convert.ToInt64( value.ToString("yyyyMMddHHmmssffff"));

        }
    }
}
