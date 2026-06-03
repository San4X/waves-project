using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField] private int damageValue;


    public virtual int GetDemageValue()
    {
        return damageValue;
    }
}
