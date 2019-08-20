using System.Threading.Tasks;

namespace User.Identity.Services
{
    public interface IUserService
    {
        /// <summary>
        /// 检查手机号码是否已经注册，如果没有注册的话就注册一个用户
        /// </summary>
        /// <param name="phone"></param>
        Task<int> CheckOrCreateAsync(string phone);
    }
}
