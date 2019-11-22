using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Scholarships.Models.Forms;

namespace Scholarships.Views.Shared.TagHelpers
{
    [HtmlTargetElement("option", Attributes = OptSelectedAttributeName)]

    public class OptionSelectTagHelper : TagHelper
    {
        private const string OptSelectedAttributeName = "opt-selected-value";

        [HtmlAttributeName(OptSelectedAttributeName)]
        public string ModelType { get; set; }

        [HtmlAttributeName("value")] public string OptionType { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ModelType == OptionType)
                output.Attributes.SetAttribute("selected", "selected");

        }
    }
}
