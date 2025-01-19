using System;
using System.Windows.Forms;

namespace Speedie
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create an instance of MainForm
            SpeedieForm speedieForm = new SpeedieForm();

            // Run the application
            Application.Run();
        }
    }
}
