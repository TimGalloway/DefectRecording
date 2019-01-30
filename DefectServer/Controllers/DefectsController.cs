using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using DefectServer.Models;
using System.IO;
using System.Drawing;
using System.Web.Hosting;

namespace DefectServer.Controllers
{
    public class DefectsController : ApiController
    {
        private DefectServerContext db = new DefectServerContext();

        // GET: api/Defects
        public IQueryable<Defect> GetDefects()
        {
            return db.Defects;
        }

        // GET: api/Defects/5
        [ResponseType(typeof(Defect))]
        public async Task<IHttpActionResult> GetDefect(int id)
        {
            Defect defect = await db.Defects.FindAsync(id);
            if (defect == null)
            {
                return NotFound();
            }

            return Ok(defect);
        }

        // PUT: api/Defects/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDefect(int id, Defect defect)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != defect.Id)
            {
                return BadRequest();
            }

            db.Entry(defect).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DefectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Defects
        [ResponseType(typeof(Defect))]
        public async Task<IHttpActionResult> PostDefect(Defect defect)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(defect.ImageBase64)))
            {
                using (Bitmap bm2 = new Bitmap(ms))
                {
                    string filePath = HostingEnvironment.MapPath("~/Images/");
                    bm2.Save(@filePath + defect.ImageName);
                }
            }

            db.Defects.Add(defect);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = defect.Id }, defect);
        }

        // DELETE: api/Defects/5
        [ResponseType(typeof(Defect))]
        public async Task<IHttpActionResult> DeleteDefect(int id)
        {
            Defect defect = await db.Defects.FindAsync(id);
            if (defect == null)
            {
                return NotFound();
            }

            db.Defects.Remove(defect);
            await db.SaveChangesAsync();

            return Ok(defect);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DefectExists(int id)
        {
            return db.Defects.Count(e => e.Id == id) > 0;
        }
    }
}