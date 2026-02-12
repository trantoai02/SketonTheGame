using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MatrixAreaDetector : MonoBehaviour
{
    public PathMatrixController controller;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        //controller = GetComponent<PathMatrixController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            controller.PlayerEnteredMatrix();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            controller.PlayerExitedMatrix();
    }
}
