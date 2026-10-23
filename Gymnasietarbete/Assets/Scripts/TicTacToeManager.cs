using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TicTacToeManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private Button[] boardButtons;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Door / Reward Reference")]
    [SerializeField] private GameObject exitDoor;

    private string[] board = new string[9];
    private bool playerTurn = true;
    private bool gameActive = false;

    public void StartGame()
    {
        gamePanel.SetActive(true);
        gameActive = true;
        playerTurn = true;
        statusText.text = "Your Turn! (X)";

        for (int i = 0; i < boardButtons.Length; i++)
        {
            board[i] = "";

            if (boardButtons[i] == null)
            {
                Debug.LogError($"Board Button at index {i} is NULL in the Inspector!");
                continue;
            }

            TextMeshProUGUI tmpText = boardButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null)
            {
                tmpText.text = "";
            }
            else
            {
                Debug.LogError($"Button at index {i} ({boardButtons[i].name}) missing TextMeshProUGUI child!");
            }

            boardButtons[i].interactable = true;

            int index = i; // Cache loop variable for lambda
            boardButtons[i].onClick.RemoveAllListeners();
            boardButtons[i].onClick.AddListener(() => OnCellClicked(index));
        }
    }

    private void MakeMove(int index, string symbol)
    {
        board[index] = symbol;

        if (boardButtons[index] == null)
        {
            Debug.LogError($"boardButtons[{index}] is not assigned in the Inspector!");
            return;
        }

        TextMeshProUGUI tmpText = boardButtons[index].GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = symbol;
        }
        else
        {
            Debug.LogError($"TextMeshProUGUI missing on button at index {index}!");
        }

        boardButtons[index].interactable = false;
    }

    private void OnCellClicked(int index)
    {
        if (!gameActive || !playerTurn || board[index] != "") return;

        MakeMove(index, "X");

        if (CheckWin("X"))
        {
            EndGame("You win! The path is open.");
            OpenExitDoor();
            return;
        }

        if (IsBoardFull())
        {
            EndGame("It's a draw! Try again.");
            return;
        }

        playerTurn = false;
        StartCoroutine(NPCMoveRoutine());
    }

    private IEnumerator NPCMoveRoutine()
    {
        yield return new WaitForSeconds(0.8f);

        List<int> emptyIndices = new List<int>();
        for (int i = 0; i < 9; i++)
        {
            if (board[i] == "") emptyIndices.Add(i);
        }

        if (emptyIndices.Count > 0)
        {
            int randomIndex = emptyIndices[Random.Range(0, emptyIndices.Count)];
            MakeMove(randomIndex, "O");

            if (CheckWin("O"))
            {
                EndGame("NPC wins! Try again.");
            }

            if (IsBoardFull())
            {
                EndGame("It's a draw! Try again.");
            }
        }

        playerTurn = true;
        statusText.text = "Your Turn! (X)";
    }

    private bool CheckWin(string symbol)
    {
        int[][] winConditions = new int[][]
        {
            new int[] {0, 1, 2}, new int[] {3, 4, 5}, new int[] {6, 7, 8},
            new int[] {0, 3, 6}, new int[] {1, 4, 7}, new int[] {2, 5, 8},
            new int[] {0, 4, 8}, new int[] {2, 4, 6}
        };

        foreach (var condition in winConditions)
        {
            if (board[condition[0]] == symbol &&
                board[condition[1]] == symbol &&
                board[condition[2]] == symbol)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsBoardFull()
    {
        foreach (string cell in board)
        {
            if (cell == "") return false;
        }
        return true;
    }

    private void EndGame(string message)
    {
        gameActive = false;
        statusText.text = message;

        foreach (Button btn in boardButtons)
        {
            btn.interactable = false;
        }
    }

    private void OpenExitDoor()
    {
        if (exitDoor != null)
        {
            exitDoor.SetActive(false);
        }
    }

    public void CloseUI()
    {
        gamePanel.SetActive(false);
    }
}