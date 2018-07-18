using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GamePlus.bean
{
    public class PurchaseItem
    {
        public PurchaseItem(float price, string displayName, string itemType, string productId)
        {
            this.price = price;
            this.displayName = displayName;
            this.itemType = itemType;
            this.productId = productId;
        }

        public PurchaseItem()
        {

        }

        private float price;

        public float Price
        {
            get { return price; }
            set { price = value; }
        }
        private string displayName;

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }
        private string itemType;

        public string ItemType
        {
            get { return itemType; }
            set { itemType = value; }
        }
        private string productId;

        public string ProductId
        {
            get { return productId; }
            set { productId = value; }
        }
    }
}
