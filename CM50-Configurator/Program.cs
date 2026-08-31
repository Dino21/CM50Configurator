using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CM50_Configurator.Support.UI;

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

            bool backToMenu;
            do
            {
                backToMenu = false;

                using (var start = new StartForm())
                {
                    if (start.ShowDialog() != DialogResult.OK || start.SelectedFormFactory == null)
                    {
                        break;
                    }

                    using (Form module = start.SelectedFormFactory())
                    {
                        Application.Run(module);

                        // Il bottone "Menu principale" nell'header del modulo chiude il form con
                        // questo DialogResult per chiedere di ri-mostrare lo StartForm invece di
                        // terminare l'applicazione (vedi SesamiTheme.AddHeaderBanner).
                        backToMenu = module.DialogResult == SesamiTheme.BackToMenuResult;
                    }
                }
            }
            while (backToMenu);
        }
    }
}
