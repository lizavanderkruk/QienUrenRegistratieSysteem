﻿using System.Collections.Generic;
using UrenRegistratieQien.Models;

namespace UrenRegistratieQien.Repositories
{
    public interface IDeclarationFormRepository
    {

        public DeclarationFormModel GetForm(int declarationFormId, string userId);
        public List<DeclarationFormModel> GetAllForms();
        public List<DeclarationFormModel> GetAllFormsOfUser(string userId);
        public void EditDeclarationForm(DeclarationFormModel formModel);

    }
}