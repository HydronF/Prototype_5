using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image playerHealthBar;
    public Image cooldownBar;
    public GameObject promptPrefab;
    Dictionary<Transform, GameObject> promptLookup;
    // Start is called before the first frame update
    void Start()
    {
        promptLookup = new Dictionary<Transform, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateCooldown(float currCooldown, float cooldown) {
        cooldownBar.fillAmount = Mathf.Clamp((cooldown - currCooldown) / cooldown, 0.0f, 1.0f);
    }

    internal void ShowPickupPrompt(Transform compTransform)
    {   
        if (promptLookup.ContainsKey(compTransform)) {
            promptLookup[compTransform].SetActive(true);
        }
        else {
            GameObject prompt = Instantiate(promptPrefab, transform);
            promptLookup.Add(compTransform, prompt);
            prompt.GetComponent<UIFollow>().Initialize(compTransform, GetComponent<RectTransform>());
        }
    }

    internal void HidePickupPrompt(Transform compTransform) {
        promptLookup[compTransform].SetActive(false);
    }

    internal void RemovePickupPrompt(Transform compTransform)
    {
        if (promptLookup.ContainsKey(compTransform)) {
            Destroy(promptLookup[compTransform]);
            promptLookup.Remove(compTransform);
        }
    }

    public void UpdatePlayerHealth(float currHealth, float maxHealth) {
        playerHealthBar.fillAmount = Mathf.Clamp(currHealth / maxHealth, 0.0f, 1.0f);
    }

    public void DeathScreen() {

    }

}
