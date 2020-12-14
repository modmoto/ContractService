﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ContractService.Controllers
{
    [ApiController]
    [Route("weather-forecast")]
    public class ContractController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Contract>> Get()
        {
            return new ActionResult<List<Contract>>(new List<Contract>());
        }

        // TODO:
        // Sign Contract
        // 
        [HttpPost("Post")]
        public async Task<IEnumerable<Contract>> Post([FromBody] Contract c, string userId, int type, int years)
        {
            var profile = await LoadProfile(new Guid(userId));
            var v = ContractUtils.ValidateAmount(c.Amount, profile.Max);
            if (!v)
            {
                Console.WriteLine("invalid amount");
                throw new Exception("not valid");
            }

            var v1 = ContractUtils.ValidateAmount(c.ApplicantAdress, c.ApplicantName);
            if (!v1)
            {
                Console.WriteLine("invalid amount");
                throw new Exception("not valid");
            }

            c.Fee = Calculate(decimal.Parse(c.Amount.ToString()), type, years);

            await Save(c);

            return new List<Contract>();
        }

        private decimal Calculate(decimal amount, int type, int years)
        {
            decimal result = 0;
            decimal disc = (years > 5) ? (decimal)5/100 : (decimal)years/100; 
            if (type == 1)
            {
                result = amount;
            }
            else if (type == 2)
            {
                result = (amount - (0.1m * amount)) - disc * (amount - (0.1m * amount));
            }
            else if (type == 3)
            {
                result = (0.7m * amount) - disc * (0.7m * amount);
            }
            else if (type == 4)
            {
                result = (amount - (0.5m * amount)) - disc * (amount - (0.5m * amount));
            }
            return result;
        }

        private async Task<Profile> LoadProfile(Guid guid)
        {
            // does not matter where it comes from right now
            return new Profile
            {
                Max = 10000
            };
        }

        private async Task Save(Contract contract)
        {
            // does not matter where it comes from right now
        }
    }

    internal class Profile
    {
        public double Max { get; set; }
    }

    public class ContractUtils
    {
        public static bool ValidateAmount(double a, double m)
        {
            bool b = false;
            if (a < 0 && a > m)
            {
                b = true;
            }
            else
            {
                b = false;
            }

            return b;
        }

        public static bool ValidateAmount(string a, string b2)
        {
            bool b = false;
            if (a.Length > 50 && b2.Length > 500)
            {
                b = true;
            }
            else
            {
                b = false;
            }

            return b;
        }
    }

    public class Contract
    {
        public double Amount { get; set; }
        public string Currency { get; set; }
        public bool Signed { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantAdress { get; set; }
        public string IBAN { get; set; }
        public decimal Fee { get; set; }
    }
}