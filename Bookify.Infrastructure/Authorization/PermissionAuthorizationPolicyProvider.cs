using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Authorization;

// Permission-based Authorization
internal sealed class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
	private readonly AuthorizationOptions _authorizationOptions;

	public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
	{
		_authorizationOptions = options.Value;
	}

	public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
	{
		// Calling the base implementation of the get policy async method, and see if we
		// get back a value
		var policy = await base.GetPolicyAsync(policyName);

		// If the policy that we get back is not null, we are going to return it from this method
		if (policy is not null) return policy;

		// Otherwise, we need to create a new permission policy instance. 
		// Creating a new authorization policy builder, and we are going to add a requirement to this
		// builder, and we are going to give it a new value of the permission requirement policy.
		// We will pass the policy name as our permission, and the we are just going to 
		// build our authorization policy. 
		var permissionPolicy = new AuthorizationPolicyBuilder()
		   .AddRequirements(new PermissionRequirement(policyName))
		   .Build();

		// IMPORTANT! We also must not forget to cache our policy value, which is why we use the
		// authorization options instance, because we can now say add policy and specify a policy with our name
		// So we specify the policy name and the permission policy value, and finally, we can return the permission
		// policy value from our get policy async method.
		_authorizationOptions.AddPolicy(policyName, permissionPolicy);

		return permissionPolicy;
	}
}