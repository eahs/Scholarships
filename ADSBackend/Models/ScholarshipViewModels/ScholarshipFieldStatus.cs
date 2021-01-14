namespace Scholarships.Models.ScholarshipViewModels
{
    public class ScholarshipFieldStatus
    {
        public string FieldName { get; set; }
        public bool Validated { get; set; }
        public string ErrorMessage { get; set; }
        public string FormURI { get; set; }  // URI to place user can fix this element
    }
}
