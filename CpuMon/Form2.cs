using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpuMon
{
    public partial class Form2 : Form
    {
        private readonly ProcessInfo _processInfo;
        private PerformanceCounter _cpuTimeCounter;
        private int _x, _y;
        private CounterSample _preSample = default;

        public Form2(ProcessInfo processInfo, bool topMost)
        {
            _processInfo = processInfo;
            TopMost = topMost;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                _cpuTimeCounter = new PerformanceCounter("Process", "% Processor Time", _processInfo.ProcessName);
                Invoke(new Action(timer1.Start));
            });
            Width = Height = 50;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var sample1 = _cpuTimeCounter.NextSample();
            label1.Text = $"{CounterSample.Calculate(_preSample, sample1)}";
            _preSample = sample1;
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
