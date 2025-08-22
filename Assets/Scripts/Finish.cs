using System.Collections;
using UnityEngine;

public class Finish : MonoBehaviour
{

    private IEnumerator OnTriggerEnter(Collider other)
    {
        //PlayerMovementController.Instance.SetMove(false);
        yield return new WaitForSeconds(2f);

        if(other.CompareTag("Player"))
        {
            SceneController.Instance.LoadScene("MainScene");
        }
    }
}
