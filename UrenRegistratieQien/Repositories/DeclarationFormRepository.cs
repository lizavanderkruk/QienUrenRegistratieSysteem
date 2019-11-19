﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrenRegistratieQien.Data;
using UrenRegistratieQien.Models;

namespace UrenRegistratieQien.Repositories
{
    public class DeclarationFormRepository : IDeclarationFormRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IHourRowRepository hourRowRepo;

        public DeclarationFormRepository(ApplicationDbContext context, IHourRowRepository hourRowRepo)
        {
            this.context = context;
            this.hourRowRepo = hourRowRepo;
        }


        public DeclarationFormModel GetForm(string userId, string month)
        {
            var entity = context.DeclarationForms.Single(d => d.EmployeeId == userId && d.Month == month);
            var form = new DeclarationFormModel
            {
                FormId = entity.DeclarationFormId,
                HourRows = hourRowRepo.GetHourRows(userId, entity.Month), //niet heel netjes om en andere repo te gebruiken
                EmployeeId = entity.EmployeeId,
                Month = entity.Month,
                Approved = entity.Approved,
                Submitted = entity.Submitted,
                Comment = entity.Comment
            };
            return form;
        }



        public List<DeclarationFormModel> GetAllForms(string userId)
        {
            var entities = context.DeclarationForms.Where(d => d.EmployeeId == userId).ToList();
            var forms = new List<DeclarationFormModel>();
            foreach (var form in entities)
            {
                var newModel = new DeclarationFormModel
                {
                    FormId = form.DeclarationFormId,
                    HourRows = hourRowRepo.GetHourRows(userId, form.Month), //niet heel netjes om en andere repo te gebruiken
                    EmployeeId = form.EmployeeId,
                    Month = form.Month,
                    Approved = form.Approved,
                    Submitted = form.Submitted,
                    Comment = form.Comment
                };
                forms.Add(newModel);
            }
            return forms;
        }


    }
}
