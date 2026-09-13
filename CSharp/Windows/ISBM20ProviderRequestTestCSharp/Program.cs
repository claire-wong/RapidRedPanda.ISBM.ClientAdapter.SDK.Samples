/*
 * Purpose: Application entry point for the ISBM Provider Request sample.
 *
 * Author: Claire Wong
 * Originally created: 2020/08/15
 * Updated: 2026
 *
 * Licensed under the MIT License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ISBM20ProviderRequestTestCSharp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
