using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image cooldownBar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCooldown(float currCooldown, float cooldown) {
        cooldownBar.fillAmount = Mathf.Clamp((cooldown - currCooldown) / cooldown, 0.0f, 1.0f);
    }
}
