﻿using invoice_api.Models;
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

        [HttpGet("{id}")]
        public ActionResult<Invoice> Get(int id)
        {
            var invoice = InvoiceRepository.Get(id);

            if (invoice == null)
                return NotFound();

            return Ok(invoice);
        }

        [HttpPost]
        public ActionResult<Invoice> Post([FromBody] Invoice invoice)
        {
            invoice = InvoiceRepository.Post(invoice);

            if (invoice == null)
                return BadRequest();

            return Created("/invoice/" + invoice.Id, invoice);
        }
    }
}
