using invoice_api.Models;
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
            return Ok(new List<Invoice>());
        }
    }
}
