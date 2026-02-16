using SaveSystem.Core;
using SaveSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SaveSystem.Examples
{
    /// <summary>
    /// Example card data.
    /// </summary>
    [Serializable]
    public class Card
    {
        public string Id;
        public string Name;
        public int Attack;
        public int Defense;
        public string Rarity;
        public List<string> Abilities;
        
        public Card()
        {
            Abilities = new List<string>();
        }
    }
    
    /// <summary>
    /// Example player inventory with complex data types.
    /// Demonstrates List and Dictionary usage.
    /// </summary>
    [Serializable]
    public class PlayerInventory : ISaveData
    {
        public int Version => 1;
        
        public List<Card> OwnedCards;
        public SerializableDictionary<string, int> CardCounts;
        public SerializableDictionary<string, Card> FavoriteCards;
        
        public PlayerInventory()
        {
            OwnedCards = new List<Card>();
            CardCounts = new SerializableDictionary<string, int>();
            FavoriteCards = new SerializableDictionary<string, Card>();
        }
        
        public bool Validate()
        {
            // Check if counts match owned cards
            if (OwnedCards == null || CardCounts == null || FavoriteCards == null)
                return false;
            
            // Validate each card
            foreach (var card in OwnedCards)
            {
                if (string.IsNullOrEmpty(card.Id) || string.IsNullOrEmpty(card.Name))
                    return false;
                
                if (card.Attack < 0 || card.Defense < 0)
                    return false;
            }
            
            return true;
        }
        
        public string GetChecksum()
        {
            // Generate checksum from inventory data
            StringBuilder sb = new StringBuilder();
            
            foreach (var card in OwnedCards.OrderBy(c => c.Id))
            {
                sb.Append(card.Id);
                sb.Append(card.Attack);
                sb.Append(card.Defense);
            }
            
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
