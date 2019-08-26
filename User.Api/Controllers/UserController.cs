using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using User.Api.Data;
using User.Api.Events;
using User.Api.Model;

namespace User.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : BaseController
    {
        private UserContext _userContext;
        private ILogger<UserController> _logger;
        private readonly ICapPublisher _publisher;
        public UserController(UserContext userContext, ILogger<UserController> logger, ICapPublisher publisher)
        {
            _userContext = userContext;
            _logger = logger;
            _publisher = publisher;
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
            var user = await _userContext.Users
                .SingleOrDefaultAsync(u => u.Id == UserIdentity.UserId);
            patch.ApplyTo(user);

            using (var transaction = _userContext.Database.BeginTransaction())
            {
                try
                {
                    RaiseUserprofileChangedEvent(user);
                    await _userContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"更新用户信息失败：{ex.Message}");
                    transaction.Rollback();
                }
            }

            return Json(user);
        }

        private void RaiseUserprofileChangedEvent(Model.AppUser user)
        {
            if (_userContext.Entry(user).Property(nameof(user.Name)).IsModified ||
                _userContext.Entry(user).Property(nameof(user.Avatar)).IsModified ||
                _userContext.Entry(user).Property(nameof(user.Company)).IsModified ||
                _userContext.Entry(user).Property(nameof(user.Title)).IsModified)
            {
                _publisher.Publish("finbook_userapi_userprofilechanged", new UserProfileChangedEvent
                {
                    Name = user.Name,
                    Avatar = user.Avatar,
                    Company = user.Company,
                    Title = user.Title,
                    UserId = user.Id,

                });
            }
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
