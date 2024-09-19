using WebService.Models;

namespace WebService.Interfaces
{
    public interface IRankingComment
    {
        Task AddRankForVendor(Ranking ranking);
        Task<List<Ranking>> GetRankingForVendor(string id);

        Task AddCommentForVendor(Comment comment);
        Task EditCommentForVendor(string commentId, string customerId, string comment);

        Task<List<Comment>> GetCommentsByVendorId(string vendorId);
    }
}
