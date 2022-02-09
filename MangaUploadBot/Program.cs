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

            Application.Run(new Login(driver, user));

            if (!user.UserName.Equals("") && !user.Password.Equals(""))
            {
                Application.Run(new MainUi(driver, user));
            }

            driver.Close();
            driver.Quit();
        }
    }
}
