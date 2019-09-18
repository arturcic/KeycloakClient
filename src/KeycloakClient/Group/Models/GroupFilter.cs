namespace KeycloakClient.Group.Models
{
    public class GroupFilter : IFilter
    {
        [AliasAs("search")]
        public string Search { get; set; }
        [AliasAs("first")]
        public int? First { get; set; }
        [AliasAs("max")]
        public int? Max { get; set; }
    }
}