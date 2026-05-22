using CommunityToolkit.Mvvm.ComponentModel;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Supply_Control_WPF.Services
{
    public partial class PlotTrace : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private bool visible;

        public ScottPlot.Color TraceColor { get; set; }


        public DataLogger? Logger
        {
            get;
            set;
        }

        public DataLogger? PopUpLogger
        {
            get;
            set;
        }

        public void AddPoint(double x,double y)
        {
            Logger!.Add(y);

            if(PopUpLogger != null)
                PopUpLogger!.Add(y);
        }

        public void Clear()
        {
            Logger!.Clear();
            if (PopUpLogger != null)
                PopUpLogger!.Clear();
        }
    }
}
