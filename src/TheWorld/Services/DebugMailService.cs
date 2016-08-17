﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWorld.Services
{
    public class DebugMailService : IMailService
    {
        public void SendMail(string to, string @from, string subject, string body)
        {
            //throw new NotImplementedException();
            Debug.WriteLine($"Sedning Mail: To: {to} From: {from} Subject: {subject}");
        }
    }
}
