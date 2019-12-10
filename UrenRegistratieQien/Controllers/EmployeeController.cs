﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrenRegistratieQien.DatabaseClasses;
using UrenRegistratieQien.GlobalClasses;
using UrenRegistratieQien.Models;
using UrenRegistratieQien.Repositories;

namespace UrenRegistratieQien.Controllers
{

    public class EmployeeController : Controller
    {
        private readonly IClientRepository clientRepo;
        private readonly IDeclarationFormRepository declarationRepo;
        private readonly IEmployeeRepository employeeRepo;
        private readonly IHourRowRepository hourRowRepo;
        private readonly UserManager<Employee> _userManager;
        private readonly IWebHostEnvironment he;

        public EmployeeController(IClientRepository ClientRepo, 
                                  IDeclarationFormRepository DeclarationRepo, 
                                  IEmployeeRepository EmployeeRepo, 
                                  IHourRowRepository HourRowRepo, 
                                  IWebHostEnvironment he,
                                  UserManager<Employee> userManager = null)
        {
            clientRepo = ClientRepo;
            declarationRepo = DeclarationRepo;
            employeeRepo = EmployeeRepo;
            hourRowRepo = HourRowRepo;
            _userManager = userManager;
            this.he = he;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
        
        public async Task<IActionResult> Dashboard(string year = null, string month = null, string approved = null, string submitted = null, string sortDate = null)////// de filter toepassen in de model van deze
        {
            if (await employeeRepo.UserIsEmployeeOrTrainee() || !await employeeRepo.UserIsOutOfService() && !await employeeRepo.UserIsAdmin()) 
            {
                var userId = _userManager.GetUserId(HttpContext.User); //ophalen van userId die is ingelogd
                ViewBag.userId = userId;
                ViewBag.User = await employeeRepo.GetEmployee(userId);
                ViewBag.Client = await clientRepo.GetClientByUserId(userId);
                ViewBag.AllForms = await declarationRepo.GetAllForms();
                await employeeRepo.CheckIfYearPassedForAllTrainees();
                //var inputModel = declarationRepo.GetAllFormsOfUser(userId);
                var inputModel = await declarationRepo.GetFilteredForms(year, userId, month, approved, submitted, sortDate);
                return View(inputModel);
            }
            else
            {
                return await AccessDeniedView();
            }
        }
        [HttpGet]
        public async Task<IActionResult> SearchOverview(string searchString)
        {
           List<EmployeeModel> listAccounts = await employeeRepo.GetAllAccounts(searchString);

            //return RedirectToAction("ShowEmployees", "Admin", new { Employeelist = listAccounts });
            return View(listAccounts);
        }
        public async Task<FileContentResult> DownloadExcel(int formId)
        {
            Download download = new Download();
            DeclarationFormModel declarationForm = await declarationRepo.GetForm(formId);

            download.MakeExcel(Convert.ToString(formId), declarationForm.HourRows);
            var fileName = Convert.ToString(formId) + ".xlsx";

            byte[] fileBytes = System.IO.File.ReadAllBytes("Downloads/" + fileName);
            return File(fileBytes, "Application/x-msexcel", fileName);

        }

        public async Task<IActionResult> Info()
        {
            if (await employeeRepo.UserIsEmployeeOrTrainee())
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                var Employee = await employeeRepo.GetEmployee(userId);
                ViewBag.Client = await clientRepo.GetClientByUserId(userId);
                return View(Employee);
            }
            else
            {
                return await AccessDeniedView();
            }
        }

        public async Task<IActionResult> ChangePicture()
        {
            if (await employeeRepo.UserIsEmployeeOrTrainee())
            {
                return View();
            }
            else
            {
                return await AccessDeniedView();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePicture(IFormFile picture)
        {
            if (await employeeRepo.UserIsEmployeeOrTrainee())
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                await employeeRepo.UploadPicture(picture, userId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return await AccessDeniedView();
            }
        }

        public async Task<ViewResult> AccessDeniedView()
        {
            return View("~/Views/Home/AccessDenied.cshtml");
        }
    }
}
