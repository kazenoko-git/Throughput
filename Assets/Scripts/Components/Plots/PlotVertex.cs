using Unity.Entities;
using Unity.Mathematics;

namespace Throughput.Plots
{
    public struct PlotVertex : IBufferElementData
    {
        public float2 Value;
    }
}
