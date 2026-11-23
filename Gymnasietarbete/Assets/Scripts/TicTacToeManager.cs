using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TicTacToeManager : MonoBehaviour
{
    [Header("UI References - Game")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private Button[] boardButtons;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button restartButton;

    [Header("UI References - Dialogue")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcDialogueText;
    [SerializeField] private Button choiceButtonA;
    [SerializeField] private Button choiceButtonB;
    [SerializeField] private Button choiceButtonC;
    [SerializeField] private TextMeshProUGUI textA;
    [SerializeField] private TextMeshProUGUI textB;
    [SerializeField] private TextMeshProUGUI textC;

    [Header("World Barriers & Player Constraints")]
    [SerializeField] private GameObject exitDoor;             // Opened upon winning
    [SerializeField] private Collider npcBlockingCollider;    // Invisible wall blocking the path
    [SerializeField] private MonoBehaviour playerMovementScript; // e.g. PlayerController script to freeze player

    private string[] board = new string[9];
    private bool playerTurn = true;
    private bool gameActive = false;

    // Dialogue State
    private int dialogueStep = 0;
    private bool passedFirstTwoQuestions = false;
    private bool playerUnlockedWinningCondition = false;
    private int q1Answer = -1;
    private int q2Answer = -1;

    // ANSWER KEYS:
    // Q1 Correct: Choice B (1) -> "Patience..."
    // Q2 Correct: Choice A (0) -> "A mirror of trapdoors..."
    // Q3 (Solvable Riddle) Correct: Choice C (2) -> "A shadow"
    private const int Q1_CORRECT = 1;
    private const int Q2_CORRECT = 0;
    private const int Q3_RIDDLE_CORRECT = 2;

    private void Start()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
            restartButton.gameObject.SetActive(false);
        }

        // Lock player in place when approaching NPC
        FreezePlayer(true);
    }

    #region Dialogue System (Simon Gruber x Indiana Jones)

    public void StartDialogueSequence()
    {
        dialogueStep = 0;
        passedFirstTwoQuestions = false;
        playerUnlockedWinningCondition = false;
        q1Answer = -1;
        q2Answer = -1;

        if (gamePanel != null) gamePanel.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (restartButton != null) restartButton.gameObject.SetActive(false);

        DisplayDialogueStep();
    }

    private void DisplayDialogueStep()
    {
        choiceButtonA.onClick.RemoveAllListeners();
        choiceButtonB.onClick.RemoveAllListeners();
        choiceButtonC.onClick.RemoveAllListeners();

        switch (dialogueStep)
        {
            case 0: // QUESTION 1
                npcDialogueText.text = "Keeper: 'Simon says: Halt, treasure hunter! You seek the Golden Idol behind this door, yet you stride in like a blind bull. Tell me... what is a explorer's greatest weapon?'";
                textA.text = "A) A loaded revolver and a quick trigger.";
                textB.text = "B) Patience, observation, and watching your step.";
                textC.text = "C) Dumb luck and a heavy whip.";

                choiceButtonA.onClick.AddListener(() => OnDialogueChoice(0));
                choiceButtonB.onClick.AddListener(() => OnDialogueChoice(1));
                choiceButtonC.onClick.AddListener(() => OnDialogueChoice(2));
                break;

            case 1: // QUESTION 2
                npcDialogueText.text = "Keeper: 'Tick-tock, Dr. Jones... time is ticking! Look at this ancient grid before us. What does an amateur see vs. a true master?'";
                textA.text = "A) The master sees a mirror of trapdoors; the fool sees 9 plain tiles.";
                textB.text = "B) Just a simple game of noughts and crosses.";
                textC.text = "C) An insurmountable curse designed to crush me.";

                choiceButtonA.onClick.AddListener(() => OnDialogueChoice(0));
                choiceButtonB.onClick.AddListener(() => OnDialogueChoice(1));
                choiceButtonC.onClick.AddListener(() => OnDialogueChoice(2));
                break;

            case 2: // QUESTION 3 (BRANCHING RIDDLE)
                // Evaluate if Q1 and Q2 were correct
                passedFirstTwoQuestions = (q1Answer == Q1_CORRECT && q2Answer == Q2_CORRECT);

                if (passedFirstTwoQuestions)
                {
                    // SOLVABLE RIDDLE
                    npcDialogueText.text = "Keeper: 'Impressive... You actually have a brain under that fedora! Very well, here is your final riddle: I follow you into the darkest tombs, I mimic your every step without a sound, yet I perish the moment the torch goes out. What am I?'";
                    textA.text = "A) An ancient curse.";
                    textB.text = "B) A poison dart.";
                    textC.text = "C) Your own shadow.";
                }
                else
                {
                    // IMPOSSIBLE / TRICK QUESTION
                    npcDialogueText.text = "Keeper: 'Wrong, wrong, WRONG! You lack the wit for these ruins! Here is your final test: As I was going to St. Ives, I met a man with seven wives... how many traps will spring when you take your next step?'";
                    textA.text = "A) Seven traps!";
                    textB.text = "B) Forty-nine!";
                    textC.text = "C) Just one big explosion!";
                }

                choiceButtonA.onClick.AddListener(() => OnDialogueChoice(0));
                choiceButtonB.onClick.AddListener(() => OnDialogueChoice(1));
                choiceButtonC.onClick.AddListener(() => OnDialogueChoice(2));
                break;

            default:
                if (dialoguePanel != null) dialoguePanel.SetActive(false);
                StartGame();
                break;
        }
    }

    private void OnDialogueChoice(int choiceIndex)
    {
        if (dialogueStep == 0) q1Answer = choiceIndex;
        if (dialogueStep == 1) q2Answer = choiceIndex;
        if (dialogueStep == 2)
        {
            // If passed first two questions AND solved riddle (Choice C)
            if (passedFirstTwoQuestions && choiceIndex == Q3_RIDDLE_CORRECT)
            {
                playerUnlockedWinningCondition = true;
                Debug.Log("RIDDLE SOLVED! CPU defense lowered.");
            }
            else
            {
                playerUnlockedWinningCondition = false;
                Debug.Log("FAILED RIDDLE/QUESTIONS! CPU playing Unbeatable Minimax.");
            }
        }

        dialogueStep++;
        DisplayDialogueStep();
    }

    #endregion

    #region Tic-Tac-Toe Game Flow

    public void StartGame()
    {
        if (gamePanel != null) gamePanel.SetActive(true);
        if (restartButton != null) restartButton.gameObject.SetActive(false);

        gameActive = true;
        playerTurn = true;
        statusText.text = "Your Turn! (X)";

        for (int i = 0; i < boardButtons.Length; i++)
        {
            board[i] = "";
            if (boardButtons[i] == null) continue;

            TextMeshProUGUI tmpText = boardButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null) tmpText.text = "";

            boardButtons[i].interactable = true;

            int index = i;
            boardButtons[i].onClick.RemoveAllListeners();
            boardButtons[i].onClick.AddListener(() => OnCellClicked(index));
        }
    }

    private void MakeMove(int index, string symbol)
    {
        board[index] = symbol;

        if (boardButtons[index] != null)
        {
            TextMeshProUGUI tmpText = boardButtons[index].GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null) tmpText.text = symbol;
            boardButtons[index].interactable = false;
        }
    }

    private void OnCellClicked(int index)
    {
        if (!gameActive || !playerTurn || board[index] != "") return;

        MakeMove(index, "X");

        if (CheckWin("X"))
        {
            EndGame("You win! The keeper steps aside and the path opens.", isWin: true);
            UnlockPath();
            return;
        }

        if (IsBoardFull())
        {
            EndGame("It's a draw! Simon says: Try again!", isWin: false);
            return;
        }

        playerTurn = false;
        StartCoroutine(NPCMoveRoutine());
    }

    private IEnumerator NPCMoveRoutine()
    {
        yield return new WaitForSeconds(0.6f);

        int moveIndex = playerUnlockedWinningCondition ? GetRandomMove() : GetBestMove();

        if (moveIndex != -1)
        {
            MakeMove(moveIndex, "O");

            if (CheckWin("O"))
            {
                EndGame("Keeper wins! 'Too slow!'", isWin: false);
                yield break;
            }

            if (IsBoardFull())
            {
                EndGame("It's a draw! 'You cannot pass until you beat me!'", isWin: false);
                yield break;
            }
        }

        playerTurn = true;
        statusText.text = "Your Turn! (X)";
    }

    public void RestartGame()
    {
        StartDialogueSequence();
    }

    #endregion

    #region Path Blocking & Unlocking

    private void UnlockPath()
    {
        // 1. Open exit door
        if (exitDoor != null) exitDoor.SetActive(false);

        // 2. Disable blocking invisible wall
        if (npcBlockingCollider != null) npcBlockingCollider.enabled = false;

        // 3. Unfreeze player movement
        FreezePlayer(false);
    }

    private void FreezePlayer(bool freeze)
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = !freeze;
        }
    }

    #endregion

    #region AI Logic (Minimax vs. Flawed)

    private int GetRandomMove()
    {
        List<int> availableMoves = new List<int>();
        for (int i = 0; i < 9; i++)
        {
            if (board[i] == "") availableMoves.Add(i);
        }
        return availableMoves.Count > 0 ? availableMoves[Random.Range(0, availableMoves.Count)] : -1;
    }

    private int GetBestMove()
    {
        int bestScore = int.MinValue;
        int bestMove = -1;

        for (int i = 0; i < 9; i++)
        {
            if (board[i] == "")
            {
                board[i] = "O";
                int score = Minimax(board, 0, false);
                board[i] = "";

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                }
            }
        }
        return bestMove;
    }

    private int Minimax(string[] currentBoard, int depth, bool isMaximizing)
    {
        if (CheckWinOnBoard(currentBoard, "O")) return 10 - depth;
        if (CheckWinOnBoard(currentBoard, "X")) return depth - 10;
        if (IsBoardFullOnBoard(currentBoard)) return 0;

        if (isMaximizing)
        {
            int maxEval = int.MinValue;
            for (int i = 0; i < 9; i++)
            {
                if (currentBoard[i] == "")
                {
                    currentBoard[i] = "O";
                    int eval = Minimax(currentBoard, depth + 1, false);
                    currentBoard[i] = "";
                    maxEval = Mathf.Max(maxEval, eval);
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            for (int i = 0; i < 9; i++)
            {
                if (currentBoard[i] == "")
                {
                    currentBoard[i] = "X";
                    int eval = Minimax(currentBoard, depth + 1, true);
                    currentBoard[i] = "";
                    minEval = Mathf.Min(minEval, eval);
                }
            }
            return minEval;
        }
    }

    private bool CheckWinOnBoard(string[] b, string symbol)
    {
        int[][] winConditions = new int[][]
        {
            new int[] {0, 1, 2}, new int[] {3, 4, 5}, new int[] {6, 7, 8},
            new int[] {0, 3, 6}, new int[] {1, 4, 7}, new int[] {2, 5, 8},
            new int[] {0, 4, 8}, new int[] {2, 4, 6}
        };

        foreach (var condition in winConditions)
        {
            if (b[condition[0]] == symbol && b[condition[1]] == symbol && b[condition[2]] == symbol)
                return true;
        }
        return false;
    }

    private bool IsBoardFullOnBoard(string[] b)
    {
        foreach (string cell in b)
        {
            if (cell == "") return false;
        }
        return true;
    }

    private bool CheckWin(string symbol) => CheckWinOnBoard(board, symbol);
    private bool IsBoardFull() => IsBoardFullOnBoard(board);

    private void EndGame(string message, bool isWin)
    {
        gameActive = false;
        statusText.text = message;

        foreach (Button btn in boardButtons)
        {
            if (btn != null) btn.interactable = false;
        }

        if (!isWin && restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
        }
    }

    public void CloseUI()
    {
        if (gamePanel != null) gamePanel.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    #endregion
}