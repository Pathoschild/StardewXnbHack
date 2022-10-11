using System.Reflection;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace StardewXnbHack.Framework.Writers
{
    /// <summary>A Json.NET contract resolver which ignores properties marked with <see cref="ContentSerializerIgnoreAttribute"/>.</summary>
    internal class IgnoreDefaultOptionalPropertiesResolver : DefaultContractResolver
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (member.GetCustomAttribute<ContentSerializerIgnoreAttribute>() != null)
                property.ShouldSerialize = _ => false;

            return property;

        }
    }
}
