using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AppAuthorizeAttribute : TypeFilterAttribute
{
    #region Ctor

    public AppAuthorizeAttribute(bool ignore = false) : base(typeof(AuthorizeCustomerFilter))
    {
        IgnoreFilter = ignore;
        Arguments = new object[] { ignore };
    }

    #endregion

    #region Properties

    public bool IgnoreFilter { get; }

    #endregion

    #region Nested filter

    private class AuthorizeCustomerFilter : IAsyncAuthorizationFilter
    {
        #region Fields

        private readonly bool _ignoreFilter;
        private readonly IWebHostEnvironment _env;

        #endregion

        #region Ctor

        public AuthorizeCustomerFilter(bool ignoreFilter, IWebHostEnvironment env)
        {
            _ignoreFilter = ignoreFilter;
            _env = env;
        }

        #endregion

        #region Methods

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            /*if (_env.IsDevelopment())
                return Task.CompletedTask;*/

            //check whether this filter has been overridden for the Action
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<AppAuthorizeAttribute>()
                .FirstOrDefault();

            //ignore filter (the action is available even if navigation is not allowed)
            if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                return Task.CompletedTask;

            if (context.HttpContext.Items["Uid"] is null)
                // not logged in
                context.Result = new JsonResult(new { message = "Unauthorized" })
                    { StatusCode = StatusCodes.Status401Unauthorized };
            return Task.CompletedTask;
        }

        #endregion
    }

    #endregion
}