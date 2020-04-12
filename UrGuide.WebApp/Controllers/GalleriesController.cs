using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrGuide.WebApp.Data;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
   
    [ApiController]
    [Route("[controller]")]
    public class GalleriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GalleriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Gallery
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CreateNewGallery>>> GetGalleries_Table()
        {
            var galleries =  await _context.Galleries_Table.ToListAsync();

            List<CreateNewGallery> myList = new List<CreateNewGallery>();

            int index = 0;
            
            foreach( var gallery in galleries)
            {

                index++;

                var shots = _context.Shots_Table.Where(x => x.GalleryId == gallery.Id).ToList();

                List<File> files = new List<File>();

                int num = 0;

                foreach (var shot in shots)
                {
                    num++;

                    files.Add(new File { Href = shot.FilePath, Description = shot.Description, Id = num });
                }

                CreateNewGallery model = new CreateNewGallery { Id = index ,Title = gallery.Title, Description = gallery.Description, Location = gallery.Location, UserId = gallery.UserId, Files = files.ToArray() };

                myList.Add(model);
            }

            return myList;
        }

        // GET: api/Gallery/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CreateNewGallery>> GetGallery(long id)
        {
            var gallery = await _context.Galleries_Table.FindAsync(id);

            if (gallery == null)
            {
                return NotFound();
            }

            var shots = _context.Shots_Table.Where(x => x.GalleryId == gallery.Id).ToList();

            List<File> files = new List<File>();

            foreach( var shot in shots)
            {
               files.Add(new File { Href = shot.FilePath, Description = shot.Description });
            }

            CreateNewGallery model = new CreateNewGallery { Title = gallery.Title, Description = gallery.Description, Location = gallery.Location, UserId = gallery.UserId, Files = files.ToArray() };

            return model;
        }

        // PUT: api/Gallery/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGallery(long id, CreateNewGallery model)
        {
            if (!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }

            if (!_context.Galleries_Table.Any( x => x.Id == id))
            {
                return BadRequest();
            }

            var gallery = _context.Galleries_Table.Find(id);

            gallery = ConvertToGallery(gallery, model);

            _context.Galleries_Table.Update(gallery);

            await _context.SaveChangesAsync();

            foreach (var file in model.Files)
            {
                Shot shot = new Shot { GalleryId = gallery.Id, FilePath = file.Href, HasPost = false, Description = file.Description, UserId = gallery.UserId };

                _context.Shots_Table.Add(shot);

                await _context.SaveChangesAsync();

            }

            return NoContent();
        }

        // POST: api/Galleries
        [HttpPost]
        public async Task<ActionResult<Gallery>> PostGallery(CreateNewGallery model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var user = _context.Users.Find(model.UserId);

            //if (user == null)
            //{
            //    return BadRequest(ErrorEnvelop.Create(new[] { "No user found." }));
            //}

            Gallery gallery = new Gallery { Title = model.Title, Location = model.Location, Description = model.Description, Date = DateTime.Now, UserId = null };

            _context.Galleries_Table.Add(gallery);
            await _context.SaveChangesAsync();

            foreach( var file in model.Files)
            {
                Shot shot = new Shot { GalleryId = gallery.Id, FilePath = file.Href, HasPost = false, Description = file.Description, UserId = gallery.UserId };

                _context.Shots_Table.Add(shot);

                await _context.SaveChangesAsync();

            }

            return Ok();
        }

        // DELETE: api/Galleries/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Gallery>> DeleteGallery(long id)
        {
            var gallery = await _context.Galleries_Table.FindAsync(id);
            if (gallery == null)
            {
                return NotFound();
            }

            var shots = _context.Shots_Table.Where(x => x.GalleryId == gallery.Id).ToList();

            foreach( var shot in shots)
            {
                _context.Shots_Table.Remove(shot);
                await _context.SaveChangesAsync();
            }

            _context.Galleries_Table.Remove(gallery);
            await _context.SaveChangesAsync();

            return Ok(new { action = "deleted" });
        }

        private bool GalleryExists(long id)
        {
            return _context.Galleries_Table.Any(e => e.Id == id);
        }


        private Gallery ConvertToGallery(Gallery gallery, CreateNewGallery model)
        {
            gallery.Description = model.Description;
            gallery.Date = DateTime.Now;
            gallery.Title = model.Title;
            gallery.Location = model.Location;

            return gallery;
        }


    }
}
