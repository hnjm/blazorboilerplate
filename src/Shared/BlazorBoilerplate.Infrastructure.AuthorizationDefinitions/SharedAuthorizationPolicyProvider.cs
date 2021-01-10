﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace BlazorBoilerplate.Infrastructure.AuthorizationDefinitions
{
    public class SharedAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;

        public SharedAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
            _options = options.Value;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName);

            if (policy == null)
            {
                bool created = false;
                switch (policyName)
                {
                    //In DatabaseInitializer: await _userManager.AddClaimAsync(applicationUser, new Claim($"Is{role}", "true"));
                    case Policies.IsAdmin:
                        policy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .RequireClaim("IsAdministrator")
                            .Build();

                        created = true;
                        break;

                    case Policies.IsUser:
                        policy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .RequireClaim("IsUser")
                            .Build();

                        created = true;
                        break;

                    //https://docs.microsoft.com/it-it/aspnet/core/security/authentication/mfa
                    case Policies.TwoFactorEnabled:
                        policy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .RequireClaim("amr", "mfa")
                            .Build();

                        created = true;
                        break;

                    case Policies.IsMyEmailDomain:
                        policy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .AddRequirements(new DomainRequirement("blazorboilerplate.com"))
                            .Build();

                        created = true;
                        break;
                }

                if (created)
                    _options.AddPolicy(policyName, policy);
            }

            return policy;
        }
    }
}