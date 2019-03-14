using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace Neuro.Classes
{
    class COB_WebLaunch
    {
        public LoginInfo LoginInformation = new LoginInfo();
        public SearchCriteria SearchInformation = new SearchCriteria();
        public CurrentCoverageInformation CurrentCoverage = new CurrentCoverageInformation();
        public OtherInsuranceInformation OICoverage = new OtherInsuranceInformation();
        public GroupInformation GroupInfo = new GroupInformation();
        public CSQInformation CSQInfo = new CSQInformation();
        public ChromeOptions options = new ChromeOptions();
        public ChromeDriver driver { get; set; }

        public static bool OperateHeadless { get; set; }

        public COB_WebLaunch()
        {

        }
        public bool Authenticate()
        {
            if (!string.IsNullOrEmpty(LoginInformation.UserName) && !string.IsNullOrEmpty(LoginInformation.Password))
            {
                ChromeOptions option = new ChromeOptions();
                if (OperateHeadless == true)
                {
                    option.AddArgument("--headless");
                }
                string subNum = FormatSubNumber(SearchInformation.SubscriberNumber);

                driver = new ChromeDriver(option);
                
                IWebElement elem;
                string url = "";
                driver.Navigate().GoToUrl(url);

                Console.WriteLine(driver.Title);
                if (driver.Title == "CCSP COB")
                {
                    elem = driver.FindElement(By.Name("j_username"));
                    elem.SendKeys(LoginInformation.UserName);
                    elem = driver.FindElement(By.Name("j_password"));
                    elem.SendKeys(LoginInformation.Password);
                    elem = driver.FindElement(By.Name("submitButton"));
                    elem.Click();                    
                }
                return ValidateLogin(driver);
            }
            else
            {
                Console.WriteLine("Username or Password is empty/null. Login unsuccessful.");
                return false;
            }
        }
        private bool ValidateLogin(IWebDriver driver)
        {
            int timeOut = 0;
            while (driver.Title == "CCSP COB")
            {
                if (timeOut == 15)
                {
                    Console.WriteLine("Timeout Reached, Login unsuccessful.");
                    return false;
                }
                timeOut++;
                System.Threading.Thread.Sleep(1000);
            }
            if (driver.Title != "CCSP COB")
            {
                Console.WriteLine("Login Successful");
                return true;
            }
            Console.WriteLine("Unknown error, login unsuccessful.");
            return false;
        }
        public void GetCOBInformation()
        {/*
            ChromeOptions option = new ChromeOptions();
            if (OperateHeadless == true)
            {
                option.AddArgument("--headless");
            }
            IWebDriver driver = new ChromeDriver(option);*/

            string subNum = FormatSubNumber(SearchInformation.SubscriberNumber);
            IWebElement elem;
            ICollection<IWebElement> elemList;
            if (driver.Title == "COB Home")
            {
                driver.Navigate().GoToUrl("");
                elem = driver.FindElement(By.Id("memberCorporateEntity"));
                var selectElement = new SelectElement(elem);
                selectElement.SelectByIndex(SearchInformation.CorpEntityCode);

                elem = driver.FindElement(By.Id("memberSubscriberNumber"));
                elem.SendKeys(SearchInformation.SubscriberNumber);

                elem = driver.FindElement(By.Id("memberGroupNumber"));
                elem.SendKeys(SearchInformation.GroupNumber);

                elem = driver.FindElement(By.Id("searchMembutton"));
                elem.Click();
                if (driver.Title == "")
                {
                    //find number of members returned, loop through until you have your member
                    elem = driver.FindElement(By.Id("printerFriendly"));
                    elemList = elem.FindElements(By.TagName("td"));
                    foreach (IWebElement e in elemList)
                    {
                        if (e.Text == SearchInformation.FirstName + " " + SearchInformation.LastName ||
                            e.Text == SearchInformation.FirstName + " " + SearchInformation.MidInitial + " " + SearchInformation.LastName)  //instring name!!
                        {
                            e.Click();
                            break;
                        }
                    }
                }
                // A new webpage opens up, if the title is not correct, we redeclare the title 
                if (driver.Title != "COB Main")
                {
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
                }

                if (driver.Title == "COB Main")
                {
                    elem = driver.FindElement(By.Id("dijit_layout__TabButton_1"));
                    elem.Click();
                    elem = driver.FindElement(By.Id("dijit_layout__TabButton_2"));
                    elem.Click();
                    elem = driver.FindElement(By.Id("dijit_layout__TabButton_3"));
                    elem.Click();
                    elem = driver.FindElement(By.Id("dijit_layout__TabButton_0"));
                    elem.Click();

                    GatherCurrentCoverage(driver, this.CurrentCoverage, this.SearchInformation);
                    elem = driver.FindElement(By.Id("dijit_layout__TabButton_1"));
                    elem.Click();
                    GatherGroupInfo(driver, this.GroupInfo);
                    elem = driver.FindElement(By.Id("dijit_layout__TabButton_2"));
                    elem.Click();
                    GatherOICoverage(driver);
                    elem = driver.FindElement(By.Id("dijit_layout__TabButton_3"));
                    elem.Click();
                    GatherCSQInfo(driver);
                }
            }
            else
            {
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
            }
        }
        private static async void GatherCurrentCoverage(IWebDriver driver, CurrentCoverageInformation CC, SearchCriteria SrchCrit)
        {
            //await Task.Run(
                //async () =>
                {
                    int elemCount = 0;
                    bool flipswitch = false;
                    IWebElement tempElement = driver.FindElement(By.Id("coverageAgreement")); //overall table
                    IWebElement table = tempElement.FindElement(By.ClassName("los")); // sub table
                    IWebElement element = tempElement; //used to get the specific patient section

                    ICollection<IWebElement> tableRows = table.FindElements(By.TagName("tr"));

                    foreach(IWebElement row in tableRows)
                    {
                        if (flipswitch == true)
                        {
                            tempElement = row; // this is the sub row that holds additional information. 
                            break;
                        }

                        foreach(IWebElement cell in row.FindElements(By.TagName("td")))
                        {
                            if (cell.Text == SrchCrit.FirstName + " " + SrchCrit.LastName ||
                                cell.Text == SrchCrit.FirstName + " " + SrchCrit.MidInitial + " " + SrchCrit.LastName)
                            {
                                element = row; //cell with teh primary patient data
                                cell.Click();
                                flipswitch = true;
                                break;
                            }
                        }
                    }

                    foreach (IWebElement elem in element.FindElements(By.TagName("td")))
                    {
                        if (elemCount == 0)
                            CC.CC_Relationship = elem.Text;
                        if (elemCount == 1)
                            CC.CC_MemberName = elem.Text;
                        if (elemCount == 2)
                            CC.CC_BCBSBenefitOrder = elem.Text;
                        if (elemCount == 3)
                            CC.CC_BenefitOrderEffectiveDates = elem.Text;
                        elemCount++;
                        if (elemCount == 4)
                            break;
                    }
                    
                    elemCount = 1;
                    foreach(IWebElement elem in tempElement.FindElements(By.TagName("td")))
                    {
                        if (elemCount == 3)
                            CC.CC_AdditionalCoverage = elem.Text;
                        if (elemCount == 5)
                            CC.CC_DateOfBirth = elem.Text;
                        if (elemCount == 7)
                            CC.CC_DivorceDecree = elem.Text;
                        if (elemCount == 9)
                            CC.CC_BOBIndicator = elem.Text;
                        elemCount++;
                        if (elemCount >= 10)
                            break;
                    }
                tempElement = null;
                element = null;
                table = null;
                tableRows = null;
                }
                //);
        }
        private void GatherGroupInfo(IWebDriver driver, GroupInformation GG)
        {
            int count = 1;
            IWebElement div = driver.FindElement(By.Id("groupView"));
            foreach(IWebElement td in div.FindElements(By.TagName("td")))
            {
                if (count == 11)
                    GG.Grp_AlphaPrefix = td.Text;
                if (count == 13)
                    GG.Grp_ContractType = td.Text;
                if (count == 15)
                    GG.Grp_ProductType = td.Text;
                if (count == 19)
                    GG.Grp_EffectiveDate = td.Text;
                if (count == 21)
                    GG.Grp_CancelDate = td.Text;
                if (count == 23)
                    GG.Grp_GenderRule = td.Text;
                if (count == 25)
                    GG.Grp_GenderRuleDates = td.Text;
                if (count == 27)
                    GG.Grp_BirthdayRuleIndicator = td.Text;
                if (count == 29)
                    GG.Grp_BirthdayRuleDateSpan = td.Text;
                count++;
                if (count >= 30)
                    break;
            }
            count = 1;
            IWebElement groupInfo = div.FindElement(By.Id("groupinforight"));
            foreach(IWebElement td in groupInfo.FindElements(By.TagName("td")))
            {
                if (count == 2)
                    GG.Grp_CSQTimeLimit = td.Text;
                if (count == 4)
                    GG.Grp_CSQSuppresion = td.Text;
                if (count == 6) // held within an <a> tag
                {
                    
                    GG.Grp_DatesApplied = td.FindElement(By.TagName("a")).Text;
                }
                count++;
                if (count >= 7)
                    break;
            }
            GroupInfo = null;
            div = null;
        }
        private void GatherOICoverage(IWebDriver driver)
        {

        }
        private void GatherCSQInfo(IWebDriver driver)
        {

        }

        private bool inString(string needle, string haystack)
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
        private string FormatSubNumber(string subNum)
        {
            if(subNum.Length < 12)
            {
                int increase = 12 - subNum.Length;
                for(int i = 1; i <= increase; i++)
                {
                    subNum = "0" + subNum;
                    if (subNum.Length == 12)
                        return subNum; 
                }
            }
            return subNum;
        }
    }

    /// <summary>
    /// Must provide a username and password
    /// </summary>
    class LoginInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    /// <summary>
    /// All Required as subscriber, groupnumber, First Last Mid Name and corp entity code
    /// </summary>
    class SearchCriteria
    {
        public string FirstName { get; set; }
        public string MidInitial { get; set; }
        public string LastName { get; set; }
        public string SubscriberNumber { get; set; }
        public string GroupNumber { get; set; }
        /// <summary>
        /// CorpEntity Codes: 2=IL, 3=MT, 4=NM, 5=OK, 6=TX
        /// </summary>
        public int CorpEntityCode { get; set; }
    }
    class CurrentCoverageInformation
    {
        public string CC_AdditionalCoverage { get; set; }
        public string CC_DateOfBirth { get; set; }
        public string CC_DivorceDecree { get; set; }
        public string CC_BOBIndicator { get; set; }
        public string CC_Relationship { get; set; }
        public string CC_MemberName { get; set; }
        public string CC_BCBSBenefitOrder { get; set; }
        public string CC_BenefitOrderEffectiveDates { get; set; }
        public DateTime CC_BenefitEffectiveStartDate { get; set; }
        public DateTime CC_BenefitEffectiveEndDate { get; set; }
    }
    class OtherInsuranceInformation
    {
        public Coverage[] OtherInsuranceList = new Coverage[5];
        public string OI_CarrierName { get; set; }
        public string OI_BOBIndicator { get; set; }
        public string OI_GroupNumber { get; set; }
        public string OI_SubscriberNumber { get; set; }
        public string OI_BenefitOrder { get; set; }
        public string OI_EffectiveDateSpan { get; set; }
        public string OI_PolicyHolder { get; set; }
        public string OI_RelationshipToBCBCPolicyHolder { get; set; }
        public string OI_BenefitOrderDetermination { get; set; }
        public string OI_CoverageReason { get; set; }
        public string OI_OriginalEffectiveDate { get; set; }
        public string OI_CancelDate { get; set; }
        public DateTime OI_BenefitEffectiveStartDate { get; set; }
        public DateTime OI_BenefitEffectiveEndDate { get; set; }
    }
    class GroupInformation
    {
        public string Grp_AlphaPrefix { get; set; }
        public string Grp_ContractType { get; set; }
        public string Grp_ProductType { get; set; }
        public string Grp_EffectiveDate { get; set; }
        public string Grp_CancelDate { get; set; }
        public string Grp_GenderRule { get; set; }
        public string Grp_GenderRuleDates { get; set; }
        public string Grp_BirthdayRuleIndicator { get; set; }
        public string Grp_BirthdayRuleDateSpan { get; set; }
        public string Grp_CSQTimeLimit { get; set; }
        public string Grp_CSQSuppresion { get; set; }
        public string Grp_DatesApplied { get; set; }
        public string Grp_GroupOverrides { get; set; }
    }
    class CSQInformation
    {
        public string CSQ_SystemGenerated { get; set; }
        public string CSQ_DocControlNumber { get; set; }
        public string CSQ_AlternateRecipients { get; set; }
        public string CSQ_AlternateAddress { get; set; }
        public string CSQ_SentDate { get; set; }
        public string CSQ_ReturnDate { get; set; }
        public List<string> CSQ_ResponseType = new List<string>();
    }
}
