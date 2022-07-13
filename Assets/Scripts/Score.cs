using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
  [SerializeField] public int CurrentScore;
  [SerializeField] private TextMeshProUGUI scoreDisplay;
  // Start is called before the first frame update
  void Start()
  {
    // CurrentScore = 0;
  }
  void Update()
  {
    scoreDisplay.SetText(CurrentScore.ToString());
  }
  public void AddPoints(int pointsToAdd)
  {
    CurrentScore += pointsToAdd;
  }
  public void removePoints(int pointsToRemove)
  {
    CurrentScore -= pointsToRemove;
  }
}
