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

        /// <summary>
        /// 手机号可以查询，但给一天次数的限制
        /// userid 查找则需要限制，是 内部或外部api 查询时可以，外部api 比如 用户好友api
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search/{phone}")]
        public async Task<IActionResult> Search(string phone)
        {
            var data = await _userContext.Users
                .Include(u => u.Properties)
                .SingleOrDefaultAsync(u => u.Id == UserIdentity.UserId);
            return Ok();
        }

        //TBD  FromBody 的api 调用问题待解决
        [HttpPut]
        [Route("tags")]
        public async Task<IActionResult> UpdateUserTags([FromBody]List<string> tags)
        {
            var originTags = await _userContext
                .UserTags
                .Where(u => u.AppUserId == UserIdentity.UserId)
                .ToListAsync();
            var newTags = tags.Except(originTags.Select(t => t.Tag.ToString()));

            await _userContext.UserTags.AddRangeAsync(newTags.Select(t => new Model.UserTag
            {
                CreationTime = DateTime.Now,
                AppUserId = UserIdentity.UserId,
                Tag = int.Parse(t)
            }));
            await _userContext.SaveChangesAsync();
            return Ok();
        }
    }
}
