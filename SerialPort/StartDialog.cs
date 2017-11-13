using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace SerialPort
{
    public partial class StartDialog : Form
    {
        public int Num;

        public StartDialog()
        {
            InitializeComponent();
        }

        public Serial InputPort { get; set; }
        public Serial OutputPort { get; set; }

        private void StartDialog_Load(object sender, EventArgs e)
        {
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                InputPort = new Serial(textBox1.Text, 9600, Parity.None, 8, StopBits.One);
                InputPort.Open();
                OutputPort = new Serial(textBox2.Text, 9600, Parity.None, 8, StopBits.One);
                OutputPort.Open();
                Num = Convert.ToInt32(textBox3.Text);
                if (Num < 1 && Num > 4) throw new Exception();
                var chat = new Chat(InputPort, OutputPort, Num);
                chat.Show();
                Hide();
            }
            catch (Exception)
            {
                textBox1.Clear();
                textBox1.AppendText("Wrong name, try again");
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}