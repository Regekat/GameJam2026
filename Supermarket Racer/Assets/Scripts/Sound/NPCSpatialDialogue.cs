using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NPCSpatialDialogue : MonoBehaviour
{
    [Header("Dialogue Clips")]
    [SerializeField] private AudioClip[] dialogueClips = new AudioClip[5];

    [Header("Timing")]
    [SerializeField] private float dialogueInterval = 30f;
    [SerializeField] private bool startTimerOnAwake = true;

    [Header("Playtesting")]
    [SerializeField] private KeyCode testPlayKey = KeyCode.T;

    [Header("Playback")]
    [SerializeField] private bool allowInterruptingCurrentLine = false;
    [SerializeField] private bool avoidRepeatingLastLine = true;
    [SerializeField][Range(0f, 1f)] private float volumeScale = 1f;

    private AudioSource audioSource;
    private float timer;
    private int lastPlayedIndex = -1;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError($"[{nameof(NPCSpatialDialogue)}] No AudioSource found on {gameObject.name}.");
            enabled = false;
            return;
        }

        if (startTimerOnAwake)
        {
            timer = dialogueInterval;
        }
        else
        {
            timer = 0f;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(testPlayKey))
        {
            PlayRandomDialogue();
        }

        timer += Time.deltaTime;

        if (timer >= dialogueInterval)
        {
            timer = 0f;
            PlayRandomDialogue();
        }
    }

    public void PlayRandomDialogue()
    {
        if (dialogueClips == null || dialogueClips.Length == 0)
        {
            Debug.LogWarning($"[{nameof(NPCSpatialDialogue)}] No dialogue clips assigned on {gameObject.name}.");
            return;
        }

        int validClipCount = 0;
        for (int i = 0; i < dialogueClips.Length; i++)
        {
            if (dialogueClips[i] != null)
            {
                validClipCount++;
            }
        }

        if (validClipCount == 0)
        {
            Debug.LogWarning($"[{nameof(NPCSpatialDialogue)}] Dialogue array contains no valid clips on {gameObject.name}.");
            return;
        }

        if (!allowInterruptingCurrentLine && audioSource.isPlaying)
        {
            return;
        }

        int randomIndex = GetRandomClipIndex();
        if (randomIndex == -1)
        {
            Debug.LogWarning($"[{nameof(NPCSpatialDialogue)}] Failed to find a valid dialogue clip on {gameObject.name}.");
            return;
        }

        AudioClip chosenClip = dialogueClips[randomIndex];
        lastPlayedIndex = randomIndex;

        audioSource.PlayOneShot(chosenClip, volumeScale);
    }

    private int GetRandomClipIndex()
    {
        if (dialogueClips.Length == 1)
        {
            return dialogueClips[0] != null ? 0 : -1;
        }

        int safety = 0;
        int randomIndex = -1;

        do
        {
            randomIndex = Random.Range(0, dialogueClips.Length);
            safety++;

            if (safety > 50)
            {
                break;
            }
        }
        while (
            dialogueClips[randomIndex] == null ||
            (avoidRepeatingLastLine && randomIndex == lastPlayedIndex && HasMoreThanOneValidClip())
        );

        if (randomIndex >= 0 && randomIndex < dialogueClips.Length && dialogueClips[randomIndex] != null)
        {
            return randomIndex;
        }

        for (int i = 0; i < dialogueClips.Length; i++)
        {
            if (dialogueClips[i] != null)
            {
                return i;
            }
        }

        return -1;
    }

    private bool HasMoreThanOneValidClip()
    {
        int count = 0;

        for (int i = 0; i < dialogueClips.Length; i++)
        {
            if (dialogueClips[i] != null)
            {
                count++;
            }
        }

        return count > 1;
    }
}