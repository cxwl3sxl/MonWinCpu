using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CpuMon
{
    public partial class Form1 : Form
    {
        private readonly List<ProcessInfo> _allList = new List<ProcessInfo>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            Task.Factory.StartNew(() =>
            {
                var ps = Process.GetProcesses();
                foreach (var process in ps)
                {
                    try
                    {
                        if (process.HasExited) continue;
                        var item = new ProcessInfo(process);
                        _allList.Add(item);
                        Invoke(new Action(() =>
                        {
                            listBox1.Items.Add(item);
                        }));
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning($"加载进程信息出错:{ex}");
                    }
                }

                Invoke(new Action(() =>
                {
                    button1.Text = "开始监控";
                    button1.Enabled = true;
                    textBox1_KeyUp(null, null);
                }));
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!(listBox1.SelectedItem is ProcessInfo pi))
            {
                MessageBox.Show("请选择需要监控的进程");
                return;
            }

            var form2 = new Form2(pi, checkBox1.Checked);
            form2.Closed += Form2_Closed;
            WindowState = FormWindowState.Minimized;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                Invoke(new Action(Hide));
            });
            form2.Show();
        }

        private void Form2_Closed(object sender, EventArgs e)
        {
            if (!(sender is Form form)) return;
            form.Closed -= Form2_Closed;
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text)) return;
            listBox1.Items.Clear();
            foreach (var info in _allList)
            {
                if (!info.ProcessName.StartsWith(textBox1.Text, StringComparison.OrdinalIgnoreCase)) continue;
                listBox1.Items.Add(info);
            }
        }
    }
}
