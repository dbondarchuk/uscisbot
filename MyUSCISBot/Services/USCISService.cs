using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using MyUSCISBot.Models;

namespace MyUSCISBot.Services
{
    public static class UscisService
    {
        private const string UscisCheckUrl = "https://egov.uscis.gov/casestatus/mycasestatus.do";
        private const string Pattern = "<div class=\"current-status-sec\">((.|\\n)*?)<\\/div>";

        public static bool GetCaseStatus(VisaStatusRequest request, out string text)
        {
            try
            {
                var webClient = new WebClient();
                var collection = new NameValueCollection
                {
                    {"appReceiptNum", request.CaseNumber},
                    {"initCaseSearch", "CHECK STATUS"}
                };

                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                var response = webClient.UploadValues(UscisCheckUrl, collection);
                var html = Encoding.UTF8.GetString(response);
                var match = Regex.Matches(html, Pattern);
                var group = match[0].Groups[1];
                text = Regex.Replace(group.Value, "<strong>.*?<\\/strong>", "");
                text = Regex.Replace(text, "<span.*?<\\/span>", "");
                text = text.Trim();

                return true;
            }
            catch (WebException e)
            {
                text = "Sorry, we can't connect to USCIS server. Please try again later";
                return false;
            }
            catch (ArgumentOutOfRangeException e)
            {
                text =
                    "Sorry, it seems, that USCIS doesn't know your case number. Please check your case number and then try again";
                return false;
            }
            catch (Exception e)
            {
                text = "Sorry, some problem was encountered. Please try again later";
                return false;
            }
        }
    }
}