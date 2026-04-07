using System.Collections.Generic;

namespace UrGuide.Model.Messages
{
    public class SendDirectMessageCommand
    {
        public string To { get; set; }
        public string ToName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public string LinkText { get; set; }

        /// <summary>
        /// BCP 47 language tag (e.g. "en", "fr", "es"). Defaults to "en".
        /// Used to select the correct localised template.
        /// </summary>
        public string Language { get; set; } = "en";

        /// <summary>
        /// Name of the admin-managed email template to render.
        /// When set, the proprietary template engine fetches the template
        /// from the database, substitutes <see cref="TemplateVariables"/>,
        /// and uses the rendered output instead of <see cref="Content"/>.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Key/value pairs substituted into the template using {{Key}} syntax.
        /// Standard variables (ToName, Link, LinkText, Content) are automatically
        /// added from the matching properties when a template is specified.
        /// </summary>
        public Dictionary<string, string> TemplateVariables { get; set; }
    }
}
