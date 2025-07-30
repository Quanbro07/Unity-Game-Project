using Pathfinding;
using UnityEngine;

public class PlayerDetect : MonoBehaviour
{
    [SerializeField] private AIPath aiPath;
    [SerializeField] private AIDestinationSetter destinationSetter;
    [SerializeField] private BoarController boarController;
    private Transform _playerTarget; // Target theo player
    private Transform _lastPosTarget; // Target cho vị trí cuối cùng
    Vector3 lastPos;
    private bool LOS;

    private void Start()
    {
        LOS = true;
        aiPath.enabled = false;
        destinationSetter.enabled = false;

        // Tạo target cho vị trí cuối cùng
        _lastPosTarget = new GameObject("LastPlayerPosition").transform;
    }

    private void FixedUpdate()
    {
        // Không cần DisableChasing nữa vì logic đã chuyển vào BoarController
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            boarController.SetState(BoarController.State.Chase);
            _playerTarget = collision.transform;
            destinationSetter.target = _playerTarget; // Theo player trực tiếp
            LOS = false;
            aiPath.enabled = true;
            destinationSetter.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Player"))
        {
            // Lưu vị trí cuối cùng của player
            lastPos = collision.transform.position;

            // Set vị trí cuối cùng cho target tĩnh
            _lastPosTarget.position = lastPos;

            // Chuyển destination sang vị trí cuối cùng (không theo player nữa)
            destinationSetter.target = _lastPosTarget;
            boarController.SetState(BoarController.State.GoToLastPos);

            LOS = true;
        }
    }
}