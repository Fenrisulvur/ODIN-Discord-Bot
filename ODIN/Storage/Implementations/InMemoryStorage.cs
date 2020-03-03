using System;
using System.Collections.Generic;

namespace IslaBot.Storage.Implementations
{
    public class InMemoryStorage
    {
        private Dictionary<string, string> _dictionary = JSON_RW.Read(); //Load JSON data for users to cache

        public void StoreObject(string key, string value)
        {
            if (_dictionary.ContainsKey(key)) return; //Do not overrite keys
            _dictionary.Add(key, value); //Add key if it doesnt exist
            JSON_RW.Write(_dictionary);
        }

        public string GrabObject(string key)
        {
            if (!_dictionary.ContainsKey(key))
                return null; //Return null if no key exists
            return _dictionary[key]; //Return key value if exists
        }

        public dynamic GetFromDiscordId(string value)
        {
            foreach(KeyValuePair<string, string> set  in _dictionary)
            {
                if(set.Value == value)
                {
                    return set;
                }
            }
            return "Null";
        }

    }
}
