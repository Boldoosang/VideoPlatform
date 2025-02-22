using Microsoft.AspNetCore.Razor.TagHelpers;

namespace VideoPlatform.Web.Helpers {
    [HtmlTargetElement("*", Attributes = "requires-role")]
    public class RequiresRoleTagHelper : TagHelper {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequiresRoleTagHelper(IHttpContextAccessor httpContextAccessor) {
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName("requires-role")]
        public string RequiredRoles { get; set; } = "";

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null) {
                output.SuppressOutput();
                return;
            }

            var roles = RequiredRoles.Split(',').Select(role => role.Trim()).ToList();

            var hasRequiredRole = roles.Any(role => user.IsInRole(role));

            if (!hasRequiredRole) {
                output.SuppressOutput();
            }
        }
    }
}
