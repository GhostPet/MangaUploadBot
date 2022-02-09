using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;

namespace MangaUploadBot
{
    public class User
    {
        public String UserName;
        public String Password;

        public User(String u, String p)
        {
            this.UserName = u;
            this.Password = p;
        }
    }

    public class Sharer
    {
        IWebDriver driver;

        public Sharer (IWebDriver d)
        {
            this.driver = d;
        }

        public void Share (IList<Object> manga, IList<Object> cover, String filename, String filepath, System.ComponentModel.BackgroundWorker backgroundWorker2, int a, int total)
        {
            // Resim Yükleme Sayfasını Aç
            this.driver.Navigate().GoToUrl("https://turktoon.com/wp-admin/media-new.php");
            
            // Resimlerin Konumlarını Listele
            String[] imgdirs = Directory.GetFiles(filepath);

            // Hepsini Sırasıyla Siteye Yükle
            foreach (String imgdir in imgdirs)
            {
                this.driver.FindElement(By.XPath("//input[starts-with(@id,'html5_')]")).SendKeys(imgdir);
            }

            //------- Yükleme Yüzdesi +0,4
            backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * (a + 0.4)) / total));

            // Resimlerin yüklendiği tabloda sıraları bul
            IReadOnlyCollection<IWebElement> rows = this.driver.FindElement(By.Id("media-items")).FindElements(By.ClassName("media-item"));

            // Tek tek yüklemesi bitti mi kontrol et ve linkini al.
            List<string> imglinks = new List<string>();
            foreach (IWebElement row in rows)
            {
                while (true)
                {
                    try
                    {
                        IWebElement pinkynail = row.FindElement(By.ClassName("pinkynail"));
                        imglinks.Add(pinkynail.GetAttribute("src"));
                        break;
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(200);
                    }
                }
            }

            //------- Yükleme Yüzdesi +0,7
            backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * (a + 0.7) / total)));

            // Yükleme Sayfasını Aç
            this.driver.Navigate().GoToUrl("https://turktoon.com/wp-admin/post-new.php?post_type=post&ts_add_chapter=" + manga[0]);

            // Bölüm Bilgilerini Doldur
            this.driver.FindElement(By.CssSelector("#title")).SendKeys("#" + filename);
            this.driver.FindElement(By.Id("ero_chapter")).SendKeys(filename);
            try { this.driver.FindElement(By.ClassName("switch-html")).Click(); }
            catch { System.Threading.Thread.Sleep(10); }

            // Bölüm Kapağını Ekle
            this.driver.FindElement(By.Id("content")).SendKeys(cover[3].ToString() + Keys.Enter + Keys.Enter);
            
            // Bölüm Resimlerini Ekle
            foreach (String imglink in imglinks)
            {
                this.driver.FindElement(By.Id("content")).SendKeys("<img src=\"" + imglink + "\" alt = \"\" class=\"aligncenter size-full\" />" + Keys.Enter + Keys.Enter);
            }

            //------- Yükleme Yüzdesi +0,9
            backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * (a + 0.9) / total)));

            // Paylaş
            this.driver.FindElement(By.Name("publish")).Click();
            System.Threading.Thread.Sleep(200);
        }

    }
}
