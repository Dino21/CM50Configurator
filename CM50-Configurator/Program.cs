using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CM50_Configurator
{
    internal static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var start = new StartForm())
            {
                if (start.ShowDialog() == DialogResult.OK && start.SelectedFormFactory != null)
                {
                    Application.Run(start.SelectedFormFactory());
                }
            }
        }
    }
}
