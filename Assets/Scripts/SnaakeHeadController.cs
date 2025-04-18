using UnityEngine;
using System.Collections.Generic;

public class SnakeHeadController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float minMouseDelta = 0.1f;
    public float bodyPartDistance = 0.5f;

    public float deathTimerThreshold = 2f;
    private float deathTimer = 0f;
    private Vector3 lastPosition;

    public GameObject snakeBodyPrefab;
    [SerializeField] private List<Transform> bodyParts = new List<Transform>();

    public string foodTag = "Apple";
    public string obstacleTag = "Obstacle";
    public string snakeBodyTag = "SnakeBody";

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        targetPosition = transform.position;
        lastPosition = transform.position;
        bodyParts.Add(transform);
    }

    void Update()
    {
        HandleMovement();
        CheckForDeath();
        MoveBodyParts();
    }

    void HandleMovement()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0)); // สร้าง Plane ที่ระดับความสูงของงู
        if (groundPlane.Raycast(ray, out float hitDistance))
        {
            Vector3 hitPoint = ray.GetPoint(hitDistance);
            if (Vector3.Distance(hitPoint, targetPosition) > minMouseDelta)
            {
                targetPosition = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
                isMoving = true;
                deathTimer = 0f;
            }
        }

        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
            }
            lastPosition = transform.position;
        }
    }

    void CheckForDeath()
    {
        if (!isMoving)
        {
            deathTimer += Time.deltaTime;
            if (deathTimer >= deathTimerThreshold)
            {
                Die("snake not move!");
            }
        }
    }
    void MoveBodyParts()
    {
        if (bodyParts.Count <= 1) return;

        for (int i = 1; i < bodyParts.Count; i++)
        {
            Transform currentPart = bodyParts[i];
            Transform lastPart = bodyParts[i - 1];
            SetBodySpawnPoint bodySpawnPointSetter = lastPart.GetComponent<SetBodySpawnPoint>();

            Vector3 targetPosition = bodySpawnPointSetter.spawnPos.transform.position;
            Quaternion targetRotation = lastPart.rotation;

            currentPart.position = Vector3.MoveTowards(currentPart.position, targetPosition, (moveSpeed * (i * 1.5f)) * Time.deltaTime);
            currentPart.rotation = Quaternion.Slerp(currentPart.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(foodTag))
        {
            Grow();
            Destroy(other.gameObject); // ทำลายแอปเปิลที่เก็บแล้ว
        }
        else if (other.CompareTag(obstacleTag))
        {
            Die("snake hit obstacle!");
        }
        else if (other.CompareTag(snakeBodyTag) && bodyParts.Count > 1) // ไม่ให้ชนส่วนหัวตัวเอง
        {
            Die("snake hit his body!");
        }
    }

    void Grow()
    {
        if (snakeBodyPrefab != null)
        {
            Transform lastPart = bodyParts[bodyParts.Count - 1];
            SetBodySpawnPoint bodySpawnPointSetter = lastPart.GetComponent<SetBodySpawnPoint>();
            Quaternion spawnRotation = bodyParts[bodyParts.Count - 1].rotation;

            GameObject newBodyPart = Instantiate(snakeBodyPrefab, bodySpawnPointSetter.spawnPos.transform.position, spawnRotation);
            bodyParts.Add(newBodyPart.transform);
            newBodyPart.tag = snakeBodyTag;
        }
        else
        {
            Debug.LogWarning("Snake Body Prefab ยังไม่ได้ถูกกำหนด!");
        }
    }

    void Die(string reason)
    {
        Debug.Log("snake die because :" + reason);
        foreach (Transform part in bodyParts)
        {
            Destroy(part.gameObject);
        }
        this.enabled = false;
    }
}