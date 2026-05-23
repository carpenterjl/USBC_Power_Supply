using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Supply_Control_WPF.Services
{
    public class AxisManager : IAxisLimitManager
    {
        public AxisLimits GetLimits(AxisLimits dataLimits, AxisLimits currentLimits) 
        {
            CoordinateRange xRange = GetRangeX(currentLimits.XRange, dataLimits.XRange);
            CoordinateRange yRange = GetRangeY(currentLimits.YRange, dataLimits.YRange);
            return new AxisLimits(xRange, yRange);
        }

        public CoordinateRange GetRangeX(CoordinateRange viewRangeX, CoordinateRange dataRangeX)
        {
            //Display last 30 seconds
            CoordinateRange coordinates = new CoordinateRange(dataRangeX.Value2 - 30000, dataRangeX.Value2);
            return coordinates;
        }

        public CoordinateRange GetRangeY(CoordinateRange viewRangeY, CoordinateRange dataRangeY)
        {
            double dataSpan = dataRangeY.Value2 - dataRangeY.Value1;

            if (dataSpan == 0)
            {
                dataSpan = 1.0;
            }

            double padding = dataSpan * 0.1;

            double yMin = dataRangeY.Value1 - padding;
            double yMax = dataRangeY.Value2 + padding;

            return new CoordinateRange(yMin, yMax);
        }
    }
}
