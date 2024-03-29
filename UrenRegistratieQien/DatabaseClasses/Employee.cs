﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrenRegistratieQien.DatabaseClasses
{
    public class Employee : IdentityUser
    {
        public Client Client { get; set; }
        public int ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }
        public string Address { get; set; }
        public int Phone { get; set; }
        public string ZIPCode { get; set; }
        public string Residence { get; set; }
        public DateTime DateRegistered { get; set; }
        public DateTime StartDateRole { get; set; }

        public int Role { get; set; }
        public bool OutOfService { get; set; }

    }
}
