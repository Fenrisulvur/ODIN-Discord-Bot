using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace IslaBot.Storage.Implementations
{
    public class JSON_RW
    {
        public static void Write(IDictionary dict)
        {
            File.WriteAllText("Storage/DB.json", JsonConvert.SerializeObject(dict));
        }
        public static dynamic Read()
        {
           
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("Storage/DB.json"));
            return dictionary;
        }

    }
}
