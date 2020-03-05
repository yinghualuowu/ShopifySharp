using System.Collections.Generic;
using System.Linq;
using ShopifySharp.Fluent.Properties;

namespace ShopifySharp.Fluent
{

    public interface IPropertyValue
    {
        object GetPropertyValue();
    }
    
    public class PropertyValue : IPropertyValue
    {
        public PropertyValue(object value)
        {
            Value = value;
        }
        
        public object Value { get; }
        
        public object GetPropertyValue()
        {
            return Value;
        }
    }
    
    public class NullPropertyValue : IPropertyValue
    {
        public object GetPropertyValue()
        {
            return null;
        }
    }
    
    public class ScriptTagBuilder
    {
        private Dictionary<ScriptTagProperty, IPropertyValue> _properties { get; } = new Dictionary<ScriptTagProperty, IPropertyValue>();
        
        public ScriptTagBuilder AddSrc(string src)
        {
            _properties[ScriptTagProperty.Src] = new PropertyValue(src);
            
            return this;
        }
        
        public ScriptTagBuilder AddEvent(string eventType)
        {
            _properties[ScriptTagProperty.Event] = new PropertyValue(eventType);
            
            return this;
        }

        public ScriptTagBuilder AddDisplayScope(string displayScope)
        {
            _properties[ScriptTagProperty.DisplayScope] = new PropertyValue(displayScope);
            
            return this;
        }

        public ScriptTagBuilder MakePropertyNull(ScriptTagProperty prop)
        {
            _properties[prop] = new NullPropertyValue();
            
            return this;
        }

        public ScriptTagBuilder RemoveProperty(ScriptTagProperty prop)
        {
            if (_properties.ContainsKey(prop))
            {
                _properties.Remove(prop);
            }
            
            return this;
        }

        public Dictionary<string, object> ToDictionary()
        {
            var result = _properties
                .Select(kvp => 
                    new KeyValuePair<string, object>(kvp.Key.ToSerializedString(), kvp.Value.GetPropertyValue()));

            var output = new Dictionary<string, object>();

            foreach (var kvp in result)
            {
                output.Add(kvp.Key, kvp.Value);
            }

            return output;
        }
    }
}