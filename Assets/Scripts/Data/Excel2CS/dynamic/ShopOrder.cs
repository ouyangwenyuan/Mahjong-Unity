using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Data.BeanMap;
using UnityEngine;
using SQLite4Unity3d;

public class ShopOrder
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int ItemOrder { get; set; }
    public int Tag { get; set; }

    public ShopOrder()
    {}

    public ShopOrder(ShopOrderMap map)
    {
        id = map.id;
        ItemOrder = map.ItemOrder;
        Tag = map.Tag;
    }
}
