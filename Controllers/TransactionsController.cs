using budget_tracker.Models;
using budget_tracker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace budget_tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ILogger<TransactionsController> _logger;
        private ITransactionsService transactionService;

        public TransactionsController(ILogger<TransactionsController> logger, ITransactionsService _transactionService)
        {
            _logger = logger;
            transactionService = _transactionService;
        }

        //// GET: api/<MembersController>
        //[HttpGet("GetAllMembers")]
        //public IEnumerable<members> GetAllMembers()
        //{
        //    return (IEnumerable<members>)groupService.GetMembers();
        //}

        //// GET api/<MembersController>/5
        //[HttpGet("GetMemberById")]
        //public members Get(int memberId)
        //{
        //    return groupService.GetMemberById(memberId);
        //}

        // POST api/<MembersController>
        [HttpPost("SaveTransaction")]
        public bool Post([FromBody] general_ledger transaction)
        {
            return transactionService.SaveTransaction(transaction);
        }

        //// PUT api/<MembersController>/5
        //[HttpPut("UpdateMemberProfile")]
        //public result Put([FromBody] members member)
        //{
        //    return groupService.UpdateMemberProfile(member);
        //}

        //// DELETE api/<MembersController>/5
        //[HttpPost("ChangeMemberStatus")]
        //public result Delete(int memberId)
        //{
        //    return groupService.ChangeMemberStatus(memberId);
        //}
    }
}
