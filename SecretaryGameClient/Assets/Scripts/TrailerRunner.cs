using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrailerRunner : MonoBehaviour
{
    enum Step { Idle, Ringing, TypingCall, AwaitingTransfer, Dialing, ShowingSuccess, Done }

    [Serializable]
    class Call
    {
        [TextArea] public string transcript;
        // Children are revealed one per click in Hierarchy order
        public GameObject transferIcons;
    }

    [SerializeField] GameObject mainActiveBackground;
    [SerializeField] GameObject mainInactiveBackground;
    [SerializeField] GameObject transferWindow;

    [Header("Ringing")]
    [SerializeField] GameObject callRinging;
    [SerializeField] Image ringingImage;
    [SerializeField] Sprite ringA;
    [SerializeField] Sprite ringB;
    [SerializeField] float ringSwapInterval = 0.25f;

    [Header("Answered")]
    [SerializeField] GameObject callAnswered;
    [SerializeField] TMP_Text callText;
    [SerializeField] float charsPerSecond = 30f;

    [Header("Calls")]
    [SerializeField] Call[] calls =
    {
        new Call { transcript = "Hi, this is Bob from accounting. Could you put me through to Linda? It's about the quarterly report." },
        new Call { transcript = "HELLO?! Is anyone there? I need the complaints department, and I need it NOW." },
    };
    [SerializeField] GameObject successMessage;

    Step step = Step.Idle;
    int callIndex;
    int iconsShown;
    Coroutine ringRoutine;

    Call CurrentCall => calls[callIndex];

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

        switch (step)
        {
            case Step.Idle:
                callRinging.SetActive(true);
                ringRoutine = StartCoroutine(Ring());
                step = Step.Ringing;
                break;

            case Step.Ringing:
                StopCoroutine(ringRoutine);
                callRinging.SetActive(false);
                step = Step.TypingCall;
                StartCoroutine(TypeCall());
                break;

            case Step.AwaitingTransfer:
                transferWindow.SetActive(true);
                SetMainFocused(false);
                CurrentCall.transferIcons.SetActive(true);
                foreach (Transform icon in CurrentCall.transferIcons.transform)
                {
                    icon.gameObject.SetActive(false);
                }
                iconsShown = 0;
                step = Step.Dialing;
                break;

            case Step.Dialing:
                Transform icons = CurrentCall.transferIcons.transform;
                if (iconsShown < icons.childCount)
                {
                    icons.GetChild(iconsShown).gameObject.SetActive(true);
                    iconsShown++;
                }
                else
                {
                    transferWindow.SetActive(false);
                    CurrentCall.transferIcons.SetActive(false);
                    callAnswered.SetActive(false);
                    SetMainFocused(true);
                    successMessage.SetActive(true);
                    step = Step.ShowingSuccess;
                }
                break;

            case Step.ShowingSuccess:
                successMessage.SetActive(false);
                callIndex++;
                step = callIndex < calls.Length ? Step.Idle : Step.Done;
                break;

            // TypingCall advances itself when the coroutine finishes; Done ignores clicks until reset
        }
    }

    void SetMainFocused(bool focused)
    {
        // MainWindow ships with its inactive background disabled, so both must flip
        mainActiveBackground.SetActive(focused);
        mainInactiveBackground.SetActive(!focused);
    }

    IEnumerator Ring()
    {
        bool showA = true;
        while (true)
        {
            ringingImage.sprite = showA ? ringA : ringB;
            showA = !showA;
            yield return new WaitForSeconds(ringSwapInterval);
        }
    }

    IEnumerator TypeCall()
    {
        callAnswered.SetActive(true);
        callText.text = CurrentCall.transcript;
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
