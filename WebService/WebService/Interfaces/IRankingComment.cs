/************************************************************
 * File:        IRankingComment.cs
 * Author:      IT21210174 - Tharushi Lakshika V.G
 * Date:        2024-09-22
 * Description: This file defines the IRankingComment interface, 
 *              which provides methods for managing vendor rankings 
 *              and comments within the system.
 ************************************************************/


using WebService.Models;

namespace WebService.Interfaces
{
    public interface IRankingComment
    {
        /// Adds a new ranking for a vendor.
        Task AddRankForVendor(Ranking ranking);

        /// Retrieves a list of rankings for a specific vendor.
        Task<List<Ranking>> GetRankingForVendor(string id);

        /// Adds a comment for a vendor.
        Task AddCommentForVendor(Comment comment);

        /// Edits an existing comment for a vendor, verifying the customer's ownership of the comment.
        Task EditCommentForVendor(string commentId, string customerId, string comment);

        /// Retrieves a list of comments for a specific vendor.
        Task<List<Comment>> GetCommentsByVendorId(string vendorId);
    }
}
