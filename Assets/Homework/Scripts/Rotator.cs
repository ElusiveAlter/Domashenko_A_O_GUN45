using System.Collections;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Vector3 _rotate;

    private IEnumerator Start()
    {
        Rigidbody body = GetComponent<Rigidbody>();

        while (true)
        {
            yield return new WaitForFixedUpdate();
            Quaternion delta = Quaternion.Euler(_rotate * Time.fixedDeltaTime);
            body.MoveRotation(body.rotation * delta);
        }
    }
}