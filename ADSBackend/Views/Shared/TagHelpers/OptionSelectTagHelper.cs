using Microsoft.AspNetCore.Razor.TagHelpers;
using Scholarships.Models.Forms;
using System;

namespace Scholarships.Views.Shared.TagHelpers
{
    [HtmlTargetElement("option", Attributes = OptSelectedAttributeName)]

    public class OptionSelectTagHelper : TagHelper
    {
        private const string OptSelectedAttributeName = "opt-selected-value";
        private QuestionType _optionType = QuestionType.MultipleChoice;

        [HtmlAttributeName(OptSelectedAttributeName)]
        public QuestionType ModelType { get; set; }

        [HtmlAttributeName("value")]
        public string OptionType
        {
            get { return OptionType; }
            set { _optionType = (QuestionType)Enum.Parse(typeof(QuestionType), value); }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ModelType == _optionType)
                output.Attributes.SetAttribute("selected", "selected");

            output.Attributes.SetAttribute("value", (int)_optionType);
        }
    }
}
