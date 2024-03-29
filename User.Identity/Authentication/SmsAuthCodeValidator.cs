﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using User.Identity.Services;

namespace User.Identity.Authentication
{
    public class SmsAuthCodeValidator : IExtensionGrantValidator
    {
        private readonly IAuthCodeService _authCodeService;
        private readonly IUserService _userService;
        public SmsAuthCodeValidator(IAuthCodeService authCodeService, IUserService userService)
        {
            _authCodeService = authCodeService;
            _userService = userService;
        }
        public string GrantType => "sms_auth_code";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var phone = context.Request.Raw["phone"];
            var code = context.Request.Raw["auth_code"];
            var errorValidationResult = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(code))
            {
                errorValidationResult.ErrorDescription = "手机或者验证码为空错误";
                context.Result = errorValidationResult;
                return;
            }
            //检查验证码
            if (!_authCodeService.Validate(phone, code))
            {
                errorValidationResult.ErrorDescription = "手机或者验证码错误";
                context.Result = errorValidationResult;
                return;
            }
            //完成用户注册
            var userId = await _userService.CheckOrCreateAsync(phone);
            if (userId <= 0)
            {
                errorValidationResult.ErrorDescription = "检查或创建用户错误";
                context.Result = errorValidationResult;
                return;
            }
            context.Result = new GrantValidationResult(userId.ToString(), GrantType);
        }
    }
}
