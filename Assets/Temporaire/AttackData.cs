using UnityEngine;

public enum AttackType
{
    Damage,      // Inflige des dégâts
    Heal,        // Soigne le joueur
    Buff,        // Améliore les stats temporairement
    Debuff       // Réduit les stats de l'ennemi
}

[CreateAssetMenu(fileName = "New Attack", menuName = "Game/Attack Data")]
public class AttackData : ScriptableObject
{
    [Header("Attack Info")]
    public string attackName;
    public Sprite attackIcon;

    [Header("Damage")]
    public int damageAmount; // Dégâts infligés à l'ennemi quand on clique sur ce bouton
}