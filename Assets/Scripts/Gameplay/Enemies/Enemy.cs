using System;

public interface IEnemy
{
    event Action<IEnemy> OnDeath;
}
