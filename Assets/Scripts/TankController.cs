using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Так как вводные данные игрового танка и танков противника практически идентичные + большая часть логики и механик у них
//так же идентичны, используем Энам, чтобы избежать дублирования кода и используем для них один общий скрипт:
public class TankController : MonoBehaviour
{
    //Создаем энам, в котором указываем какой тип управления у данного танка:
    public enum ControlType {player, pc};                   
    [SerializeField] private ControlType controlType;

    private AudioSource shootSound;
    public int hp;
    public float speed = 300f;
    public Transform tank;
    public Transform shootPoint;
    public Rigidbody2D bullet;
    public float bulletSpeed = 20f;
    private Rigidbody2D body;
    private Vector2 moveDirection;
    private Vector3 rotation;
    private int move;
    private bool fire;
    private float minTimeWaitForMove = 1;
    private float maxTimeWaitForMove = 5;
    private float minTimeWaitForFire = 1.5f;
    private float maxTimeWaitForFire = 3.5f;

    /// <summary>
    /// На старте проверяем какой энам у объекта, если это игрок, указываем количество здоровья,если АИ, указываем другое
    /// здоровье и вызываем корутины для обработки значений движения танков и стрельбы:
    /// </summary>
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        shootSound = GetComponent<AudioSource>();
        if (controlType == ControlType.pc)
        {
            hp = 1;
            move = 0;
            StartCoroutine(WaitMove(Random.Range(minTimeWaitForMove, maxTimeWaitForMove)));
            StartCoroutine(WaitFire(Random.Range(minTimeWaitForFire, maxTimeWaitForFire)));
        }
        else hp = 2;
    }

   
    void Update()
    {
        tank.localRotation = Quaternion.Euler(rotation);
        MoveController();
        CaseBehavior();
        CheckStatusLife();
        Shooting();
    }

    void FixedUpdate()
    {
        body.AddForce(moveDirection * speed);

        if (Mathf.Abs(body.velocity.x) > speed / 100f)
        {
            body.velocity = new Vector2(Mathf.Sign(body.velocity.x) * speed / 100f, body.velocity.y);
        }
        if (Mathf.Abs(body.velocity.y) > speed / 100f)
        {
            body.velocity = new Vector2(body.velocity.x, Mathf.Sign(body.velocity.y) * speed / 100f);
        }
    }

    /// <summary>
    /// Так как поведение танков игрока и АИ отличается, прописываем свитч с кейсами, чтобы можно было не дублируя применить
    /// к обоим энамам одни и те же условия выполнения движения
    /// </summary>
    public void CaseBehavior()
    {
        switch (move)
        {
            case 1:
                moveDirection = new Vector2(0, 1);
                rotation = new Vector3(0, 0, 0);
                break;

            case 2:
                moveDirection = new Vector2(0, -1);
                rotation = new Vector3(0, 0, 180);
                break;

            case 3:
                moveDirection = new Vector2(-1, 0);
                rotation = new Vector3(0, 0, 90);
                break;

            case 4:
                moveDirection = new Vector2(1, 0);
                rotation = new Vector3(0, 0, -90);
                break;

            default:
                moveDirection = new Vector2(0, 0);
                break;
        }
    }

    /// <summary>
    /// метод для энама игрока, присваивает значения кейсов, в зависимости от управления игрока:
    /// </summary>
    private void MoveController()
    {
        if(controlType == ControlType.player)
        {
            if (Input.GetKey(KeyCode.W))
            {
                move = 1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                move = 2;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                move = 3;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                move = 4;
            }
            else
            {
                move = 0;
            }
        }
    }

    /// <summary>
    /// метод проверяет текущее здоровье танков, и в случае смерти убирает объекты со сцены:
    /// </summary>
    private void CheckStatusLife()
    {
        if(hp <= 0)
        {
            if(controlType == ControlType.pc)
            {
                Destroy(gameObject);
            }
            else
            {
                GameController.playerDead = true;
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Корутина для обработки логики стрельбы АИ танков, выбирает рандомное значение промежутков выстрелов и вызывает сама
    /// себя через рандомный промежуток времени:
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    IEnumerator WaitFire(float t)
    {
        yield return new WaitForSeconds(t);
        fire = true;
        StartCoroutine(WaitFire(Random.Range(minTimeWaitForFire, maxTimeWaitForFire)));
    }

    /// <summary>
    /// Аналолгично стрельбе, корутина для АИ танков, которая рандомит промежутки, определяющие следующее направление танков:
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    IEnumerator WaitMove(float t)
    {
        move = Random.Range(0, 4);
        yield return new WaitForSeconds(t);
        StartCoroutine(WaitMove(Random.Range(minTimeWaitForMove, maxTimeWaitForMove)));
    }

    /// <summary>
    /// Метод непосредственно стрельбы для обоих энамов, так как оба энама используют один и тот же префаб пули, при выстреле
    /// энама игрока, пуля будет менять цвет на красный, что позволит танкам АИ не уивать друг друга:
    /// </summary>
    private void Shooting()
    {
        if(controlType == ControlType.pc)
        {
            if (fire)
            {
                fire = false;
                Rigidbody2D bulletInstance = Instantiate(bullet, shootPoint.position, Quaternion.identity) as Rigidbody2D;
                bulletInstance.velocity = shootPoint.TransformDirection(Vector2.up * bulletSpeed);
                shootSound.Play();
            }
        }
        else if(controlType == ControlType.player)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Rigidbody2D bulletInstance = Instantiate(bullet, shootPoint.position, Quaternion.identity) as Rigidbody2D;
                bulletInstance.velocity = shootPoint.TransformDirection(Vector2.up * bulletSpeed);
                bulletInstance.GetComponent<SpriteRenderer>().color = Color.red;
                shootSound.Play();
            }
        }
    }
}
