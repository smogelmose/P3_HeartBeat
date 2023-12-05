using UnityEngine;
using System.Collections;


public class heartControl : MonoBehaviour
{
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
    }

    void Update()
    {
        float bpmValue = bpmReceiver.GetLatestBPM();

        if (bpmValue <= 0)
        {
            bpmValue = 95f;
        }

        ControlHeart(bpmValue);
        ApplyPulsatingEffect(bpmValue);
        PlayHeartSound(bpmValue);
        HeartAnimation(bpmValue);
    }

    void HeartAnimation(float bpmValue)
    {
        if (bpmValue > 27 && bpmValue < 54)
        {
            SetAnimationTrigger("VerySlowTrigger");
        }
        else if (bpmValue >= 54 && bpmValue < 84)
        {
            SetAnimationTrigger("CalmTrigger");
        }
        else if (bpmValue >= 84 && bpmValue < 100)
        {
            SetAnimationTrigger("FastTrigger");
        }
        else if (bpmValue >= 100 && bpmValue < 112)
        {
            SetAnimationTrigger("PanicTrigger");
        }
        else if (bpmValue >= 112 && bpmValue < 142)
        {
            SetAnimationTrigger("VeryFastTrigger");
        }
        else if (bpmValue >= 142 && bpmValue < 178)
        {
            SetAnimationTrigger("ArrhythmiaTrigger");
        }
        else if (bpmValue >= 178 && bpmValue < 210)
        {
            SetAnimationTrigger("SevereArrhythmiaTrigger");
        }
        else if (bpmValue >= 210 && bpmValue < 234)
        {
            SetAnimationTrigger("TachycardiaTrigger");
        }
        else if (bpmValue <= 5 && bpmValue < 27)
        {
            SetAnimationTrigger("NearDeathTrigger");
        }
        else if (bpmValue <= 0 && bpmValue < 5)
        {
            SetAnimationTrigger("DeathTrigger");
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
