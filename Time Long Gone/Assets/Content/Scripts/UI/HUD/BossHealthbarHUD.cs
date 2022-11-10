using Content.Scripts.Variables;
using DG.Tweening;
using Enemy;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HUD
{
    public class BossHealthbarHUD : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private BoolVariable IsRewinding;

        private IEnemy enemy;
        private float maxHealth;
        private float currHealth;
        private void Start()
        {
            enemy = FindObjectsOfType<MonoBehaviour>().OfType<IEnemy>().SingleOrDefault();
            maxHealth = enemy.Health;
            currHealth = maxHealth;
            slider.value = 1;
        }

        private void Update()
        {
            if(currHealth != enemy.Health)
            {
                float dif = (currHealth - enemy.Health)/maxHealth;
                currHealth = enemy.Health;
                slider.DOValue(currHealth / maxHealth, 0.5f);
                if(!IsRewinding.Value)slider.transform.DOShakePosition(0.2f, dif*1000);
            }
        }

    }
}
