using Unity.Entities;

namespace Throughput.Plots
{
    public struct PlotData : IComponentData
    {
        public ZoningType Zone;
        public float Area;
    }
}
