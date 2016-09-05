using System;
using Microsoft.Bot.Builder.FormFlow;

namespace MyUSCISBot.Models
{
    [Serializable]
    public class VisaStatusRequest
    {
        [Prompt("Please enter your case number")]
        [Describe("Your visa case number, that you want to check (3 letters + 10 numbers, i.e. WAC1234567890)")]
        public string CaseNumber { get; set; }
    }
}