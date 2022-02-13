using OpenQA.Selenium.Chrome;
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

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless");

            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;


            ChromeDriver driver = new ChromeDriver(service, options);
            User user = new User("", "");

            // Botu kendinize uyarlamak isterseniz buradakileri değiştirmeniz gerekebilir.
            string credentials = "turktoon-bot-55d1a7b67f2f.json";
            string spreadsheetId = "1-71OojtQ3941aO203ZIYUMFAtAxsYoXSSPCxrvDsRpY";

            Application.Run(new Login(driver, user, credentials));

            if (!user.UserName.Equals("") && !user.Password.Equals(""))
            {
                Application.Run(new MainUi(driver, user, credentials, spreadsheetId));
            }

            driver.Close();
            driver.Quit();
        }
    }
}
