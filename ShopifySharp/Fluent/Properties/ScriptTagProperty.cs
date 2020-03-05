using System.Runtime.Serialization;

namespace ShopifySharp.Fluent.Properties
{
    public enum ScriptTagProperty
    {
        [EnumMember(Value = "display_scope")]
        DisplayScope,
        
        [EnumMember(Value = "event")]
        Event,
        
        [EnumMember(Value = "src")]
        Src
    }
}