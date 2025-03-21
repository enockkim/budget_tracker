﻿using budget_tracker.Models;
using MySqlX.XDevAPI.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using budget_tracker.Models;

namespace budget_tracker.Services
{
    public interface IFeePaymentService
    {
        //Transactions
        Task<bool> SaveFeePayment(fee_payment transaction, string account, string amount);

        Task<bool> UpdateAdmissionStatus(int admissionNumber);
    }
}