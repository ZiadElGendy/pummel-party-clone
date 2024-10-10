using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] private GameObject _boom;
    [SerializeField] private int[] _sideValues = { 1, 2, 3, 4, 5, 6 };

    private Vector3[] _sideVectors = new Vector3[6]; //Vectors Of Every Side Relative To Camera
    private Vector3 _cameraVector;
    private Hashtable _diceRotationPerSide = new Hashtable();

    // Start is called before the first frame update
    void Start()
    {
        _cameraVector = Camera.main.transform.forward;
        //Defining the HashSet for the rotation of the dice corresponding to each side
        _diceRotationPerSide.Add(5, new Vector3(0, 0, 0));
        _diceRotationPerSide.Add(4, new Vector3(90, 0, 0));
        _diceRotationPerSide.Add(2, new Vector3(180, 0, 0));
        _diceRotationPerSide.Add(3, new Vector3(270, 0, 0));
        _diceRotationPerSide.Add(6, new Vector3(0, 90, 0));
        _diceRotationPerSide.Add(1, new Vector3(0, 270, 0));
    }

    #region Roll Manager
    public void AssignVectors()
    {
        for (int i = 0; i < 6; i++)
        {
            _sideVectors[i] = transform.Find("" + (i + 1)).forward;
        }

        CheckVectors();
    }

    private void CheckVectors() //check the closest vector to the camera vector
    {
        int counter = 1; //to keep note of which side has the minimum

        Vector3 minVector = _cameraVector - _sideVectors[0];
        for (int i = 1; i < 6; i++)
        {
            if (minVector.sqrMagnitude < (_cameraVector - _sideVectors[i]).sqrMagnitude)
            {
                minVector = _cameraVector - _sideVectors[i];
                counter = i + 1;
            }
        }

        StartCoroutine(WaitAndDestroy(1, counter));
    }


    private IEnumerator WaitAndDestroy(int time, int counter) //waits a "time" amount of seconds before displaying and destroying the dice
    {
        yield return new WaitForSeconds(time);

        Vector3 diceRotation = (Vector3)_diceRotationPerSide[counter];

        transform.localRotation = Quaternion.identity;
        transform.Rotate(diceRotation.x, diceRotation.y, diceRotation.z);

        Debug.Log(counter); //prints the side closest to the camera

        GameObject boomObject = Instantiate(_boom, new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.7f), Quaternion.identity);

        yield return new WaitForSeconds(1);

        DiceManager.Instance.SaveValueOfDice(_sideValues[counter - 1]);

        gameObject.GetComponent<MeshRenderer>().enabled = false;

        yield return new WaitForSeconds(1);
        Camera.main.transform.position = DiceManager.Instance.InitialCameraPosition;
        
        Destroy(transform.gameObject);
        Destroy(boomObject);
    }

    #endregion
}
