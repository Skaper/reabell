using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Invector
{
    public class PlayerHealthController : vHealthController
    {
        public CircleProgressBar hpbar;
        // Update is called once per frame
        private void Start()
        {
            base.Start();
            hpbar.maxValue = MaxHealth;
        }
        void Update()
        {
            hpbar.value = (int)currentHealth;
        }

        public override void TakeDamage(vDamage damage)
        {
            base.TakeDamage(damage);
        }

        public void TakeDamage(float value)
        {
            currentHealth -= value;
        }

    }
}