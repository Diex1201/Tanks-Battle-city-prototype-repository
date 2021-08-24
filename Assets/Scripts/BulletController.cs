using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulletController : MonoBehaviour
{
    public int damage = 1;
    public bool isEnemy;

	void OnTriggerEnter2D(Collider2D collider)
	{
		//���� �� �������� � �����:
		if (collider.transform.CompareTag("Wall"))  
		{
			Destroy(gameObject);
		}

		//���� �� �������� � �����:
		if (collider.transform.CompareTag("Block"))
		{
			Destroy(collider.transform.gameObject);
			Destroy(gameObject);
		}

		//���� �������� ��:
		if (!isEnemy)
		{
			if (collider.transform.CompareTag("Enemy") && gameObject.GetComponent<SpriteRenderer>().color == Color.red)
			{
				TankController enemyHP = collider.transform.GetComponent<TankController>();
				enemyHP.hp -= damage;
				Destroy(gameObject);
			}
		}
		//���� �������� ��������� ����:
		else
		{
			if (collider.transform.CompareTag("Player"))
			{
				TankController playerHP = collider.transform.GetComponent<TankController>();
				playerHP.hp -= damage;
				Destroy(gameObject);
			}
			else if(collider.transform.CompareTag("Finish"))
            {
				SceneManager.LoadScene(0);
            }
		}
	}
}
