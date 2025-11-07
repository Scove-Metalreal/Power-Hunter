using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Script này quản lý việc bật/tắt các GameObject con theo một pattern được chọn.
/// Gắn script này vào một GameObject cha, và nó sẽ tự động điều khiển các con của nó.
/// </summary>
public class ChildPatternController : MonoBehaviour
{
    public enum PatternType
    {
        None,
        SequentialToggle,
        SequentialOnOff,
        RandomToggle,
        PingPong,
        SlidingGap,
        // --- 3 PATTERNS MỚI ---
        ConvergeDiverge, // Hội tụ & Phân kỳ
        RandomFill,      // Lấp đầy & Làm trống Ngẫu nhiên
        FollowPlayer     // Theo dấu Người chơi
    }

    [Header("Pattern Settings")]
    [Tooltip("Kiểu pattern để bật/tắt các object con.")]
    public PatternType pattern = PatternType.SequentialToggle;

    [Tooltip("Thời gian chờ giữa các bước trong pattern (giây).")]
    public float delayBetweenSteps = 0.5f;

    [Tooltip("Với các pattern 'Toggle', đây là thời gian một object bị tắt trước khi bật lại.")]
    public float toggleDuration = 0.2f;

    [Tooltip("Với pattern 'SlidingGap' hoặc 'FollowPlayer', đây là độ rộng của khoảng trống.")]
    public int gapSize = 3;

    [Header("Specific Pattern Fields")]
    [Tooltip("Với pattern 'FollowPlayer', kéo Transform của người chơi vào đây.")]
    public Transform playerToFollow;

    [Header("Control")]
    [Tooltip("Tự động chạy pattern khi game bắt đầu.")]
    public bool startOnAwake = true;

    [Tooltip("Lặp lại pattern sau khi chạy xong một chu kỳ.")]
    public bool loopPattern = true;

    private List<GameObject> children = new List<GameObject>();
    private Coroutine activePatternCoroutine;

