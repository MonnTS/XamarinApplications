using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ReadWriteProperty
{
    public class JsonData
    {
        public List<JObject> JsonObject { get; } = new List<JObject>();
    }
}