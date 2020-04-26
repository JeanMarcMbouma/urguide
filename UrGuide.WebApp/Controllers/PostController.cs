using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrGuide.WebApp.Data;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{


    //[ApiController]
    //[Route("[controller]")]
    //public class PostController : ControllerBase
    //{
    //    private readonly ApplicationDbContext _context;

    //    public PostController(ApplicationDbContext context)
    //    {
    //        _context = context;
    //    }

    //    [HttpGet]
    //    public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
    //    {
    //        return await _context.Posts_Table.ToListAsync();
    //    }

    //    //[Authorize]
    //    [HttpPost("newpost")]
    //    public async Task<ActionResult<Post>> NewPost([FromBody]CreatePostModel model)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return BadRequest(ModelState);
    //        }
    //        var user = _context.Users.Find(model.UserId);

    //        if(user == null)
    //        {
    //            return BadRequest(ErrorEnvelop.Create(new[] { "No user found." }));
    //        }

    //        Post post = new Post
    //        {
    //            Text = model.Text,
    //            Date = DateTime.Now,
    //            UserId = user.Id,

    //        };

    //        _context.Posts_Table.Add(post);
    //        await _context.SaveChangesAsync();

    //        foreach (var shot in model.Photos)
    //        {
    //            Shot img = new Shot { FilePath = shot, HasPost = true, PostId = post.Id, UserId = user.Id };

    //            _context.Shots_Table.Add(img);
    //            await _context.SaveChangesAsync();
    //        }

    //        return Ok( new { action = "inserted" });

    //        //return CreatedAtAction("GetPost", new { id = post.Id }, post);
    //    }

    //    [HttpGet("{id}")]
    //    public async Task<ActionResult<Post>> GetPost(long id)
    //    {
    //        var post = await _context.Posts_Table.FindAsync(id);

    //        if (post == null)
    //        {
    //            return NotFound();
    //        }

    //        return post;
    //    }

    //    [Authorize]
    //    [HttpPut("{id}")]
    //    public async Task<IActionResult> EditPost(long id, Post post)
    //    {
    //        if (id != post.Id)
    //        {
    //            return BadRequest();
    //        }

    //        _context.Entry(post).State = EntityState.Modified;

    //        try
    //        {
    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //            if (!PostExists(id))
    //            {
    //                return NotFound();
    //            }
    //            else
    //            {
    //                throw;
    //            }
    //        }

    //        return Ok(new { action = "updated" });
    //    }

    //    [Authorize]
    //    [HttpDelete("{id}")]
    //    public async Task<ActionResult<Post>> DeletePost(long id)
    //    {
    //        var post = await _context.Posts_Table.FindAsync(id);
    //        if (post == null)
    //        {
    //            return NotFound();
    //        }

    //        var shots = _context.Shots_Table.Where(x => x.PostId == post.Id).ToList();

    //        foreach( var shot in shots)
    //        {

    //            _context.Shots_Table.Remove(shot);
    //            await _context.SaveChangesAsync();
    //        }

    //        _context.Posts_Table.Remove(post);
    //        await _context.SaveChangesAsync();

    //        return Ok(new { action = "deleted" });
    //    }

    //    private bool PostExists(long id)
    //    {
    //        return _context.Posts_Table.Any(e => e.Id == id);
    //    }
    //}
}
