﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using UrenRegistratieQien.Repositories;
using UrenRegistratieQien.Models;
using UrenRegistratieQien.MailService;
using Microsoft.AspNetCore.Http;
using UrenRegistratieQien.Exceptions;

namespace UrenRegistratieQien.Controllers
{
    public class MailserviceController : Controller
    {
        private readonly IDeclarationFormRepository declarationFormRepo;
        private readonly IEmployeeRepository employeeRepo;

        public MailserviceController(IDeclarationFormRepository DeclarationFormRepo, IEmployeeRepository EmployeeRepo)
        {
            declarationFormRepo = DeclarationFormRepo;
            employeeRepo = EmployeeRepo;
        }

        [HttpPost]
        public async Task<IActionResult> MailService(DeclarationFormModel decModel, string uniqueId, string formId, string employeeName, IFormFile file)
        {
            employeeRepo.UploadFile(file, decModel.FormId);
            if (!await employeeRepo.UserIsEmployeeOrTrainee())
            {
                return AccessDeniedView();
            }
            
            try
            {
                await declarationFormRepo.EditDeclarationForm(decModel);
            }
            catch (MoreThan24HoursException e)
            {
                return RedirectToAction("HourReg", "DeclarationForm", new { declarationFormId = decModel.FormId, userId = decModel.EmployeeId, year = decModel.Year, month = decModel.Month, errorMessage = e.Message });
            }

            await declarationFormRepo.SubmitDeclarationForm(decModel);
            await declarationFormRepo.CalculateTotalHours(decModel);

            //hier word mail opgesteld en verstuurd
            Mailservice.MailFormToClient(decModel, uniqueId, formId, employeeName);
            return RedirectToRoute(new { controller = "Home", action = "Index" });
        }

        public async Task<IActionResult> ApproveOrReject(string uniqueId, string formId, bool commentNotValid)
        {
            var formIdAsInt = Convert.ToInt32(formId);

            if (await declarationFormRepo.CheckIfIdMatches(uniqueId))
            {
                var declarationFormModel = await declarationFormRepo.GetForm(formIdAsInt);
                return View(new RejectFormModel { declarationFormModel = declarationFormModel, commentNotValid = commentNotValid });
            }
            else
            {
                return View("~/Views/Mailservice/ErrorUnknownId.cshtml");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Approve(string formId)
        {
            await declarationFormRepo.ApproveForm(Convert.ToInt32(formId));
            return View();
        }

        public async Task<IActionResult> ApproveMailChange(string newMail, string oldMail)
        {
            await employeeRepo.EditEmployeeMail(oldMail, newMail);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Qien", "hanshanshans812@gmail.com"));
            message.To.Add(new MailboxAddress(newMail, newMail));
            message.Subject = "Email wijziging bevestigd door Admin";
            message.Body = new TextPart("html")
            {
                Text = $"Beste,<br>Je e-mailadres wijziging is bevestigd door de Admin. <br> Vanaf nu kan er inlogd worden met: {newMail} <br> Met vriendelijke groet <br> "
            };

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect("Smtp.gmail.com", 587, false);
                client.Authenticate("hanshanshans812@gmail.com", "Hans123!"); //
                client.Send(message);
                client.Disconnect(true);
            }

            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Reject(int FormId, RejectFormModel rejectFormModel, string employeeName, string clientName)
        {
            var declarationFormModel = await declarationFormRepo.GetForm(FormId);
            var uniqueId = declarationFormModel.uniqueId;
            var comment = rejectFormModel.comment;

            if (ModelState.IsValid)
            {
                await declarationFormRepo.RejectForm(FormId, comment);
                //aanroepen mailservice afkeuren
                Mailservice.RejectMailToAdminAndEmployee(declarationFormModel, employeeName, clientName);
                return View();
            } else
            {
                return RedirectToAction("ApproveOrReject", new { uniqueId = uniqueId, formId = FormId, commentNotValid = true});
            }
        }
        public ViewResult AccessDeniedView()
        {
            return View("~/Views/Home/AccessDenied.cshtml");
        }
    }
}
