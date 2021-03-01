using invoice_api.Models;
using invoice_api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace invoice_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoiceController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Invoice>> Get()
        {
            var invoices = InvoiceRepository.Get();

            return Ok(invoices);
        }
    }
}
