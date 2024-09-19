using WebService.Models;

namespace WebService.Interfaces
{
    public interface IVendorService
    {
        Task RegisterVendor( Vendor vendor);
    }
}
