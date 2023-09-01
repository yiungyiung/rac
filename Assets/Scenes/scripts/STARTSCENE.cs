using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class STARTSCENE : MonoBehaviour
{
  public void rescene(){
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }
}
