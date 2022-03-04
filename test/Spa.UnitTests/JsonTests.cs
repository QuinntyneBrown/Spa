using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Spa.UnitTests
{
    public class JsonTests
    {
        [Fact]
        public void Recursive()
        {
            var path = @"C:\projects\file.json";

            var json = File.ReadAllText(path);

            JObject jObject = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(json);

            Process(jObject);

            void Process(JObject jObject)
            {
                foreach (var prop in jObject.Children<JProperty>())
                {
                    switch(prop.Value)
                    {
                        case JObject childJObject:
                            Process(childJObject);
                            break;

                        case JValue value:
                            Console.WriteLine($"{value}");
                            break;
                    }
                }
            }

        }
    }
}
