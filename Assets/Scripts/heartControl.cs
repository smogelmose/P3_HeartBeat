using UnityEngine;
using System.Collections;


public class heartControl : MonoBehaviour
{
    public float defaultBPM = 80f;

    public float baseScale = 1.0f;
    public float scaleMultiplier = 0.1f;
    public BPMReceiver bpmReceiver;

    public AudioClip beat1Sound;
    public AudioClip beat2Sound;
    private AudioSource audioSource;

    public float volume = 1.0f;
    public float pitch = 1.0f;

    private float timeBetweenBeats;
    private float lastPlayTime;
    public float cooldownTime = 1.0f;
 
    private Animator heartAnimator;

    void Start()
    {
        if (bpmReceiver == null)
        {
            Debug.LogError("BPMReceiver is not assigned to HeartControl.");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        lastPlayTime = Time.time;

        heartAnimator = GetComponent<Animator>();
        if (heartAnimator == null)
        {
            Debug.LogError("Animator component not found on the GameObject.");
        }

        baseScale = 1.0f; 
    }

    void Update()
    {
        float bpmValue = bpmReceiver.GetLatestBPM();

        if (bpmValue >= 300)
        {
            bpmValue = 0;
        }
        else if (bpmValue <= 0)
        {
            bpmValue = defaultBPM;
        }

        volume = Mathf.Clamp(bpmValue / 200f, 0.2f, 1.0f);
        pitch = Mathf.Clamp(bpmValue / 100f, 0.5f, 2.0f);
        cooldownTime = Mathf.Clamp(60f / bpmValue, 0.1f, 2.0f);

        // Correlate animation speed with BPM
        if (heartAnimator != null)
        {
            heartAnimator.speed = Mathf.Clamp(bpmValue / 80f, 0.5f, 2.0f); // 80 BPM is normal speed
        }

        HeartAnimation(bpmValue);
        ApplyPulsatingEffect(bpmValue); 
        PlayHeartSound(bpmValue);
    }

    void HeartAnimation(float bpmValue)
    {
        if (bpmValue == 0)
        {
            scaleMultiplier = 0.02f;
            SetAnimationTrigger("DeathTrigger");
            return; // Exit so no other triggers are set
        }
        else if (bpmValue > 0 && bpmValue < 40)
        {
            SetAnimationTrigger("NearDeathTrigger");
            scaleMultiplier = 0.08f;
        }
        else if (bpmValue >= 40 && bpmValue < 60)
        {
            SetAnimationTrigger("VerySlowTrigger");
            scaleMultiplier = 0.12f;
        }
        else if (bpmValue >= 60 && bpmValue < 100)
        {
            SetAnimationTrigger("CalmTrigger");
            scaleMultiplier = 0.15f;
        }
        else if (bpmValue >= 100 && bpmValue < 120)
        {
            SetAnimationTrigger("FastTrigger");
            scaleMultiplier = 0.18f;
        }
        else if (bpmValue >= 120 && bpmValue < 140)
        {
            SetAnimationTrigger("PanicTrigger");
            scaleMultiplier = 0.22f;
        }
        else if (bpmValue >= 140 && bpmValue < 160)
        {
            SetAnimationTrigger("VeryFastTrigger");
            scaleMultiplier = 0.25f;
        }
        else if (bpmValue >= 160 && bpmValue < 180)
        {
            SetAnimationTrigger("ArrhythmiaTrigger");
            scaleMultiplier = 0.28f;
        }
        else if (bpmValue >= 180 && bpmValue < 220)
        {
            SetAnimationTrigger("SevereArrhythmiaTrigger");
            scaleMultiplier = 0.32f;
        }
        else if (bpmValue >= 220)
        {
            SetAnimationTrigger("TachycardiaTrigger");
            scaleMultiplier = 0.35f;
        }
    }

    void SetAnimationTrigger(string triggerName)
    {
        if (heartAnimator != null)
        {
            heartAnimator.SetTrigger(triggerName);
        }
    }

    void ControlHeart(float bpmValue)
    {
        float scale = baseScale + scaleMultiplier * bpmValue;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    void ApplyPulsatingEffect(float bpmValue)
    {
        timeBetweenBeats = 60.0f / bpmValue;
        float pulsatingScale = Mathf.PingPong(Time.time / timeBetweenBeats, 1.0f);
        float newScale = baseScale + scaleMultiplier * pulsatingScale;
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    void PlayHeartSound(float bpmValue)
    {
        float currentTime = Time.time;
        float elapsedTimeSinceLastPlay = currentTime - lastPlayTime;
        
        if (elapsedTimeSinceLastPlay >= cooldownTime && bpmValue > 0)
        {
            audioSource.clip = beat1Sound;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.Play(); ;
            
            float delay = audioSource.clip.length;
            StartCoroutine(PlayBeat2AfterDelay(delay));

            lastPlayTime = currentTime + delay;
        }
    }

    IEnumerator PlayBeat2AfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        audioSource.clip = beat2Sound;
        audioSource.Play();
    }

}
