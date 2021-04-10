using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICatalogPanel : MonoBehaviour
{

    public E_CatalogType CurrentCatalogType { get; private set; }

    public void Init(E_CatalogType catalogType)
    {
        CurrentCatalogType = catalogType;
    }
}
