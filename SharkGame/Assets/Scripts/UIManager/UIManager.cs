using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Image playerHealthBar;
    public RectTransform cooldownBG;
    public Image cooldownBar;
    public GameObject startText;
    public GameObject endText;
    public GameObject promptPrefab;
    public Text killText;
    Dictionary<Transform, GameObject> promptLookup;
    int killCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        promptLookup = new Dictionary<Transform, GameObject>();
        startText.SetActive(true);
        endText.SetActive(false);
        killText.text = "Kill: " + killCount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && startText.activeSelf) {
            startText.SetActive(false);
        }
        if (Input.GetKeyUp(KeyCode.R)) {
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            AudioListener.volume = 1.0f;
        }
    }

    internal void TrackKill()
    {
        ++killCount;
        killText.text = "Kill: " + killCount.ToString();
    }

    public void UpdateCooldown(float currCooldown, float cooldown) {
        cooldownBG.sizeDelta = new Vector2 (cooldown * 100, 10.0f);
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
        endText.SetActive(true);
        AudioListener.volume = AudioListener.volume * 0.4f;
    }

}
