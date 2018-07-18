using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GamePlus.bean;
using Assets.Script.gameplus.define;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Assets.Scripts.FirebaseController
{
    public class PurController
    {
        public static void InitPurchase(ref ConfigurationBuilder builder, ref Hashtable purchaseItems)
        {
            IEnumerable<Shop> shops = StaticDataBaseService.GetInstance().GetPurchase(); 
            List<Shop> itemShopCoin = shops.Where(x => x.product_code.Contains("coin")).ToList();
            List<Shop> itemShopBag = shops.Where(x => x.product_code.Contains("bag")).ToList();
            List<Shop> itemShops=new List<Shop>();
            itemShops.AddRange(itemShopCoin);
            itemShops.AddRange(itemShopBag);
            foreach (var itemShop in itemShops)
            {
                builder.AddProduct(itemShop.product_code, ProductType.Consumable);
                PurchaseItem item = new PurchaseItem(itemShop.usd,itemShop.product_code,"consumable",itemShop.product_code);
                purchaseItems.Add(itemShop.product_code, item);
            }
        }
    }
}
