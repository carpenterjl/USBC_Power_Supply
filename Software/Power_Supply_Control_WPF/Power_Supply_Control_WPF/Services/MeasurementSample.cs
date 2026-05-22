using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Supply_Control_WPF.Services
{
    public class MeasurementSample
    {
        public DateTime Timestamp
        {
            get;
            set;
        }

        public float Voltage
        {
            get;
            set;
        }

        public float Current
        {
            get;
            set;
        }

        public float Power
        {
            get;
            set;
        }
    }
}
