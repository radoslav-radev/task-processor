namespace Radoslav
{
    /// <summary>
    /// Provides properties for getting information about the computer's memory.
    /// </summary>
    public static class ComputerInfo
    {
        /// <summary>
        /// Gets the total amount of physical memory for the computer.
        /// </summary>
        /// <value>The total amount of physical memory for the computer.</value>
        public static ulong TotalPhysicalMemory
        {
            get
            {
                return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
            }
        }
    }
}