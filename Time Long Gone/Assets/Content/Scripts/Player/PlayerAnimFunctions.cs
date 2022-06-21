using Content.Scripts.Player;
using UnityEngine;

public class PlayerAnimFunctions : MonoBehaviour
{
    private PlayerScript _player;

    public void Start() => _player = GetComponentInParent<PlayerScript>();

    public void Hit() => _player.combat.Hit(false);

    public void Finisher() => _player.combat.Hit(true);
}
