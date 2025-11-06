using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RandomDraw : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject DrawShop;
    public GameObject DrawWindow;
    
    private BoxCollider2D DrawCollider; 
    private Rigidbody2D DrawRigidbody;

    public Image DrawImage;

    public int RandomInt;

    [Header("가챠결과물")]
    public Sprite Image1;
    public Sprite Image2;
    public Sprite Image3;
    public Sprite Image4;
    public Sprite Image5;
    public Sprite Image6;
    public Sprite Image7;
    public Sprite Image8;
    public Sprite Image9;
    public Sprite Image10; // 리스트 혹은 배열로 관리하는게 좋을듯함


    private void Awake()
    {
        DrawCollider = GetComponent<BoxCollider2D>();
        DrawRigidbody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        RandomInt = Random.Range(0, 9);
    }

    public void OnDraw()
    { 
        DrawWindow.SetActive(true);

        if (RandomInt == 0)
        {
            DrawImage.sprite = Image1;
        }
        else if (RandomInt == 1)
        {
            DrawImage.sprite = Image2;
        }
        else if (RandomInt == 2)
        {
            DrawImage.sprite = Image3;
        }
        else if (RandomInt == 3)
        {
            DrawImage.sprite = Image4;
        }
        else if (RandomInt == 4)
        {
            DrawImage.sprite = Image5;
        }
        else if (RandomInt == 5)
        {
            DrawImage.sprite = Image6;
        }
        else if (RandomInt == 6)
        {
            DrawImage.sprite = Image7;
        }
        else if (RandomInt == 7)
        {
            DrawImage.sprite = Image8;
        }
        else if (RandomInt == 8)
        {
            DrawImage.sprite = Image9;
        }
        else if (RandomInt == 9)
        {
            DrawImage.sprite = Image10;
        }
    }

    public void CloseDraw()
    { 
        DrawImage.sprite = null;
        DrawWindow.SetActive(false);
        DrawShop.SetActive(false);
    }

   private void OnCollisionEnter2D(Collision2D collision)
   {
        if (collision.gameObject.CompareTag("Player"))
        {
            DrawShop.SetActive(true);            
        }
   }
}

