using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealthManager
{
    void Harm(int damage);
    public int GetHealth();
    void ResetHealth();
    void TakeDamage(int damage);
}
