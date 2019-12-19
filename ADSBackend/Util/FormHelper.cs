using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace Scholarships.Util
{
    public class FHItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
    }
    public class FormHelper
    {
        public static string JsonStatus(string statusMessage)
        {
            return JsonConvert.SerializeObject(new { Status = statusMessage });
        }

        public static string JsonStatus(object result)
        {
            return JsonConvert.SerializeObject(result);
        }

        public static IEnumerable<FHItem> Genders = new[]
        {
                new FHItem { Name = "Male", Id = 0},
                new FHItem { Name = "Female", Id = 1},
                new FHItem { Name = "Other", Id = 2}
            }.ToList();

        public static IEnumerable<FHItem> LivingSituations = new[]
        {
                new FHItem { Name = "Live On Campus", Key = "oncampus"},
                new FHItem { Name = "Commute", Key = "commute"}
            }.ToList();

        public static IEnumerable<FHItem> Relationships = new[]
            {
                new FHItem { Name = "", Id = 0},
                new FHItem { Name = "Self", Id = 1},
                new FHItem { Name = "Father", Id = 2},
                new FHItem { Name = "Mother", Id = 3},
                new FHItem { Name = "Guardian", Id = 4}
            }.ToList();

        public static IEnumerable<FHItem> EmploymentStatus = new[]
        {
                new FHItem { Name = "", Id = 0},
                new FHItem { Name = "Full Time", Id = 1},
                new FHItem { Name = "Part Time", Id = 2},
                new FHItem { Name = "Unemployed", Id = 3}
        }.ToList();

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

        /*
        public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(_config["StoredFilesPath"],
                        Path.GetRandomFileName());

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, filePath });
        }
        */

        /*
        public static string GetDisplayName<TModel>(Expression<Func<TModel, object>> expression)
        {
            Type type = typeof(TModel);

            string propertyName = null;
            string[] properties = null;
            IEnumerable<string> propertyList;
            //unless it's a root property the expression NodeType will always be Convert
            switch (expression.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = expression.Body as UnaryExpression;
                    propertyList = (ue != null ? ue.Operand : null).ToString().Split(".".ToCharArray()).Skip(1); //don't use the root property
                    break;
                default:
                    propertyList = expression.Body.ToString().Split(".".ToCharArray()).Skip(1);
                    break;
            }

            //the propert name is what we're after
            propertyName = propertyList.Last();
            //list of properties - the last property name
            properties = propertyList.Take(propertyList.Count() - 1).ToArray(); //grab all the parent properties

            foreach (string property in properties)
            {
                PropertyInfo propertyInfo = type.GetProperty(property);
                type = propertyInfo.PropertyType;
            }

            DisplayNameAttribute attr;
            attr = (DisplayNameAttribute)type.GetProperty(propertyName).GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();

            // Look for [MetadataType] attribute in type hierarchy
            // http://stackoverflow.com/questions/1910532/attribute-isdefined-doesnt-see-attributes-applied-with-metadatatype-class
            if (attr == null)
            {
                MetadataTypeAttribute metadataType = (MetadataTypeAttribute)type.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault();
                if (metadataType != null)
                {
                    var property = metadataType.MetadataClassType.GetProperty(propertyName);
                    if (property != null)
                    {
                        attr = (DisplayNameAttribute)property.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    }
                }
            }
            return (attr != null) ? attr.DisplayName : String.Empty;
        }
        */
    }
}
