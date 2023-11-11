namespace EmpyrionModdingFramework.Teleport
{
    public class PortalOffsetRecord
    {
        public PortalOffsetRecord() { } // For CsvHelper?

        /// <summary>
        /// The name of the PortalRecord entry
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The radius of the circle to build points on
        /// </summary>
        public int Radius { get; set; }
        
        /// <summary>
        /// How many warp in points we should make on the created circle
        /// </summary>

        public int Count { get; set; }
    }
}
