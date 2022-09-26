using System;
using System.Diagnostics;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpuMon
{
    public partial class Form2 : Form
    {
        private readonly ProcessInfo _processInfo;
        private ManagementObjectSearcher _mos;
        private int _x, _y;
        DateTime _firstSample = DateTime.MinValue, _secondSample = DateTime.MinValue;
        ulong _uOldCpu = 0;
        private double _msPassed;

        public Form2(ProcessInfo processInfo, bool topMost)
        {
            _processInfo = processInfo;
            TopMost = topMost;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(label1, $"正在监控{_processInfo.ProcessName}的CPU使用率，双击退出");

            Task.Factory.StartNew(() =>
            {
                var objQuery = new ObjectQuery("select * from Win32_Process WHERE ProcessID = " + _processInfo.Id);
                _mos = new ManagementObjectSearcher(objQuery);
                Invoke(new Action(timer1.Start));
            });
            Width = Height = 50;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var gets = _mos.Get();
            foreach (ManagementObject mObj in gets)
            {
                try
                {
                    if (_firstSample == DateTime.MinValue)
                    {
                        _firstSample = DateTime.Now;
                        mObj.Get();
                        _uOldCpu = (ulong)mObj["UserModeTime"] + (ulong)mObj["KernelModeTime"];
                    }
                    else
                    {
                        _secondSample = DateTime.Now;
                        mObj.Get();
                        ulong u_newCPU = (ulong)mObj["UserModeTime"] + (ulong)mObj["KernelModeTime"];

                        _msPassed = (_secondSample - _firstSample).TotalMilliseconds;
                        var pu = (u_newCPU - _uOldCpu) / (_msPassed * 100.0 * Environment.ProcessorCount);

                        _uOldCpu = u_newCPU;
                        _firstSample = _secondSample;
                        label1.Text = $"{pu:0}";
                    }

                }
                catch (Exception ex)
                {
                    Trace.Write(ex.ToString());
                }
            }
        }

        private void label1_DoubleClick(object sender, EventArgs e)
        {
            Close();
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            //获取鼠标左键按下时的位置
            _x = e.Location.X;
            _y = e.Location.Y;
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            //计算鼠标移动距离
            Left += e.Location.X - _x;
            Top += e.Location.Y - _y;
        }
    }
}