    void Awake()
    {
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }
    }

    void Start()
    {
        if (startOnAwake)
        {
            StartPattern();
        }
    }

    public void StartPattern()
    {
        if (activePatternCoroutine != null) StopCoroutine(activePatternCoroutine);

        switch (pattern)
        {
            case PatternType.SequentialToggle:
                activePatternCoroutine = StartCoroutine(SequentialTogglePattern());
                break;
            case PatternType.SequentialOnOff:
                activePatternCoroutine = StartCoroutine(SequentialOnOffPattern());
                break;
            case PatternType.RandomToggle:
                activePatternCoroutine = StartCoroutine(RandomTogglePattern());
                break;
            case PatternType.PingPong:
                activePatternCoroutine = StartCoroutine(PingPongPattern());
                break;
            case PatternType.SlidingGap:
                activePatternCoroutine = StartCoroutine(SlidingGapPattern());
                break;
            // --- CASES MỚI ---
            case PatternType.ConvergeDiverge:
                activePatternCoroutine = StartCoroutine(ConvergeDivergePattern());
                break;
            case PatternType.RandomFill:
                activePatternCoroutine = StartCoroutine(RandomFillPattern());
                break;
            case PatternType.FollowPlayer:
                activePatternCoroutine = StartCoroutine(FollowPlayerPattern());
                break;
            case PatternType.None:
            default:
                break;
        }
    }

    public void StopPattern()
    {
        if (activePatternCoroutine != null)
        {
            StopCoroutine(activePatternCoroutine);
            activePatternCoroutine = null;
        }
    }

    // --- CÁC HÀM THỰC THI PATTERN ---

    private IEnumerator SequentialTogglePattern()
    {
        if (children.Count == 0) yield break;
        do { for (int i = 0; i < children.Count; i++) { if (children[i] == null) continue; children[i].SetActive(false); yield return new WaitForSeconds(toggleDuration); children[i].SetActive(true); yield return new WaitForSeconds(delayBetweenSteps); } } while (loopPattern);
    }

    private IEnumerator SequentialOnOffPattern()
    {
        if (children.Count == 0) yield break;
        do { foreach (var child in children) { if (child == null) continue; child.SetActive(true); yield return new WaitForSeconds(delayBetweenSteps); } yield return new WaitForSeconds(delayBetweenSteps * 2); foreach (var child in children) { if (child == null) continue; child.SetActive(false); yield return new WaitForSeconds(delayBetweenSteps); } } while (loopPattern);
    }

    private IEnumerator RandomTogglePattern()
    {
        if (children.Count == 0) yield break;
        do { int randomIndex = Random.Range(0, children.Count); if (children[randomIndex] != null) { children[randomIndex].SetActive(false); yield return new WaitForSeconds(toggleDuration); children[randomIndex].SetActive(true); } yield return new WaitForSeconds(delayBetweenSteps); } while (loopPattern);
    }

    private IEnumerator PingPongPattern()
    {
        if (children.Count == 0) yield break;
        do { for (int i = 0; i < children.Count; i++) { if (children[i] == null) continue; children[i].SetActive(true); yield return new WaitForSeconds(delayBetweenSteps); } for (int i = children.Count - 1; i >= 0; i--) { if (children[i] == null) continue; children[i].SetActive(false); yield return new WaitForSeconds(delayBetweenSteps); } } while (loopPattern);
    }

    private IEnumerator SlidingGapPattern()
    {
        if (children.Count == 0) yield break;
        int totalLasers = children.Count;
        int currentLeadingEdge = 0;
        do { for (int i = 0; i < totalLasers; i++) { bool shouldBeOff = false; for (int j = 0; j < gapSize; j++) { int gapMemberIndex = (currentLeadingEdge - j + totalLasers) % totalLasers; if (i == gapMemberIndex) { shouldBeOff = true; break; } } if (children[i] != null) { children[i].SetActive(!shouldBeOff); } } yield return new WaitForSeconds(delayBetweenSteps); currentLeadingEdge = (currentLeadingEdge + 1) % totalLasers; } while (loopPattern);
    }

    // --- 3 PATTERNS MỚI ---

    private IEnumerator ConvergeDivergePattern()
    {
        if (children.Count == 0) yield break;
        int left = 0;
        int right = children.Count - 1;
        do
        {
            // Converge (Hội tụ)
            left = 0;
            right = children.Count - 1;
            while (left <= right)
            {
                if (children[left] != null) children[left].SetActive(true);
                if (children[right] != null && left != right) children[right].SetActive(true);
                left++;
                right--;
                yield return new WaitForSeconds(delayBetweenSteps);
            }

            yield return new WaitForSeconds(delayBetweenSteps * 2);

            // Diverge (Phân kỳ)
            left = children.Count / 2;
            right = left - (children.Count % 2 == 0 ? 1 : 0); // Handle even/odd counts
            while (left < children.Count && right >= 0)
            {
                if (children[left] != null) children[left].SetActive(false);
                if (children[right] != null && left != right) children[right].SetActive(false);
                left++;
                right--;
                yield return new WaitForSeconds(delayBetweenSteps);
            }
        } while (loopPattern);
    }

    private IEnumerator RandomFillPattern()
    {
        if (children.Count == 0) yield break;
        var indices = Enumerable.Range(0, children.Count).ToList();
        
        do
        {
            // Random Fill
            Shuffle(indices);
            foreach (int index in indices)
            {
                if (children[index] != null) children[index].SetActive(true);
                yield return new WaitForSeconds(delayBetweenSteps);
            }

            yield return new WaitForSeconds(delayBetweenSteps * 2);

            // Random Empty
            Shuffle(indices);
            foreach (int index in indices)
            {
                if (children[index] != null) children[index].SetActive(false);
                yield return new WaitForSeconds(delayBetweenSteps);
            }
        } while (loopPattern);
    }

    private IEnumerator FollowPlayerPattern()
    {
        if (playerToFollow == null)
        {
            Debug.LogError("PlayerToFollow is not assigned! This pattern cannot run.", gameObject);
            yield break;
        }
        if (children.Count == 0) yield break;

        // This pattern runs every frame for responsiveness
        while (true) // Loop is controlled by the parent Start/StopPattern methods
        {
            // Find the child closest to the player's X position
            float playerX = playerToFollow.position.x;
            int closestIndex = -1;
            float minDistance = float.MaxValue;

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] == null) continue;
                float distance = Mathf.Abs(children[i].transform.position.x - playerX);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }

            // Create a gap around the closest index
            int gapStart = closestIndex - (gapSize / 2);
            for (int i = 0; i < children.Count; i++)
            {
                bool isInsideGap = (i >= gapStart && i < gapStart + gapSize);
                if (children[i] != null)
                {
                    children[i].SetActive(!isInsideGap);
                }
            }

            yield return null; // Wait for the next frame
        }
    }

    // Helper method to shuffle a list (Fisher-Yates shuffle)
    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
