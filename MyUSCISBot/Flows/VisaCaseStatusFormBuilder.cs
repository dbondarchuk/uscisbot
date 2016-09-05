using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using MyUSCISBot.Models;
using MyUSCISBot.Services;

namespace MyUSCISBot.Flows
{
    public static class VisaCaseStatusFormBuilder
    {
        public static IForm<VisaStatusRequest> MakeForm()
        {
            FormBuilder<VisaStatusRequest> _order = new FormBuilder<VisaStatusRequest>();

            return _order
                .Message("Welcome to USCIS status checker!")
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                .Field(nameof(VisaStatusRequest.CaseNumber), state => true, async (state, value) =>
                {
                    var caseStatus = value.ToString();
                    var validateResult = new ValidateResult();
                    if (!Regex.IsMatch(caseStatus, "^[A-Z]{3}[0-9]{10}$"))
                    {
                        validateResult.IsValid = false;
                        validateResult.Feedback = "Visa case number, that you want to check should contain 3 letters + 10 numbers, i.e. WAC1234567890";
                    }
                    else
                    {
                        validateResult.IsValid = true;
                        validateResult.Value = value;
                    }

                    return validateResult;
                })
                .Message("Please wait. We are checking your status..")
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                .OnCompletion((context, state) =>
                {
                    string status;
                    var result = UscisService.GetCaseStatus(state, out status);
                    if (result)
                    {
                        return context.PostAsync($"Your case status: {status}");
                    }
                    else
                    {
                        return context.PostAsync(status);
                    }
                }).Build();
        }
    }
}