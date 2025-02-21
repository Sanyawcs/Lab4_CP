using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Lab4_CP
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        const uint TH32CS_SNAPPROCESS = 0x00000002;

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        };

        private ListView listView1;

        public Form1()
        {
            InitializeComponent();
            InitializeListView();
            LoadProcesses();
        }

        private void InitializeListView()
        {
            listView1 = new ListView();
            listView1.Dock = DockStyle.Fill;
            listView1.View = View.Details;
            listView1.Columns.Add("Process Name", 200);
            listView1.Columns.Add("Process ID", 100);
            this.Controls.Add(listView1);
        }

        private void LoadProcesses()
        {
            IntPtr hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
            if (hSnapshot == IntPtr.Zero)
            {
                MessageBox.Show("Failed to create snapshot.");
                return;
            }

            PROCESSENTRY32 procEntry = new PROCESSENTRY32();
            procEntry.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));

            if (Process32First(hSnapshot, ref procEntry))
            {
                do
                {
                    ListViewItem item = new ListViewItem(procEntry.szExeFile);
                    item.SubItems.Add(procEntry.th32ProcessID.ToString());
                    listView1.Items.Add(item);
                }
                while (Process32Next(hSnapshot, ref procEntry));
            }

            CloseHandle(hSnapshot);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadProcesses(); // Вызываем метод для загрузки процессов при загрузке формы
        }
    }
}
