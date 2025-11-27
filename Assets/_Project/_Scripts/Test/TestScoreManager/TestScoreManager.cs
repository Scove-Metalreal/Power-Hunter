using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class TestScoreManager
{
    // A Test behaves as an ordinary method
    [Test]
    public void AddScoreTest()
    {
        ScoreManager sm = new ScoreManager();

        sm.score = 100;
        sm.addScore(10);
        
        Assert.AreEqual(110, sm.score);
    }

    [Test]
    public void ResetScoreTest()
    {
        ScoreManager sm = new ScoreManager();

        sm.resetScore();
        
        Assert.AreEqual(0, sm.score);
    }


    [Test]
    public void SetScoreTest()
    {
        ScoreManager sm = new ScoreManager();

        sm.setScore(300);
        
        Assert.AreEqual(300, sm.score);
    }


    [Test]
    public void ScoreUITest()
    {
        ScoreManager sm = new ScoreManager();
        sm.setScore(300);
        sm.ScoreUI();
        
        Assert.AreEqual("300", sm.scoreText);
    }


}
