using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrailerRunner : MonoBehaviour
{
    enum Step { Idle, PlayingCall, AwaitingTransfer, Done }

    [SerializeField] GameObject mainActiveBackground;
    [SerializeField] GameObject mainInactiveBackground;
    [SerializeField] GameObject transferWindow;

    [Header("Ringing")]
    [SerializeField] GameObject callRinging;
    [SerializeField] Image ringingImage;
    [SerializeField] Sprite ringA;
    [SerializeField] Sprite ringB;
    [SerializeField] float ringDuration = 5f;
    [SerializeField] float ringSwapInterval = 0.25f;

    [Header("Answered")]
    [SerializeField] GameObject callAnswered;
    [SerializeField] TMP_Text callText;
    [SerializeField, TextArea] string callTranscript = "Hi, this is Bob from accounting. Could you put me through to Linda? It's about the quarterly report.";
    [SerializeField] float charsPerSecond = 30f;

    Step step = Step.Idle;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        if (step == Step.Idle)
        {
            step = Step.PlayingCall;
            StartCoroutine(PlayCall());
        }
        else if (step == Step.AwaitingTransfer)
        {
            transferWindow.SetActive(true);
            mainActiveBackground.SetActive(false);
            mainInactiveBackground.SetActive(true);
            step = Step.Done;
        }
    }

    IEnumerator PlayCall()
    {
        callRinging.SetActive(true);
        bool showA = true;
        for (float elapsed = 0f; elapsed < ringDuration; elapsed += ringSwapInterval)
        {
            ringingImage.sprite = showA ? ringA : ringB;
            showA = !showA;
            yield return new WaitForSeconds(ringSwapInterval);
        }
        callRinging.SetActive(false);

        callAnswered.SetActive(true);
        callText.text = callTranscript;
        callText.maxVisibleCharacters = 0;
        // characterCount excludes rich-text tags, unlike string.Length
        callText.ForceMeshUpdate();
        int total = callText.textInfo.characterCount;
        for (int i = 1; i <= total; i++)
        {
            callText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(1f / charsPerSecond);
        }

        step = Step.AwaitingTransfer;
    }
}
