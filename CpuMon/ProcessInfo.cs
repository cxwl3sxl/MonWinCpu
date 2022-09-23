using System.Diagnostics;

namespace CpuMon
{
    public class ProcessInfo
    {
        public ProcessInfo(Process p)
        {
            Id = p.Id;
            ProcessName = p.ProcessName;
        }

        public int Id { get; set; }

        public string ProcessName { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {ProcessName}";
        }
    }
}