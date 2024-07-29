using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class STARTSCENE : MonoBehaviour
{
  public void rescene(){
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  public void first(){
SceneManager.LoadScene("1stfloor");
  }

  public void sixth(){
SceneManager.LoadScene("6thFloor");
  }

  public void third(){SceneManager.LoadScene("3rd Floor");}

  public void fourth(){SceneManager.LoadScene("4thFloor");}

  public void fifth(){SceneManager.LoadScene("5thFloor");}


}
