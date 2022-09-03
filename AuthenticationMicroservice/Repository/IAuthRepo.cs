using AuthenticationMicroservice.Model;
using System.Threading.Tasks;

namespace AuthenticationMicroservice.Repository
{
    public interface IAuthRepo
    {
        public Task<LoginResponse> Login(LoginDTO dto);
    }
}
