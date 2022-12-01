using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    Vector2 startPosition;
    [SerializeField] private float alphaReducerCooldown = 1f;
    bool isDead;
    float alphaReducerTimer;

    [SerializeField] private List<GameObject> goldList;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        alphaReducerTimer = alphaReducerCooldown;
        startPosition = transform.position;
    }

    private void Update()
    {
        if (!isDead)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.position += Vector3.down * moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            alphaReducerTimer -= Time.deltaTime;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alphaReducerTimer);
            
            if (alphaReducerTimer <= 0)
            {
                isDead = false;
                alphaReducerTimer = alphaReducerCooldown;
                transform.position = startPosition;
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);

                // Create golds again
                foreach (GameObject gold in goldList)
                {
                    gold.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            isDead = true;
        }

        if (collision.CompareTag("Gold"))
        {
            collision.gameObject.SetActive(false);
        }
    }

}
