﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrenRegistratieQien.Models
{
    public class EmployeeModel
    {
        public string EmployeeId { get; set; }
        public int ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }
        public string Email { get; set; }
        public string Address { get; set; }

        public string Phone { get; set; }
        public int Role { get; set; }
        public string RoleAsString { get; set; }
        public string ZIPCode { get; set; }
        public string Residence { get; set; }
        public DateTime DateRegistered { get; set; }
        public DateTime StartDateRole { get; set; }
        public bool OutOfService { get; set; }
    }
}
