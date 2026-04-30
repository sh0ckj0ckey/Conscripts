using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Conscripts.Models
{
    [JsonSerializable(typeof(List<ShortcutModel>))]
    public partial class SourceGenerationContext : JsonSerializerContext { }
}
