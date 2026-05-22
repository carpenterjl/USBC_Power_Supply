using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Supply_Control_WPF.Services
{
    public class PlotSource
    {
        public string Name
        {
            get;
            set;
        }

        public ObservableCollection<PlotTrace> Traces { get; }  = new();

        public PlotTrace VoltageTrace => Traces[0];
        public PlotTrace CurrentTrace => Traces[1];
        public PlotTrace PowerTrace  => Traces[2];
    }
}
