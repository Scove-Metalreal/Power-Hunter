using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public Text scoreText;

    public void addScore(int score)
    {
        this.score += score;
    }

    public void resetScore()
    {
        this.score = 0;
    }

    public void setScore(int score)
    {
        this.score = score;
    }

    public void ScoreUI()
    {
        scoreText.text = this.score.ToString();
    }
}
