﻿using System;
using System.Collections.Generic;
using System.Text;
using UrenRegistratieQien.Models;
using UrenRegistratieQien.Repositories;

namespace UrenRegistratieQienTest.Fakes
{
    class FakeClientRepository : IClientRepository
    {
        public void AddNewClient(ClientModel clientModel)
        {
            var implemented = true;
        }

        public void EditAClient(ClientModel clientModel)
        {
            var implemented = true;
        }

        public List<ClientModel> GetAllClients()
        {
            return new List<ClientModel>();
        }
        
        public ClientModel GetClient(int clientId)
        {
            throw new NotImplementedException();
        }
        public ClientModel GetClientByUserId(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
