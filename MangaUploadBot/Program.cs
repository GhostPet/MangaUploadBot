using System;
using System.Windows.Forms;

namespace MangaUploadBot
{
    internal static class Program
    {
        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string version = "1.1.0";

            // Botu kendinize uyarlamak isterseniz buradakileri değiştirmeniz gerekebilir.
            string credentials = "turktoon-bot-333a3035d81b.json";
            string spreadsheetId = "1-71OojtQ3941aO203ZIYUMFAtAxsYoXSSPCxrvDsRpY";

            GoogleApi googleapi = new GoogleApi(credentials, spreadsheetId);
            if (!googleapi.IsCredentialExists) return;

            Driver driver = new Driver(true);
            User user = new User();
            Application.Run(new Login(driver, user));
            if (user.LoggedIn) Application.Run(new MainUi(user, googleapi, driver, version));
            driver.Exit();
        }
    }
}
