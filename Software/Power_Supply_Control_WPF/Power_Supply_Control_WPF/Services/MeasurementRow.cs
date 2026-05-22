using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Supply_Control_WPF.Services
{
    public partial class MeasurementRow : ObservableObject
    {
        public string Name { get; set; }

        [ObservableProperty] private double voltage;
        [ObservableProperty] private double current;
        public double Power => Voltage * Current;
    }
}
