﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UrenRegistratieQien.Repositories;
using UrenRegistratieQien.Models;
using UrenRegistratieQien.GlobalClasses;

namespace UrenRegistratieQien.Controllers
{
    public class AdminController : Controller
    {
        private readonly IDeclarationFormRepository declarationFormRepo;
        public List<string> monthList { get; set; }

        public AdminController(IDeclarationFormRepository DeclarationFormRepo)
        {

            declarationFormRepo = DeclarationFormRepo;
            monthList = new List<string> { "Januari", "Februari", "March", "April", "May", "June", "Juli", "August", "September", "October", "November", "December" };

        }



        public IActionResult ViewDeclarationForm(int formId)
        {
            var form = declarationFormRepo.GetFormByFormId(formId);
            return View(form);
        }


        public IActionResult Admin()
        {

            ////// in admin.cshtml moeten de links nog zo geupdate worden dat ie de juiste filters doorgeeft
            ViewBag.AllForms = declarationFormRepo.GetAllForms();
            ViewBag.Months = monthList;
            var forms = declarationFormRepo.GetAllForms();
            //AdminOverviewModel adminOverViewModel = new AdminOverviewModel();
            //adminOverViewModel.declarationFormModels = forms;

            //is filterlijst[0] null? anders:
            //pakt dan de forms, en selecteert alleen de forms met overeenkomende naam


            //naar volgende filter
            //is filterlijst[1] null? anders:


            //
            //


            // door Januari parameter uit de IRepo en Repo (+if statement) eruit te halen, kan je sowieso alle uren inzien. 
            // Nu geeft ie alleen januari zoals hieronder staat, selectie andere maand is nog niet werkend
            ViewBag.TotalHoursWorked = declarationFormRepo.TotalHoursWorked(forms, "Januari");
            ViewBag.TotalHoursOvertime = declarationFormRepo.TotalHoursOvertime(forms, "Januari");
            ViewBag.TotalHoursSickness = declarationFormRepo.TotalHoursSickness(forms, "Januari");
            ViewBag.TotalHoursVacation = declarationFormRepo.TotalHoursVacation(forms, "Januari");
            ViewBag.TotalHoursHoliday = declarationFormRepo.TotalHoursHoliday(forms, "Januari");
            ViewBag.TotalHoursTraining = declarationFormRepo.TotalHoursTraining(forms, "Januari");
            ViewBag.TotalHoursOther = declarationFormRepo.TotalHoursOther(forms, "Januari");
            return View(forms);
        }

        public IActionResult AdminWithEmployeeId(string employeeId)
        {

            ViewBag.AllForms = declarationFormRepo.GetAllForms();
            ViewBag.Months = monthList;
            var forms = declarationFormRepo.GetAllFormsOfUser(employeeId);
            return View("~/Views/Admin/Admin.cshtml", forms);
        }

        public IActionResult AdminWithMonthYear(string month, int year)
        {
            ViewBag.AllForms = declarationFormRepo.GetAllForms();
            ViewBag.Months = monthList;
            var forms = declarationFormRepo.GetAllFormsOfMonth(MonthConverter.ConvertMonthToInt(month));
            return View("~/Views/Admin/Admin.cshtml", forms);
        }



        public IActionResult EmployeeForms(string employeeId)
        {
            var forms = declarationFormRepo.GetAllFormsOfUser(employeeId);
            return View(forms);
        }
    }
}