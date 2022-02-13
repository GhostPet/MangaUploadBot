using OpenQA.Selenium.Chrome;

namespace MangaUploadBot
{
    public class Driver
    {
        public ChromeDriver driver;

        public Driver(bool hidden)
        {
            ChromeOptions options = new ChromeOptions();
            if (hidden) options.AddArgument("headless");
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            this.driver = new ChromeDriver(service, options);
        }

        public void Exit()
        {
            this.driver.Close();
            this.driver.Quit();
        }
    }
}
