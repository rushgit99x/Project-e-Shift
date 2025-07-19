using e_Shift.Models;
using System.Threading.Tasks;

namespace e_Shift.Repository.Interface
{
    public interface ICustomerRepository
    {
        Task<Customer> RegisterCustomerAsync(Customer customer); // Changed from Task<bool> to Task<Customer>
        Task<Customer> GetCustomerByCredentialsAsync(string email, string password);
    }
}