using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarships.Util
{
    public class FormHelper
    {
        public static string JsonStatus(string message)
        {
            return JsonConvert.SerializeObject(new { Status = message });
        }

        public static SelectList States = new SelectList(
            new[]
            {
                new { Text="Alabama", Value="AL"},
                new { Text="Alaska", Value="AK"},
                new { Text="Arizona", Value="AZ"},
                new { Text="Arkansas", Value="AR"},
                new { Text="California", Value="CA"},
                new { Text="Colorado", Value="CO"},
                new { Text="Connecticut", Value="CT"},
                new { Text="District of Columbia", Value="DC"},
                new { Text="Delaware", Value="DE"},
                new { Text="Florida", Value="FL"},
                new { Text="Georgia", Value="GA"},
                new { Text="Hawaii", Value="HI"},
                new { Text="Idaho", Value="ID"},
                new { Text="Illinois", Value="IL"},
                new { Text="Indiana", Value="IN"},
                new { Text="Iowa", Value="IA"},
                new { Text="Kansas", Value="KS"},
                new { Text="Kentucky", Value="KY"},
                new { Text="Louisiana", Value="LA"},
                new { Text="Maine", Value="ME"},
                new { Text="Maryland", Value="MD"},
                new { Text="Massachusetts", Value="MA"},
                new { Text="Michigan", Value="MI"},
                new { Text="Minnesota", Value="MN"},
                new { Text="Mississippi", Value="MS"},
                new { Text="Missouri", Value="MO"},
                new { Text="Montana", Value="MT"},
                new { Text="Nebraska", Value="NE"},
                new { Text="Nevada", Value="NV"},
                new { Text="New Hampshire", Value="NH"},
                new { Text="New Jersey", Value="NJ"},
                new { Text="New Mexico", Value="NM"},
                new { Text="New York", Value="NY"},
                new { Text="North Carolina", Value="NC"},
                new { Text="North Dakota", Value="ND"},
                new { Text="Ohio", Value="OH"},
                new { Text="Oklahoma", Value="OK"},
                new { Text="Oregon", Value="OR"},
                new { Text="Pennsylvania", Value="PA"},
                new { Text="Rhode Island", Value="RI"},
                new { Text="South Carolina", Value="SC"},
                new { Text="South Dakota", Value="SD"},
                new { Text="Tennessee", Value="TN"},
                new { Text="Texas", Value="TX"},
                new { Text="Utah", Value="UT"},
                new { Text="Vermont", Value="VT"},
                new { Text="Virginia", Value="VA"},
                new { Text="Washington", Value="WA"},
                new { Text="West Virginia", Value="WV"},
                new { Text="Wisconsin", Value="WI"},
                new { Text="Wyoming", Value="WY"}
            }, "Value", "Text");
    }
}
