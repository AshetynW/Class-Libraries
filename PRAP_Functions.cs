using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace Neuro
{
    class PRAP_Functions
    {
        public string onsetDate { get; set; }
        public string prapAuthCode { get; set; }
        public static string prapRendProvNPI { get; set; }
        public string prapRenderingNPI { get; set; }
        public string prapRendProvName { get; set; }
        public string prapRendProvPFIN { get; set; }
        public string prapRendProvAddr { get; set; }
        public string prapFacilityNPI { get; set; }
        public string prapFacilityName { get; set; }
        public string prapFacilityAddress { get; set; }
        public string prapOtherInsProvName { get; set; }
        public string prapBillingProvPFIN { get; set; }
        public string prapBillingProvNPI { get; set; }
        public string prapBillingProvAddr { get; set; }
        public string prapBillingProvName { get; set; }
        public PRAP_Functions()
        {
            ChromeOptions option = new ChromeOptions();
            //option.AddArgument("--headless");

            IWebDriver driver = new ChromeDriver(option);
            IWebElement elem;
            ICollection<IWebElement> elemList;

            string url = "";
            driver.Navigate().GoToUrl(url);

            Console.WriteLine(driver.Title);
            if (driver.Title == "Enterprise Authentication and Authorication (\"EAA\") - Common Login Page")
            {
                elem = driver.FindElement(By.Name("USER"));
                elem.SendKeys(System.Environment.UserName);
                elem = driver.FindElement(By.Name("PASSWORD"));
                elem.SendKeys(Sessions.GuiSession.myPassword);
                elemList = driver.FindElements(By.TagName("input"));
                foreach (IWebElement e in elemList)
                {
                    if (e.GetAttribute("alt") == "Log In")
                    {
                        e.Click();
                        break;
                    }
                }
            }
            elem = driver.FindElement(By.Id("advancedSearch"));
            elem.Click();
            driver.Navigate().GoToUrl("");

            elem = driver.FindElement(By.Name("documentControlNumber"));
            elem.SendKeys("0201" + IndexedClaim.guiFuncDcn);
            elem = driver.FindElement(By.Id("searchButn"));
            elem.Click();

            elemList = driver.FindElements(By.TagName("a"));
            foreach(IWebElement e in elemList)
            {
                if(inString("0201"+IndexedClaim.guiFuncDcn, e.Text))
                {
                    e.Click();
                    break;
                }
            }

            Console.WriteLine(driver.Title);

            string BaseWindow = driver.CurrentWindowHandle;
            ReadOnlyCollection<string> handles = driver.WindowHandles;
            foreach (string handle in handles)
            {
                if (handle != BaseWindow)
                {
                    string title = driver.SwitchTo().Window(handle).Title;
                    driver.SwitchTo().Window(handle).Title.Equals(title);
                }
            }
            System.Threading.Thread.Sleep(3000);
            Console.WriteLine(driver.Title);

            
            ICollection<IWebElement> LosTables = driver.FindElements(By.ClassName("los"));
            
            prapAuthCode = PRAP_Chrome_FindElementInElement(LosTables, "Treatment Authorization Code:", "td");
            
            elem = driver.FindElement(By.Id("tab3"));
            elem.Click();
            
            onsetDate = PRAP_Chrome_FindElementInElement(LosTables, "Onset Date:", "td");
            

            //prapAuthCode = PRAP_Chrome_FindElementInList(driver, "Treatment Authorization Code:", "td");
            //onsetDate = PRAP_Chrome_FindElementInList(driver, "Onset Date:", "td");
            elem = driver.FindElement(By.Id("tab2"));
            elem.Click();
            
            string[] ElementsList = new string[]
            {
                "Rendering Provider NPI:",
                 "Rendering Provider Name:",
                 "Rendering Provider Number:",
                 "Rendering Provider Address:",
                 "Billing Provider NPI: ",
                 "Service Facility Location Name: ",
                 "Service Facility Location Address: ",
                 "Billing Provider Number: ",
                 "Billing Provider NPI: ",
                 "Billing Provider Address: ",
                 "Billing Provider Name: "
        };
            Console.WriteLine("1");
            //PRAP_Chrome_FindListElementsInElement(LosTables, ElementsList, "td");
            //PRAP_Chrome_FindListElements(driver, ElementsList, "td");
            elemList = driver.FindElements(By.Id("claimTab2"));
            
            prapRendProvNPI = PRAP_Chrome_FindElementInElement(elemList, "Rendering Provider NPI:", "td");
            prapRenderingNPI = prapRendProvNPI;
            prapRendProvName = PRAP_Chrome_FindElementInElement(elemList, "Rendering Provider Name:", "td");
            prapRendProvPFIN = PRAP_Chrome_FindElementInElement(elemList, "Rendering Provider Number:", "td");
            prapRendProvAddr = PRAP_Chrome_FindElementInElement(elemList, "Rendering Provider Address:", "td");
            prapFacilityNPI = PRAP_Chrome_FindElementInElement(elemList, "Billing Provider NPI:", "td");
            prapFacilityName = PRAP_Chrome_FindElementInElement(elemList, "Service Facility Location Name:", "td");
            prapFacilityAddress = PRAP_Chrome_FindElementInElement(elemList, "Service Facility Location Address:", "td");
            prapBillingProvPFIN = PRAP_Chrome_FindElementInElement(elemList, "Billing Provider Number:", "td");
            prapBillingProvNPI = PRAP_Chrome_FindElementInElement(elemList, "Billing Provider NPI:", "td");
            prapBillingProvAddr = PRAP_Chrome_FindElementInElement(elemList, "Billing Provider Address:", "td");
            prapBillingProvName = PRAP_Chrome_FindElementInElement(elemList, "Billing Provider Name:", "td");
            
            Console.WriteLine("2");

            /*
            prapRendProvNPI = PRAP_Chrome_FindElementInList(driver, "Rendering Provider NPI:", "td");
            prapRenderingNPI = prapRendProvNPI;
            prapRendProvName = PRAP_Chrome_FindElementInList(driver, "Rendering Provider Name:", "td");
            prapRendProvPFIN = PRAP_Chrome_FindElementInList(driver, "Rendering Provider Number:", "td");
            prapRendProvAddr = PRAP_Chrome_FindElementInList(driver, "Rendering Provider Address:", "td");
            prapFacilityNPI = PRAP_Chrome_FindElementInList(driver, "Billing Provider NPI: ", "td");
            prapFacilityName = PRAP_Chrome_FindElementInList(driver, "Service Facility Location Name: ", "td");
            prapFacilityAddress = PRAP_Chrome_FindElementInList(driver, "Service Facility Location Address: ", "td");
            prapBillingProvPFIN = PRAP_Chrome_FindElementInList(driver, "Billing Provider Number: ", "td");
            prapBillingProvNPI = PRAP_Chrome_FindElementInList(driver, "Billing Provider NPI: ", "td");
            prapBillingProvAddr = PRAP_Chrome_FindElementInList(driver, "Billing Provider Address: ", "td");
            prapBillingProvName = PRAP_Chrome_FindElementInList(driver, "Billing Provider Name: ", "td");
            */
            //prapOtherInsProvName = PRAP_Chrome_FindElementInList(driver, "Billing Provider Name: ", "td");
            
            driver.Close();
            return;

            //******************************Old**********************************
            SHDocVw.ShellWindows shells = new SHDocVw.ShellWindows();
            PRAP_Begin(IndexedClaim.guiFuncDcn, Sessions.GuiSession.myPassword);
            SHDocVw.InternetExplorer iePrap = IELib.IEGet(shells, "Statistics Header");
            while(iePrap == null)
            {
                iePrap = IELib.IEGet(shells, "Statistics Header");
                if (iePrap != null) { break; }
            }
            IELib.IeWait(iePrap);
            Console.WriteLine("!!!: " + IndexedClaim.guiFuncDcn);
            PRAP_NewClaimSearch(iePrap, shells, IndexedClaim.guiFuncDcn);
            iePrap = IELib.IEGet(shells, "Claim Inquiry Details");
            if (iePrap != null)
            {
                prapAuthCode = PRAP_GetAuthCode(iePrap, shells);
                PRAP_GetOnsetDate(iePrap, shells);
                prapOtherInsProvName = PRAP_GetOtherInsurancePaymentName(iePrap, shells);
                prapAuthCode = PRAP_GetAuthCode(iePrap, shells);
                prapRendProvNPI = PRAP_GetProviderInfo(iePrap, shells, "Rendering Provider NPI:", "0201" + IndexedClaim.guiFuncDcn);
                prapRenderingNPI = prapRendProvNPI;
                prapRendProvName = PRAP_GetProviderInfo(iePrap, shells, "Rendering Provider Name:", "0201" + IndexedClaim.guiFuncDcn);
                prapFacilityNPI = PRAP_GetProviderInfo(iePrap, shells, "Billing Provider NPI:", "0201" + IndexedClaim.guiFuncDcn);
                prapFacilityName = PRAP_GetProviderInfo(iePrap, shells, "Service Facility Location Name:", "0201" + IndexedClaim.guiFuncDcn);
                prapFacilityAddress = PRAP_GetProviderInfo(iePrap, shells, "Service Facility Location Address:", "0201" + IndexedClaim.guiFuncDcn);
                prapBillingProvPFIN = PRAP_GetProviderInfo(iePrap, shells, "Billing Provider Number:", "0201" + IndexedClaim.guiFuncDcn);
                prapBillingProvNPI = PRAP_GetProviderInfo(iePrap, shells, "Billing Provider NPI:", "0201" + IndexedClaim.guiFuncDcn);
                prapBillingProvAddr = PRAP_GetProviderInfo(iePrap, shells, "Billing Provider Address:", "0201" + IndexedClaim.guiFuncDcn);
                prapBillingProvName = PRAP_GetProviderInfo(iePrap, shells, "Billing Provider Name:", "0201" + IndexedClaim.guiFuncDcn);
                prapRendProvPFIN = PRAP_GetProviderInfo(iePrap, shells, "Rendering Provider Number:", "0201" + IndexedClaim.guiFuncDcn);
                prapRendProvAddr = PRAP_GetProviderInfo(iePrap, shells, "Rendering Provider Address:", "0201" + IndexedClaim.guiFuncDcn);

            }

            /*
            if (PRAP_NewClaimSearch(iePrap, shells, IndexedClaim.guiFuncDcn) != false)
            {
                iePrap = IELib.IEGet(shells, "Claim Inquiry Details");
                if (iePrap != null)
                {
                    Console.WriteLine("PRAP 1");
                    prapAuthCode = PRAP_GetAuthCode(iePrap, shells);
                    PRAP_GetOnsetDate(iePrap, shells);
                    Console.WriteLine("PRAP 2");
                    PRAP_GetOtherInsurancePaymentBool(iePrap, shells); Console.WriteLine("PRAP 3");
                }
            }
            */
            PRAP_CloseWindows(shells);
            PRAP_CloseWindows(shells);
        }
        public static string PRAP_Chrome_FindElementInList(IWebDriver driver, string desiredText, string Tag)
        {
            ICollection<IWebElement> elemList = driver.FindElements(By.TagName(Tag));
            bool j = false;

            foreach (IWebElement element in elemList)
            {
                if (j == true)
                {
                    return element.Text;
                }
                if (element.Text == desiredText)
                {
                    j = true;
                }
            }
            return "";
        }
        public static string PRAP_Chrome_FindElementInElement(ICollection<IWebElement> element, string desiredText, string Tag)
        {
            bool j = false;
            foreach(IWebElement elem in element)
            {
                ICollection<IWebElement> ElemList = elem.FindElements(By.TagName(Tag));
                foreach (IWebElement tagElement in ElemList)
                {
                    Console.WriteLine("looking for ." + desiredText + ". in ." + tagElement.Text+".");
                    if (j == true)
                    {
                        Console.WriteLine("!!!!returning " + tagElement.Text + " for " + desiredText);
                        return tagElement.Text;
                    }
                    if (tagElement.Text == desiredText)
                    {
                        j = true;
                    }
                }
            }
            return null;
        }
        public void PRAP_Chrome_FindListElementsInElement(ICollection<IWebElement> Tables, string[] desiredText, string Tag)
        {
            bool j = false;
            string lastStr = "";
            int CountToBreak = 0;
            foreach (IWebElement table in Tables)
            {
                ICollection<IWebElement> TagList = table.FindElements(By.TagName(Tag));
                foreach (IWebElement tagElement in TagList)
                {
                    foreach (string str in desiredText)
                    {
                        if (j == true)
                        {
                            switch (lastStr)
                            {
                                case "Rendering Provider NPI:":
                                    prapRendProvNPI = tagElement.Text;
                                    lastStr = ""; CountToBreak++; j = false;
                                    break;
                                case "Rendering Provider Name:":
                                    prapRendProvName = tagElement.Text;
                                    lastStr = ""; CountToBreak++; j = false;
                                    break;
                                case "Rendering Provider Number:":
                                    prapRendProvPFIN = tagElement.Text;
                                    lastStr = ""; CountToBreak++; j = false;
                                    break;
                                case "Rendering Provider Address:":
                                    prapRendProvAddr = tagElement.Text;
                                    lastStr = ""; CountToBreak++; j = false;
                                    break;
                                case "Billing Provider NPI: ":
                                    prapFacilityNPI = tagElement.Text;
                                    prapBillingProvNPI = tagElement.Text;
                                    lastStr = ""; CountToBreak++; j = false;
                                    break;
                                case "Service Facility Location Name: ":
                                    prapFacilityName = tagElement.Text;
                                    lastStr = ""; CountToBreak++; j = false;
                                    break;
                                case "Service Facility Location Address: ":
                                    prapFacilityAddress = tagElement.Text;
                                    lastStr = ""; CountToBreak++; j = false;
                                    break;
                                case "Billing Provider Number: ":
                                    prapBillingProvPFIN = tagElement.Text;
                                    lastStr = ""; CountToBreak++; j = false;
                                    break;
                                case "Billing Provider Address: ":
                                    prapBillingProvAddr = tagElement.Text;
                                    lastStr = ""; CountToBreak++; j = false;
                                    break;
                                case "Billing Provider Name: ":
                                    prapBillingProvName = tagElement.Text;
                                    lastStr = ""; CountToBreak++; j = false;
                                    break;
                            }
                        }
                        if (tagElement.Text == str)
                        {
                            j = true;
                            lastStr = str;
                            break;
                        }
                        if (CountToBreak == 10)
                        {
                            break;
                        }
                    }
                }
            }
        }
        public void PRAP_Chrome_FindListElements(IWebDriver driver, string[] desiredText, string Tag)
        {
            ICollection<IWebElement> elemList = driver.FindElements(By.TagName(Tag));
            
            bool j = false;
            string lastStr = "";
            int CountToBreak = 0;
            foreach (IWebElement element in elemList)
            {
                foreach(string str in desiredText)
                {
                    if (j == true)
                    {
                        switch (lastStr)
                        {
                            case "Rendering Provider NPI:":
                                prapRendProvNPI = element.Text;
                                lastStr = ""; CountToBreak++;
                                break;
                            case "Rendering Provider Name:":
                                prapRendProvName = element.Text;
                                lastStr = ""; CountToBreak++;
                                break;
                            case "Rendering Provider Number:":
                                prapRendProvPFIN = element.Text;
                                lastStr = ""; CountToBreak++;
                                break;
                            case "Rendering Provider Address:":
                                prapRendProvAddr = element.Text;
                                lastStr = ""; CountToBreak++;
                                break;
                            case "Billing Provider NPI: ":
                                prapFacilityNPI = element.Text;
                                prapBillingProvNPI = element.Text;
                                lastStr = ""; CountToBreak++;
                                break;
                            case "Service Facility Location Name: ":
                                prapFacilityName = element.Text;
                                lastStr = ""; CountToBreak++;
                                break;
                            case "Service Facility Location Address: ":
                                prapFacilityAddress = element.Text;
                                lastStr = ""; CountToBreak++;
                                break;
                            case "Billing Provider Number: ":
                                prapBillingProvPFIN = element.Text;
                                lastStr = ""; CountToBreak++;
                                break;
                            case "Billing Provider Address: ":
                                prapBillingProvAddr = element.Text;
                                lastStr = ""; CountToBreak++;
                                break;
                            case "Billing Provider Name: ":
                                prapBillingProvName = element.Text;
                                lastStr = ""; CountToBreak++;
                                break;
                        }
                    }
                    if (element.Text == str)
                    {
                        j = true;
                        lastStr = str;
                    }
                    if (CountToBreak == 10)
                    {
                        break;
                    }
                }                
            }
        }


        public static void PRAP_Begin(string DCN, string pass)
        {
            // Checks the IE shells and if Prap does not exist, it will open a new instance of the login screen. it then logs in
            SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindows();
            SHDocVw.InternetExplorer iePRAP = IELib.IEGet(shellWindows, "Statistics Header");
            if (iePRAP == null)
            {
                iePRAP = IELib.IEGet(shellWindows, "");
                if (iePRAP == null)
                {
                    IELib.NewBrowser(shellWindows, "", "");
                    iePRAP = IELib.IEGet(shellWindows, "");
                }
                IELib.IeWait(iePRAP);
                PRAP_Login(iePRAP, pass);
            }
        }
        public static bool PRAP_NewClaimSearch(SHDocVw.InternetExplorer wb, SHDocVw.ShellWindows shells , string Dcn)
        {
            if (wb != null)
            {
                mshtml.HTMLDocument iePrapDoc = wb.Document;

                mshtml.HTMLInputElement prapDCNinput = null;

                iePrapDoc.getElementById("advancedSearch").click();

                while (IELib.IEGet(shells, "Advanced Search") == null)
                { }

                wb = IELib.IEGet(shells, "Advanced Search");
                IELib.IeWait(wb);
                iePrapDoc = wb.Document;

                int timeOutCounter = 0;
                while (prapDCNinput == null)
                {
                    prapDCNinput = iePrapDoc.getElementsByName("documentControlNumber").item(0);
                    timeOutCounter++;
                    if (timeOutCounter == 50)
                    { PRAP_Functions.PRAP_CloseWindows(shells); return false; }
                }

                try { prapDCNinput.value = "0201" + Dcn; } catch (Exception) { PRAP_Functions.PRAP_CloseWindows(shells); return false; }

                try { iePrapDoc.getElementById("searchButn").click(); } catch (Exception) { PRAP_Functions.PRAP_CloseWindows(shells); return false; }

                while (IELib.IEGet(shells, "Search Results") == null)
                { }

                wb = IELib.IEGet(shells, "Search Results");
                iePrapDoc = wb.Document;
                foreach (mshtml.IHTMLElement a in iePrapDoc.getElementsByTagName("a"))
                {
                    if (a.innerText == "0201" + Dcn)
                    {
                        a.click();
                    }
                }

                while (IELib.IEGet(shells, "Claim Inquiry Details") == null)
                { }
                return true;
            }
            else return false;
        }
        public static int PRAP_DetectScreens(SHDocVw.InternetExplorer iePRAP, SHDocVw.ShellWindows shellWindows)
        {
            int PRAP_Screen = 0;
            foreach (SHDocVw.InternetExplorer wb in shellWindows)
            {
                if (wb.LocationName == "Advanced Search")
                {
                    PRAP_Screen = 1;
                }
                if (wb.LocationName == "Search Results")
                {
                    PRAP_Screen = 2;
                }
                if (wb.LocationName == "Claim Inquiry Details")
                {
                    PRAP_Screen = 3;
                }
            }
            return PRAP_Screen;
        }
        public static void PRAP_Login(SHDocVw.InternetExplorer wb, string p) //Start back here
        {
            mshtml.HTMLDocument wbDoc = wb.Document;
            mshtml.HTMLButtonElement wbButton;
            wbDoc.getElementById("user-name").innerText = System.Environment.UserName;
            wbDoc.getElementById("password").innerText = p;
            wbButton = wbDoc.getElementsByTagName("button").item(0);
            try { wbButton.click(); } catch (Exception) { }
            IELib.IeWait(wb);
        }
        public static string PRAP_GetProviderInfo(SHDocVw.InternetExplorer wb, SHDocVw.ShellWindows shells, string Type, string DCN)
        {
            /*
             * This is to return the single string listed (null or valid) in PRAP's claim inquiry details screen
             * Parameters are as follows:
             * WB: current valid shdocvw.internet explorer object
             * Shells: current shdocvw Shellwindows
             * Type: must be the exact field you're looking for in the provider tab. e.g. "Rendering Provider NPI:"
             * DCN: must be the full DCN number. e.g. 02018888888888x
             */
            mshtml.HTMLDocument wbDoc = wb.Document;
            int p = 0;
                        
            wbDoc = wb.Document;
            foreach (mshtml.IHTMLElement a in wbDoc.getElementsByTagName("a"))
            {
                if (a.innerText == "Provider")
                {
                    a.click();
                }
            }
            //IELib.IeWait(wb);
            
            if (Type != null)
            {
                foreach (mshtml.IHTMLElement a in wbDoc.getElementsByTagName("td"))
                {
                    if (p == 1)
                    {
                        Console.WriteLine("returnd: " + a.innerText);
                        return a.innerText;
                    }
                    if (a.innerText == Type)
                    {
                        p = 1;
                    }
                }
            }
            return null;
        }
        public static string PRAP_GetAuthCode(SHDocVw.InternetExplorer wb, SHDocVw.ShellWindows shells)
        {
            mshtml.HTMLDocument wbDoc = wb.Document;
            int p = 0;

            foreach(mshtml.IHTMLElement td in wbDoc.getElementsByTagName("td"))
            {
                if (p == 1)
                {
                    //Console.WriteLine("PRAP Auth: " + td.innerText);
                    return td.innerText;
                }
                if (td != null)
                {
                    if (td.innerText == "Treatment Authorization Code:")
                    {
                        p = 1;
                    }
                }
            }
            return null;
        }
        public static string PRAP_GetOtherInsurancePaymentName(SHDocVw.InternetExplorer wb, SHDocVw.ShellWindows shells)
        {
            mshtml.HTMLDocument wbDoc = wb.Document;
            
            foreach(mshtml.IHTMLElement a in wbDoc.getElementsByTagName("a"))
            {
                if (a.innerText == "Third Party Liability")
                {
                    a.click();
                }
            }
            /*
             find the table by document index of tables. if that table only has 1 row, the table is empty. otherwise, those rows should
             be investigated for TPL payments
            
             * Table[1] > TR[1]
             * foreach table in TR[1]
             *  table[0] > TR ID="claimBody" > td id="claimTab4" > table[0] > tr[3] > table[0] > *tr[1]*
             */
             // todo: test elements and values found
            mshtml.HTMLTableCell claimTabCell = wb.Document.getElementById("claimTab4");
            mshtml.HTMLTable otherInsTable = claimTabCell.getElementsByTagName("table").item(0);
            mshtml.HTMLTableRow otherInsTableRow = otherInsTable.rows.item(3); //.getElementsByTagName("tr").item(4);
            mshtml.HTMLTableCell otherInshtableCell = otherInsTableRow.getElementsByTagName("td").item(0);
            mshtml.HTMLTable oiSubTable = otherInshtableCell.getElementsByTagName("table").item(0);
            
            foreach(mshtml.HTMLTableRow Row in oiSubTable.rows)
            {
                //Console.WriteLine("row counts of subTable: " + oiSubTable.rows.length);
                try
                {
                    mshtml.HTMLTable table = Row.getElementsByTagName("table").item(0);
                    otherInsTableRow = table.getElementsByTagName("tr").item(1);
                    table = otherInsTableRow.getElementsByTagName("table").item(0);
                    otherInsTableRow = table.getElementsByTagName("tr").item(0);
                    mshtml.IHTMLElement oiName =  otherInsTableRow.getElementsByTagName("a").item(0);
                    string carrier = oiName.innerText;
                    //Console.WriteLine("carrier: " + carrier);
                    if (carrier != "")
                    {
                        return carrier; //Console.WriteLine("OI found, provider: " + carrier);
                    }
                } catch (Exception) { Console.WriteLine("OI Exception"); return null; }
            }
            return null;
        }
        public static void PRAP_CloseWindows(SHDocVw.ShellWindows shells)
        {
            foreach(SHDocVw.InternetExplorer ie in shells)
            {
                try
                {
                    if (ie.LocationName == "Claim Inquiry Details")
                    {
                        Console.WriteLine("quitting: " + ie.LocationName);
                        ie.Quit();
                    }
                    if (ie.LocationName == "Search Results")
                    {
                        Console.WriteLine("quitting: " + ie.LocationName);
                        ie.Quit();
                    }
                    if (ie.LocationName == "Advanced Search")
                    {
                        Console.WriteLine("quitting: " + ie.LocationName);
                        ie.Quit();
                    }
                    if (ie.LocationName == "Statistics Header")
                    {
                        Console.WriteLine("quitting: " + ie.LocationName);
                        ie.Quit();
                    }
                } catch (Exception) { }
            }
        }
        public static void PRAP_ResetSearch(SHDocVw.InternetExplorer wb, SHDocVw.ShellWindows shells)
        {
            wb.Quit();
            foreach(SHDocVw.InternetExplorer ie in shells)
            {
                if (ie.LocationName =="Search Results")
                {
                    wb = IELib.IEGet(shells, "Search Results");
                    if (wb != null)
                    {
                        mshtml.HTMLDocument wbDoc = wb.Document;
                        foreach(mshtml.IHTMLElement elem in wbDoc.getElementsByTagName("a"))
                        {
                            if (elem.innerText == "Return to Search")
                            {
                                elem.click();
                                IELib.IeWait(ie);
                            }
                        }
                    }
                }
            }
        }
        public static string PRAP_GetOnsetDate(SHDocVw.InternetExplorer wb, SHDocVw.ShellWindows shells)
        {
            mshtml.HTMLDocument wbDoc = wb.Document;
            int p = 0;

            foreach (mshtml.IHTMLElement td in wbDoc.getElementsByTagName("td"))
            {
                if (p == 1)
                {
                    //Console.WriteLine("PRAP Auth: " + td.innerText);
                    return td.innerText;
                }
                if (td != null)
                {
                    if (td.innerText == "Onset Date:")
                    {
                        p = 1;
                    }
                }
            }
            return null;
        }
        public bool inString(string needle, string haystack)
        {
            needle = Regex.Replace(needle.ToLower(), @"\r\n", string.Empty);
            haystack = Regex.Replace(haystack.ToLower(), @"\r\n", string.Empty);

            string[] needleArr = needle.Split(' ');
            string[] haystackArr = haystack.Split(' ');

            for (int i = 0; i < needleArr.Length; i++)
            {
                for (int j = 0; j < haystackArr.Length; j++)
                {
                    if (needleArr[i] == haystackArr[j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
