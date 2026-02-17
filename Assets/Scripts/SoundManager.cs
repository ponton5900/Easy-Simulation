using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioSource audioSource;//SEópÇÃAudioSource
    public AudioClip playerTurnClip;
    public AudioClip enemyTurnClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //å¯â âπÇñ¬ÇÁÇ∑ä÷êî
    public void PlayTurnSound(AudioClip turnClip)
    {
        audioSource.PlayOneShot(turnClip);
        
    }
}
