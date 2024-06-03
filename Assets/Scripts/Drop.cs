using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    public enum Type {Essence, Health, Life}

    protected Type type;
    public Type DropType { get => type; }
}
