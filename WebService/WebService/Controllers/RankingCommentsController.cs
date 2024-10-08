/************************************************************
 * File:        RankingCommentsController.cs
 * Author:      IT21210174 - Tharushi Lakshika V.G
 * Date:        2024-09-22
 * Description: This file implements the RankingCommentsController class,
 *              which provides API endpoints for managing rankings and 
 *              comments related to vendors. It interacts with the 
 *              IRankingComment service and User collection to manage 
 *              vendor rankings and comments.
 ************************************************************/

using DnsClient;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebService.Interfaces;
using WebService.Models;
using WebService.Services;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RankingCommentsController : ControllerBase
    {
        private readonly IRankingComment _rankingCommnetService;
        private readonly IMongoCollection<User> _userCollection;

        public RankingCommentsController(IRankingComment rankingCommentService, IMongoCollection<User> userCollection)
        {
            _rankingCommnetService = rankingCommentService;
            _userCollection = userCollection;
        }

        /// Adds a new ranking for a vendor.

        [HttpPost]
        public async Task<IActionResult> AddRankForVendor(Ranking ranking)
        {
            try
            {
                // Adding the rank
                await _rankingCommnetService.AddRankForVendor(ranking);

                // Fetch the vendor (from the User collection) and update the AverageRating
                var vendor = await _userCollection.Find(u => u.Id == ranking.VendorId && u.Role == "3").FirstOrDefaultAsync();

                if (vendor != null)
                {
                    // Calculate the new average rating after adding the new ranking
                    var vendorRankings = await _rankingCommnetService.GetRankingForVendor(ranking.VendorId);
                    vendor.AverageRating = vendorRankings.Average(r => r.ranking);

                    // Update the vendor's average rating in the User collection
                    var update = Builders<User>.Update.Set(u => u.AverageRating, vendor.AverageRating);
                    await _userCollection.UpdateOneAsync(u => u.Id == vendor.Id, update);
                }

                return Created("", new { message = "Ranking added successfully!", vendorAverageRating = vendor?.AverageRating });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while adding ranking", Details = ex.Message });
            }
        }

        /// Retrieves the ranking for a specific vendor.
        [HttpGet("vendorRatings/{id}")]
        public async Task<IActionResult> GetRankingForVendor(string id)
        {
            try
            {
                var ranking = await _rankingCommnetService.GetRankingForVendor(id);

                return Ok(ranking.Select(r => new
                {
                    r.Id,
                    r.CustomerId,
                    r.VendorId,
                    r.ranking,
                    r.CustomerName
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Fail to get all rankings for the specific vendor", Details = ex.Message });
            }
        }

        // Adds a new comment for a vendor.
        [HttpPost("addComment")]
        public async Task<IActionResult> AddCommentForVendor([FromBody] Comment comment)
        {
            try
            {
                // Verify if the customer and vendor exist
                var customer = await _userCollection.Find(u => u.Id == comment.CustomerId && u.Role == "0").FirstOrDefaultAsync();
                var vendor = await _userCollection.Find(u => u.Id == comment.VendorId && u.Role == "3").FirstOrDefaultAsync();

                if (customer == null)
                {
                    return BadRequest("Invalid Customer.");
                }

                if (vendor == null)
                {
                    return BadRequest("Invalid Venoder.");
                }

                // Add the comment
                await _rankingCommnetService.AddCommentForVendor(comment);

                return Created("", new { message = "Comment added successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while adding the comment", Details = ex.Message });
            }
        }

        // Edit a comment (only the customer who owns it can edit it)
        [HttpPut("editComment/{commentId}")]
        public async Task<IActionResult> EditCommentForVendor(string commentId, [FromBody] Comment comment)
        {
            try
            {
                // Check if the customer has permission to edit the comment
                await _rankingCommnetService.EditCommentForVendor(commentId, comment.CustomerId, comment.comment);

                return Ok(new { message = "Comment updated successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the comment", Details = ex.Message });
            }
        }

        /// Retrieves comments for a specific vendor.
        [HttpGet("vendorComments/{vendorId}")]
        public async Task<IActionResult> GetCommentsByVendorId(string vendorId)
        {
            try
            {
                // Verify if the vendor exists
                var vendor = await _userCollection.Find(u => u.Id == vendorId && u.Role == "3").FirstOrDefaultAsync();

                if (vendor == null)
                {
                    return NotFound(new { message = "Vendor not found." });
                }

                var comments = await _rankingCommnetService.GetCommentsByVendorId(vendorId);

                return Ok(comments.Select(c => new
                {
                    c.Id,
                    c.CustomerId,
                    c.VendorId,
                    c.comment,
                    c.CreatedAt,
                    c.CustomerName
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving comments", Details = ex.Message });
            }
        }
    }
}
