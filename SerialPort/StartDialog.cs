using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace SerialPort
{
    public partial class StartDialog : Form
    {
        private Serial SerialPort { get; set; }

        public StartDialog() => InitializeComponent();

        private void StartDialog_Load(object sender, EventArgs e)
        {
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                SerialPort = new Serial(textBox1.Text, 9600, Parity.None, 8, StopBits.One);
                SerialPort.Open();
                var chat = new Chat(SerialPort);
                chat.Show();
                Hide();
            }
            catch (Exception)
            {
                textBox1.Clear();
                textBox1.AppendText("Wrong name, try again");
            }
        }

        private void Button2_Click(object sender, EventArgs e) => Close();

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter)
            {
                Button1_Click(sender,e);
            }
        }
    }
}