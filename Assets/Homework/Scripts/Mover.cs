using System.Collections;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private Vector3 _start;
    [SerializeField] private Vector3 _end;
    [SerializeField] private float _speed;
    [SerializeField] private float _delay;

    private Rigidbody _body;
    private Vector3 _offset;

    private IEnumerator Start()
    {
        _body = GetComponent<Rigidbody>();

        // Ноль «локальных координат» — там, где объект стоит в сцене.
        // Rigidbody не умеет localPosition, поэтому запоминаем смещение.

        _offset = _body.position;

        yield return Move(_start, _end);

        while (true)
        {
            yield return new WaitForSeconds(_delay);
            yield return Move(_end, _start);
            yield return new WaitForSeconds(_delay);
            yield return Move(_start, _end);
        }
    }

    private IEnumerator Move(Vector3 from, Vector3 to)
    {
        Vector3 fromWorld = _offset + from;
        Vector3 toWorld = _offset + to;

        while ((toWorld - _body.position).sqrMagnitude > 0.0001f)
        {
            yield return new WaitForFixedUpdate();
            _body.MovePosition(
                Vector3.MoveTowards(_body.position, toWorld, _speed * Time.fixedDeltaTime));
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 origin = Application.isPlaying ? _offset : transform.position;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(origin + _start, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(origin + _end, 0.1f);
        Gizmos.DrawLine(origin + _start, origin + _end);
    }
}