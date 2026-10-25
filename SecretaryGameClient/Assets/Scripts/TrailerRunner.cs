using UnityEngine;
using UnityEngine.SceneManagement;

public class TrailerRunner : MonoBehaviour
{
    [SerializeField] GameObject mainActiveBackground;
    [SerializeField] GameObject mainInactiveBackground;
    [SerializeField] GameObject transferWindow;

    bool transferShown;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetMouseButtonDown(0) && !transferShown)
        {
            transferWindow.SetActive(true);
            mainActiveBackground.SetActive(false);
            mainInactiveBackground.SetActive(true);
            transferShown = true;
        }
    }
}
