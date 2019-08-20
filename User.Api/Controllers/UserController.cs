using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using User.Api.Data;
using User.Api.Model;

namespace User.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : BaseController
    {
        private UserContext _userContext;
        private ILogger<UserController> _logger;

        public UserController(UserContext userContext, ILogger<UserController> logger)
        {
            _userContext = userContext;
            _logger = logger;
        }
        [Route("get")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await _userContext.Users.AsNoTracking()
                .Include(u => u.Properties)
                .FirstOrDefaultAsync(u => u.Id == UserIdentity.UserId);
            if (user == null)
            {
                _logger.LogError("获取用户失败，用户为null");
                throw new UserOperationException("错误的用户上下文ID");
            }
            return Json(user);
        }

        [Route("update")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]JsonPatchDocument<AppUser> patch)
        {
            /*
                       {
                           "op":"replace",
                           "path":"/Company",
                           "value":"adminA"
                       } 
             */
            var user = await _userContext.Users.FirstOrDefaultAsync(u => u.Id == UserIdentity.UserId);
            patch.ApplyTo(user);

            foreach (var property in user?.Properties)
            {
                _userContext.Entry(property).State = EntityState.Detached;
            }

            var originProperties = await _userContext.UserProperty.AsNoTracking()
                .Where(u => u.AppUserId == UserIdentity.UserId).ToListAsync();
            //合并，去重
            var allProperties = originProperties.Union(user.Properties).Distinct();
            //这里的意思是strList1中哪些是strList2中没有的,并将获得的差值存放在strList3(即: strList1中有, strList2中没有)
            var removedProperties = originProperties.Except(user.Properties);
            var newProperties = allProperties.Except(originProperties);

            foreach (var property in removedProperties)
            {
                _userContext.Remove(property);
            }
            foreach (var item in newProperties)
            {
                _userContext.Add(item);
            }
            await _userContext.SaveChangesAsync();
            return Json(user);
        }

        /// <summary>
        /// 判断用户是否存在，不存在创建用户
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [Route("checkorcreate")]
        [HttpPost]
        public async Task<IActionResult> CheckOrCreate([FromForm]string phone)
        {
            var user = await _userContext.Users.FirstOrDefaultAsync(u => u.Phone == phone);
            if (user == null)
            {
                user = new AppUser() { Phone = phone };
                _userContext.Users.Add(user);
                await _userContext.SaveChangesAsync();
            }
            return Ok(user.Id);
        }
    }
}
