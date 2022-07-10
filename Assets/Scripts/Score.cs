using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI scoreDisplay;
  private int currentScore;
  // Start is called before the first frame update
  void Start()
  {
    currentScore = 0;
  }
  void Update()
  {
    scoreDisplay.SetText(currentScore.ToString());
  }
  public void AddPoints(int pointsToAdd)
  {
    currentScore += pointsToAdd;
  }
  public void removePoints(int pointsToRemove)
  {
    currentScore -= pointsToRemove;
  }
}
