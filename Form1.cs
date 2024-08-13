using Sydesoft.NfcDevice;
using System;
using System.Text;
using System.Windows.Forms;

namespace OnlineCardReader_WinForm_NET_
{
    public partial class Form1 : Form
    {
        private static MyACR122U acr122u = new MyACR122U();

        public Form1()
        {
            InitializeComponent();

            // Initialize NFC reader
            try
            {
                acr122u.Init(false, 50, 4, 4, 200);  // NTAG213
                acr122u.CardInserted += Acr122u_CardInserted;
                acr122u.CardRemoved += Acr122u_CardRemoved;
            }
            catch (ArgumentException)
            {
                MessageBox.Show(this, "Failed to find a reader connected to the system", "No reader connected", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Set textBox1 as read-only
            textBox1.ReadOnly = true;
        }

        private static void Acr122u_CardInserted(PCSC.ICardReader reader)
        {
            // Display NFC ID in textBox1
            Form1 form = Application.OpenForms["Form1"] as Form1;
            if (form != null)
            {
                form.Invoke(new Action(() =>
                {
                    acr122u.ReadId = BitConverter.ToString(acr122u.GetUID(reader)).Replace("-", "");
                    form.textBox1.Text = acr122u.ReadId;

                    // Optional: Write and read data from the NFC tag
                    string data = "Hello World";
                    bool ret = acr122u.WriteData(reader, Encoding.UTF8.GetBytes(data));
                    Console.WriteLine("Write result: " + (ret ? "Success" : "Failed"));
                    Console.WriteLine("Read data from tag: " + Encoding.UTF8.GetString(acr122u.ReadData(reader)));
                }));
            }
        }

        private static void Acr122u_CardRemoved()
        {
            // Optional: Handle card removal
            Console.WriteLine("NFC tag removed.");
        }

        // Optional: Add timer if needed
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Update textBox1 with the current NFC ID
            textBox1.Text = acr122u.ReadId;
        }
    }

    internal class MyACR122U : ACR122U
    {
        private string readId;
        public string ReadId
        {
            get { return readId; }
            set { readId = value; }
        }

        public MyACR122U()
        {
        }
    }
}
