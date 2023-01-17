using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemShops.Utils;

[CreateAssetMenu(fileName = "Tag", menuName = "ItemShop/Tag")]
public class TagObject : ScriptableObject, IProvider<Tag>
{
    public string name;

    public Tag GetValue()
    {
        return new Tag(name);
    }
}
