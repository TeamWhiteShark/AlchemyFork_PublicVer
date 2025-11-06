using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CustomerManager : MonoSingleton<CustomerManager>
{
    public StageData stageData;
    
    [SerializeField] private float _spawnDelay;
    [SerializeField] public int _maxSpawnCount;
    [SerializeField] public Vector3 _spawnPosition;
    [SerializeField] private int _customerCount;
    [SerializeField] private List<Customer> _customerList = new  List<Customer>();
    public int spawnCountBonus;
    public int totalSpawnCount;
    
    private Coroutine _spawnCoroutine;
    public GameObject deSpawnPoint;
    public Vector3 deSpawnPosition;
    public GameObject[] customerPool;
    public bool isReadyInTutorial = false;

    protected override bool isDestroy => true;

    private void Update()
    {
        if (_customerCount < totalSpawnCount && _spawnCoroutine == null && ArchitectureManager.Instance.stands.Count != 0)
        {
            if (SceneManager.GetActiveScene().name == "TutorialScene")
            {
                if (!isReadyInTutorial)
                {
                    return;
                }
            }
            
            _spawnCoroutine = StartCoroutine(SpawnCustomer());
        }
    }

    private IEnumerator SpawnCustomer()
    {
        try
        {
            yield return new WaitForSeconds(_spawnDelay);

            _customerCount++;

            Customer customer;
            if (SceneManager.GetActiveScene().name == "TutorialScene")
            {
                customer = ObjectPoolManager.Instance.GetObject(customerPool[0], _spawnPosition, Quaternion.identity)
                    .GetComponent<Customer>();
            }
            else
            {
                customer = Random.Range(0, 100) < 70
                    ? ObjectPoolManager.Instance.GetObject(customerPool[0], _spawnPosition, Quaternion.identity)
                        .GetComponent<Customer>()
                    : ObjectPoolManager.Instance.GetObject(customerPool[1], _spawnPosition, Quaternion.identity)
                        .GetComponent<Customer>();
            }

            if (stageData.orderItems != null && stageData.orderItems.Length > 0)
            {
                ItemSO desiredItem;
                do
                {
                    desiredItem = stageData.orderItems[Random.Range(0, stageData.orderItems.Length)];
                } 
                while (!ArchitectureManager.Instance.stands.ContainsKey(desiredItem));

                ItemSO money = Resources.Load<ItemSO>($"{GameConstants.Paths.ITEM_DATA_PATH}{GameConstants.UI.MONEY_ITEM_ID}");

                customer.SetOrder(desiredItem, money);
                customer.OnSpawn();
            }

            customer.gameObject.transform.position = _spawnPosition;
            customer.gameObject.transform.SetParent(this.transform);
            
            customer.loading.SetActive(false);
            customer.loadingAnimator.SetBool("IsLoad", false);

            _customerList.Add(customer);
        }
        finally
        {
            _spawnCoroutine = null;
        }
    }
    
    public void DespawnCustomer(Customer customer)
    {
        if (_customerList.Contains(customer))
        {
            _customerList.Remove(customer);
            _customerCount--;

            // 풀로 반환
            ObjectPoolManager.Instance.ReturnObject(customer.gameObject);
        }
    }

    public void SpawnCustomerInTutorial()
    {
        _customerCount++;

        var customer = Random.Range(0, 100) < 70
            ? ObjectPoolManager.Instance.GetObject(customerPool[0], _spawnPosition, Quaternion.identity)
                .GetComponent<Customer>()
            : ObjectPoolManager.Instance.GetObject(customerPool[1], _spawnPosition, Quaternion.identity)
                .GetComponent<Customer>();
        ;

        if (stageData.orderItems != null && stageData.orderItems.Length > 0)
        {
            ItemSO desiredItem;
            do
            {
                desiredItem = stageData.orderItems[Random.Range(0, stageData.orderItems.Length)];
            } 
            while (!ArchitectureManager.Instance.stands.ContainsKey(desiredItem));

            ItemSO money = Resources.Load<ItemSO>("Data/Item/9999");

            customer.SetOrder(desiredItem, money);
            customer.OnSpawn();
        }

        customer.gameObject.transform.position = _spawnPosition;
        customer.gameObject.transform.SetParent(this.transform);
            
        customer.loading.SetActive(false);
        customer.loadingAnimator.SetBool("IsLoad", false);

        _customerList.Add(customer);
    }
}
