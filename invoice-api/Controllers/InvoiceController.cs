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

        [HttpPut]
        public ActionResult<Invoice> Put([FromBody] Invoice invoice)
        {
            invoice = InvoiceRepository.Update(invoice);

            if (invoice == null)
                return BadRequest();

            return Ok(invoice);
        }

        [HttpPatch("{id}")]
        public ActionResult<Invoice> Patch(int id, [FromBody] Invoice patchInvoice)
        {
            var invoice = InvoiceRepository.Get(id);

            if (invoice == null)
                return NotFound();

            invoice.ReferenceMonth = patchInvoice.ReferenceMonth != 0 ? patchInvoice.ReferenceMonth : invoice.ReferenceMonth;
            invoice.ReferenceYear = patchInvoice.ReferenceYear != 0 ? patchInvoice.ReferenceYear : invoice.ReferenceYear;
            invoice.Document = !string.IsNullOrEmpty(patchInvoice.Document) ? patchInvoice.Document : invoice.Document;
            invoice.Description = !string.IsNullOrEmpty(patchInvoice.Description) ? patchInvoice.Description : invoice.Description;
            invoice.Amount = patchInvoice.Amount != 0 ? patchInvoice.Amount : invoice.Amount;

            invoice = InvoiceRepository.Update(invoice);

            if (invoice == null)
                return BadRequest();

            return Ok(invoice);
        }

        [HttpDelete("{id}")]
        public ActionResult<Invoice> Delete(int id)
        {
            var invoice = InvoiceRepository.Delete(id);

            if (invoice == null)
                return NotFound();

            if (invoice.IsActive && invoice.DeactivatedAt != null)
                return BadRequest();

            return Accepted(invoice);
        }
    }
}
