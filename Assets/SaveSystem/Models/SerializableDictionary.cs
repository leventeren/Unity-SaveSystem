using System;
using System.Collections.Generic;

namespace SaveSystem.Models
{
    /// <summary>
    /// Serializable dictionary for Unity.
    /// Unity's JsonUtility doesn't support Dictionary, so we use a list-based approach.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    [Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        [UnityEngine.SerializeField]
        private List<TKey> keys = new List<TKey>();
        
        [UnityEngine.SerializeField]
        private List<TValue> values = new List<TValue>();
        
        private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
        
        public TValue this[TKey key]
        {
            get => dictionary[key];
            set
            {
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key] = value;
                    int index = keys.IndexOf(key);
                    values[index] = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }
        
        public void Add(TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                keys.Add(key);
                values.Add(value);
            }
        }
        
        public bool Remove(TKey key)
        {
            if (dictionary.Remove(key))
            {
                int index = keys.IndexOf(key);
                keys.RemoveAt(index);
                values.RemoveAt(index);
                return true;
            }
            return false;
        }
        
        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);
        
        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);
        
        public void Clear()
        {
            dictionary.Clear();
            keys.Clear();
            values.Clear();
        }
        
        public int Count => dictionary.Count;
        
        public IEnumerable<TKey> Keys => dictionary.Keys;
        
        public IEnumerable<TValue> Values => dictionary.Values;
        
        /// <summary>
        /// Called before serialization to sync dictionary to lists.
        /// </summary>
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (var kvp in dictionary)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }
        
        /// <summary>
        /// Called after deserialization to rebuild dictionary from lists.
        /// </summary>
        public void OnAfterDeserialize()
        {
            dictionary = new Dictionary<TKey, TValue>();
            for (int i = 0; i < keys.Count && i < values.Count; i++)
            {
                dictionary[keys[i]] = values[i];
            }
        }
    }
}
