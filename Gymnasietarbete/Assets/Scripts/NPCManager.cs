using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField] private TicTacToeManager ticTacToeManager;
    [SerializeField] private GameObject interactPrompt;

    private bool playerInProximity = false;

    void Update()
    {
        if (playerInProximity && Input.GetKeyDown(KeyCode.F))
        {
            if (ticTacToeManager != null)
            {
                ticTacToeManager.StartGame();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInProximity = true;
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInProximity = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);
            if (ticTacToeManager != null) ticTacToeManager.CloseUI();
        }
    }
}